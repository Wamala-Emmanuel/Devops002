using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using GatewayService.DTOs;
using GatewayService.DTOs.Credentials;
using GatewayService.DTOs.NitaCredentials;
using GatewayService.Helpers;
using GatewayService.Helpers.Nira;
using GatewayService.Models;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using NiraWebService;

namespace GatewayService.Tests
{
    
    public class TestHelper
    {
        private static readonly string[] resultOptions = new[] { "Valid", "Invalid", "Pending" };
        private static readonly string[] statusOptions = new[] { "Ok", "Failed" };

        /// <summary>
        /// Generates a search request
        /// </summary>
        public static Request GetTestRequest()
        {
            var requestFaker = new Faker<Request>()
                .RuleFor(r => r.CardNumber, f => f.Random.Int(min: 100000000, max: 999999999).ToString())
                .RuleFor(r => r.DateOfBirth, f => f.Date.Past(18, new DateTime(2002, 1, 1, 0, 0, 0)).Date)
                .RuleFor(r => r.Surname, f => f.Name.FirstName())
                .RuleFor(r => r.GivenNames, f => f.Name.LastName())
                .RuleFor(n => n.Nin, f => $"C{f.PickRandom<Gender>()}{StringExtensions.GetRandomString(StringExtensions.randomSequence, 12)}")
                .RuleFor(r => r.RequestStatus, f => f.PickRandom<RequestStatus>())
                .RuleFor(r => r.Initiator, f => f.Name.FirstName())
                .RuleFor(r => r.Id, f => f.Random.Guid())
                .RuleFor(r => r.ParticipantId, f => f.Random.Guid())
                .RuleFor(r => r.ReceivedAt, f => f.Date.Past().ToUniversalTime())
                .RuleFor(r => r.ReceivedFromNira, f => f.Date.Past().ToUniversalTime())
                .RuleFor(r => r.SubmittedAt, f => f.Date.Past().ToUniversalTime());
            return requestFaker.Generate();
        }

        /// <summary>
        /// Generates a NationalIdVerificationRequest request
        /// </summary>
        public static NationalIdVerificationRequest GetTestVerificationRequest()
        {
            var requestFaker = new Faker<NationalIdVerificationRequest>()
                .RuleFor(r => r.CardNumber, f => f.Random.Int(min: 100000000, max: 999999999).ToString())
                .RuleFor(r => r.DateOfBirth, f => f.Date.Past(18, new DateTime(2002, 1, 1, 0, 0, 0)))
                .RuleFor(r => r.Surname, f => f.Name.FirstName())
                .RuleFor(r => r.GivenNames, f => f.Name.LastName())
                .RuleFor(n => n.Nin, f => $"C{f.PickRandom<Gender>()}{StringExtensions.GetRandomString(StringExtensions.randomSequence, 12)}");
            return requestFaker.Generate();
        }

        /// <summary>
        /// Generates a nira response 
        /// </summary>
        public static VerificationResult GetTestNiraResult()
        {
            var resultOptions = new[] { "Valid", "Invalid", "Pending" };
            var statusOptions = new[] { "Ok", "Error" };
         
            var transactionStatus = new Faker<TransactionStatus>()
                .RuleFor(n => n.Status, f => f.Random.ArrayElement(statusOptions))
                .RuleFor(n => n.PasswordDaysLeft, f => f.Random.Number(min: 0, max: 180))
                .RuleFor(n => n.ExecutionCost, f => Convert.ToDouble(f.Commerce.Price(min: 0, max: 2000, decimals: 1)))
                .Generate();

            var niraResult = new Faker<VerificationResult>()
                .RuleFor(n => n.MatchingStatus, f => f.Random.Bool())
                .RuleFor(n => n.CardStatus, f => f.Random.ArrayElement(resultOptions))
                .RuleFor(n => n.Status, f => f.Random.ArrayElement(statusOptions))
                .RuleFor(n => n.PasswordDaysLeft, f => f.Random.Number(min: 0, max: 180))
                .RuleFor(n => n.ExecutionCost, f => Convert.ToDouble(f.Commerce.Price(min: 0, max: 2000, decimals: 1)));

            return niraResult.Generate();
        }
        
        public static RenewPasswordRequest GetTestRenewPasswordRequest()
        {
            return new RenewPasswordRequest
            {
                NewPassword = "12abMK"
            };
        }

        public static List<Request> GetTestPagedRequests(SearchRequest request)
        {
            var totalItems = request.Id != null ? 1 : new Faker().Random.Number(request.Pagination?.ItemsPerPage ?? 50, 1000);

            var transactionStatus = new Faker<TransactionStatus>()
                .RuleFor(n => n.Status, f => f.Random.ArrayElement(statusOptions))
                .RuleFor(n => n.PasswordDaysLeft, f => f.Random.Number(min: 0, max: 180))
                .RuleFor(n => n.ExecutionCost, f => Convert.ToDouble(f.Commerce.Price(min: 0, max: 2000, decimals: 1)));

            var niraResult = new Faker<VerificationResultViewModel>()
                .RuleFor(n => n.MatchingStatus, f => f.Random.Bool())
                .RuleFor(n => n.CardStatus, f => f.Random.ArrayElement(resultOptions))
                .RuleFor(n => n.Status, f => f.Random.ArrayElement(statusOptions))
                //.RuleFor(n => n.PasswordDaysLeft, f => f.Random.Number(min: 0, max: 180))
                //.RuleFor(n => n.ExecutionCost, f => Convert.ToDouble(f.Commerce.Price(min: 0, max: 2000, decimals: 1)))
                ;
            
            var requestDetails = new Faker<Request>()
                .RuleFor(r => r.CardNumber, f => request.CardNumber ?? f.Random.Int(min: 100000000, max: 999999999).ToString())
                .RuleFor(r => r.DateOfBirth, f => f.Date.Past(18, new DateTime(2002, 1, 1, 0, 0, 0)).Date)
                .RuleFor(r => r.Surname, f => request.Surname ?? f.Name.FirstName())
                .RuleFor(r => r.GivenNames, f => request.GivenNames ?? f.Name.LastName())
                .RuleFor(n => n.Nin, f => request.Nin ?? $"C{f.PickRandom<Gender>()}{StringExtensions.GetRandomString(StringExtensions.randomSequence, 12)}")
                .RuleFor(r => r.RequestStatus, f => f.PickRandom<RequestStatus>())
                .RuleFor(r => r.Initiator, f => request.Initiator)
                .RuleFor(r => r.Id, f => request.Id ?? f.Random.Guid())
                .RuleFor(r => r.ParticipantId, f => f.Random.Guid())
                .RuleFor(r => r.ReceivedAt, f => f.Date.Past().ToUniversalTime())
                .RuleFor(r => r.ReceivedFromNira, f => f.Date.Past().ToUniversalTime())
                .RuleFor(r => r.SubmittedAt, f => f.Date.Past().ToUniversalTime());

            return request.Id != null ? requestDetails.Generate(1) : requestDetails.Generate(request.Pagination?.ItemsPerPage ?? 50);
        }

        public static List<RequestViewModel> GetTestRequestViewModel(int limit)
        {

            var niraResult = new Faker<VerificationResultViewModel>()
                .RuleFor(n => n.MatchingStatus, f => f.Random.Bool())
                .RuleFor(n => n.CardStatus, f => f.Random.ArrayElement(resultOptions))
                .RuleFor(n => n.Status, f => f.Random.ArrayElement(statusOptions))
                //.RuleFor(n => n.PasswordDaysLeft, f => f.Random.Number(min: 0, max: 180))
                //.RuleFor(n => n.ExecutionCost, f => Convert.ToDouble(f.Commerce.Price(min: 0, max: 2000, decimals: 1)))
                ;

            var requestDetails = new Faker<RequestViewModel>()
                .RuleFor(r => r.CardNumber, f => f.Random.Int(min: 100000000, max: 999999999).ToString())
                .RuleFor(r => r.DateOfBirth, f => f.Date.Past(18, new DateTime(2002, 1, 1, 0, 0, 0)).Date)
                .RuleFor(r => r.Surname, f => f.Name.FirstName())
                .RuleFor(r => r.GivenNames, f => f.Name.LastName())
                .RuleFor(n => n.Nin, f => $"C{f.PickRandom<Gender>()}{StringExtensions.GetRandomString(StringExtensions.randomSequence, 12)}")
                .RuleFor(r => r.RequestStatus, f => f.PickRandom<RequestStatus>())
                .RuleFor(r => r.Initiator, f => f.Name.FirstName())
                .RuleFor(r => r.ResultJson, f => niraResult.Generate())
                .RuleFor(r => r.Id, f => f.Random.Guid())
                .RuleFor(r => r.ParticipantId, f => f.Random.Guid())
                .RuleFor(r => r.ReceivedAt, f => f.Date.Past().ToUniversalTime())
                .RuleFor(r => r.ReceivedFromNira, f => f.Date.Past().ToUniversalTime())
                .RuleFor(r => r.SubmittedAt, f => f.Date.Past().ToUniversalTime());

            return requestDetails.Generate(limit);
        }

        public static (List<RequestViewModel>, int totalItems) GetTestSearchResults(SearchRequest request)
        {
            var totalItems = request.Id != null ? 1 : new Faker().Random.Number(request.Pagination?.ItemsPerPage ?? 50, 1000);

            var transactionStatus = new Faker<TransactionStatus>()
                .RuleFor(n => n.Status, f => f.Random.ArrayElement(statusOptions))
                .RuleFor(n => n.PasswordDaysLeft, f => f.Random.Number(min: 0, max: 180))
                .RuleFor(n => n.ExecutionCost, f => Convert.ToDouble(f.Commerce.Price(min: 0, max: 2000, decimals: 1)));

            var niraResult = new Faker<VerificationResultViewModel>()
                .RuleFor(n => n.MatchingStatus, f => f.Random.Bool())
                .RuleFor(n => n.CardStatus, f => f.Random.ArrayElement(resultOptions))
                .RuleFor(n => n.Status, f => f.Random.ArrayElement(statusOptions))
                //.RuleFor(n => n.PasswordDaysLeft, f => f.Random.Number(min: 0, max: 180))
                //.RuleFor(n => n.ExecutionCost, f => Convert.ToDouble(f.Commerce.Price(min: 0, max: 2000, decimals: 1)))
                ;
            
            var requestDetails = new Faker<RequestViewModel>()
                .RuleFor(r => r.CardNumber, f => request.CardNumber ?? f.Random.Int(min: 100000000, max: 999999999).ToString())
                .RuleFor(r => r.DateOfBirth, f => f.Date.Past(18, new DateTime(2002, 1, 1, 0, 0, 0)).Date)
                .RuleFor(r => r.Surname, f => request.Surname ?? f.Name.FirstName())
                .RuleFor(r => r.GivenNames, f => request.GivenNames ?? f.Name.LastName())
                .RuleFor(n => n.Nin, f => request.Nin ?? $"C{f.PickRandom<Gender>()}{StringExtensions.GetRandomString(StringExtensions.randomSequence, 12)}")
                .RuleFor(r => r.RequestStatus, f => f.PickRandom<RequestStatus>())
                .RuleFor(r => r.Initiator, f => request.Initiator)
                .RuleFor(r => r.ResultJson, f => niraResult.Generate())
                .RuleFor(r => r.Id, f => request.Id ?? f.Random.Guid())
                .RuleFor(r => r.ParticipantId, f => f.Random.Guid())
                .RuleFor(r => r.ReceivedAt, f => f.Date.Past().ToUniversalTime())
                .RuleFor(r => r.ReceivedFromNira, f => f.Date.Past().ToUniversalTime())
                .RuleFor(r => r.SubmittedAt, f => f.Date.Past().ToUniversalTime());

            return request.Id != null ? (requestDetails.Generate(1), totalItems) : (requestDetails.Generate(request.Pagination?.ItemsPerPage ?? 50), totalItems);
        }

        public static SearchResponse GetTestSearchResponseResults(SearchRequest request)
        {
            var resultOptions = new[] { "Valid", "Invalid", "Pending" };
            var statusOptions = new[] { "Ok", "Failed" };
            var totalItems = request.Id != null ? 1 : new Faker().Random.Number(request.Pagination?.ItemsPerPage ?? 50, 1000);

            var transactionStatus = new Faker<TransactionStatus>()
                .RuleFor(n => n.Status, f => f.Random.ArrayElement(statusOptions))
                .RuleFor(n => n.PasswordDaysLeft, f => f.Random.Number(min: 0, max: 180))
                .RuleFor(n => n.ExecutionCost, f => Convert.ToDouble(f.Commerce.Price(min: 0, max: 2000, decimals: 1)));

            var niraResult = new Faker<VerificationResultViewModel>()
                .RuleFor(n => n.MatchingStatus, f => f.Random.Bool())
                .RuleFor(n => n.CardStatus, f => f.Random.ArrayElement(resultOptions))
                .RuleFor(n => n.Status, f => f.Random.ArrayElement(statusOptions))
                //.RuleFor(n => n.PasswordDaysLeft, f => f.Random.Number(min: 0, max: 180))
                //.RuleFor(n => n.ExecutionCost, f => Convert.ToDouble(f.Commerce.Price(min: 0, max: 2000, decimals: 1)))
                ;

            var requestDetails = new Faker<RequestViewModel>()
                .RuleFor(r => r.CardNumber, f => request.CardNumber ?? f.Random.Int(min: 100000000, max: 999999999).ToString())
                .RuleFor(r => r.DateOfBirth, f => f.Date.Past(18, new DateTime(2002, 1, 1, 0, 0, 0)).Date)
                .RuleFor(r => r.Surname, f => request.Surname ?? f.Name.FirstName())
                .RuleFor(r => r.GivenNames, f => request.GivenNames ?? f.Name.LastName())
                .RuleFor(n => n.Nin, f => request.Nin ?? $"C{f.PickRandom<Gender>()}{StringExtensions.GetRandomString(StringExtensions.randomSequence, 12)}")
                .RuleFor(r => r.RequestStatus, f => f.PickRandom<RequestStatus>())
                .RuleFor(r => r.Initiator, f => request.Initiator ?? f.Name.FirstName())
                .RuleFor(r => r.ResultJson, f => niraResult.Generate())
                .RuleFor(r => r.Id, f => request.Id ?? f.Random.Guid())
                .RuleFor(r => r.ParticipantId, f => f.Random.Guid())
                .RuleFor(r => r.ReceivedAt, f => f.Date.Past().ToUniversalTime())
                .RuleFor(r => r.ReceivedFromNira, f => f.Date.Past().ToUniversalTime())
                .RuleFor(r => r.SubmittedAt, f => f.Date.Past().ToUniversalTime());

            return request.Id != null ?
                            new SearchResponse
                            {
                                Requests = requestDetails.Generate(1),
                                Pagination = new SearchPagination
                                {
                                    TotalItems = totalItems
                                }

                            } : new SearchResponse
                            {
                                Requests = requestDetails.Generate(request.Pagination?.ItemsPerPage ?? 50),
                                Pagination = new SearchPagination
                                {
                                    TotalItems = totalItems
                                }
                            };
        }

        public static SearchRequest GetTestSearchRequest()
        {
            return new SearchRequest
            {
                Initiator = "Diego"
            };
        }


        public static CredentialRequest GetTestCredentialRequest()
        {
            return new CredentialRequest
            {
                Password = "12abMK",
                Username = "Emata@ROOT"
            };
        }
        
        public static Credential GetLatestTestCredentials()
        {
            return new Credential
            {
                Id = Guid.NewGuid(),
                Password = "12abMK",
                Username = "Emata@ROOT",
                CreatedOn = DateTime.Now,
                ExpiresOn = DateTime.Now.AddDays(59),
                JobId = "125"
            };
        }
        
        public static Credential GetNewTestCredentials()
        {
            return new Credential
            {
                Id = Guid.NewGuid(),
                Password = "12abMK",
                Username = "Emata@ROOT",
                CreatedOn = DateTime.Now
            };
        }

        public static Mock<HttpRequest> CreateMockRequest(object body)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            var json = JsonConvert.SerializeObject(body);

            sw.Write(json);
            sw.Flush();

            ms.Position = 0;

            var headerDict = new HeaderDictionary
            {
                { "Name", new string[1] { "foo" } }
            };

            var httpContext = new Mock<HttpContext>();

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(ms);
            mockRequest.Setup(x => x.Headers).Returns(headerDict);
            mockRequest.Setup(x => x.HttpContext).Returns(httpContext.Object);

            return mockRequest;
        }
        public static ExportRequest GetTestExportRequest()
        {
            return new ExportRequest
            {
                DateRange = new DateFilter
                {
                    From = new DateTime(2020, 01, 25),
                    To = DateTime.Now
                }
            };
        }

        /// <summary>
        /// Generates a requests export object
        /// </summary>
        public static RequestsExport GetTestRequestsExport()
        {
            var requestFaker = new Faker<RequestsExport>()
                .RuleFor(r => r.Id, f => f.Random.Guid())
                .RuleFor(r => r.GenerationStatus, f => ExportStatus.Complete)
                .RuleFor(r => r.CreatedOn, f => f.Date.Past().ToUniversalTime())
                .RuleFor(r => r.UserName, f => f.Name.FullName())
                .RuleFor(r => r.DownloadedOn, f => f.Date.Past().ToUniversalTime())
                .RuleFor(r => r.FileName, (f, r) => $"{r.Id}.csv")
                .RuleFor(r => r.IsDeleted, f => f.Random.Bool());
            return requestFaker.Generate();
        }
        
        /// <summary>
        /// Generates a list of requests export 
        /// </summary>
        public static List<RequestsExport> GetTestListRequestsExport(int count = 20)
        {
            var requestFaker = new Faker<RequestsExport>()
                .RuleFor(r => r.Id, f => f.Random.Guid())
                .RuleFor(r => r.GenerationStatus, f => f.PickRandom<ExportStatus>())
                .RuleFor(r => r.CreatedOn, f => f.Date.Past(1).ToUniversalTime())
                .RuleFor(r => r.UserName, f => f.Name.FullName())
                .RuleFor(r => r.FileName, (f, r) => $"{r.Id}.csv")
                .RuleFor(r => r.IsDeleted, f => f.Random.Bool());
            return requestFaker.Generate(count);
        }
        
        /// <summary>
        /// Generates a list of Export Dto objects
        /// </summary>
        public static List<Request> GetTestListRequest()
        {
            var requestFaker = new Faker<Request>()
                .RuleFor(r => r.Id, f => f.Random.Guid())
                .RuleFor(r => r.ReceivedAt, f => f.Date.Past().ToUniversalTime())
                .RuleFor(r => r.Surname, f => f.Name.LastName())
                .RuleFor(r => r.GivenNames, f => f.Name.FirstName())
                //.RuleFor(r => r.MatchStatus, f => f.Random.Bool())
                .RuleFor(r => r.CardNumber, f => NiraUtils.MaskCardDetails(f.Random.Int(min: 100000000, max: 999999999).ToString(), charsToShow: 5))
                .RuleFor(n => n.Nin, f => NiraUtils.MaskCardDetails($"C{f.PickRandom<Gender>()}{StringExtensions.GetRandomString(StringExtensions.randomSequence, 12)}", charsToShow: 6));        
            return requestFaker.Generate(20);
        }

        public static AuthServiceSettings GetAuthSettings()
        {
            return new AuthServiceSettings
            {
                Authority = "https://bou-auth-api-foo.bar001.laboremus.no",
                AuthClaims = new AuthClaims
                {
                    SubClaim = "sub",
                    NameClaim = "name",
                    GivenNameClaim = "given_name",
                    RoleClaim = "role"
                },
                AuthHelpers = new AuthHelpers
                { 
                    HeaderName = "Authorization",
                    HeaderValue = "Bearer ",
                    TokenKey = "access_token"
                }
            };
        }
        public static NiraSettings GetNiraSettings()
        {
            return new NiraSettings
            {
                Url = "http://196.0.118.1:8080/pilatusp2-tpi2-ws/ThirdPartyInterfaceNewWS?wsdl",
                CredentialConfig = new CredentialConfig {
                    UseDatabaseCredentials = false,
                    Username = "Emata@ROOT",
                    Password = "987Wkln",
                    PasswordExpirationDays = 14,
                    PasswordDaysLimit = 14,
                    PasswordLifeSpan = 59,
                    CertificatePath = "niraug.crt",
                },
                NiraDateTimeConfig = new NiraDateTimeConfig {
                    Offset = 3,
                    Culture = "en-US",
                    DateFormat = "dd/MM/yyyy",
                    ExportDateFormat = "dd.MM.yyyy"
                },
                BillableErrorCodes = new List<string>
                {
                    "321",
                    "322",
                    "323",
                    "324",
                }
            };
        }
        
        public static NiraSettings GetOtherNiraSettings()
        {
            return new NiraSettings
            {
                Url = "http://196.0.118.1:8080/pilatusp2-tpi2-ws/ThirdPartyInterfaceNewWS?wsdl",
                CredentialConfig = new CredentialConfig {
                    UseDatabaseCredentials = true,
                    Username = "Emata@ROOT",
                    Password = "987Wkln",
                    PasswordDaysLimit = 14,
                    PasswordLifeSpan = 59,
                    CertificatePath = "niraug.crt",
                },
                NiraDateTimeConfig = new NiraDateTimeConfig {
                    Offset = 3,
                    Culture = "en-US",
                    DateFormat = "dd/MM/yyyy",
                    ExportDateFormat = "dd.MM.yyyy"
                },
                BillableErrorCodes = new List<string>
                {
                    "321",
                    "322",
                    "323",
                    "324",
                }
            };
        }
        
        public static ExportSettings GetExportSettings()
        {
            return new ExportSettings
            {
                DelayInHours = 2,
                DaysBack = 3,
                Buffer = 1024,
                FolderPath = "folder",
                PageSize = 10,
                RequestLimit = 1000000
            };
        }

        public static SubscriptionSettings GetSubscriptionSettings()
        {
            return new SubscriptionSettings
            {
                HeaderName = "Ocp-Apim-Subscription-Key"
            };
        }
        public static VerificationResult GetTestVerificationResult()
        {
            var niraResultFaker = new Faker<VerificationResult>()
                .RuleFor(n => n.MatchingStatus, f => f.Random.Bool())
                .RuleFor(n => n.CardStatus, f => f.Random.ArrayElement(resultOptions))
                .RuleFor(n => n.Status, f => f.Random.ArrayElement(statusOptions))
                .RuleFor(n => n.PasswordDaysLeft, f => f.Random.Number(min: 0, max: 180))
                .RuleFor(n => n.ExecutionCost, f => Convert.ToDouble(f.Commerce.Price(min: 0, max: 2000, decimals: 1)));

            return niraResultFaker.Generate();
        }


        public static NitaCredential GetTestNitaCredentials()
        {
            return new NitaCredential
            {
                Id = Guid.NewGuid(),
                ClientSecret = "12abMK",
                ClientKey = "Emata@ROOT",
                CreatedOn = DateTime.Now.AddDays(-3),
                UpdatedOn = DateTime.Now.AddDays(-1),
            };
        }

        public static NitaCredential GetNewTestNitaCredentials()
        {
            return new NitaCredential
            {
                ClientSecret = "12abMK",
                ClientKey = "Emata@ROOT",
                UpdatedOn = DateTime.Now
            };
        }

        public static NitaCredentialRequest GetTestNitaCredentialRequest()
        {
            return new NitaCredentialRequest
            {
                ClientSecret = "12abMK",
                ClientKey = "Emata@ROOT",
            };
        }

        public static verifyPersonInformationRequest GetTestVerifyPersonInformationRequest()
        {
            return new verifyPersonInformationRequest
            {
                documentId = "000092564",
                dateOfBirth = "01/01/1993",
                givenNames = "Johnson",
                surname = "Tipiyai",
                otherNames =  string.Empty,
                nationalId = "CM930121003EGE",
            };
        }

        public static verifyPersonInformationRequest GetTestDeceasedVerifyPersonInformationRequest()
        {
            return new verifyPersonInformationRequest
            {
                documentId = "000092634",
                dateOfBirth = "01/01/2000",
                givenNames = "IGA",
                surname = "KAGGWA",
                otherNames =  string.Empty,
                nationalId = "CF000121003X3A",
            };
        }

        public static verifyPersonInformationRequest GetTestFailedVerifyPersonInformationRequest()
        {
            return new verifyPersonInformationRequest
            {
                documentId = "000091740",
                dateOfBirth = "18/01/1961",
                givenNames = "BENARD",
                surname = "AYEET",
                otherNames = string.Empty,
                nationalId = "CM6103910007KL",
            };
        }

        public static changePasswordRequest GetTestChangePasswordRequest()
        {
            return new changePasswordRequest
            {
                newPassword = "12abMK"
            };
        }

        public static NitaSettings GetNitaSettings()
        {
            return new NitaSettings
            {
                ApiVersion = "1.0.0",
                Host = "https://api-uat.integration.go.ug",
                CertificatePath = "niragoug.crt",
                Culture = "en-US",
                Offset = 3,
                RateLimit = 12,
                Segments = new Segments  
                {
                    EnvironmentSegment = "t/nira.go.ug/nira-api",
                    TokenSegment = "token",
                    GetPersonSegment = "getPerson",
                    GetIdCardSegment = "getIdCard",
                    GetPlaceOfResidenceSegment = "getPlaceOfResidence",
                    GetPlaceOfBirthSegment = "getPlaceOfBirth",
                    GetPlaceofOriginSegment = "getPlaceofOrigin",
                    GetVoterDetailsSegment = "getVoterDetails",
                    VerifyPersonSegment = "verifyPerson",
                    IdentifyPersonSegment = "identifyPerson",
                    IdentifyPersonFullSearchSegment= "identifyPersonFullSearch",
                    GetApplicationStatusSegment = "getApplicationStatus",
                    GetSpousesSegment  = "getSpouses",
                    GetParentsSegment  = "getParents",
                    VerifyPersonInformationSegment = "verifyPersonInformation",
                    CheckAccountSegment = "checkAccount",
                    ChangePasswordSegment = "changePassword"
                }
            };
        }
        public static VerificationSettings GetVerificationSettings()
        {
            return new VerificationSettings
            {
                ConnectionType = ConnectionType.Nira
            };
        }

    }

}
