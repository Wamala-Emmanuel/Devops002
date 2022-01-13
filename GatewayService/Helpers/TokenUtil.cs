using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace GatewayService.Helpers
{
#nullable disable
    public interface ITokenUtil
    {
        bool ClaimExists(List<Claim> claims, string claimType);

        string TryGetClaimValue(List<Claim> claims, string claimType);

        Task<string> GetTokenAsync();
        
        List<Claim> GetTokenClaims(HttpRequest httpRequest);

        List<Claim> GetTokenClaims(string token);

        Task<List<Claim>> GetUserInfo(HttpRequest httpRequest);

        Task<string> GetUsername(HttpRequest httpRequest);
    }

    public class TokenUtil : ITokenUtil
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenUtil> _logger;

        //private string _token; 
        private DateTime? _expiryDate;
        private string _token;

        public TokenUtil(IConfiguration configuration, ILogger<TokenUtil> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Returns the value of a given claim. Otherwise null will be returned.
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public string TryGetClaimValue(List<Claim> claims, string claimType)
        {
            var claim = claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value;
        }
        
        /// <summary>
        /// Determines whether a given claim exists
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public bool ClaimExists(List<Claim> claims, string claimType)
        {
            return claims.Exists(claim => claim.Type == claimType);
        }

        public async Task<string> GetTokenAsync()
        {
            //Token is still valid
            if (_expiryDate.HasValue && DateTime.Now.CompareTo(_expiryDate) < 0)
            {
                return _token;
            }

            return await GetNewTokenAsync();
        }

        public List<Claim> GetTokenClaims(HttpRequest httpRequest)
        {

            _logger.LogInformation($"Retrieving user token claims.");

            var handler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters();
            var jsonToken = handler.ReadJwtToken(GetTokenValue(httpRequest));

            _logger.LogInformation($"Token claims retrieved...");
            var claims = jsonToken.Claims.ToList();

            _logger.LogTrace($"Token claims: {string.Join(",", claims.Select(x => x.Value))}");

            return claims;
        }

        public List<Claim> GetTokenClaims(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);

            return jsonToken.Claims.ToList();
        }

        public async Task<List<Claim>> GetUserInfo(HttpRequest httpRequest)
        {
            try
            {
                _logger.LogInformation("Getting user name and email and sub...");

                var settings = _configuration.GetAuthServiceSettings();

                using var client = new HttpClient();
                var discoveryDoc = await client.GetDiscoveryDocumentAsync(settings.Authority);
                if (discoveryDoc.IsError)
                {
                    _logger.LogError(discoveryDoc.Error);

                    throw new ClientFriendlyException("Failed to reach auth server.");
                }

                var response = await client.GetUserInfoAsync(new UserInfoRequest
                {
                    Address = discoveryDoc.UserInfoEndpoint,
                    Token = GetTokenValue(httpRequest)
                });

                if (response.IsError)
                {
                    _logger.LogError(response.Error);
                }
                else
                {
                    return response.Claims.ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to reach auth server.");
            }

            return null;
        }

        public async Task<string> GetUsername(HttpRequest httpRequest)
        {
            var userInfo = await GetUserInfo(httpRequest);

            var userName = TryGetClaimValue(userInfo, "given_name");

            return userName;
        }

        private async Task<string> GetNewTokenAsync()
        {
            try
            {
                var settings = _configuration.GetAuthServiceSettings();

                using var tokenClient = new HttpClient();
                var discoveryDoc = await tokenClient.GetDiscoveryDocumentAsync(settings.Authority);
                if (discoveryDoc.IsError)
                {
                    _logger.LogError(discoveryDoc.Error);

                    throw new ClientFriendlyException("Failed to reach auth server.");
                }

                var tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = discoveryDoc.TokenEndpoint,
                    ClientId = settings.ClientId,
                    ClientSecret = settings.ClientSecret,
                    Scope = settings.AccessScope
                });

                if (tokenResponse.IsError)
                {
                    _logger.LogError(tokenResponse.Error);
                }
                else
                {
                    _expiryDate = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn);
                    _token = tokenResponse.AccessToken;
                    return tokenResponse.AccessToken;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to reach auth server.");
            }

            return null;
        }

        private string GetTokenValue(HttpRequest httpRequest)
        {
            var settings = _configuration.GetAuthServiceSettings();

            var success = httpRequest.Headers.TryGetValue(settings.AuthHelpers.HeaderName, out Microsoft.Extensions.Primitives.StringValues token);

            if (!success)
            {
                throw new ClientFriendlyException("Failed to find authorization header");
            }

            var tokenValue = token.ToString()[7..];

            return tokenValue;
        }
    }
}