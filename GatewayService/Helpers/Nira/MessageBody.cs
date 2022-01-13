using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Xml;

namespace GatewayService.Helpers.Nira
{
    public class MessageBody : Message
    {
        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public override MessageHeaders Headers { get; }
        public override MessageProperties Properties { get; }
        public override MessageVersion Version { get; }
    }
}
