using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Core;

namespace GatewayService.HangFire
{
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private static readonly string HangFireCookieName = "HangFireCookie";
        private static readonly int CookieExpirationMinutes = 30;
        private TokenValidationParameters tokenValidationParameters;
        private string role;

        public HangFireAuthorizationFilter(TokenValidationParameters tokenValidationParameters, string? role = null)
        {
            this.tokenValidationParameters = tokenValidationParameters;
            this.role = role;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            var access_token = string.Empty;
            var setCookie = false;

            // try to get token from query string
            if (httpContext.Request.Query.ContainsKey("access_token"))
            {
                access_token = httpContext.Request.Query["access_token"].FirstOrDefault();
                setCookie = true;
            }
            else
            {
                access_token = httpContext.Request.Cookies[HangFireCookieName];
            }

            if (string.IsNullOrEmpty(access_token))
            {
                return false;
            }

            try
            {
                JwtSecurityTokenHandler hand = new JwtSecurityTokenHandler();
                var jsonToken = hand.ReadJwtToken(access_token);
                var claims = jsonToken.Claims.ToList();
                var foundRole = claims.FirstOrDefault(c => c.Type == "role" && c.Value == this.role);

                if (!string.IsNullOrEmpty(this.role)
                    && !jsonToken.Audiences.Contains(tokenValidationParameters.ValidAudience)
                    && jsonToken.ValidTo < DateTime.UtcNow
                    && foundRole is null)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error during dashboard hangfire jwt validation process");
                throw e;
            }

            if (setCookie)
            {
                httpContext.Response.Cookies.Append(HangFireCookieName,
                access_token,
                new CookieOptions()
                {
                    Expires = DateTime.Now.AddMinutes(CookieExpirationMinutes)
                });
            }


            return true;
        }
    }
}

