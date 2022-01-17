using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Hubs.Contracts;
using GatewayService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace GatewayService.Hubs.Implementations
{
    [Authorize]
    public class RequestHub : Hub<IRequestClient>
    {
        private readonly IVerificationRequestService _verificationRequestService;
        private readonly ILogger<VerificationRequestService> _logger;

        public RequestHub(IVerificationRequestService service, ILogger<VerificationRequestService> logger)
        {
            _logger = logger;
            _verificationRequestService = service;
        }

        public async Task GetNinRequests()
        {
            var searchRequest = new SearchRequest();

            var result = await _verificationRequestService.GetRequestsAsync(searchRequest);

            await Clients.All.ReceiveNinRequests(result);

            _logger.LogInformation("Search response with {TotalItems} resquests to be sent", result?.Pagination.TotalItems);
        }
    }
}
