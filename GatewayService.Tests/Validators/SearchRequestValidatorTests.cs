using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            _validator.ShouldHaveValidationErrorFor(r => r.Date.To!.Value, request)
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

            _validator.ShouldNotHaveValidationErrorFor(r => r.Date.To, request);
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

            _validator.ShouldNotHaveValidationErrorFor(r => r.Date.To, request);
        }
    }
}
