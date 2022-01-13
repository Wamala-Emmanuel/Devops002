using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.Services
{
    public interface IZipService
    {

        Task DeleteDownloadedZipFileAsync(Guid requestId);

        Task DeleteRequestExportAsync();
        
        Task<byte[]> GetZipFileBytesAsync(string filePath);

        Task ZipFileAsync(Guid requestId);

    }
}
