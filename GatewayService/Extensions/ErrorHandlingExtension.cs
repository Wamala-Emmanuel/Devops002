using Microsoft.AspNetCore.Builder;

namespace GatewayService.Extensions
{
    public static class ErrorHandlingExtension
    {
        /// <summary>
        /// Insert error handling middle-ware
        /// </summary>
        /// <param name="builder">IApplication Builder extension</param>    
        /// <returns></returns>
        public static IApplicationBuilder UseCustomErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}
