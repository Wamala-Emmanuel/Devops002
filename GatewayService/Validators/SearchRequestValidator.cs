using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using GatewayService.DTOs;

namespace GatewayService.Validators
{
    public class SearchRequestValidator : AbstractValidator<SearchRequest>
    {
        public SearchRequestValidator()
        {
            RuleFor(r => r.Date.To!.Value )
                .GreaterThanOrEqualTo(r => r.Date.From!.Value)
                .WithMessage("The start date '{ComparisonValue}' must be equal or after '{PropertyValue}'")
                .WithErrorCode("SearchRequest.DateTo.DateToLessThanDateFrom")
                .When(r => r.Date != null && r.Date.To.HasValue && r.Date.From.HasValue);
        }
    }
}
