using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Models;
using GatewayService.Repositories.Contracts;
using GatewayService.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace GatewayService.Tests.Services
{
#nullable disable
    public class VerificationRequestServiceTests
    {
        private readonly ILogger<VerificationRequestService> _logger;
        private readonly Mock<IRequestRepository> _mockRepository;
        private readonly SearchRequest _searchRequest;
        private readonly List<Request> _requestList;
        private readonly VerificationRequestService _service;

        public VerificationRequestServiceTests(ITestOutputHelper testOutputHelper)
        {
            _searchRequest = new SearchRequest
            {
                Initiator = "Bottas",
                Nin = "CF901031000HZJ",
                Pagination = new SearchPagination
                {
                    ItemsPerPage = 50,
                    Page = 1
                },
            };

            _logger = testOutputHelper.BuildLoggerFor<VerificationRequestService>();

            _mockRepository = new Mock<IRequestRepository>();

            _requestList = TestHelper.GetTestPagedRequests(_searchRequest);

            _mockRepository.Setup(mr => mr.GetAllPagedListAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(_requestList);

            _service = new VerificationRequestService(_mockRepository.Object, _logger);
        }

        [Fact]
        public async Task Service_ShouldNotThrowException()
        {
            await _service.GetRequestsAsync(_searchRequest);
        }

        [Fact]
        public async Task Service_ShouldGetPagedListOfRequests()
        {
            await _service.GetRequestsAsync(_searchRequest);

            _mockRepository.Verify(
                mr => mr.GetAllPagedListAsync(It.Is<SearchRequest>(
                                                x => x.Initiator == _searchRequest.Initiator 
                                                && x.Nin == _searchRequest.Nin),
                                                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Service_ShouldGetSearchResponseResult()
        {
            var response = await _service.GetRequestsAsync(_searchRequest);

            Assert.NotNull(response);
            Assert.Equal(_searchRequest.Pagination.ItemsPerPage, response.Pagination.ItemsPerPage);
            Assert.Equal(_searchRequest.Pagination.Page, response.Pagination.Page);
            Assert.Equal(_requestList.Count, response.Pagination.TotalItems);
            Assert.All(response.Requests, r => r.Initiator.Contains(_searchRequest.Initiator));
            Assert.IsType<SearchResponse>(response);
        }
    }
}
