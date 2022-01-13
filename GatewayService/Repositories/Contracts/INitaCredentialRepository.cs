using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.Models;

namespace GatewayService.Repositories.Contracts
{
    public interface INitaCredentialRepository : IBaseRepository<NitaCredential>
    {
        Task<List<NitaCredential>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<NitaCredential> GetLatestAsync(CancellationToken cancellationToken = default);
    }
}
