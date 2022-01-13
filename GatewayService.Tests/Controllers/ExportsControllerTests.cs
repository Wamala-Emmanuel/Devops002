using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using GatewayService.Controllers;
using GatewayService.DTOs;
using GatewayService.Helpers;
using GatewayService.Models;
using GatewayService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace GatewayService.Tests.Controllers
{

    public class ExportsControllerTests : BaseControllerTests
    {
        private readonly Mock<HttpRequest> _mockRequest;
        private readonly Mock<IExportService> _serviceMock;
        private readonly ILogger<ExportService> _logger; 
        private readonly ExportsController _exportController;
        private readonly ExportRequest _request = TestHelper.GetTestExportRequest();
        private readonly RequestsExport _savedRequest;
        private readonly ExportStatusResponse _createdResult;
        private readonly ExportStatusResponse _statusResult;
        private readonly Guid _requestId = Guid.NewGuid();
        private readonly FileViewModel _returnedFile;

        public ExportsControllerTests(ITestOutputHelper testOutputHelper)
        {
            _mockRequest = TestHelper.CreateMockRequest(_request);

            _request = TestHelper.GetTestExportRequest();
            
            _savedRequest = TestHelper.GetTestRequestsExport();

            _serviceMock = new Mock<IExportService>();

            _logger = testOutputHelper.BuildLoggerFor<ExportService>();

            var sampleData = $"Nin, CardNumber, CardValidity";

            _returnedFile = new FileViewModel
            {
                Name = $"{_requestId}.zip",
                Contents = sampleData.GetByteArrayFromString(),
                ContentType = MediaTypeNames.Application.Octet,
            };

            _statusResult = new ExportStatusResponse
            {
                Id = _requestId,
                Status = _savedRequest.GenerationStatus,
            };

            _createdResult = new ExportStatusResponse
            {
                Id = _requestId,
                Status = ExportStatus.Processing,
                RequestUri = $"/api/exports/{_requestId}/status"
            };

            _serviceMock.Setup(ms => ms.CheckRequestStatusAsync(It.IsAny<Guid>(), It.IsAny<ApiVersion>())).ReturnsAsync(_statusResult);

            _serviceMock.Setup(ms => ms.ExportAsync(It.IsAny<ExportRequest>(), It.IsAny<HttpRequest>())).ReturnsAsync(_createdResult);

            _serviceMock.Setup(ms => ms.DownloadRequestsExportAsync(It.IsAny<Guid>())).ReturnsAsync(_returnedFile);

            _exportController = new ExportsController(_serviceMock.Object, _logger)
            {
                ControllerContext = controllerContext
            };
        }

        [Fact]
        public async Task Create_ShouldNotThrowExceptionAsync()
        {
            await _exportController.Create(_request);
        }

        [Fact]
        public async Task Create_ShouldReturnResponseAndValidStatusAsync()
        {
            var response = await _exportController.Create(_request);

            Assert.NotNull(response);
            Assert.IsType<AcceptedAtActionResult>(response);
        }
        
        [Fact]
        public async Task GetRequestStatus_ShouldNotThrowExceptionAsync()
        {
            await _exportController.GetRequestStatus(_requestId);
        }

        [Fact]
        public async Task GetRequestStatus_ShouldReturnResponseAndValidStatusAsync()
        {
            var response = await _exportController.GetRequestStatus(_requestId);

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response);
        }
        
        [Fact]
        public async Task DownloadExport_ShouldNotThrowExceptionAsync()
        {
             await _exportController.DownloadAsync(_requestId);
        }

        [Fact]
        public async Task DownloadExport_ShouldReturnResponseAndValidStatusAsync()
        {
            var response = await _exportController.DownloadAsync(_requestId);

            Assert.NotNull(response);
            Assert.IsType<FileContentResult>(response);
        }
    }
}
