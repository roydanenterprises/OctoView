using Octokit;
using Octokit.Internal;
using OctoView.Github.EventArgs;
using OctoView.Github.Models;
using OctoView.Github.Services.GithubRequestCache;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OctoView.Github.Services
{
	public interface IGithubService
	{
		Task<IReadOnlyList<Branch>> GetAllBranchesForRepo(string accessToken, string repoOwner, string repoName);
		Task<IReadOnlyList<PullRequest>> GetAllPullRequestsForRepo(string accessToken, string repoOwner, string repoName);
		Task<IReadOnlyList<Repository>> GetAllRepositories(string accessToken);
		Task<OauthToken> GetOauthAccessToken(string clientId, string clientSecret, string code);
		Task<IReadOnlyList<PullRequestReview>> GetReviewsForPullRequest(string accessToken, string repoOwner, string repoName, int pullNumber);
		Task<IEnumerable<GithubBranch>> CreateGithubBranches(string accessToken, Repository repo);
		Uri GetOauthRequestUrl(string clientId, string clientSecret, string csrfToken);
		Task<string> GetUsername(string accessToken);
		void ParseWebhookEvent(string action, string data);
		void UpdatePullRequest(PullRequest pull);

		event EventHandler<GithubLogEventArgs> Log;
		event EventHandler<GithubBranchUpdatedEvent> BranchUpdated;
		event EventHandler<GithubBranchDeletedEvent> BranchDeleted;
		event EventHandler<GithubPullUpdatedEvent> PullUpdated;
		event EventHandler<GithubPullReviewUpdatedEvent> PullReviewUpdated;
	}

	public class GithubService : IGithubService
	{
		private readonly ICache _cachingService;

		private static readonly ApiOptions ApiOptions = new ApiOptions
		{
			PageSize = 100
		};

		public GithubService(ICache cachingService)
		{
			_cachingService = cachingService;
		}

		public event EventHandler<GithubLogEventArgs> Log;
		public event EventHandler<GithubBranchUpdatedEvent> BranchUpdated;
		public event EventHandler<GithubBranchDeletedEvent> BranchDeleted;
		public event EventHandler<GithubPullUpdatedEvent> PullUpdated;
		public event EventHandler<GithubPullReviewUpdatedEvent> PullReviewUpdated;

		public Uri GetOauthRequestUrl(string clientId, string clientSecret, string csrfToken)
		{
			Trace.TraceInformation("Beginning OAuth request");

			var request = new OauthLoginRequest(clientId)
			{
				Scopes = { "user", "repo" },
				State = csrfToken
			};

			return GitHubClient().Oauth.GetGitHubLoginUrl(request);
		}

		private GitHubClient GitHubClient()
		{
			return new GitHubClient(new Connection(
					new ProductHeaderValue("GithubDashboard"),
					new CachingHttpClient(new HttpClientAdapter(HttpMessageHandlerFactory.CreateDefault), _cachingService)
				));
		}

		private GitHubClient GitHubClient(string accessToken)
		{
			var client = GitHubClient();
			client.Credentials = new Credentials(accessToken);

			return client;
		}

		public async Task<OauthToken> GetOauthAccessToken(string clientId, string clientSecret, string code)
		{
			Trace.TraceInformation("Obtaining OAuth Access Token");

			var request = new OauthTokenRequest(clientId, clientSecret, code);
			var token = await GitHubClient().Oauth.CreateAccessToken(request);

			Trace.TraceInformation($"Token obtained for {await GetUsername(token.AccessToken)}");
			return token;
		}

		public async Task<IReadOnlyList<Repository>> GetAllRepositories(string accessToken)
		{
			if (string.IsNullOrEmpty(accessToken))
			{
				return null;
			}
			return await GitHubClient(accessToken).Repository.GetAllForCurrent(ApiOptions);
		}

		public async Task<IReadOnlyList<Branch>> GetAllBranchesForRepo(string accessToken, string repoOwner, string repoName)
		{
			return await GitHubClient(accessToken).Repository.Branch.GetAll(repoOwner, repoName, ApiOptions);
		}

		public async Task<IReadOnlyList<PullRequest>> GetAllPullRequestsForRepo(string accessToken, string repoOwner, string repoName)
		{
			return await GitHubClient(accessToken).Repository.PullRequest.GetAllForRepository(repoOwner, repoName,
				new PullRequestRequest
				{
					State = ItemStateFilter.All
				}, ApiOptions);
		}

		public async Task<IReadOnlyList<PullRequestReview>> GetReviewsForPullRequest(string accessToken, string repoOwner, string repoName, int pullNumber)
		{
			return await GitHubClient(accessToken).PullRequest.Review.GetAll(repoOwner, repoName, pullNumber, ApiOptions);
		}

		public async Task<IEnumerable<GithubBranch>> CreateGithubBranches(string accessToken, Repository repo)
		{
			var branches = await GetAllBranchesForRepo(accessToken, repo.Owner.Login, repo.Name);
			var pulls = await GetAllPullRequestsForRepo(accessToken, repo.Owner.Login, repo.Name);

			var tasks = branches
				.Where(x => x.Name != repo.DefaultBranch)
				.AsParallel()
				.Select(async x => new GithubBranch
				{
					BranchName = x.Name,
					Repo = $"{repo.Owner.Login}/{repo.Name}",
					Pulls = (await Task.WhenAll(pulls
						.Where(pull => pull.Head.Ref == x.Name && pull.Head.Repository.FullName == repo.FullName).AsParallel()
						.Select(async y => await CreateGithubPull(accessToken, repo, x, y)))).ToList()
				});

			return (await Task.WhenAll(tasks)).ToList();
		}

		public async Task<GithubPull> CreateGithubPull(string accessToken, Repository repo, Branch branch,
			PullRequest pull)
		{
			IReadOnlyList<PullRequestReview> pullReviews = null;

			if (pull.State.Value == ItemState.Open)
			{
				pullReviews = await GetReviewsForPullRequest(accessToken, repo.Owner.Login, repo.Name, pull.Number);
			}

			var reviewers = pullReviews?.Select(x => x.User.Login).Distinct().Where(x => x != pull.User.Login);

			return new GithubPull
			{
				Name = pull.Title,
				Number = pull.Number,
				Status = pull.State.StringValue,
				Url = pull.HtmlUrl,
				Reviews = reviewers?.Select(x =>
																		{
																			var review = pullReviews?.Where(y => y.User.Login == x)
																				.OrderByDescending(y => y.Id)
																				.FirstOrDefault();

																			return new GithubReview
																			{
																				Name = x,
																				Url = review?.HtmlUrl,
																				Status = review?.State.StringValue
																			};
																		}).Union(pull.RequestedReviewers.Select(x => new GithubReview
																		{
																			Name = x.Login,
																			Status = "PENDING"
																		})).ToList()
			};
		}

		public void ParseWebhookEvent(string action, string data)
		{
			try
			{
				switch (action)
				{
					case "pull_request":
						var pullRequest = new SimpleJsonSerializer().Deserialize<PullRequestEventPayload>(data);
						UpdatePullRequest(pullRequest?.PullRequest);
						break;
					case "pull_request_review":
						var pullRequestReview = new SimpleJsonSerializer().Deserialize<PullRequestReviewEventPayload>(data);
						UpdatePullRequestReview(pullRequestReview?.PullRequest, pullRequestReview?.Review);
						break;
					case "create":
						var branchCreate = new SimpleJsonSerializer().Deserialize<BranchEventPayload>(data);
						UpdateBranch(branchCreate?.Ref, branchCreate?.Repository.FullName);
						break;
					case "delete":
						var branchDelete = new SimpleJsonSerializer().Deserialize<BranchEventPayload>(data);
						DeleteBranch(branchDelete?.Ref, branchDelete?.Repository.FullName);
						break;
					default:
						Log?.Invoke(this, new GithubLogEventArgs
						{
							Data = new { action, data }
						});
						break;
				}
			}
			catch (Exception e)
			{
				Log?.Invoke(this, new GithubLogEventArgs
				{
					Data = e
				});
			}
		}

		public void UpdateBranch(string branchName, string repoName)
		{
			if (string.IsNullOrWhiteSpace(branchName) || string.IsNullOrWhiteSpace(repoName))
			{
				Log?.Invoke(this, new GithubLogEventArgs
				{
					Data = "Attempted to update, but no Branch data"
				});
				return;
			}

			var githubBranch = new GithubBranch
			{
				BranchName = branchName,
				Repo = repoName
			};

			BranchDeleted?.Invoke(this, new GithubBranchDeletedEvent
			{
				RepositoryName = repoName,
				Branch = githubBranch
			});
		}

		public void DeleteBranch(string branchName, string repoName)
		{
			if (string.IsNullOrWhiteSpace(branchName) || string.IsNullOrWhiteSpace(repoName))
			{
				Log?.Invoke(this, new GithubLogEventArgs
				{
					Data = "Attempted to update, but no Branch data"
				});
				return;
			}

			var githubBranch = new GithubBranch
			{
				BranchName = branchName,
				Repo = repoName
			};

			BranchDeleted?.Invoke(this, new GithubBranchDeletedEvent
			{
				RepositoryName = repoName,
				Branch = githubBranch
			});
		}

		public void UpdatePullRequest(PullRequest pull)
		{
			if (pull == null)
			{
				Log?.Invoke(this, new GithubLogEventArgs
				{
					Data = "Attempted to update, but no Pull Request data"
				});
				return;
			}

			var githubPull = new GithubPull
			{
				Name = pull.Title,
				Number = pull.Number,
				Status = pull.State.StringValue,
				Url = pull.HtmlUrl,
				Reviews = pull
					.RequestedReviewers
					.Select(x => new GithubReview
					{
						Name = x.Login,
						Status = "PENDING"
					})
					.ToList()
			};

			var repoName = pull.Head.Repository.FullName;
			var branchName = pull.Head.Ref;

			PullUpdated?.Invoke(this, new GithubPullUpdatedEvent
			{
				RepositoryName = repoName,
				BranchName = branchName,
				Pull = githubPull
			});
		}

		public void UpdatePullRequestReview(PullRequest pull, PullRequestReview review)
		{
			if (pull == null || review == null)
			{
				Log?.Invoke(this, new GithubLogEventArgs
				{
					Data = "Attempted to update, but no Pull Request Review data"
				});
				return;
			}

			var pullReview = new GithubReview
			{
				Name = review.User.Login,
				Status = review.State.StringValue.ToUpper(),
				Url = review.HtmlUrl
			};

			var repoName = pull.Head.Repository.FullName;
			var branchName = pull.Head.Ref;
			var pullNumber = pull.Number;

			PullReviewUpdated?.Invoke(this, new GithubPullReviewUpdatedEvent
			{
				RepositoryName = repoName,
				BranchName = branchName,
				PullNumber = pullNumber,
				Review = pullReview
			});
		}

		public async Task<string> GetUsername(string accessToken)
		{
			if (string.IsNullOrWhiteSpace(accessToken))
			{
				return string.Empty;
			}

			return (await GitHubClient(accessToken).User.Current()).Login;
		}
	}
}
