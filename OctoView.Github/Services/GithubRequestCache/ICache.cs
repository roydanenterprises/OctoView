using Octokit;
using System.Threading.Tasks;

namespace OctoView.Github.Services.GithubRequestCache
{
	public interface ICache
	{
		Task<Response> GetAsync(string key);
		Task SetAsync(string key, IResponse value);
		Task ClearAsync();
	}
}
