using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GatewayService.Services.Nira
{
    public class NiraRequest
    {
        [XmlRoot(ElementName = "Password",
        Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
        public class Password
        {
            [XmlAttribute(AttributeName = "Type")]
            public string Type { get; set; }

            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "UsernameToken",
            Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
        public class UsernameToken
        {
            [XmlElement(ElementName = "Username",
                Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
            public string Username { get; set; }

            [XmlElement(ElementName = "Password",
                Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
            public Password Password { get; set; }

            [XmlElement(ElementName = "Nonce",
                Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
            public string Nonce { get; set; }

            [XmlElement(ElementName = "Created",
                Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
            public string Created { get; set; }
        }


        [XmlRoot(ElementName = "Header",
            Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public class Header
        {
            [XmlElement(ElementName = "UsernameToken",
                Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
            public UsernameToken UsernameToken { get; set; }
        }

    }
}
