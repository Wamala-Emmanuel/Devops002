using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.Services
{
    public interface IDirectoryService
    {
        public void CreateTempFile(string folderPath, string fileName, string requestId, int buffer);

        public bool FileExists(string fileName);

        public void RenameFile(string oldPath, string newPath);

        public void DeleteDirectory(string folderName);

        public void DeleteFile(string fileName);

    }
}
