using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using OctoView.Github.Models;
using OctoView.Github.Services;
using OctoView.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OctoView.Web.Hubs
{
	public class GithubHub : Hub
	{
		private readonly IGithubService _githubService;
		private readonly UserManager<ApplicationUser> _userManager;

		public GithubHub(IGithubService githubService, UserManager<ApplicationUser> userManager)
		{
			_githubService = githubService;
			_userManager = userManager;
		}

		public override async Task OnConnectedAsync()
		{
			var repositories = _githubService.GetUserRepositories(_userManager.GetUserId(Context.User)).Select(x => x.FullName);

			foreach (var repo in repositories)
			{
				await Groups.AddAsync(Context.ConnectionId, repo);
				await Clients.Caller.SendAsync("groupAdded", repo);
			}

			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			var repositories = _githubService.GetUserRepositories(_userManager.GetUserId(Context.User)).Select(x =>
				x.FullName);

			foreach (var repo in repositories)
			{
				await Groups.RemoveAsync(Context.ConnectionId, repo);
			}

			await base.OnConnectedAsync();
		}

		public static void BranchUpdated(string repoName, GithubBranch branch)
		{

		}
	}
}
