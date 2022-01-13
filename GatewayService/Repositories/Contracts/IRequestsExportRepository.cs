using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.Models;

namespace GatewayService.Repositories.Contracts
{
    public interface IRequestsExportRepository : IBaseRepository<RequestsExport>
    {
        /// <summary>
        /// Returns a list of Requestsexports that are completed but not downloaded after x days
        /// </summary>
        /// <param name="days"></param>
        /// <param name="offset"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<RequestsExport>> GetNotDownloadedRequestsExportListAsync(int days, double offset, CancellationToken cancellationToken = default);
    }
}
