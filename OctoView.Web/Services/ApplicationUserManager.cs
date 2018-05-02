using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OctoView.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestApplicationReact.Models.ManageViewModels;

namespace OctoView.Web.Services
{
	public class ApplicationUserManager : UserManager<ApplicationUser>
	{
		public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
		{
		}

		public async Task<List<ApplicationUserRepository>> GetGithubRepositories(string userId)
		{
			return (await Store.FindByIdAsync(userId, new CancellationToken()))?.Repositories.ToList();
		}

		public async Task<IdentityResult> UpdateSelectedRepositories(string userId, IEnumerable<GithubRepositoryViewModel> repositories)
		{
			var passedRepos = repositories.Select(x => new ApplicationUserRepository
			                                           {
				                                           RepositoryName = $"{x.Owner}/{x.Name}",
				                                           Id = x.Id ?? 0
			                                           }).ToList();

			//var user = await Store..FindByIdAsync(userId);

			var context = ((UserStore<ApplicationUser>)Store).Context;

			var user = context.Set<ApplicationUser>().Include("Repositories").FirstOrDefault(x => x.Id == userId);

			if (user == null)
			{
				return IdentityResult.Failed();
			}

			var deletedRepos = user
				.Repositories
				.Except(passedRepos, x => $"{x.Owner}/{x.Name}")
				.ToList();
			var addedRepos = passedRepos
				.Except(user.Repositories, x => $"{x.Owner}/{x.Name}")
				.ToList();

			deletedRepos.ForEach(x => user.Repositories.Remove(x));
			context.Set<ApplicationUserRepository>().RemoveRange(deletedRepos);

			foreach (var repo in addedRepos)
			{
				user.Repositories.Add(repo);
			}

			await context.SaveChangesAsync();

			return IdentityResult.Success;
		}


	}
}
