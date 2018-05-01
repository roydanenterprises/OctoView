using Octokit;
using System.Collections.Generic;
using System.Net;

namespace GithubDashboard.Github.Services.GithubRequestCache
{
	public class Response : IResponse
	{
		public object Body { get; set; }
		public IReadOnlyDictionary<string, string> Headers { get; set; }
		public ApiInfo ApiInfo { get; set; }
		public HttpStatusCode StatusCode { get; set; }
		public string ContentType { get; set; }
	}
}