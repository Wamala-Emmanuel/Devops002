using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace GatewayService.Extensions
{
    public class WebSocketsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _hubPath = "/hubs";
        private readonly IConfiguration _configuration;

        public WebSocketsMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _configuration = configuration;
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var settings = _configuration.GetAuthServiceSettings();

            // web sockets cannot pass headers so we must take the access token from query param and
            // add it to the header before authentication middleware runs
            if (request.Path.StartsWithSegments(_hubPath, StringComparison.OrdinalIgnoreCase) &&
                request.Query.TryGetValue(settings.AuthHelpers.TokenKey, out var accessToken))
            {
                request.Headers.Add(settings.AuthHelpers.HeaderName, $"{settings.AuthHelpers.HeaderValue}{accessToken}");
            }

            await _next(httpContext);
        }
    }
}
