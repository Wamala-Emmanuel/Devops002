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
    public class NitaCredentialRepositoryTests
    {
        private readonly Mock<INitaCredentialRepository> _mockRepo = new Mock<INitaCredentialRepository>();
        private readonly NitaCredential _request = TestHelper.GetTestNitaCredentials();

        [Fact]
        public async Task AddCredentialTest_Async()
        {
            // Arrange
            _mockRepo.Setup(mr => mr.AddAsync(_request, It.IsAny<CancellationToken>())).ReturnsAsync(_request);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.AddAsync(_request);

            // Assert
            Assert.Equal(_request.Id, result.Id);
            Assert.Equal(_request.CreatedOn, result.CreatedOn);
            Assert.Equal(_request.UpdatedOn, result.UpdatedOn);
            Assert.Equal(_request.ClientKey, result.ClientKey);
            Assert.Equal(_request.ClientSecret, result.ClientSecret);
        }

        [Fact]
        public async Task CountCredential_TestAsync()
        {
            // Arrange
            _mockRepo.Setup(mr => mr.GetCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(50);

            // Act
            var repository = _mockRepo.Object;
            var expected = 50;
            var result = await repository.GetCountAsync();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetLatestAsyncCredential_TestAsync()
        {
            // Arrange
            _mockRepo.Setup(mr => mr.GetLatestAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_request);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.GetLatestAsync();

            // Assert
            Assert.Equal(_request.Id, result.Id);
            Assert.Equal(_request.CreatedOn, result.CreatedOn);
            Assert.Equal(_request.UpdatedOn, result.UpdatedOn);
            Assert.Equal(_request.ClientKey, result.ClientKey);
            Assert.Equal(_request.ClientSecret, result.ClientSecret);
        }

        [Fact]
        public async Task UpdateRequestTestAsync()
        {
            // Arrange
            _mockRepo.Setup(mr => mr.UpdateAsync(_request, It.IsAny<CancellationToken>())).ReturnsAsync(_request);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.UpdateAsync(_request);

            // Assert
            Assert.Equal(_request, result);
        }
    }
}
