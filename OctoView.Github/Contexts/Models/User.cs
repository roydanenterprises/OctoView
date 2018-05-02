using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OctoView.Github.Contexts.Models
{
	public class User
	{
		[Key]
		[MaxLength(450)]
		public string UserId { get; set; }

		public virtual ICollection<UserRepository> UserRepositories { get; set; } = new List<UserRepository>();
	}
}