using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.DTOs;

namespace GatewayService.Hubs.Contracts
{
    public interface IRequestClient
    {
        Task ReceiveNinRequest(RequestViewModel requestViewModel);

        Task ReceiveNinRequests(SearchResponse response);
    }
}
