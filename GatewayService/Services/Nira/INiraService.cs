using System;
using System.Threading.Tasks;
using GatewayService.Models;
using NiraWebService;

namespace GatewayService.Services.Nira
{
    public interface INiraService
    {
        Task SendRequest(Guid requestId);

        Task RenewPasswordAsync(Guid credentialsId);
    }
}
