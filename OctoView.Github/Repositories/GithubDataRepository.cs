using OctoView.Github.Contexts;
using OctoView.Github.Contexts.Models;
using OctoView.Github.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
			return _context.Users.FirstOrDefault(x => x.UserId == userId).UserRepositories.Select(x => x.Repository).ToList();
		}

		public bool UpdateUserRepositories(string userId, List<GithubRepository> repos)
		{
			var existingList = GetUserRepositories(userId);

			var deleted = existingList.Except(repos, x => x.FullName).ToList();
			var added = repos.Except(existingList, x => x.FullName).ToList();

			var reposToAdd = added.Where(x => x.Id == 0).ToList();
			if (reposToAdd.Any())
			{
				_context.Set<GithubRepository>().AddRange(reposToAdd);
			}

			_context.Set<UserRepository>().RemoveRange(deleted.Select(x => new UserRepository{ UserId = userId, RepositoryId = x.Id }));
			

			//deleted.ForEach(x => existingList.Remove(x));
			//_context.Set<GithubRepository>().Remo

			throw new System.NotImplementedException();
		}
	}
}
