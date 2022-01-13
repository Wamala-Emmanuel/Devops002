using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GatewayService.Extensions
{
#nullable disable
    public static class UserClaimExtensions
    {
        public static (string userId, IDictionary<string, object> userClaims) GetUser(
            this ClaimsPrincipal user)
        {
            IDictionary<string, object> data = new Dictionary<string, object>
            {
                ["username"] = user.Identity?.Name ?? ""
            };
            foreach (var userClaim in user.Claims)
            {
                data[userClaim.Type] = userClaim.Value;
            }

            data["role"] = GetRoles(user);
            var id = "";
            if (data.ContainsKey("sub"))
            {
                id = data["sub"].ToString();
            }
            else if (data.ContainsKey("client_id"))
            {
                id = data["client_id"].ToString();
            }
            return (userId: id, userClaims: data);
        }

        public static string[] GetRoles(ClaimsPrincipal user)
        {
            return user.Claims
                .Where(it => it.Type == ClaimTypes.Role || it.Type.ToLower().Contains("role"))
                .Select(it => it.Value.ToUpper())
                .Distinct().ToArray();
        }
    }
}
