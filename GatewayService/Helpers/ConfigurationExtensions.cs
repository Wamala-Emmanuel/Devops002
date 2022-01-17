using System.Collections.Generic;
using GatewayService.DTOs;
using Laboremus.Extensions.Telemetry.Helpers;
using Microsoft.Extensions.Configuration;
using NiraWebService;

namespace GatewayService.Helpers
{

#nullable disable
    public static class ConfigurationExtensions
    {
        public static string GetTempFolder(this IConfiguration config)
        {
            return config["TempFileUploadLocation"];
        }

        public static bool ShowHangfireDashboard(this IConfiguration config)
        {
            return bool.Parse(config["ShowHangfireDashboard"] ?? "false");
        }

        public static RabbitMqConfig GetRabbitMqConfig(this IConfiguration config)
        {
             return config.GetSection("RabbitMq").Get<RabbitMqConfig>();
        }
        public static NotificationConfig GetNotificationConfig(this IConfiguration configuration)
        {
            return configuration.GetSection("Notification").Get<NotificationConfig>();
        }

        public static AuthServiceSettings GetAuthServiceSettings(this IConfiguration configuration)
        {
            return configuration.GetSection("Authentication").Get<AuthServiceSettings>();
        }
        public static ProxySettings GetProxySettings(this IConfiguration configuration)
        {
            return configuration.GetSection("ProxySettings").Get<ProxySettings>();
        }

        public static TelemetryConfiguration GetTelemetrySettings(this IConfiguration configuration)
        {
            var telemetryConfiguration = new TelemetryConfiguration();

            configuration.GetSection("Telemetry").Bind(telemetryConfiguration);

            return telemetryConfiguration;
        }

        public static DigitalArchiveConfig GetDigitalArchiveConfig(this IConfiguration configuration)
        {
            return configuration.GetSection("DigitalArchive").Get<DigitalArchiveConfig>();
        }

        public static SwaggerConfig GetSwaggerConfig(this IConfiguration configuration)
        {
            return configuration.GetSection("Swagger").Get<SwaggerConfig>();
        }

        public static NiraConfig GetNiraSettings(this IConfiguration configuration)
        {
            return configuration.GetSection("NiraConfig").Get<NiraConfig>();
        }
        
        public static BillingServiceConfig GetBillingServiceSettings(this IConfiguration configuration)
        {
            return configuration.GetSection("BillingService").Get<BillingServiceConfig>();
        }
        
        public static List<verifyPersonInformationRequest> GetValidNationalIdsList(this IConfiguration configuration)
        {
            return configuration.GetSection("ValidNationalIds").Get<List<verifyPersonInformationRequest>>();
        }

        public static ExportSettingsConfig GetRequestExportConfig(this IConfiguration configuration)
        {
            return configuration.GetSection("RequestExport").Get<ExportSettingsConfig>();
        }

        public static ClientCredentialsSettings GetClientCredentialsSettings(this IConfiguration configuration)
        {
            return configuration.GetSection("ClientCredentials").Get<ClientCredentialsSettings>();
        }

        public static string[] GetAllowedOrigins(this IConfiguration config)
        {
            var originSection = config.GetSection("AllowedOrigins");
            var origins = originSection.Get<string[]>();

            return origins;
        }

        public static VerificationSettings GetVerificationSettings(this IConfiguration configuration)
        {
            return configuration.GetSection("VerificationSettings").Get<VerificationSettings>();
        }

        public static NitaSettings GetNitaSettings(this IConfiguration configuration)
        {
            return configuration.GetSection("NitaConfig").Get<NitaSettings>();
        }

        public static HangfireConfig GetHangfireConfig(this IConfiguration configuration)
        {
            return configuration.GetSection("HangfireConfig").Get<HangfireConfig>();
        }
    }

    public class RabbitMqConfig
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Queue { get; set; }
    }
    public class DigitalArchiveConfig
    {
        public string TokenUrl { get; set; }
        public int Username { get; set; }
        public string Password { get; set; }
        public string BaseUrl { get; set; }
        public string Cookie { get; set; }
    }

    public class NotificationConfig
    {
        public string SmsTemplate { get; set; }
        public string ClientId { get; set; }
        public string Server { get; set; }
    }

    public class SwaggerConfig
    {
        public bool Enabled { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string ContactUrl { get; set; }
        public License License { get; set; }
    }

    public class License
    {
        public string Name { get; set; }
        public string Url { get; set; }

    }

    public class AuthServiceSettings
    {
        public const string ConfigurationName = "Authentication";

        public string Authority { get; set; }
        public string ApiName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AccessScope { get; set; }
        public string ImplicitDescription { get; set; }
        public string ImplicitDefinition { get; set; }
        public AuthHelpers AuthHelpers { get; set; }
        public AuthClaims AuthClaims { get; set; }
    }

    public class AuthHelpers
    {
        public string HeaderName { get; set; }
        public string HeaderValue { get; set; }
        public string TokenKey { get; set; }
    }

    public class AuthClaims
    {
        public string SubClaim { get; set; }
        public string NameClaim { get; set; }
        public string GivenNameClaim { get; set; }
        public string RoleClaim { get; set; }
        public string AdminRole { get; set; }
    }

    public class NiraConfig
    {
        public int MaxRetries { get; set; }
        public string Url { get; set; }
        public CredentialConfig CredentialConfig { get; set; }
        public NiraDateTimeConfig NiraDateTimeConfig { get; set; }
        public List<string> BillableErrorCodes { get; set; }
    }

    public class EmailConfiguration
    {
        public string EmailSubject { get; set; }
        public string FromEmail { get; set; }
        public string MailServer { get; set; }
        public string[] ToEmails { get; set; }

        public int Port { get; set; }
        public bool EnableSsl { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class BillingServiceConfig
    {
        public string Url { get; set; }
        public string Docs { get; set; }
    }
    
    public class ExportSettingsConfig
    {
        public int Buffer { get; set; }

        public int DaysBack { get; set; }

        public double Delay { get; set; }

        public string FolderPath { get; set; }

        public int PageSize { get; set; }

        public int RequestLimit { get; set; }
    }

    public class HangfireConfig
    {
        /// <summary>
        /// The number of worker threads in a dedicated pool that run inside Hangfire Server subsystem
        /// </summary>
        public int WorkerCount { get; set; }
    }

}
