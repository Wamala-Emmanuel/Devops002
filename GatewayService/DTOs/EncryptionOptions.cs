using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayService.DTOs
{
    public class EncryptionOptions
    {
        public const string SectionName = "EncryptionOptions";

        public string Key { get; set; } = "6fcb6050650a435780f3420b158b7001";

        public byte[] InitializationVector => new byte[] {26, 19, 18, 90, 117, 17, 87, 43, 24, 103, 11, 44, 18, 113, 93, 14};

        public byte[] KeyBytes => Encoding.Default.GetBytes(Key);
    }
}
