using OctoView.Github.Contexts.Models;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TestApplicationReact.Models.ManageViewModels;
using TestApplicationReact.Services;

namespace TestApplicationReact.Extensions
{
	public static class EmailSenderExtensions
	{
		public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
		{
			return emailSender.SendEmailAsync(email, "Confirm your email",
					$"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
		}
	}

	public static class Extensions
	{
		public static GithubRepository ToGithubRepository(this GithubRepositoryViewModel viewModel)
		{
			return new GithubRepository
			{
				Id = viewModel.Id ?? 0,
				Owner = viewModel.Owner,
				Name = viewModel.Name
			};
		}
	}
}
