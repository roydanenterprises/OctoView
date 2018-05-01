using GithubDashboard.Contexts.GithubCache.Models;
using Microsoft.EntityFrameworkCore;

namespace GithubDashboard.Github.Contexts.GithubCache
{
	public class GithubCacheContext : DbContext
	{
		public GithubCacheContext()
		//: base("DefaultConnection")
		{
			//Database.SetInitializer(new MigrateDatabaseToLatestVersion<GithubCacheContext, Configuration>());
		}

		public DbSet<GithubRequest> Requests { get; set; }
	}
}