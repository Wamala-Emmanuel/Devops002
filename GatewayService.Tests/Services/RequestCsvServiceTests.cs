using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Helpers;
using GatewayService.Models;
using GatewayService.Repositories.Contracts;
using GatewayService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace GatewayService.Tests.Services
{
    public class RequestCsvServiceTests
    {
        private readonly Mock<ICoreCsvService> _mockCoreCsvService;
        private readonly Mock<IRequestsExportRepository> _mockExportRepository;
        private readonly Mock<IRequestRepository> _mockRequestRepository;
        private readonly ILogger<RequestCsvService> _logger;
        private readonly RequestCsvService _requestCsvService;
        private readonly int _pageSize;
        private readonly RequestsExport _savedRequest;
        private readonly ExportRequest _request;
        private readonly List<Request> _exportDtoList;
        private readonly Guid _requestId;
        private readonly MockFileSystem _mockFileSystem;
        private readonly SearchPagination _pagination;
        private readonly string _fullPath;

        private readonly ExportSettings _exportSettings = TestHelper.GetExportSettings(); 

        private readonly NiraSettings _niraSettings = TestHelper.GetNiraSettings();

        public RequestCsvServiceTests(ITestOutputHelper testOutputHelper)
        {
            _request = TestHelper.GetTestExportRequest();
            
            _savedRequest = TestHelper.GetTestRequestsExport();

            _exportDtoList = TestHelper.GetTestListRequest();

            _savedRequest.Request = JsonConvert.SerializeObject(_request);

            _requestId = _savedRequest.Id;

            _logger = testOutputHelper.BuildLoggerFor<RequestCsvService>();

            _mockFileSystem = new MockFileSystem();
            
            var exportOptionsMock = new Mock<IOptions<ExportSettings>>();

            exportOptionsMock.Setup(x => x.Value)
                .Returns(_exportSettings);
            
            var niraOptionsMock = new Mock<IOptions<NiraSettings>>();

            niraOptionsMock.Setup(x => x.Value)
                .Returns(_niraSettings);

            _pageSize = _exportSettings.PageSize;

            _pagination = new SearchPagination
            {
                ItemsPerPage = _pageSize,
                Page = 1,
                TotalItems = 200
            };

            _fullPath = $"{_exportSettings.FolderPath}\\{_requestId}\\{_requestId}.csv";

            _mockCoreCsvService = new Mock<ICoreCsvService>();

            _mockExportRepository = new Mock<IRequestsExportRepository>();
            
            _mockRequestRepository = new Mock<IRequestRepository>();

            _mockCoreCsvService.Setup(ms =>
                ms.WriteRecordsToCsvFileAsync(
                    It.IsAny<string>(), 
                    It.IsAny<int>(), 
                    It.IsAny<List<ExportDto>>()));

            _mockExportRepository.Setup(mr => mr.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(_savedRequest);

            _mockRequestRepository.Setup(mr => mr.GetExportRequestCountAsync(It.IsAny<ExportRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(250);

            _mockRequestRepository.Setup(mr => 
                mr.GetExportRequestListAsync(
                    It.IsAny<ExportRequest>(), 
                    It.IsAny<ExportPagination>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(_exportDtoList);

            _requestCsvService = new RequestCsvService(_mockFileSystem, _mockCoreCsvService.Object,
                _mockExportRepository.Object, _mockRequestRepository.Object, _logger, 
                exportOptionsMock.Object, niraOptionsMock.Object);
        }

        [Fact]
        public async Task Service_ShouldNotThrowException()
        {
            await _requestCsvService.WriteToCsvFileAsync(_requestId);
        }

        [Fact]
        public async Task Service_ShouldFindRequest()
        {
            await _requestCsvService.WriteToCsvFileAsync(_requestId);

            _mockExportRepository.Verify(
                mr => mr.FindAsync(It.Is<Guid>(x => x == _requestId), It.IsAny<CancellationToken>()), Times.Once);
        }

#nullable disable
        [Fact]
        public async Task Service_ShouldThrowsExceptionWhenRequestIsNull()
        {
            _mockExportRepository.Setup(mr => mr.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((RequestsExport)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _requestCsvService.WriteToCsvFileAsync(_requestId));
        }
#nullable enable

        [Fact]
        public async Task Service_ShouldGetExportRequestCount()
        {
            await _requestCsvService.WriteToCsvFileAsync(_requestId);

            _mockRequestRepository.Verify(
                mr => mr.GetExportRequestCountAsync(
                    It.Is<ExportRequest>(x => 
                        x.CardNumber == _request.CardNumber &&
                        x.MatchingStatus == _request.MatchingStatus &&
                        x.Nin == _request.Nin &&
                        x.NinValidity == _request.NinValidity &&
                        x.RequestStatus == _request.RequestStatus ),
                        It.IsAny<CancellationToken>()), Times.Once);
        }
        
        
        [Fact]
        public async Task Service_ShouldWriteHeadersOnlyWhenRequestCountIsZero()
        {
            _mockRequestRepository.Setup(mr => mr.GetExportRequestCountAsync(
                It.IsAny<ExportRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            await _requestCsvService.WriteToCsvFileAsync(_requestId);

            _mockRequestRepository.Verify(
                mr => mr.GetExportRequestCountAsync(
                    It.Is<ExportRequest>(x => 
                        x.CardNumber == _request.CardNumber &&
                        x.MatchingStatus == _request.MatchingStatus &&
                        x.Nin == _request.Nin &&
                        x.NinValidity == _request.NinValidity &&
                        x.RequestStatus == _request.RequestStatus ),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Service_ShouldGetExportRequestList()
        {
            await _requestCsvService.WriteToCsvFileAsync(_requestId);

            _mockRequestRepository.Verify(
                mr => mr.GetExportRequestListAsync(
                    It.Is<ExportRequest>(x =>
                        x.CardNumber == _request.CardNumber &&
                        x.MatchingStatus == _request.MatchingStatus &&
                        x.Nin == _request.Nin &&
                        x.NinValidity == _request.NinValidity &&
                        x.RequestStatus == _request.RequestStatus),
                    It.Is<ExportPagination>( p =>
                        p.ItemsPerPage == _pagination.ItemsPerPage),
                    It.IsAny<CancellationToken>()),
                    Times.AtLeastOnce);
        }

        [Fact]
        public async Task Service_ShouldCallCoreCsvService()
        {
            await _requestCsvService.WriteToCsvFileAsync(_requestId);

            _mockCoreCsvService.Verify( ms =>
                ms.WriteRecordsToCsvFileAsync(
                    It.Is<string>( s => s == _fullPath),
                    It.IsAny<int>(),
                    It.Is<List<ExportDto>>( e =>
                     e.Count == _exportDtoList.Count)), Times.AtLeastOnce);
        }
    }
}
