using GithubDashboard.Github.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using OctoView.Web.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OctoView.Web.Hubs
{
	public class GithubHub : Hub
	{
		private readonly IUserStore<ApplicationUser> _userStore;

		public GithubHub(IUserStore<ApplicationUser> userStore)
		{
			_userStore = userStore;
		}
		public override async Task OnConnectedAsync()
		{
			var repositories =
				(await _userStore.FindByNameAsync(Context.User.Identity.Name, new CancellationToken()))?.Repositories.Select(x =>
					x.RepositoryName);

			foreach (var repo in repositories)
			{
				await Groups.AddAsync(Context.ConnectionId, repo);
				await Clients.Caller.SendAsync("groupAdded", repo);
			}

			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			var repos =
				(await _userStore.FindByNameAsync(Context.User.Identity.Name, new CancellationToken()))?.Repositories.Select(x =>
					x.RepositoryName);

			foreach (var repo in repos)
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
