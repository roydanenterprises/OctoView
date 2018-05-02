using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OctoView.Github.Models;
using OctoView.Github.Services;
using OctoView.Web.Helpers;
using OctoView.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OctoView.Web.Controllers
{
	[Route("api/github")]
	public class GithubController : Controller
	{
		private readonly IConfiguration _configuration;
		private readonly IGithubService _githubService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUserStore<ApplicationUser> _userStore;

		public GithubController(UserManager<ApplicationUser> userManager,
			IGithubService githubService,
			IUserStore<ApplicationUser> userStore,
			IConfiguration configuration)
		{
			_userManager = userManager;
			_githubService = githubService;
			_userStore = userStore;
			_configuration = configuration;
		}

		[HttpGet("fakeBranches")]
		public object GetBranchesFake()
		{
			return new List<GithubBranch>
			{
				new GithubBranch
				{
					Repo = "Cool Stuff",
					BranchName = "feature/testing1",
					Pulls = new List<GithubPull>
					{
						new GithubPull
						{
							Name = "implement testing1 feature.",
							Number = 125,
							Status = "Open",
							Reviews = new List<GithubReview>()
						}
					}
				},
				new GithubBranch
				{
					Repo = "Cool Stuff In other Repo",
					BranchName = "feature/testing1",
					Pulls = new List<GithubPull>
					{
						new GithubPull
						{
							Name = "implement testing1 feature.",
							Number = 127,
							Status = "Open",
							Reviews = new List<GithubReview>()
						}
					}
				},
				new GithubBranch
				{
					Repo = "Not Very cool Stuff Stuff",
					BranchName = "feature/testing2",
					Pulls = new List<GithubPull>
					{
						new GithubPull
						{
							Name = "implement testing2 feature.",
							Number = 34,
							Status = "Open",
							Reviews = new List<GithubReview>()
							{
								new GithubReview()
								{
									Name = "This guy",
									Status = "Declined",
									Url = "http://www.google.com"
								}
							}
						}
					}
				}
			};
		}

		[HttpGet("branches")]
		public async Task<object> GetBranches()
		{
			var token = User.Claims.FirstOrDefault(x => x.Type == "GithubAccessToken")?.Value;
			var allRepos = await _githubService.GetAllRepositories(token);
			var selectedRepos = _githubService.GetUserRepositories(_userManager.GetUserId(User));

			var tasks = allRepos
				.Where(x => selectedRepos?.Any(y => y.FullName == x.FullName) ?? false)
				.AsParallel()
				.Select(async x => await _githubService.CreateGithubBranches(token, x));
			var result = (await Task.WhenAll(tasks)).SelectMany(x => x).ToList();
			return result;
		}

		[HttpGet("BeginOauth")]
		public ActionResult BeginOauth()
		{
			var csrf = Password.Generate(24, 1);
			HttpContext.Session.SetString("CSRF:State", csrf);
			var clientId = _configuration["AppSettings:GithubClientId"];
			var clientSecret = _configuration["AppSettings:GithubClientSecret"];
			var uri = _githubService.GetOauthRequestUrl(clientId, clientSecret, csrf);
			return Redirect(uri.ToString());
		}

		[HttpGet("Authorize")]
		public async Task<ActionResult> Authorize(string code, string state)
		{
			if (string.IsNullOrEmpty(code))
			{
				return RedirectToAction("Index", "Home");
			}

			var expectedState = HttpContext.Session.GetString("CSRF:State");
			if (state != expectedState)
			{
				throw new InvalidOperationException();
			}

			HttpContext.Session.Remove("CSRF:State");
			var token = await _githubService.GetOauthAccessToken(_configuration["AppSettings:GithubClientId"],
				_configuration["AppSettings:GithubClientSecret"], code);
			HttpContext.Session.SetString("OAuthToken", token.AccessToken);
			await _userManager.AddClaimAsync(await _userManager.GetUserAsync(User),
				new Claim("GithubAccessToken", token.AccessToken));
			return RedirectToAction("Index", "Manage");
		}

		[HttpGet("Unauthorize")]
		public async Task<ActionResult> Unauthorize()
		{
			var claim = User.Claims.FirstOrDefault(x => x.Type == "GithubAccessToken");
			if (claim != null)
			{
				await _userManager.RemoveClaimAsync(await _userManager.GetUserAsync(User), claim);
			}

			return RedirectToAction("Index", "Manage");
		}
	}
}