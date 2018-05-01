using System.Collections.Generic;
using System.Threading.Tasks;
using GithubDashboard.Github.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using OctoView.Web.Models;

namespace OctoView.Web.Controllers
{
	[Route("api/github")]
	public class GithubController : Controller
	{
		private readonly IGithubService _githubService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUserStore<ApplicationUser> _userStore;

		public GithubController(UserManager<ApplicationUser> userManager,
			IGithubService githubService, IUserStore<ApplicationUser> userStore)
		{
			_userManager = userManager;
			_githubService = githubService;
			_userStore = userStore;
		}

		[HttpGet("fakeBranches")]
		public async Task<object> GetBranchesFake()
		{
			return new List<Branch>
			{
				new Branch("Test 1", null, true),
				new Branch("Test 2", null, false)
			};
		}

		/*		[HttpGet("branches")]
				public async Task<object> GetBranches()
				{
					var token = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "GithubAccessToken")?.Value;

					var allRepos = await _githubService.GetAllRepositories(token);
					var selectedRepos = (await _userStore.FindByIdAsync(_userManager.GetUserId(HttpContext.User), new CancellationToken()))?.Repositories.Select(x => x.RepositoryName)
						.ToList();

					var tasks = allRepos
						.Where(x => selectedRepos?.Any(y => y == x.FullName) ?? false)
						.AsParallel()
						.Select(async x => await _githubService.CreateGithubBranches(token, x));

					var result = (await Task.WhenAll(tasks)).SelectMany(x => x).ToList();

					new Task(() => result.ForEach(x => GithubHub.BranchUpdated(x.Repo, x))).Start();

					return result;
				}*/
	}
}