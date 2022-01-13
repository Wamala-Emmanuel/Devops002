using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs.Credentials;
using GatewayService.Hubs.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace GatewayService.Hubs.Implementations
{
    public class CredentialHub : Hub<ICredentialClient>
    {
    }
}
