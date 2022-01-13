using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.DTOs
{
    public class FileViewModel
    {
        public FileViewModel(byte[] contents, string contentType, string name)
        {
            Contents = contents;
            ContentType = contentType;
            Name = name;
        }

        public FileViewModel() { }

        public byte[] Contents { get; set; }

        public string ContentType { get; set; }

        public string Name { get; set; }
    }
}
