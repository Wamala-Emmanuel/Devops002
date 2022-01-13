using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static GatewayService.Services.Nira.NiraRequest;

namespace GatewayService.Services.Nira
{
    public class PersonInfoVerificationRequest : NiraRequest
    {
        [XmlElement(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public Envelope Envelope { get; set; }
    }

    public class Request
    {
        [XmlElement(ElementName = "dateOfBirth", Namespace = "")]
        public string DateOfBirth { get; set; }

        [XmlElement(ElementName = "documentId", Namespace = "")]
        public string DocumentId { get; set; }

        [XmlElement(ElementName = "givenNames", Namespace = "")]
        public string GivenNames { get; set; }

        [XmlElement(ElementName = "nationalId", Namespace = "")]
        public string NationalId { get; set; }

        [XmlElement(ElementName = "otherNames", Namespace = "")]
        public string OtherNames { get; set; }

        [XmlElement(ElementName = "surname", Namespace = "")]
        public string Surname { get; set; }
    }

    [XmlRoot(ElementName = "verifyPersonInformation", 
        Namespace = "http://facade.server.pilatus.thirdparty.tidis.muehlbauer.de/")]
    public class VerifyPersonInformation
    {
        [XmlElement(ElementName = "request", Namespace = "")]
        public Request Request { get; set; }
    }

    [XmlRoot(ElementName = "Body", 
        Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Body
    {
        [XmlElement(ElementName = "verifyPersonInformation", 
            Namespace = "http://facade.server.pilatus.thirdparty.tidis.muehlbauer.de/")]
        public VerifyPersonInformation VerifyPersonInformation { get; set; }
    }

    public class Envelope
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
        public Body Body { get; set; }
    }
}
