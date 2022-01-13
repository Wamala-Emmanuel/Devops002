
using Hangfire.Dashboard;
using Microsoft.Extensions.Configuration;

namespace GatewayService.Helpers
{
    public class HangfireAuth : IDashboardAuthorizationFilter
    {
        private readonly IConfiguration _configuration;

        public HangfireAuth(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Authorize(DashboardContext context)
        {
            return _configuration.ShowHangfireDashboard();
        }
    }
}