using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Octokit;
using OctoView.Github.Contexts;
using OctoView.Github.Contexts.Models;
using System.Linq;
using System.Threading.Tasks;

namespace OctoView.Github.Services.GithubRequestCache
{
	public class GithubRequestCacheService : ICache
	{
		private readonly DbContextOptions<GithubCacheContext> _config;

		public GithubRequestCacheService(DbContextOptions<GithubCacheContext> config)
		{
			_config = config;
		}

		public Task<Response> GetAsync(string key)
		{
			using (var context = new GithubCacheContext(_config))
			{
				var request = context.Requests.FirstOrDefault(x => x.Key == key)?.Request;

				if (request == null)
				{
					return Task.FromResult(default(Response));
				}

				var result = JsonConvert.DeserializeObject<Response>(request);

				return Task.FromResult(result);
			}
		}

		public Task SetAsync(string key, IResponse value)
		{
			using (var context = new GithubCacheContext(_config))
			{
				var item = context.Requests.FirstOrDefault(x => x.Key == key);

				if (item != null)
				{
					item.Request = JsonConvert.SerializeObject(value);

					return Task.FromResult(context.SaveChanges());
				}

				context.Requests.Add(new GithubRequest
				{
					Key = key,
					Request = JsonConvert.SerializeObject(value)
				});

				//Note: This should not be `context.SaveChangeAsync` as the context is disposed by the time this
				// would execute. So instead, we save it synchronously and wrap it in a task for the return value.
				return Task.FromResult(context.SaveChanges());
			}
		}

		public Task ClearAsync()
		{
			using (var context = new GithubCacheContext(_config))
			{
				return context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE [GithubRequest]");
			}
		}
	}
}