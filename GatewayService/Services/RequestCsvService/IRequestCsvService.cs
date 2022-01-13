using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.Services
{
    public interface IRequestCsvService
    {
        Task WriteToCsvFileAsync(Guid requestId);
    }
}
