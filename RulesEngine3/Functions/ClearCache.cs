using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RulesService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace RulesService
{
    public static class CacheManager
    {
        //public static HttpResponseMessage ClearCache([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
        //    HttpRequestMessage request, ILogger logger)
        //{
            //try
            //{
            //    if (logger == null)
            //    {
            //        return HttpRequestMessageExtensions.CreateResponse<string>(request, HttpStatusCode.InternalServerError, "Logger is null.");
            //    }

            //    if (request == null)
            //    {
            //        logger.LogError($"ClearCache request is empty. Cache is not cleared!");
            //        return System.Net.Http.HttpRequestMessageExtensions.CreateResponse<string>(request, HttpStatusCode.BadRequest, "Please pass a valid ClearCache request.");
            //    }

            //    // Parse query parameter.
            //    string cacheKey = System.Net.Http.HttpRequestMessageExtensions.GetQueryNameValuePairs(request)
            //        .FirstOrDefault((KeyValuePair<string, string> q) => string.Compare(q.Key, "cachekey", true) == 0)
            //        .Value;

            //    if (string.IsNullOrWhiteSpace(cacheKey))
            //    {
            //        logger.LogError($"ClearCache request is missing the cacheKey input parameter. Cache is not cleared!");
            //        return System.Net.Http.HttpRequestMessageExtensions.CreateResponse<string>(request, HttpStatusCode.BadRequest, "Please pass a cache key on the query string.");
            //    }
            //    else
            //    {
            //        var cache = new Cache<List<Rule<Event>>>();
            //        cache.Clear(cacheKey);

            //        logger.LogTrace($"ClearCache request successfully executed. Cache cleared with cachKey: {cacheKey}.");
            //        return System.Net.Http.HttpRequestMessageExtensions.CreateResponse<string>(request, HttpStatusCode.OK, $"Cache cleared for cache key {cacheKey}.");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Log critical error and continue.
            //    logger.LogCritical($"Failed to clear cache! {ex.Message}");
            //    return System.Net.Http.HttpRequestMessageExtensions.CreateResponse<string>(request, HttpStatusCode.InternalServerError, "Failed to clear cache! See log for details.");
        //    }
        //}
    }
}
