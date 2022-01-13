using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs.NitaCredentials;
using GatewayService.Models;

namespace GatewayService.Services.Nita.NitaCredentialService
{
    public interface INitaCredentialService
    {
        Task<List<NitaCredential>> GetAllNitaCredentialsAsync();

        Task<NitaCredentialResponse> GetCurrentNitaCredentialsAsync();

        Task<NitaCredential> GetLatestNitaCredentialsAsync();

        Task<NitaCredentialResponse> SetNitaCredentialsAsync(NitaCredentialRequest request);

        Task<NitaCredentialResponse> UpdateNitaCredentialsAsync(Guid id, NitaCredentialRequest request);

        Task<bool> AreNitaCredentialsSet();
    }
}
