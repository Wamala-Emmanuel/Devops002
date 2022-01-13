using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Helpers;
using GatewayService.Models;
using GatewayService.Repositories.Contracts;
using GatewayService.Services;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace GatewayService.Tests.Services
{
    public class ExportServiceTests
    {
        private readonly Mock<IBackgroundJobClient> _jobMock;
        private readonly Mock<IRequestsExportRepository> _mockExportRepository;
        private readonly Mock<IDirectoryService> _mockDirectoryService;
        private readonly Mock<IZipService> _mockZipService;
        private readonly Mock<ITokenUtil> _mockTokenUtil;
        private readonly Mock<IRequestCsvService> _mockCsvService;
        private readonly Mock<LinkGenerator> _mockGenerator;
        private readonly Mock<HttpRequest> _mockRequest;
        private readonly ILogger<ExportService> _logger;
        private readonly ExportService _exportService;
        private readonly ExportRequest _request;
        private readonly RequestsExport _savedRequest;
        private readonly RequestsExport _ongoingRequest;
        private readonly string _folderPath;
        private readonly int _buffer;
        private readonly Guid _requestId;
        private readonly byte[] _dataStream;

        private readonly ExportSettings _exportSettings = new ExportSettings
        {
            DelayInHours = 2, 
            DaysBack = 3, 
            Buffer = 1024, 
            FolderPath = "folder", 
            NamePartial = "ID_Verification_Requests_Export",
            PageSize = 10, 
            RequestLimit = 1000000
        };
        private readonly string _username = "david";
        private readonly ApiVersion _apiVersion = new ApiVersion(1, 0);

        public ExportServiceTests(ITestOutputHelper testOutputHelper)
        {
            _request = TestHelper.GetTestExportRequest();

            _mockRequest = TestHelper.CreateMockRequest(_request);

            _savedRequest = TestHelper.GetTestRequestsExport();

            _ongoingRequest = TestHelper.GetTestRequestsExport();

            _ongoingRequest.GenerationStatus = ExportStatus.Processing;

            _requestId = _savedRequest.Id;

            var sampleData = $"Nin, CardNumber, CardValidity";

            _dataStream = sampleData.GetByteArrayFromString();

            _mockGenerator = new Mock<LinkGenerator>();

            _logger = testOutputHelper.BuildLoggerFor<ExportService>();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables().Build();

            var exportSettings = config.GetRequestExportConfig();

            var niraSettings = config.GetNiraSettings();

            _savedRequest.FileName = $"{_savedRequest.Id}.csv";
            _savedRequest.DownloadedOn = DateTime.UtcNow.AddHours(niraSettings.NiraDateTimeConfig.Offset);

            _folderPath = _exportSettings.FolderPath;

            _buffer = _exportSettings.Buffer;

            _mockExportRepository = new Mock<IRequestsExportRepository>();

            _mockCsvService = new Mock<IRequestCsvService>();

            _mockDirectoryService = new Mock<IDirectoryService>();

            _mockZipService = new Mock<IZipService>();

            _mockTokenUtil = new Mock<ITokenUtil>();

            _jobMock = new Mock<IBackgroundJobClient>();

            var exportOptionsMock = new Mock<IOptions<ExportSettings>>();

            exportOptionsMock.Setup(x => x.Value)
                .Returns(_exportSettings);

            _mockExportRepository.Setup(mr => mr.AddAsync(It.IsAny<RequestsExport>(), It.IsAny<CancellationToken>())).ReturnsAsync(_savedRequest);

            _mockExportRepository.Setup(mr => mr.ExistsAsync<RequestsExport>(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            _mockExportRepository.Setup(mr => mr.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(_savedRequest);

            _mockExportRepository.Setup(mr => mr.UpdateAsync(It.IsAny<RequestsExport>(), It.IsAny<CancellationToken>())).ReturnsAsync(_savedRequest);

            _mockDirectoryService.Setup(ds => ds.CreateTempFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

            _mockDirectoryService.Setup(ds => ds.FileExists(It.IsAny<string>())).Returns(true);

            _mockZipService.Setup(zs => zs.GetZipFileBytesAsync(It.IsAny<string>())).ReturnsAsync(_dataStream);

            _jobMock.Setup(x => x.Create(It.Is<Job>(job => job.Method.Name == nameof(IRequestCsvService.WriteToCsvFileAsync)), It.IsAny<EnqueuedState>())).Returns("1");
            
            _jobMock.Setup(x => x.Create(It.Is<Job>(job => job.Method.Name == nameof(IZipService.ZipFileAsync)), It.IsAny<AwaitingState>())).Returns("2");
            
            _jobMock.Setup(x => x.Create(It.Is<Job>(job => job.Method.Name == nameof(IZipService.DeleteDownloadedZipFileAsync)), It.IsAny<AwaitingState>())).Returns("3");

            _mockTokenUtil.Setup(x => x.GetUsername(It.IsAny<HttpRequest>()))
                .ReturnsAsync(_username);

            _exportService = new ExportService(_mockCsvService.Object, _mockDirectoryService.Object,
                _mockZipService.Object, _mockExportRepository.Object, _jobMock.Object, config,
                _logger, _mockTokenUtil.Object, exportOptionsMock.Object, _mockGenerator.Object);

        }

        #region ExportRequest

        [Fact]
        public async Task Service_ShouldNotThrowException()
        {
            await _exportService.ExportAsync(_request, _mockRequest.Object);
        }

        [Fact]
        public async Task Service_ShouldSaveExportsRequest()
        {
            var jsonRequest = JsonConvert.SerializeObject(_request);
            await _exportService.ExportAsync(_request, _mockRequest.Object);

            _mockExportRepository.Verify(
                mr => mr.AddAsync(
                    It.Is<RequestsExport>(y => 
                        y.Request == jsonRequest 
                        && y.GenerationStatus == ExportStatus.Processing),
                        It.IsAny<CancellationToken>()), 
                Times.Once);
        }
        
        [Fact]
        public async Task Service_ShouldCreateExportsRequestFile()
        {
            await _exportService.ExportAsync(_request, _mockRequest.Object);

            _mockDirectoryService.Verify(
                ds => ds.CreateTempFile(It.Is<string>(y => y == _folderPath), 
                    It.Is<string>(y => y == _savedRequest.FileName),
                    It.IsAny<string>(),
                    It.Is<int>(y => y == _exportSettings.Buffer)), Times.Once);
        }

        [Fact]
        public async Task Service_ShouldCallCsvService()
        {
            await _exportService.ExportAsync(_request, _mockRequest.Object);

            _jobMock.Verify(x => x.Create(
                It.Is<Job>(y => 
                    y.Method.Name == nameof(IRequestCsvService.WriteToCsvFileAsync)),
                        It.IsAny<EnqueuedState>()), Times.Once);

        }
        
        [Fact]
        public async Task Service_ShouldCallZipServiceToZip()
        {
            await _exportService.ExportAsync(_request, _mockRequest.Object);

            _jobMock.Verify(x => x.Create(
                It.Is<Job>(y => 
                    y.Method.Name == nameof(IZipService.ZipFileAsync)),
                        It.IsAny<AwaitingState>()), Times.Once);

        }


        [Fact]
        public async Task Service_ShouldCallZipServiceToDelete()
        {
            await _exportService.ExportAsync(_request, _mockRequest.Object);

            _jobMock.Verify(x => x.Create(
                It.Is<Job>(y =>
                    y.Method.Name == nameof(ExportService.ScheduleDeleteJob)),
                        It.IsAny<AwaitingState>()), Times.Once);
        }

        [Fact]
        public async Task Service_ShouldReturnExportStatusResponse()
        {
            var result = await _exportService.ExportAsync(_request, _mockRequest.Object);

            Assert.IsType<ExportStatusResponse>(result);
        }
        #endregion

        #region GetRequestStatus 
        [Fact]
        public async Task CheckRequestStatusAsync_ShouldNotThrowException()
        {
            await _exportService.CheckRequestStatusAsync(_requestId, _apiVersion);
        }

        [Fact]
        public async Task CheckRequestStatusAsync_ShouldFindRequest()
        {
            await _exportService.CheckRequestStatusAsync(_requestId, _apiVersion);

            _mockExportRepository.Verify(
                mr => mr.FindAsync(It.Is<Guid>(x => x == _requestId), It.IsAny<CancellationToken>()), Times.Once);
        }

#nullable disable
        [Fact]
        public async Task CheckRequestStatusAsync_ShouldThrowsExceptionWhenRequestIsNull()
        {
            _mockExportRepository.Setup(mr => mr.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((RequestsExport)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _exportService.CheckRequestStatusAsync(_requestId, _apiVersion));
        }
#nullable enable

        [Fact]
        public async Task CheckRequestStatusAsync_ReturnsRequestsExportObject()
        {
            var result = await _exportService.CheckRequestStatusAsync(_requestId, _apiVersion);

            Assert.IsType<ExportStatusResponse>(result);
            Assert.Equal(_requestId, result.Id);
            
        }

        [Fact]
        public async Task CheckRequestStasusAsync_ReturnsRequestWithCompleteStatus_WhenProcessIsComplete()
        {
            _mockExportRepository.Setup(mr => mr.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RequestsExport
                {
                    Id = _requestId,
                    GenerationStatus = ExportStatus.Complete,
                    FileName = "location of file"
                });

            var result = await _exportService.CheckRequestStatusAsync(_requestId, _apiVersion);

            Assert.Equal(_requestId, result.Id);
            Assert.Equal(ExportStatus.Complete, result.Status);
        }
        #endregion

        #region DownloadExportFile
        [Fact]
        public async Task DownloadRequestsExport_ShouldNotThrowException()
        {
            await _exportService.DownloadRequestsExportAsync(_requestId);
        }

        [Fact]
        public async Task DownloadRequestsExport_ShouldFindRequest()
        {
            await _exportService.DownloadRequestsExportAsync(_requestId);

            _mockExportRepository.Verify(
                mr => mr.FindAsync(It.Is<Guid>(x => x == _requestId), It.IsAny<CancellationToken>()), Times.Once);
        }

#nullable disable
        [Fact]
        public async Task DownloadRequestsExport_ShouldThrowsExceptionWhenRequestIsNull()
        {
            _mockExportRepository.Setup(mr => 
                mr.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((RequestsExport)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _exportService.DownloadRequestsExportAsync(_requestId));
        }
#nullable enable

        [Fact]
        public async Task DownloadRequestsExportAsync_ShouldThrowsExceptionWhenRequestIsBeingProcessed()
        {
            _mockExportRepository.Setup(mr =>
                mr.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_ongoingRequest);

            await Assert.ThrowsAsync<ClientFriendlyException>(() => _exportService.DownloadRequestsExportAsync(_requestId));
        }

        [Fact]
        public async Task DownloadRequestsExport_ShouldFindFile()
        {
            await _exportService.DownloadRequestsExportAsync(_requestId);

            _mockDirectoryService.Verify(
                mr => mr.FileExists(It.Is<string>(x => x == _savedRequest.FileName)), Times.Once);
        }

#nullable disable
        [Fact]
        public async Task DownloadRequestsExport_ShouldThrowsExceptionWhenFileDoesnotExist()
        {
            _mockDirectoryService.Setup(ds => ds.FileExists(It.IsAny<string>())).Returns(false);

            await Assert.ThrowsAsync<NotFoundException>(() => _exportService.DownloadRequestsExportAsync(_requestId));
        }
#nullable enable

        [Fact]
        public async Task DownloadRequestsExport_ShouldGetFileContents()
        {
            await _exportService.DownloadRequestsExportAsync(_requestId);

            _mockZipService.Verify(
                zs => zs.GetZipFileBytesAsync(It.Is<string>(x => x == _savedRequest.FileName)), Times.Once);
        }

        [Fact]
        public async Task DownloadRequestsExport_ShouldUpdateRequestExport()
        {
            await _exportService.DownloadRequestsExportAsync(_requestId);

            _mockExportRepository.Verify(mr => 
                mr.UpdateAsync(It.Is<RequestsExport>(x => 
                    x.Id == _requestId && 
                    x.Request == _savedRequest.Request && 
                    x.UserName == _savedRequest.UserName && 
                    x.GenerationStatus == _savedRequest.GenerationStatus &&
                    x.FileName == _savedRequest.FileName &&
                    x.IsDeleted == _savedRequest.IsDeleted ),
                    It.IsAny<CancellationToken>()), Times.Once);
        }


#nullable disable
        [Fact]
        public async Task DownloadRequestsExport_ReturnsRequestsExportObject()
        {
            var fileName = $"{_savedRequest.Id}.zip";
            
            var result = await _exportService.DownloadRequestsExportAsync(_requestId);

            Assert.IsType<FileViewModel>(result);
            Assert.Equal(fileName, result.Name);
        }
        #endregion
    }
}
