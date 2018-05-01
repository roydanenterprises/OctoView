using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestApplicationReact.Models.ManageViewModels
{
	public class IndexViewModel
	{
		public string Username { get; set; }
		public IList<UserLoginInfo> Logins { get; set; }

		public bool IsEmailConfirmed { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Phone]
		[Display(Name = "Phone number")]
		public string PhoneNumber { get; set; }

		public string StatusMessage { get; set; }
		public bool HasPassword { get; set; }
		public string GithubAccount { get; set; }
		public List<string> GithubRepositories { get; set; }
		public List<string> AvailableRepositories { get; set; }

	}
}
