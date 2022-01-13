using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace GatewayService.DTOs
{
    public class VerificationSettings
    {
        public const string ConfigurationName = "VerificationSettings";

        public ConnectionType ConnectionType { get; set; }
    }

    /// <summary>
    /// The type of connection used for verifying person information
    /// </summary>
    [DataContract]
    public enum ConnectionType
    {
        /// <summary>
        /// The direct NIRA connection is used
        /// </summary>
        [EnumMember]
        Nira = 0,

        /// <summary>
        /// The NITA connection is used
        /// </summary>
        [EnumMember]
        Nita = 1,

    }
}
