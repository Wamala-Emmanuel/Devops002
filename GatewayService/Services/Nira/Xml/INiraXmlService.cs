using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NiraWebService;

namespace GatewayService.Services.Nira.Xml
{
    public interface INiraXmlService
    {
        string PrepareXmlRequest(string username, string password, verifyPersonInformationRequest request);

        string PrepareXmlRequest(string username, string password, changePasswordRequest request);

        PersonInfoVerificationResponse PrepareVerifyPersonInfoResponse(string resStr);

        ChangePasswordResponse PrepareChangePasswordResponse(string resStr);
    }
}
