using System;

namespace GatewayService.Helpers
{
    public class ClientFriendlyException : Exception
    {
        public ClientFriendlyException(string message) : base(message) { }
    }

    public class NotFoundException : ClientFriendlyException
    {
        public NotFoundException(string message) : base(message) { }
    }
}
