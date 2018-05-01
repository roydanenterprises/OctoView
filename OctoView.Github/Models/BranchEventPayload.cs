using Octokit;

namespace OctoView.Github.Models
{
	public class BranchEventPayload : ActivityPayload
	{
		public string Ref { get; set; }
		public string RefType { get; set; }
		public string MasterBranch { get; set; }
		public string Description { get; set; }
		public string PusherType { get; set; }
	}
}