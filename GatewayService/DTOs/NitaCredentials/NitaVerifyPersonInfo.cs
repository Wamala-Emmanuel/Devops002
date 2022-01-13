using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.DTOs.NitaCredentials
{
    public class NitaVerifyPersonInfo
    {
        public string DateOfBirth { get; set; }

        public string DocumentId { get; set; }

        public string GivenNames { get; set; }

        public string NationalId { get; set; }

        public string OtherNames { get; set; }

        public string Surname { get; set; }
    }
}
