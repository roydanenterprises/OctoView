using GithubDashboard.Github.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OctoView.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace OctoView.Web.Controllers
{
	[Route("api/github")]
	public class GithubController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IGithubService _githubService;

		public GithubController(UserManager<ApplicationUser> userManager,
			IGithubService githubService)
		{
			_userManager = userManager;
			_githubService = githubService;
		}

		[HttpGet("branches")]
		public async Task<object> GetBranches()
		{
			var token = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "GithubAccessToken")?.Value;

			var allRepos = await _githubService.GetAllRepositories(token);
			//var selectedRepos = await _userManager.GetGithubRepositories(_userManager.GetUserId(HttpContext.User));

			return true;
		}
	}
}
