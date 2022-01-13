using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;
using GatewayService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GatewayService.DTOs
{
    public class ExportDto
    {
        public Guid Reference { get; set; }

        [Name("Received At")]
        public DateTime ReceivedAt { get; set; }
        public string User { get; set; }

        public string? Name { get; set; }

        [Name("NIN")]
        public string Nin { get; set; }
        
        [Name("Card Number")]
        public string CardNumber { get; set; }

#nullable enable
        [DataType(DataType.Date)]
        [Name("Date of Birth")]
        public string? DateOfBirth { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [Name("Request Status")]
        public RequestStatus RequestStatus { get; set; }

        [Name("NIN Validity")]
        public string? NinResponse { get; set; }

        [Name("Match Status")]
        public string? MatchStatus { get; set; }
        
        [Name("Card Validity")]
        public string? CardStatus { get; set; }

    }
}
