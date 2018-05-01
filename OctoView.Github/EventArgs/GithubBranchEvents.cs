using OctoView.Github.Models;

namespace OctoView.Github.EventArgs
{
	public class GithubBranchEvent : System.EventArgs
	{
		public string RepositoryName { get; set; }
		public GithubBranch Branch { get; set; }
	}

	public class GithubBranchUpdatedEvent : GithubBranchEvent
	{

	}

	public class GithubBranchDeletedEvent : GithubBranchEvent
	{

	}
}