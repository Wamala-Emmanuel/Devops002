using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace GatewayService.Extensions
{
    public static class WebSocketsExtensions
    {
        /// <summary>
        /// Handle auth for websockets
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseWebSocketsHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebSocketsMiddleware>();
        }
    }
}
