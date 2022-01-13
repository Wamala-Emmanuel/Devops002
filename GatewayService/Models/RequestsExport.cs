using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GatewayService.Models
{
    public class RequestsExport
    {
        [Key]
        public Guid Id { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ExportStatus GenerationStatus { get; set; }
        
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        
        public string? UserName { get; set; }

        public string Request { get; set; }

        public DateTime? DownloadedOn { get; set; }
    
        public string FileName { get; set; }
        
        public bool IsDeleted { get; set; }
    }
}
