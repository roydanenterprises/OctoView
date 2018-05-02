using Octokit;
using OctoView.Github.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using OctoView.Github.Contexts.Models;

namespace OctoView.Github.Extensions
{
	public static class Extensions
	{
		public static GithubRepository ToGithubRepository(this Repository repo)
		{
			return new GithubRepository
			{
				Owner = repo.Owner.Login,
				Name = repo.Name
			};
		}

		public static IEnumerable<T> Except<T, TKey>(this IEnumerable<T> items, IEnumerable<T> other, Func<T, TKey> getKey)
		{
			//Magic
			return items
				.GroupJoin(other, getKey, getKey, (item, tempItems) => new { item, tempItems })
				.SelectMany(x => x.tempItems.DefaultIfEmpty(), (x, temp) => new { x, temp })
				.Where(x => ReferenceEquals(null, x.temp) || x.temp.Equals(default(T)))
				.Select(x => x.x.item);
		}
	}
}
