using OctoView.Github.Models;
using System.Collections.Generic;
using System.Linq;

namespace OctoView.Github.Contexts.Models
{
	public class GithubRepository
	{
		public int Id { get; set; }

		public string Owner { get; set; }
		public string Name { get; set; }

		public string FullName => $"{Owner}/{Name}";

		public virtual ICollection<UserRepository> UserRepositories { get; } = new List<UserRepository>();
		public virtual ICollection<CachedBranch> Branches { get; set; } = new List<CachedBranch>();
	}

	public class CachedBranch
	{
		public int Id { get; set; }
		public string BranchName { get; set; }
		public string Url { get; set; }
		public GithubRepository Repository { get; set; }

		public virtual ICollection<CachedPull> Pulls { get; set; } = new List<CachedPull>();
	}

	public class CachedPull
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Number { get; set; }
		public string Status { get; set; }
		public string Url { get; set; }
		public string Assignee { get; set; }
		public CachedBranch Branch { get; set; }

		public virtual ICollection<CachedReview> Reviews { get; set; } = new List<CachedReview>();
	}

	public class CachedReview
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Status { get; set; }
		public string Url { get; set; }
		public string AvatarUrl { get; set; }
		public CachedPull Pull { get; set; }
	}

	public static class Extensions
	{
		public static CachedBranch ToCachedBranch(this GithubBranch branch, GithubRepository repo)
		{
			var newBranch = new CachedBranch
			{
				BranchName = branch.BranchName,
				Url = branch.Url,
				Repository = repo
			};

			newBranch.Pulls = branch.Pulls.Select(x => x.ToCachedPull(newBranch)).ToList();

			return newBranch;
		}

		public static GithubBranch ToGithubBranch(this CachedBranch branch)
		{
			return new GithubBranch
			{
				BranchName = branch.BranchName,
				Url = branch.Url,
				Repo = branch.Repository.FullName,
				Pulls = branch.Pulls.Select(x => x.ToGithubPull()).ToList()
			};
		}

		public static CachedPull ToCachedPull(this GithubPull pull, CachedBranch branch)
		{
			var newPull = new CachedPull
			{
				Assignee = pull.Assignee,
				Name = pull.Name,
				Number = pull.Number,
				Status = pull.Status,
				Url = pull.Url,
				Branch = branch
			};

			newPull.Reviews = pull.Reviews.Select(x => x.ToCachedReview(newPull)).ToList();

			return newPull;
		}

		public static GithubPull ToGithubPull(this CachedPull pull)
		{
			return new GithubPull
			{
				Name = pull.Name,
				Assignee = pull.Assignee,
				Number = pull.Number,
				Status = pull.Status,
				Url = pull.Url,
				Reviews = pull.Reviews.Select(x => x.ToGithubReview()).ToList()
			};
		}

		public static CachedReview ToCachedReview(this GithubReview review, CachedPull pull)
		{
			return new CachedReview
			{
				AvatarUrl = review.AvatarUrl,
				Url = review.Url,
				Status = review.Status,
				UserName = review.Name,
				Pull = pull
			};
		}

		public static GithubReview ToGithubReview(this CachedReview review)
		{
			return new GithubReview
			{
				AvatarUrl = review.AvatarUrl,
				Name = review.UserName,
				Status = review.Status,
				Url = review.Url
			};
		}
	}
}