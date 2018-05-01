using System.Collections.Generic;
using GithubDashboard.Github.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OctoView.Web.Hubs;
using OctoView.Web.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octokit;

namespace OctoView.Web.Controllers
{
	[Route("api/github")]
	public class GithubController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUserStore<ApplicationUser> _userStore;
		private readonly IGithubService _githubService;

		public GithubController(UserManager<ApplicationUser> userManager,
			IGithubService githubService, IUserStore<ApplicationUser> userStore)
		{
			_userManager = userManager;
			_githubService = githubService;
			_userStore = userStore;
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
		}
	}
}
