using Octokit;
using System.Threading.Tasks;

namespace GithubDashboard.Github.Services.GithubRequestCache
{
	public interface ICache
	{
		Task<Response> GetAsync(string key);
		Task SetAsync(string key, IResponse value);
		Task ClearAsync();
	}
}
