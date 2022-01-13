using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.Helpers
{
    public static class Guard
    {
        /// <summary>
        /// Throws a client friendly exception when a string is empty, null or whitespace 
        /// </summary>
        /// <param name="argumentValue"></param>
        /// <param name="message"></param>
        public static void ThrowIfNullOrEmpty(string argumentValue, string message)
        {
            if (string.IsNullOrWhiteSpace(argumentValue)) throw new ClientFriendlyException(message);
        }

        public static void ThrowIfNullOrEmpty(int? argumentValue, string message)
        {
            if (argumentValue is null) throw new ClientFriendlyException(message);
        }

        public static void ThrowIfSubstringLengthIsGreaterThanStringLength(string str, int subStringLength, string message)
        {
            if (subStringLength > str.Length) throw new ClientFriendlyException(message);
        }
    }
}
