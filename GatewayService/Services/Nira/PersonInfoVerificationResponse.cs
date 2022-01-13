using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GatewayService.Services.Nira
{
    public class PersonInfoVerificationResponse : NiraResponse
    {

        public string? MatchingStatus { get; set; }
        public string? CardStatus { get; set; }

        public PersonInfoVerificationResponse()
        {
            IsError = Status != "Ok";
        }
    }
}
