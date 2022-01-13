using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.DTOs.Credentials;
using GatewayService.Models;

namespace GatewayService.Services
{
    public interface ICredentialService
    {
        Task<List<Credential>> GetAllCredentialsAsync();

        Task<CredentialResponse> GetCurrentCredentialsAsync();

        Task<Credential> GetLatestCredentialsAsync();

        Task SchedulePasswordRenewalJobAsync(Guid requestId);

        Task<CredentialResponse> SetCredentialsAsync(CredentialRequest request);

        bool AreConfigCredentialsSet();

        Task<bool> AreDatabaseCredentialsSet();
    }
}
