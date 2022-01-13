using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.Services
{
    public interface ICoreCsvService
    {
        public Task WriteRecordsToCsvFileAsync<T>(string fullPath, int page, List<T> records);
    }
}
