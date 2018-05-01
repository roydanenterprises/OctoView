using System.ComponentModel.DataAnnotations;

namespace OctoView.Github.Contexts.Models
{
	public class GithubRequest
	{
		[Key]
		[MaxLength(450)]
		public string Key { get; set; }
		public string Request { get; set; }
	}
}