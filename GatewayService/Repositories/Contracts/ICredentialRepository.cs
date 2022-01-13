using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Models;

namespace GatewayService.Repositories.Contracts
{
    public interface ICredentialRepository : IBaseRepository<Credential>
    {
        Task<List<Credential>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Credential> GetLatestAsync(CancellationToken cancellationToken = default);

        Task<bool> AnyActiveCredentials(CancellationToken cancellationToken = default);

    }
}
