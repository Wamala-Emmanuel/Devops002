using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;

namespace GatewayService.Services
{
    public class BackgroundJobWrapper : IBackgroundJobWrapper
    {
        public bool DeleteJob(string jobId)
        {
            return BackgroundJob.Delete(jobId);
        }
    }
}
