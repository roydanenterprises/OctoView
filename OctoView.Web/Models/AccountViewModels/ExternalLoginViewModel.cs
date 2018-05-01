using System.ComponentModel.DataAnnotations;

namespace TestApplicationReact.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
