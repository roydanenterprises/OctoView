using OctoView.Github.Models;

namespace OctoView.Github.EventArgs
{
	public class GithubPullReviewEvent : System.EventArgs
	{
		public string RepositoryName { get; set; }
		public string BranchName { get; set; }
		public int PullNumber { get; set; }
		public GithubReview Review { get; set; }
	}

	public class GithubPullReviewUpdatedEvent : GithubPullReviewEvent
	{

	}
}