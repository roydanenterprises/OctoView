using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using GithubDashboard.Github.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Octokit;
using OctoView.Web.Helpers;
using OctoView.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
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
		private readonly IConfiguration _configuration;

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
		public async Task<object> GetBranchesFake()
		{
			return new List<Branch>
			{
				new Branch("Test 1", null, true),
				new Branch("Test 2", null, false)
			};
		}

		[HttpGet("branches")]
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

			return result;
		}

		public ActionResult BeginOauth()
		{
			var csrf = Password.Generate(24, 1);
			HttpContext.Session.SetString("CSRF:State", csrf);
			var clientId = _configuration["AppSettings:GithubClientId"];
			var clientSecret = _configuration["AppSettings:GithubClientSecret"];
			var uri = _githubService.GetOauthRequestUrl(clientId, clientSecret, csrf);
			return Redirect(uri.ToString());
		}

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

			HttpContext.Session.SetString("CSRF:State", null);
			var token = await _githubService.GetOauthAccessToken(_configuration["AppSettings:GithubClientId"],
				_configuration["AppSettings:GithubClientSecret"], code);
			HttpContext.Session.SetString("OAuthToken", token.AccessToken);
			await _userManager.AddClaimAsync(await _userManager.GetUserAsync(HttpContext.User),
				new Claim("GithubAccessToken", token.AccessToken));
			return RedirectToAction("Index", "Manage");
		}

		public async Task<ActionResult> Unauthorize()
		{
			var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "GithubAccessToken");
			if (claim != null)
			{
				await _userManager.RemoveClaimAsync(await _userManager.GetUserAsync(HttpContext.User), claim);
			}

			return RedirectToAction("Index", "Manage");
		}
	}
}
