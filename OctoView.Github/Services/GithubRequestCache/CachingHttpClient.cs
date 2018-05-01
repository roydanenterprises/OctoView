using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Octokit;
using Octokit.Internal;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OctoView.Github.Services.GithubRequestCache
{
	public class CachingHttpClient : IHttpClient
	{
		private readonly IHttpClient _httpClient;
		private readonly ICache _cache;

		public CachingHttpClient(IHttpClient httpClient, ICache cache)
		{
			_httpClient = httpClient;
			_cache = cache;
		}

		public async Task<IResponse> Send(IRequest request, CancellationToken cancellationToken)
		{
			if (request.Method != HttpMethod.Get)
				return await _httpClient.Send(request, cancellationToken);

			request.Headers.TryGetValue("Authorization", out var authHeader);

			var key = $"{authHeader}:{request.Endpoint}";

			var telemetry = new TelemetryClient();

			IResponse response;

			using (var operation = telemetry.StartOperation<RequestTelemetry>("GithubCacheLookup"))
			{
				response = await _cache.GetAsync(key);
			}

			if (response == null)
			{
				telemetry.TrackEvent("GhReqNotCached");
				using (var operation = telemetry.StartOperation<RequestTelemetry>("GithubRequest"))
				{
					response = await _httpClient.Send(request, cancellationToken);
					operation.Telemetry.ResponseCode = response.StatusCode.ToString();
				}

				await _cache.SetAsync(key, response);

				return response;
			}

			if (!string.IsNullOrEmpty(response.ApiInfo.Etag))
			{
				request.Headers["If-None-Match"] = response.ApiInfo.Etag;

				IResponse conditionalResponse;

				using (var operation = telemetry.StartOperation<RequestTelemetry>("GithubRequest"))
				{
					conditionalResponse = await _httpClient.Send(request, cancellationToken);
					operation.Telemetry.ResponseCode = conditionalResponse.StatusCode.ToString();
				}

				if (conditionalResponse.StatusCode == HttpStatusCode.NotModified)
				{
					telemetry.TrackEvent("GhReqFromCache");
					return response;
				}

				telemetry.TrackEvent("GhReqCacheUpd");
				await _cache.SetAsync(key, conditionalResponse);

				return conditionalResponse;
			}

			using (var operation = telemetry.StartOperation<RequestTelemetry>("GithubRequest"))
			{
				response = (Response)await _httpClient.Send(request, cancellationToken);
				operation.Telemetry.ResponseCode = response.StatusCode.ToString();
			}

			await _cache.SetAsync(key, response);

			return response;
		}

		public void SetRequestTimeout(TimeSpan timeout)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			_httpClient?.Dispose();
		}
	}
}