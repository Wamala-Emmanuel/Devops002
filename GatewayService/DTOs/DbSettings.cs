using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.DTOs
{
    public class DbSettings
    {
        public const string ConfigurationName = "DbSettings";

        /// <summary>
        /// The maximum number of instances retained by the pool
        /// </summary>
        public int MaxPoolSize { get; set; }

        /// <summary>
        /// This enables one to set the number of maximum retries on the database connection
        /// </summary>
        public int RetryCount { get; set; }
    }
}
