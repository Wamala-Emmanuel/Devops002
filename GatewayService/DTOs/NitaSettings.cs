using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.DTOs
{
    public class NitaSettings
    {
        public const string ConfigurationName = "NitaConfig";

        /// <summary>
        /// version of the NITA API
        /// </summary>
        public string ApiVersion { get; set; }

        /// <summary>
        /// The host url of NITA
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Various url path segments of the NITA endpoints
        /// </summary>
        public Segments Segments { get; set; }

        /// <summary>
        /// Datetime offest in hours
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        ///  set culture that used in datetime
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        /// Where the key certifcate is located
        /// </summary>
        public string CertificatePath { get; set; }

        /// <summary>
        /// The number of requests allowed for the NITA API per minute
        /// </summary>
        public int RateLimit { get; set; }
    }

    public class Segments
    {
        public string EnvironmentSegment { get; set; }
        public string TokenSegment { get; set; }
        public string GetPersonSegment { get; set; }
        public string GetIdCardSegment { get; set; }
        public string GetPlaceOfResidenceSegment { get; set; }
        public string GetPlaceOfBirthSegment { get; set; }
        public string GetPlaceofOriginSegment { get; set; }
        public string GetVoterDetailsSegment { get; set; }
        public string VerifyPersonSegment { get; set; }
        public string IdentifyPersonSegment { get; set; }
        public string IdentifyPersonFullSearchSegment { get; set; }
        public string GetApplicationStatusSegment { get; set; }
        public string GetSpousesSegment { get; set; }
        public string GetParentsSegment { get; set; }
        public string VerifyPersonInformationSegment { get; set; }
        public string CheckAccountSegment { get; set; }
        public string ChangePasswordSegment { get; set; }
    }
}
