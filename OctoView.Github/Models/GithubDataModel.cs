using Newtonsoft.Json;
using System.Collections.Generic;

namespace OctoView.Github.Models
{
	public class GithubRepository
	{
		public string Owner { get; set; }
		public string Name { get; set; }

		public string FullName => $"{Owner}/{Name}";
	}

	public class GithubBranch
	{
		private List<GithubPull> _pulls;

		[JsonProperty("branchName")]
		public string BranchName { get; set; }
		[JsonProperty("repo")]
		public string Repo { get; set; }
		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("pulls")]
		public List<GithubPull> Pulls
		{
			get => _pulls;
			set => _pulls = value ?? new List<GithubPull>();
		}
	}

	public class GithubPull
	{
		private List<GithubReview> _reviews;

		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("number")]
		public int Number { get; set; }
		[JsonProperty("status")]
		public string Status { get; set; }
		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("reviews")]
		public List<GithubReview> Reviews
		{
			get => _reviews;
			set => _reviews = value ?? new List<GithubReview>();
		}
	}

	public class GithubReview
	{
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("status")]
		public string Status { get; set; }
		[JsonProperty("url")]
		public string Url { get; set; }
	}
}