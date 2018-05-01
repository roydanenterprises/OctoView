using OctoView.Github.Models;

namespace OctoView.Github.EventArgs
{
	public class GithubPullEvent : System.EventArgs
	{
		public string RepositoryName { get; set; }
		public string BranchName { get; set; }
		public GithubPull Pull { get; set; }
	}

	public class GithubPullUpdatedEvent : GithubPullEvent
	{

	}
}