namespace OctoView.Github.Contexts.Models
{
	public class UserRepository
	{
		public string UserId { get; set; }
		public User User { get; set; }

		public int RepositoryId { get; set; }
		public GithubRepository Repository { get; set; }
	}
}
