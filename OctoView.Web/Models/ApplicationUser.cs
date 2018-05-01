using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace OctoView.Web.Models
{
	// Add profile data for application users by adding properties to the ApplicationUser class
	public class ApplicationUser : IdentityUser
	{
		public virtual ICollection<ApplicationUserRepository> Repositories { get; set; }
	}

	public class ApplicationUserRepository
	{
		public int Id { get; set; }
		public string RepositoryName { get; set; }
	}
}
