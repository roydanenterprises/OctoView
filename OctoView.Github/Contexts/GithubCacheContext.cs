using Microsoft.EntityFrameworkCore;
using OctoView.Github.Contexts.Models;

namespace OctoView.Github.Contexts
{
	public class GithubCacheContext : DbContext
	{
		public GithubCacheContext(DbContextOptions<GithubCacheContext> options) : base(options)
		{
		}

		public DbSet<GithubRequest> Requests { get; set; }
	}
}