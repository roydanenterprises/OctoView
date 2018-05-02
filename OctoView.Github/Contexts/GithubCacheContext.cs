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
		public DbSet<GithubRepository> Repositories { get; set; }
		public DbSet<User> Users { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<GithubRepository>()
				.HasIndex(x => new { x.Owner, x.Name })
				.IsUnique();

			modelBuilder.Entity<UserRepository>()
				.HasKey(x => new { x.RepositoryId, x.UserId });

			base.OnModelCreating(modelBuilder);
		}
	}
}