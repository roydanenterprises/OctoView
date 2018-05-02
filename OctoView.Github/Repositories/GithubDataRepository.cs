using OctoView.Github.Contexts;
using OctoView.Github.Contexts.Models;
using OctoView.Github.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace OctoView.Github.Repositories
{
	public interface IGithubDataRepository
	{
		List<GithubRepository> GetUserRepositories(string userId);
		bool UpdateUserRepositories(string userId, List<GithubRepository> repos);
	}

	public class GithubDataRepository : IGithubDataRepository
	{
		private readonly GithubCacheContext _context;

		public GithubDataRepository(GithubCacheContext context)
		{
			_context = context;
		}

		public List<GithubRepository> GetUserRepositories(string userId)
		{
			return _context.Set<UserRepository>().Where(x => x.UserId == userId)?.Select(x => x.Repository).ToList() ??
						 new List<GithubRepository>();
		}

		public bool UpdateUserRepositories(string userId, List<GithubRepository> repos)
		{
			var user = _context.Users.FirstOrDefault(x => x.UserId == userId);

			if (user == null)
			{
				user = new User
				{
					UserId = userId
				};

				_context.Users.Add(user);
				_context.SaveChanges();
			}

			var existingList = GetUserRepositories(userId);

			var deleted = existingList?.Except(repos, x => x.FullName)?.ToList() ?? new List<GithubRepository>();
			var added = repos?.Except(existingList, x => x.FullName)?.ToList() ?? new List<GithubRepository>();

			foreach (var add in added)
			{
				var existingRepo = _context.Repositories.FirstOrDefault(x => x.FullName == add.FullName);
				if (existingRepo != null)
				{
					add.Id = existingRepo.Id;
				}
			}

			var reposToAdd = added.Where(x => x.Id == 0).ToList();
			if (reposToAdd.Any())
			{
				_context.Repositories.AddRange(reposToAdd);
				_context.SaveChanges();
			}

			_context.Set<UserRepository>().AddRange(added.Select(x => new UserRepository { User = user, Repository = x }));
			_context.Set<UserRepository>().RemoveRange(deleted.Select(x => new UserRepository { User = user, Repository = x }));

			_context.SaveChanges();

			return true;
		}
	}
}
