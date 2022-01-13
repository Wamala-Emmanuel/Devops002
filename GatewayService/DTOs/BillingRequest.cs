using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.DTOs
{
    public class BillingRequest
    {
        /// <summary>
        /// Request ID
        /// </summary>
        public Guid ReferenceId { get; set; }

        /// <summary>
        /// ParticipantId
        /// </summary>
        public Guid? CompanyId { get; set; }

        /// <summary>
        /// Nira Request Details
        /// </summary>
        public NiraMetaData MetaData { get; set; }
    }
}
