using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OctoView.Github.Contexts;

namespace OctoView.Github.Console
{
	public class GithubCacheContextFactory : IDesignTimeDbContextFactory<GithubCacheContext>
	{
		public GithubCacheContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<GithubCacheContext>();
			builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=config;Trusted_Connection=True;MultipleActiveResultSets=true");

			return new GithubCacheContext(builder.Options);
		}
	}
}
