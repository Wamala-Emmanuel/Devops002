using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using GatewayService.DTOs;
using GatewayService.Services;
using GatewayService.Services.Nita.NitaCredentialService;
using Microsoft.Extensions.Options;

namespace GatewayService.Validators
{
    public class NationalIdVerificationRequestValidator : AbstractValidator<NationalIdVerificationRequest>
    {
        private readonly ICredentialService _credentialService;
        private readonly INitaCredentialService _nitaCredentialService;
        private readonly IOptions<NiraSettings> _niraOptions;
        private readonly IOptions<VerificationSettings> _verificationOptions;

        public NationalIdVerificationRequestValidator(IOptions<NiraSettings> niraSettings, 
            ICredentialService credentialService, INitaCredentialService nitaCredentialService,
            IOptions<NiraSettings> niraOptions, IOptions<VerificationSettings> verificationOptions)
        {
            _credentialService = credentialService;
            _nitaCredentialService = nitaCredentialService;
            _niraOptions = niraOptions;
            _verificationOptions = verificationOptions;

            RuleFor(n => n.Nin).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithErrorCode("NationalIdVerificationRequest.Nin.NullOrEmpty")
                .Length(14)
                .WithErrorCode("NationalIdVerificationRequest.Nin.ExactLengthValidator");

            RuleFor(n => n.CardNumber).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithErrorCode("NationalIdVerificationRequest.CardNumber.NullOrEmpty")
                .Length(9)
                .WithErrorCode("NationalIdVerificationRequest.CardNumber.ExactLengthValidator"); 

            RuleFor(n => n.Surname)
                .NotEmpty()
                .When(n => string.IsNullOrEmpty(n.GivenNames))
                .When(n => !n.DateOfBirth.HasValue)
                .WithMessage($"Include atleast given names or the date of birth")
                .WithErrorCode("NationalIdVerificationRequest.OptionalFields.NullOrEmpty");
            
            RuleFor(n => n.GivenNames)
                .NotEmpty()
                .When(n => string.IsNullOrEmpty(n.Surname))
                .When(n => !n.DateOfBirth.HasValue)
                .WithMessage($"Include atleast a surname or the date of birth")
                .WithErrorCode("NationalIdVerificationRequest.OptionalFields.NullOrEmpty");
            
            RuleFor(n => n.DateOfBirth)
                .NotEmpty()
                .When(n => string.IsNullOrEmpty(n.Surname))
                .When(n => string.IsNullOrEmpty(n.GivenNames))
                .WithMessage($"Include atleast a surname or given names")
                .WithErrorCode("NationalIdVerificationRequest.OptionalFields.NullOrEmpty");

            //make check for db credentials
            When(request => niraSettings.Value.CredentialConfig.UseDatabaseCredentials, () =>
            {
                RuleFor(x => x.Nin)
                    .MustAsync((r, c) => _credentialService.AreDatabaseCredentialsSet())
                    .WithMessage(
                        $"NIRA credentials have not been set or may have expired. Please contact your system administrator.")
                    .WithErrorCode(
                        "NationalIdVerificationRequest.Base.DatabaseCredentialsHaveNotBeenSet");
            });

            //make check for config credentials
            When(request => !niraSettings.Value.CredentialConfig.UseDatabaseCredentials, () =>
            {
                RuleFor(x => x.Nin)
                    .Must((r) => _credentialService.AreConfigCredentialsSet())
                    .WithMessage(
                        $"NIRA Credentials have not been configured in the application. Please contact your system administrator.")
                    .WithErrorCode(
                        "NationalIdVerificationRequest.Base.ConfigCredentialsHaveNotBeenSet");
            });

            //make check for whether current credentials are valid
            RuleFor(x => x.Nin)
                .MustAsync(async (r, cancellation) =>
                {
                    var credentials = await _credentialService.GetCurrentCredentialsAsync();
                    
                    bool isExceeded = credentials.ExpiresOn.HasValue && DateTime.Now > credentials.ExpiresOn.Value.AddDays(-_niraOptions.Value.CredentialConfig.PasswordExpirationDays);
                    return !isExceeded;
                })
                .WithMessage(x => "NIRA credentials are expired. Please contact your system administrator.")
                .WithErrorCode("NationalIdVerificationRequest.Base.ExpiredCredentials");

            //make check for NITA credentials
            When(request => _verificationOptions.Value.ConnectionType == ConnectionType.Nita, () =>
            {
                RuleFor(x => x.Nin)
                    .MustAsync((r, c) => _nitaCredentialService.AreNitaCredentialsSet())
                    .WithMessage(
                        "NITA client credentials have not been set. Please contact your system administrator.")
                    .WithErrorCode(
                        "NationalIdVerificationRequest.Base.NitaCredentialsHaveNotBeenSet");
            });
        }
    }
}
