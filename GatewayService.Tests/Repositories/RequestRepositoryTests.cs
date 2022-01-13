using System;
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

    public class RequestRepositoryTests
    {
        private readonly Mock<IRequestRepository> _mockRepo = new Mock<IRequestRepository>();
        
        [Fact]
        public async Task AddRequestTestAsync()
        {
            // Arrange
            var request = TestHelper.GetTestRequest();
            _mockRepo.Setup(mr => mr.AddAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(request);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.AddAsync(request);

            // Assert
            Assert.Equal(request.Id, result.Id);
        }

        [Fact]
        public async Task FindRequestTestAsync()
        {
            // Arrange
            var request = TestHelper.GetTestRequest();
            _mockRepo.Setup(mr => mr.FindAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(request);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.FindAsync(request.Id);

            // Assert
            Assert.Equal(request.Id, result.Id);
        }

        [Fact]
        public async Task FindRequestInvalidTestAsync()
        {
            // Arrange
            var request = TestHelper.GetTestRequest();
            var searchId = Guid.NewGuid();
            _mockRepo.Setup(mr => mr.FindAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(request);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.FindAsync(searchId);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task RequestExistsIsTTrueTestAsync()
        {
            // Arrange
            var request = TestHelper.GetTestRequest();
            _mockRepo.Setup(mr => mr.ExistsAsync<Request>(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.ExistsAsync<Request>(request.Id);

            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public async Task RequestExistsIsFalseTestAsync()
        {
            // Arrange
            var request = TestHelper.GetTestRequest();
            _mockRepo.Setup(mr => mr.ExistsAsync<Request>(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.ExistsAsync<Request>(request.Id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateRequestTestAsync()
        {
            // Arrange
            var request = TestHelper.GetTestRequest();
            var niraResult = TestHelper.GetTestNiraResult();
            request.Result = JsonConvert.SerializeObject(niraResult);
            _mockRepo.Setup(mr => mr.UpdateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(request);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.UpdateAsync(request);

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(niraResult), request.Result);
        }

        private readonly SearchRequest _searchRequest = new SearchRequest {
          Initiator = "Lawrence"
        };

        [Fact]
        public async Task GetPagedRequestListTestAsync()
        {
            // Arrange
            var requestList = TestHelper.GetTestPagedRequests(_searchRequest);

            _mockRepo.Setup(mr => mr.GetAllPagedListAsync(_searchRequest, It.IsAny<CancellationToken>())).ReturnsAsync(requestList);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.GetAllPagedListAsync(_searchRequest);

            // Assert
            Assert.Equal(_searchRequest.Initiator, result[0].Initiator);
      
        }

        [Fact]
        public async Task GetCountRequestTestAsync()
        {
            var expected = 50;
            _mockRepo.Setup(mr => mr.GetCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.GetCountAsync();

            // Assert
            Assert.Equal(expected, result);
        }
        
        [Fact]
        public async Task GetCountRequestTestReturnsZerowhenThereIsNoRequestAsync()
        {
            var expected = 0;
            _mockRepo.Setup(mr => mr.GetCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

            // Act
            var repository = _mockRepo.Object;
            var result = await repository.GetCountAsync();

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
