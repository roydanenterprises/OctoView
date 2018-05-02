using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OctoView.Github.Services;
using OctoView.Web.Helpers;
using OctoView.Web.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OctoView.Web.Controllers
{
	[Route("api/github")]
	[Authorize]
	public class GithubController : Controller
	{
		private readonly IConfiguration _configuration;
		private readonly IGithubService _githubService;
		private readonly UserManager<ApplicationUser> _userManager;

		public GithubController(UserManager<ApplicationUser> userManager,
			IGithubService githubService,
			IConfiguration configuration)
		{
			_userManager = userManager;
			_githubService = githubService;
			_configuration = configuration;
		}

		[HttpGet("branches")]
		public async Task<object> GetBranches()
		{
			var user = await _userManager.GetUserAsync(User);
			var claims = await _userManager.GetClaimsAsync(user);

			var token = claims.FirstOrDefault(x => x.Type == "GithubAccessToken")?.Value;
			var allRepos = await _githubService.GetAllRepositories(token);
			var selectedRepos = _githubService.GetUserRepositories(_userManager.GetUserId(User));

			var tasks = allRepos
				.Where(x => selectedRepos?.Any(y => y.FullName == x.FullName) ?? false)
				.AsParallel()
				.Select(async x => await _githubService.CreateGithubBranches(token, x));
			var result = (await Task.WhenAll(tasks)).SelectMany(x => x).ToList();
			return result;
		}

		[HttpGet("user")]
		public async Task<object> GetUser()
		{
			var user = await _userManager.GetUserAsync(User);
			var claims = await _userManager.GetClaimsAsync(user);

			var token = claims.FirstOrDefault(x => x.Type == "GithubAccessToken")?.Value;

			var result = await _githubService.GetUser(token);

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