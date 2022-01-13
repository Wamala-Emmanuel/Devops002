using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using GatewayService.DTOs;
using GatewayService.Helpers;
using GatewayService.Models;
using GatewayService.Repositories.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace GatewayService.Validators
{
    public class ExportRequestValidator : AbstractValidator<ExportRequest>
    {
        private readonly ExportSettings _exportSettings;

        public ExportRequestValidator(IRequestRepository repository, IOptions<ExportSettings> exportOptions)
        {
            _exportSettings = exportOptions.Value;

            RuleFor(r => r.DateRange)
                .NotEmpty()
                .DependentRules(() => {
                    RuleFor(r => r.DateRange.From)
                        .NotEmpty();
            
                    RuleFor(r => r.DateRange.To).Cascade(CascadeMode.Stop)
                        .NotEmpty()
                        .GreaterThan(r => r.DateRange.From)
                        .WithMessage("The start date '{ComparisonValue}' must be less than '{PropertyValue}'")
                        .WithErrorCode("ExportRequest.DateRange.To.DateRangeToLessThanDateRangeFrom");
                });
            
            RuleForEach(n => n.RequestStatus)
                .IsEnumName(typeof(RequestStatus), caseSensitive: false)
                .When(r => r.RequestStatus != null);

            RuleFor(r => r.Nin)
                .Cascade(CascadeMode.Stop)
               .Length(14)
               .When(r => !string.IsNullOrWhiteSpace(r.Nin))
               .WithErrorCode("ExportRequest.Nin.ExactLengthValidator");

            RuleFor(r => r.CardNumber)
                .Cascade(CascadeMode.Stop)
                .Length(9)
                .When(r => !string.IsNullOrWhiteSpace(r.CardNumber))
                .WithErrorCode("ExportRequest.CardNumber.ExactLengthValidator");
            
            RuleFor(r => r)
                .MustAsync(async (r, cancellation) =>
                {
                    int count = await repository.GetExportRequestCountAsync(r, cancellation);
                    bool isExceeded = count > _exportSettings.RequestLimit;
                    return !isExceeded;
                })
                .When(r => r.DateRange is not null)
                .WithMessage(x => $"The export request exceeds the maximum {_exportSettings.RequestLimit} for exporting as configured.")
                .WithErrorCode("ExportRequest.MaximumLimitExceeded");

        }
    }
}
