using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.Services.Nira;
using NiraWebService;

namespace GatewayService.Services.Nita.Json
{
    public interface INitaJsonService
    {
        Task<string> GetAccessToken();

        Task<PersonInfoVerificationResponse> MakeJsonRequest(string username, string password, verifyPersonInformationRequest request);

        Task<ChangePasswordResponse> MakeJsonRequest(string username, string password, changePasswordRequest request);
    }
}
