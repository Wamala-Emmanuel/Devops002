using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using GatewayService.Helpers;
using GatewayService.Helpers.Nira;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NiraWebService;
using static GatewayService.Services.Nira.NiraRequest;

namespace GatewayService.Services.Nira.Xml
{
    public class NiraXmlService : INiraXmlService
    {
        private readonly string _certificatePath;
        private readonly string _culture;
        private readonly double _offset;
        private readonly ILogger<NiraXmlService> _logger;

        public NiraXmlService(IConfiguration configuration, ILogger<NiraXmlService> logger)
        {
            var settings = configuration.GetNiraSettings();
            _offset = settings.NiraDateTimeConfig.Offset;
            _culture = settings.NiraDateTimeConfig.Culture;
            _certificatePath = settings.CredentialConfig.CertificatePath;
            _logger = logger;
        }

        /// <summary>
        /// Prepare verifyPersonInformation xml request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string PrepareXmlRequest(string username, string password, verifyPersonInformationRequest request)
        {
            var userNameToken = NiraUtils.SetUsernameToken(username, password, _offset, _culture);

            var requestObject = new Envelope
            {
                Fac = "http://facade.server.pilatus.thirdparty.tidis.muehlbauer.de/",
                Wsse = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd",
                Soapenv = "http://schemas.xmlsoap.org/soap/envelope/",
                Header = PrepareEnvelopeHeader(userNameToken),

                Body = new Body
                {
                    VerifyPersonInformation = new VerifyPersonInformation
                    {
                        Request = new Request
                        {
                            DocumentId = request.documentId,
                            DateOfBirth = request.dateOfBirth,
                            GivenNames = request.givenNames,
                            Surname = request.surname,
                            OtherNames = request.otherNames,
                            NationalId = request.nationalId
                        }
                    }
                }
            };

            var namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty); // remove additional attributes

            var serializer = new XmlSerializer(requestObject.GetType());

            return WriteXmlDocument(requestObject, serializer, namespaces);
        }

        /// <summary>
        /// Serialize change password xml request
        /// </summary>
        /// <param name="request"></param>
        /// 
        /// <returns></returns>        
        public string PrepareXmlRequest(string username, string password, changePasswordRequest request)
        {
            var userNameToken = NiraUtils.SetUsernameToken(username, password, _offset, _culture);
            var certificate = NiraUtils.GetEncryptionCertificate(_certificatePath);
            var encryptedPassword = NiraUtils.EncryptWithRSA(certificate, request.newPassword);
            var base64Password = encryptedPassword.ToBase64();

            var requestObject = new PasswordEnvelope
            {
                Fac = "http://facade.server.pilatus.thirdparty.tidis.muehlbauer.de/",
                Wsse = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd",
                Soapenv = "http://schemas.xmlsoap.org/soap/envelope/",
                Header = PrepareEnvelopeHeader(userNameToken),

                PasswordBody = new PasswordBody
                {
                    ChangePassword = new ChangePassword
                    {
                        PasswordRequest = new PasswordRequest
                        {
                            NewPassword = base64Password
                        }
                    }
                }
            };

            var namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty); // remove additional attributes

            var serializer = new XmlSerializer(requestObject.GetType());

            return WriteXmlDocument(requestObject, serializer, namespaces);
        }

        /// <summary>
        /// Sets the password digest, nonce and userneme details for the soap envelope header
        /// </summary>
        /// <param name="usernameToken"></param>
        /// <returns></returns>
        private Header PrepareEnvelopeHeader(NiraUtils.UsernameToken usernameToken)
        {
            _logger.LogInformation(usernameToken.Created);
            return new Header
            {
                UsernameToken = new UsernameToken
                {
                    Nonce = usernameToken.Nonce,
                    Password = new Password
                    {
                        Type = "PasswordDigest",
                        Text = usernameToken.Password
                    },
                    Username = usernameToken.Username,
                    Created = usernameToken.Created
                }
            };
        }

        /// <summary>
        /// Deserialize verify person information xml response
        /// </summary>
        /// <param name="responseString"></param>
        /// <returns></returns>
        public PersonInfoVerificationResponse PrepareVerifyPersonInfoResponse(string responseString)
        {
            var response = new PersonInfoVerificationResponse();

            var responseXmlDocument = new XmlDocument();

            responseXmlDocument.LoadXml(responseString);

            var innerXml = responseXmlDocument
                .GetElementsByTagName("return")[0].ChildNodes;

            var transactionStatus = innerXml.Item(0);

            response.Status = transactionStatus.ChildNodes.Item(0).InnerText;

            if (response.Status == "Error")
            {
                var error = transactionStatus.ChildNodes.Count == 2 ? transactionStatus.ChildNodes.Item(1).ChildNodes : transactionStatus.ChildNodes.Item(2).ChildNodes;

                response.PasswordDaysLeft = transactionStatus.ChildNodes.Count == 3 ? transactionStatus.ChildNodes.Item(1).InnerText : string.Empty;

                response.Error = new ResponseError
                {
                    Code = error.Item(0).InnerText,
                    Message = error.Item(1).InnerText
                };
            }
            else
            {
                response.PasswordDaysLeft = transactionStatus.ChildNodes.Item(1).InnerText;
                response.ExecutionCost = transactionStatus.ChildNodes.Item(2).InnerText;

                response.MatchingStatus = innerXml.Item(1).InnerText;
                response.CardStatus = innerXml.Count <= 2 ? string.Empty : innerXml.Item(2).InnerText;

                response.IsError = false;
            }

            return response;
        }

        /// <summary>
        /// Deserialize change password xml response
        /// </summary>
        /// <param name="responseString"></param>
        /// <returns></returns>
        public ChangePasswordResponse PrepareChangePasswordResponse(string responseString)
        {
            var response = new ChangePasswordResponse();

            var responseXmlDocument = new XmlDocument();
            responseXmlDocument.LoadXml(responseString);

            var innerXml = responseXmlDocument
                .GetElementsByTagName("return")[0].ChildNodes;

            var transactionStatus = innerXml.Item(0);

            response.Status = transactionStatus.ChildNodes.Item(0).InnerText;

            if (response.Status == "Error")
            {
                var error = transactionStatus.ChildNodes.Count == 2 ? transactionStatus.ChildNodes.Item(1).ChildNodes : transactionStatus.ChildNodes.Item(2).ChildNodes;

                response.PasswordDaysLeft = transactionStatus.ChildNodes.Count == 3 ? transactionStatus.ChildNodes.Item(1).InnerText : string.Empty; ;

                response.Error = new ResponseError
                {
                    Code = error.Item(0).InnerText,
                    Message = error.Item(1).InnerText
                };
            }
            else
            {
                response.PasswordDaysLeft = transactionStatus.ChildNodes.Item(1).InnerText;
                response.ExecutionCost = transactionStatus.ChildNodes.Item(2).InnerText;

                response.Status = transactionStatus.ChildNodes.Item(0).InnerText;

                response.IsError = false;
            }

            return response;
        }

        /// <summary>
        /// Writes a xml document based on the verify person envelope
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="serializer"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        private string WriteXmlDocument(Envelope envelope, XmlSerializer serializer, XmlSerializerNamespaces namespaces)
        {
            string xml;
            var stringBuilder = new StringBuilder();
            using var writer = new StringWriterUtf8(stringBuilder);
            try
            {
                using var xmlWriter = XmlWriter.Create(writer);
                serializer.Serialize(xmlWriter, envelope, namespaces);

                xml = writer.ToString();
                xml = xml.Replace("Envelope", "soapenv:Envelope");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to serialize nin verification request xml");
                throw ex;
            }

            _logger.LogInformation("Serialized successfully request xml");

            _logger.LogDebug(xml);

            return xml;
        }

        /// <summary>
        /// Writes a xml document based on the new password envelope
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="serializer"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        private string WriteXmlDocument(PasswordEnvelope envelope, XmlSerializer serializer, XmlSerializerNamespaces namespaces)
        {
            string xml;
            var stringBuilder = new StringBuilder();
            using var writer = new StringWriterUtf8(stringBuilder);
            try
            {
                using var xmlWriter = XmlWriter.Create(writer);
                serializer.Serialize(xmlWriter, envelope, namespaces);

                xml = writer.ToString();
                xml = xml.Replace("PasswordEnvelope", "soapenv:Envelope");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to serialize new password request xml");
                throw ex;
            }
            
            _logger.LogDebug(xml);

            _logger.LogInformation("Serialized successfully request xml");
            
            return xml;
        }

    }
}
