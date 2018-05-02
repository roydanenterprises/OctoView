using System.Collections.Generic;

namespace OctoView.Github.Contexts.Models
{
	public class GithubRepository
	{
		public int Id { get; set; }

		public string Owner { get; set; }
		public string Name { get; set; }

		public string FullName => $"{Owner}/{Name}";

		public virtual ICollection<UserRepository> UserRepositories { get; } = new List<UserRepository>();
	}
}