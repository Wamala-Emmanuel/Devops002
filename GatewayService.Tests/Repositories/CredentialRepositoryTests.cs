using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Models;
using GatewayService.Repositories.Contracts;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace GatewayService.Tests.Repositories
{
    public class CredentialRepositoryTests
    {
        private readonly Mock<ICredentialRepository> _mockRepo = new Mock<ICredentialRepository>();
        private readonly Credential _request = TestHelper.GetLatestTestCredentials();

        [Fact]
        public async Task GetCredentialTest_Async()
        {
            // Arrange
            var requestList = new List<Credential>() { _request};
            _mockRepo.Setup(mr => mr.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(requestList);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.GetAllAsync();

            // Assert
            Assert.IsType<List<Credential>>(result);
        }
        
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
            Assert.Equal(_request.ExpiresOn, result.ExpiresOn);
            Assert.Equal(_request.JobId, result.JobId);
            Assert.Equal(_request.Password, result.Password);
            Assert.Equal(_request.Username, result.Username);
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
            Assert.Equal(_request.ExpiresOn, result.ExpiresOn);
            Assert.Equal(_request.JobId, result.JobId);
            Assert.Equal(_request.Password, result.Password);
            Assert.Equal(_request.Username, result.Username);
        }

    }
}
