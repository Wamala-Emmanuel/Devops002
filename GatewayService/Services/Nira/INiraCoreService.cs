using System;
using System.Threading.Tasks;
using GatewayService.Models;
using NiraWebService;

namespace GatewayService.Services.Nira
{
    public interface INiraCoreService
    {
        Task<ChangePasswordResponse> ChangePasswordAsync(
            string username, string password, changePasswordRequest request);
        Task<PersonInfoVerificationResponse> VerifyPersonInformation(
            string username, string password, verifyPersonInformationRequest request);
    }
}
