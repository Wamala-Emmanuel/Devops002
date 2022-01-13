using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Models;

namespace GatewayService.Repositories.Contracts
{
    public interface IRequestRepository : IBaseRepository<Request>
    {
        /// <summary>
        /// Returns a list of type Request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<Request>> GetAllPagedListAsync(SearchRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the number of requests in an export request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> GetExportRequestCountAsync(ExportRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a list of requests in an export request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pagination"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<Request>> GetExportRequestListAsync(ExportRequest request, ExportPagination pagination, CancellationToken cancellationToken = default);

    }
}
