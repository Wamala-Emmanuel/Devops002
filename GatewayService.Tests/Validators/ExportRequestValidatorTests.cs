using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using GatewayService.DTOs;
using GatewayService.Repositories.Contracts;
using GatewayService.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GatewayService.Tests.Validators
{
    public class ExportRequestValidatorTests
    {
        private readonly ExportRequestValidator _validator;
        private readonly Mock<IRequestRepository> _mockRepository;
        private ExportSettings _exportSettings = new ExportSettings
        {
            DelayInHours = 2,
            DaysBack = 3,
            Buffer = 1024,
            FolderPath = "folder",
            PageSize = 10,
            RequestLimit = 1000000
        };

        public ExportRequestValidatorTests()
        {
            _mockRepository = new Mock<IRequestRepository>();

            var exportOptionsMock = new Mock<IOptions<ExportSettings>>();

            exportOptionsMock.Setup(x => x.Value)
                .Returns(_exportSettings);

            _validator = new ExportRequestValidator(_mockRepository.Object, exportOptionsMock.Object);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenDateRangeIsNotIncluded()
        {
            var model = new ExportRequest
            {
                Nin = "CM0588012VTHUE",
                CardNumber = "426463810",
            };

            var result = _validator.Validate(model);

            Assert.False(result.IsValid);
            Assert.Contains("DateRange", result.Errors.Select(x => x.PropertyName));
            Assert.Contains("NotEmptyValidator", result.Errors.Select(x => x.ErrorCode));
        }
        
        [Fact]
        public void Validator_ShouldHaveErrorWhenRequestStatusIsInvalid()
        {
            var model = new ExportRequest
            {
                DateRange = new DateFilter
                {
                    From = new DateTime(2020, 01, 25),
                    To = DateTime.Now
                },
                RequestStatus = new List<string> 
                { 
                    "Processing",
                },
            };

            var result = _validator.Validate(model);

            Assert.False(result.IsValid);
            Assert.Contains("RequestStatus[0]", result.Errors.Select(x => x.PropertyName));
            Assert.Contains("StringEnumValidator", result.Errors.Select(x => x.ErrorCode));
        }

        [Fact]
        public void Validator_ShouldHaveNoErrorWhenRequestIsStatusValid()
        {
            var model = new ExportRequest
            {
                DateRange = new DateFilter
                {
                    From = new DateTime(2020, 01, 25),
                    To = DateTime.Now
                },
                RequestStatus = new List<string> 
                { 
                    "Pending",
                },
            };

            var expected = 0;

            var result = _validator.Validate(model);

            Assert.True(result.IsValid);
            Assert.Equal(expected, result.Errors.Count);
        }
        
        
        [Fact]
        public void Validator_ShouldHaveErrorWhenExportHasMoreOneMillionRequests()
        {
            var model = new ExportRequest
            {
                DateRange = new DateFilter
                {
                    From = new DateTime(2020, 01, 25),
                    To = DateTime.Now
                }
            };

            var count = 2000000;
            var requestLimit = 1000000;

            _mockRepository.Setup(mr => mr.GetExportRequestCountAsync(model, It.IsAny<CancellationToken>())).ReturnsAsync(count);

            var result = _validator.Validate(model);

            Assert.False(result.IsValid);
            Assert.Contains("ExportRequest.MaximumLimitExceeded", result.Errors.Select(x => x.ErrorCode));
            Assert.Contains(
                $"The export request exceeds the maximum {requestLimit} for exporting as configured.", result.Errors.Select(x => x.ErrorMessage));
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenNinIsAddedButInvalid()
        {
            var model = new ExportRequest
            {
                DateRange = new DateFilter
                {
                    From = new DateTime(2020, 01, 25),
                    To = DateTime.Now
                },
                Nin = "CF67"
            };

            var result = _validator.Validate(model);

            Assert.False(result.IsValid);
            Assert.Contains("Nin", result.Errors.Select(x => x.PropertyName));
            Assert.Contains("ExportRequest.Nin.ExactLengthValidator", result.Errors.Select(x => x.ErrorCode));
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenCardNumberIsAddedButInvalid()
        {
            var model = new ExportRequest
            {
                DateRange = new DateFilter
                {
                    From = new DateTime(2020, 01, 25),
                    To = DateTime.Now
                },
                CardNumber = "00045"
            };

            var result = _validator.Validate(model);

            Assert.False(result.IsValid);
            Assert.Contains("CardNumber", result.Errors.Select(x => x.PropertyName));
            Assert.Contains("ExportRequest.CardNumber.ExactLengthValidator", result.Errors.Select(x => x.ErrorCode));
        }
    }
}
