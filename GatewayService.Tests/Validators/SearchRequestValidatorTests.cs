using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.TestHelper;
using GatewayService.DTOs;
using GatewayService.Validators;
using Xunit;

namespace GatewayService.Tests.Validators
{
    public class SearchRequestValidatorTests
    {
        private readonly SearchRequestValidator _validator;
        public SearchRequestValidatorTests()
        {
            _validator = new SearchRequestValidator();
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenToDateLessThanFromDate()
        {
            var request = new SearchRequest
            {
                Date = new DateRange
                {
                    From = new DateTime(2020, 07, 29),
                    To = new DateTime(2020, 07, 24)
                }
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Date.To!.Value)
                .WithSeverity(Severity.Error)
                .WithErrorCode("SearchRequest.DateTo.DateToLessThanDateFrom");
        }
        
        [Fact]
        public void Validator_ShouldHaveNoErrorWhenToDateIsEqualToFromDate()
        {
            var request = new SearchRequest
            {
                Date = new DateRange
                {
                    From = new DateTime(2020, 07, 24),
                    To = new DateTime(2020, 07, 24)
                }
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(x => x.Date.To);
        }
        
        [Fact]
        public void Validator_ShouldHaveNoErrorWhenToDateIsGreaterThanFromDate()
        {
            var request = new SearchRequest
            {
                Date = new DateRange
                {
                    From = new DateTime(2020, 07, 24),
                    To = new DateTime(2020, 07, 29)
                }
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(x => x.Date.To);
        }
    }
}
