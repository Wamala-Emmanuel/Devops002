using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static GatewayService.Services.Nira.NiraRequest;

namespace GatewayService.Services.Nira
{
    public class ChangePasswordRequest : NiraRequest
    {
        [XmlElement(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public Envelope Envelope { get; set; }
    }

    public class PasswordRequest
    {
        [XmlElement(ElementName = "newPassword", Namespace = "")]
        public string NewPassword { get; set; }
    }

    [XmlRoot(ElementName = "changePassword",
        Namespace = "http://facade.server.pilatus.thirdparty.tidis.muehlbauer.de/")]
    public class ChangePassword
    {
        [XmlElement(ElementName = "request", Namespace = "")]
        public PasswordRequest PasswordRequest { get; set; }
    }

    [XmlRoot(ElementName = "Body",
        Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class PasswordBody
    {
        [XmlElement(ElementName = "changePassword",
            Namespace = "http://facade.server.pilatus.thirdparty.tidis.muehlbauer.de/")]
        public ChangePassword ChangePassword { get; set; }
    }

    public class PasswordEnvelope
    {
        [XmlElement(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public Header Header { get; set; }

        [XmlAttribute(AttributeName = "soapenv", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Soapenv { get; set; }

        [XmlAttribute(AttributeName = "fac", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Fac { get; set; }

        [XmlAttribute(AttributeName = "wsse", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Wsse { get; set; }

        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public PasswordBody PasswordBody { get; set; }
    }
}
