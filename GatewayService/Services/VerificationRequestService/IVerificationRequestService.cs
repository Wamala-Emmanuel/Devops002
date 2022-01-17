using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs;

namespace GatewayService.Services
{
    public interface IVerificationRequestService
    {
        /// <summary>
        /// Returns a paginated list of requests
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<SearchResponse> GetRequestsAsync(SearchRequest request);
    }
}
