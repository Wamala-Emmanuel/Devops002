using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.Models;
using GatewayService.Repositories.Contracts;
using Moq;
using Xunit;

namespace GatewayService.Tests.Repositories
{
    public class RequestsExportRepositoryTests
    {
        private readonly Mock<IRequestsExportRepository> _mockRepo = new Mock<IRequestsExportRepository>();

        [Fact]
        public async Task AddExportRequestTestAsync()
        {
            // Arrange
            var request = TestHelper.GetTestRequestsExport();
            _mockRepo.Setup(mr => mr.AddAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(request);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.AddAsync(request);

            // Assert
            Assert.Equal(request.Id, result.Id);
        }

        [Fact]
        public async Task FindRequestsExportTestAsync()
        {
            // Arrange
            var request = TestHelper.GetTestRequestsExport();
            _mockRepo.Setup(mr => mr.FindAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(request);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.FindAsync(request.Id);

            // Assert
            Assert.Equal(request.Id, result.Id);
        }

        [Fact]
        public async Task FindRequestsExportInvalidTestAsync()
        {
            // Arrange
            var request = TestHelper.GetTestRequestsExport();
            var searchId = Guid.NewGuid();
            _mockRepo.Setup(mr => mr.FindAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(request);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.FindAsync(searchId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RequestsExportExistsIsTTrueTestAsync()
        {
            // Arrange
            var request = TestHelper.GetTestRequestsExport();
            _mockRepo.Setup(mr => mr.ExistsAsync<RequestsExport>(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.ExistsAsync<RequestsExport>(request.Id);

            // Assert
            Assert.True(result);

        }

        [Fact]
        public async Task RequestsExportExistsIsFalseTestAsync()
        {
            // Arrange
            var request = TestHelper.GetTestRequestsExport();
            _mockRepo.Setup(mr => mr.ExistsAsync<RequestsExport>(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.ExistsAsync<RequestsExport>(request.Id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateRequestsExportTestAsync()
        {
            // Arrange
            var request = TestHelper.GetTestRequestsExport();
            var date = DateTime.Now;
            request.DownloadedOn = date;

            _mockRepo.Setup(mr => mr.UpdateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(request);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.UpdateAsync(request);

            // Assert
            Assert.Equal(date, request.DownloadedOn);
        }

        [Fact]
        public async Task GetNotDownloadedRequestsExportListTestAsync()
        {
            // Arrange
            var daysBack = 3;
            var offset = 3;
            var request = TestHelper.GetTestListRequestsExport();
            _mockRepo.Setup(mr => mr.GetNotDownloadedRequestsExportListAsync
                (daysBack, offset, It.IsAny<CancellationToken>())).ReturnsAsync(request);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.GetNotDownloadedRequestsExportListAsync(daysBack, offset);

            // Assert
            Assert.Equal(request.Count, result.Count);
        }
    }
}
