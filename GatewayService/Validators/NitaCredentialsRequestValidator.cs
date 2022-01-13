using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using GatewayService.DTOs.NitaCredentials;

namespace GatewayService.Validators
{
    public class NitaCredentialsRequestValidator : AbstractValidator<NitaCredentialRequest>
    {
        public NitaCredentialsRequestValidator()
        {
            RuleFor(c => c.ClientKey)
                .NotEmpty()
                .WithErrorCode("NitaCredentialRequest.ClientKey.NullOrEmpty");

            RuleFor(c => c.ClientSecret)
                .NotEmpty()
                .WithErrorCode("NitaCredentialRequest.ClientSecret.NullOrEmpty");
        }
    }
}
