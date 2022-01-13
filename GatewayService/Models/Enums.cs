using System.Runtime.Serialization;

namespace GatewayService.Models
{
    /// <summary>
    /// The stage at which the request is at
    /// </summary>
    [DataContract]
    public enum RequestStatus
    {
        /// <summary>
        /// The request is still being processed
        /// </summary>
        [EnumMember]
        Pending = 1,

        /// <summary>
        /// The request was complete and successful
        /// </summary>
        [EnumMember]
        Completed = 2,

        /// <summary>
        /// The request failed while being processed
        /// </summary>
        [EnumMember]
        Failed = 3,
    }
    
    /// <summary>
    /// The stage at which the export request is at
    /// </summary>
    [DataContract]
    public enum ExportStatus
    {
        /// <summary>
        /// The request is still being processed
        /// </summary>
        [EnumMember]
        Processing = 1,

        /// <summary>
        /// The request was complete and successful
        /// </summary>
        [EnumMember]
        Complete = 2,

        /// <summary>
        /// The request failed while being processed
        /// </summary>
        [EnumMember]
        Failed = 3,
    }
}
