using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs.Credentials;

namespace GatewayService.Hubs.Contracts
{
    public interface ICredentialClient
    {
        Task ReceiveCredentialUpdate(CredentialResponse response);
    }
}
