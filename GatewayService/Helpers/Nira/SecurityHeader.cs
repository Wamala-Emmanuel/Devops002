using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Configuration;

namespace GatewayService.Helpers.Nira
{
    public class SecurityHeader : MessageHeader
    {
        private readonly string _username;
        private readonly string _password;
        private readonly string _culture;
        private readonly double _offset;

        /// <summary>
        /// XML security header for soap envelope
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="configuration"></param>
        public SecurityHeader(string username, string password, IConfiguration configuration)
        {
            var settings = configuration.GetNiraSettings();

            _username = username;
            _password = password;
            _offset = settings.NiraDateTimeConfig.Offset;
            _culture = settings.NiraDateTimeConfig.Culture;
        }

        [XmlRoot(ElementName = "Password")]
        public class Password
        {
            [XmlAttribute(AttributeName = "Type")] 
            public string? Type { get; set; }

            [XmlText] public string? Text { get; set; }
        }

        [XmlRoot(ElementName = "Nonce")]
        public class Nonce
        {
            [XmlAttribute(AttributeName = "EncodingType")]
            public string? EncodingType { get; set; }

            [XmlText] public string? Text { get; set; }
        }

        [XmlRoot(ElementName = "wsse:UsernameToken")]
        public class UsernameToken
        {
            [XmlElement(ElementName = "Username")]
            public string? Username { get; set; }

            [XmlElement(ElementName = "Password")]
            public Password? Password { get; set; }

            [XmlElement(ElementName = "Nonce")]
            public Nonce? Nonce { get; set; }

            [XmlElement(ElementName = "Created")]
            public string? Created { get; set; }
        }

        public override string Name { get; } = "soapenv:Header";
        public override string Namespace { get; }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            var serializer = new XmlSerializer(typeof(UsernameToken));

            var authToken = NiraUtils.SetUsernameToken(_username, _password, _offset, _culture);

            serializer.Serialize(writer, new UsernameToken
            {
                Username = authToken.Username,
                Password = new Password
                {
                    Type = "PasswordDigest",
                    Text = authToken.Password
                },
                Nonce = new Nonce
                {
                    Text = authToken.Nonce
                },
                Created = authToken.Created
            });
        }

    }
}
