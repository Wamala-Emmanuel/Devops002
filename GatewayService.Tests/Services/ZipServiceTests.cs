using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
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
    public class ZipServiceTests
    {
        private readonly Mock<IDirectoryService> _mockDirectoryService;
        private readonly Mock<IRequestsExportRepository> _mockExportRepository;
        private readonly MockFileSystem _mockFileSystem;
        private readonly ILogger<ZipService> _logger;
        private readonly ZipService _zipService;
        private readonly Guid _requestId;
        private readonly RequestsExport _savedRequest;
        private readonly RequestsExport _updatedRequest;
        private readonly List<RequestsExport> _requestList;
        private readonly ExportRequest _request;
        private readonly double _offset;
        private readonly ExportSettings _exportSettings = new ExportSettings
        {
            DelayInHours = 2,
            DaysBack = 3,
            Buffer = 1024,
            FolderPath = "folder",
            PageSize = 10,
            RequestLimit = 1000000
        };

        public ZipServiceTests(ITestOutputHelper testOutputHelper)
        {
            _request = TestHelper.GetTestExportRequest();

            _requestList = TestHelper.GetTestListRequestsExport();
            
            _savedRequest = TestHelper.GetTestRequestsExport();

            _savedRequest.Request = JsonConvert.SerializeObject(_request);

            _updatedRequest = _savedRequest;

            _updatedRequest.GenerationStatus = ExportStatus.Complete;

            _requestId = _savedRequest.Id;

            _logger = testOutputHelper.BuildLoggerFor<ZipService>();
            
            var exportOptionsMock = new Mock<IOptions<ExportSettings>>();

            exportOptionsMock.Setup(x => x.Value)
                .Returns(_exportSettings);

            _mockFileSystem = new MockFileSystem();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables().Build();

            var settings = config.GetNiraSettings();

            _offset = settings.NiraDateTimeConfig.Offset;

            _mockFileSystem.AddFile($"./folder/{_requestId}.csv", new MockFileData("Testing,is,nin."));
            
            _mockFileSystem.AddFile($"./folder/{_requestId}.zip", new MockFileData("Testing,is,nin."));

            _mockExportRepository = new Mock<IRequestsExportRepository>();

            _mockExportRepository.Setup(mr => mr.FindAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(_savedRequest);

            _mockExportRepository.Setup(mr => mr.UpdateAsync(
                It.IsAny<RequestsExport>(), It.IsAny<CancellationToken>())).ReturnsAsync(_updatedRequest);

            _mockExportRepository.Setup(mr => 
                mr.GetNotDownloadedRequestsExportListAsync(
                    It.IsAny<int>(), It.IsAny<double>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_requestList);

            _mockDirectoryService = new Mock<IDirectoryService>();

            _mockDirectoryService.Setup(ms => ms.FileExists(It.IsAny<string>())).Returns(true);

            _mockDirectoryService.Setup(ms => ms.DeleteFile(It.IsAny<string>()));

            _zipService = new ZipService(_mockFileSystem, _mockExportRepository.Object, _mockDirectoryService.Object,
                config, _logger, exportOptionsMock.Object);
        }

        [Fact]
        public async Task Service_ShouldNotThrowException()
        {
            await _zipService.ZipFileAsync(_requestId);
        }

        [Fact]
        public async Task Service_ShouldFindRequest()
        {
            await _zipService.ZipFileAsync(_requestId);

            _mockExportRepository.Verify(
                mr => mr.FindAsync(
                    It.Is<Guid>(x => x == _requestId), It.IsAny<CancellationToken>()), Times.Once);
        }

#nullable disable
        [Fact]
        public async Task Service_ThrowsExceptionWhenRequestIsNull()
        {
            _mockExportRepository.Setup(
                mr => mr.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RequestsExport)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _zipService.ZipFileAsync(_requestId));
        }
#nullable enable

        [Fact]
        public async Task Service_ShouldUpdateRequest()
        {
            await _zipService.ZipFileAsync(_requestId);

            _mockExportRepository.Verify(
                mr => mr.UpdateAsync(It.Is<RequestsExport>(x =>
                    x.Id == _updatedRequest.Id &&
                    x.GenerationStatus == _updatedRequest.GenerationStatus &&
                    x.CreatedOn == _updatedRequest.CreatedOn &&
                    x.DownloadedOn == _updatedRequest.DownloadedOn &&
                    x.FileName == _updatedRequest.FileName && 
                    x.IsDeleted == _updatedRequest.IsDeleted),
                    It.IsAny<CancellationToken>())
                , Times.Once);
        }

        [Fact]
        public async Task DeleteDownloadedZipFile_ShouldNotThrowException()
        {
            await _zipService.DeleteDownloadedZipFileAsync(_requestId);
        }

        [Fact]
        public async Task DeleteDownloadedZipFile_ShouldFindRequest()
        {
            await _zipService.DeleteDownloadedZipFileAsync(_requestId);

            _mockExportRepository.Verify(
                mr => mr.FindAsync(
                    It.Is<Guid>(x => x == _requestId), It.IsAny<CancellationToken>()), Times.Once);
        }

#nullable disable
        [Fact]
        public async Task DeleteDownloadedZipFile_ThrowsExceptionWhenRequestIsNull()
        {
            _mockExportRepository.Setup(mr => mr.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((RequestsExport)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _zipService.ZipFileAsync(_requestId));
        }
#nullable enable

        [Fact]
        public async Task DeleteDownloadedZipFile_ThrowsExceptionWhenFileDoesnotExist()
        {
            _mockDirectoryService.Setup(ms => ms.FileExists(It.IsAny<string>())).Returns(false);

            await Assert.ThrowsAsync<DirectoryNotFoundException>(() => _zipService.ZipFileAsync(_requestId));
        }

        [Fact]
        public async Task DeleteDownloadedZipFile_ShouldUpdateRequest()
        {
            await _zipService.DeleteDownloadedZipFileAsync(_requestId);

            _mockExportRepository.Verify(
                mr => mr.UpdateAsync(It.Is<RequestsExport>(x =>
                    x.Id == _updatedRequest.Id &&
                    x.GenerationStatus == _updatedRequest.GenerationStatus &&
                    x.CreatedOn == _updatedRequest.CreatedOn &&
                    x.DownloadedOn == _updatedRequest.DownloadedOn &&
                    x.FileName == _updatedRequest.FileName &&
                    x.IsDeleted == _updatedRequest.IsDeleted),
                    It.IsAny<CancellationToken>())
                , Times.Once);
        }

        [Fact]
        public async Task DeleteRequestExport_ShouldNotThrowException()
        {
            await _zipService.DeleteRequestExportAsync();
        }

        [Fact]
        public async Task DeleteRequestExport_ShouldGetExportRequestList()
        {
            await _zipService.DeleteRequestExportAsync();

            _mockExportRepository.Verify(
                mr => mr.GetNotDownloadedRequestsExportListAsync(
                    It.Is<int>(x => x == _exportSettings.DaysBack),
                    It.Is<double>(x => x == _offset), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task DeleteRequestExport_ShouldUpdateRequest()
        {
            await _zipService.DeleteRequestExportAsync();

            _mockExportRepository.Verify(
                mr => mr.UpdateAsync(It.Is<RequestsExport>(x =>
                    x.Id == _requestList[0].Id &&
                    x.GenerationStatus == _requestList[0].GenerationStatus &&
                    x.CreatedOn == _requestList[0].CreatedOn &&
                    x.DownloadedOn == _requestList[0].DownloadedOn &&
                    x.FileName == _requestList[0].FileName &&
                    x.IsDeleted == _requestList[0].IsDeleted),
                    It.IsAny<CancellationToken>())
                , Times.Once);
        }

#nullable disable

        [Fact]
        public async Task DeleteRequestExport_ShouldNotUpdateRequestWhenListisEmpty()
        {
            _mockExportRepository.Setup(mr =>
                mr.GetNotDownloadedRequestsExportListAsync(
                    It.IsAny<int>(), It.IsAny<double>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<RequestsExport>)null);

            await _zipService.DeleteRequestExportAsync();

            _mockExportRepository.Verify(
                mr => mr.UpdateAsync(It.Is<RequestsExport>(x =>
                    x.Id == _requestList[0].Id &&
                    x.GenerationStatus == _requestList[0].GenerationStatus &&
                    x.CreatedOn == _requestList[0].CreatedOn &&
                    x.DownloadedOn == _requestList[0].DownloadedOn &&
                    x.FileName == _requestList[0].FileName &&
                    x.IsDeleted == _requestList[0].IsDeleted),
                    It.IsAny<CancellationToken>())
                , Times.Never);
        }
#nullable enable
    
    }
}
