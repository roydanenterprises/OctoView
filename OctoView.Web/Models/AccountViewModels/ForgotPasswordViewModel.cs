using System.ComponentModel.DataAnnotations;

namespace TestApplicationReact.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
