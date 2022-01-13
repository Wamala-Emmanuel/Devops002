using System;
using System.IO;
using System.Net;
using GatewayService.Context;
using GatewayService.Helpers;
using Laboremus.Extensions.Telemetry.Elasticsearch;
using Laboremus.Extensions.Telemetry.Logs;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Email;

namespace GatewayService
{
    public class Program
    {

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static int Main(string[] args)
        {
            SelfLog.Enable(Console.WriteLine);

            var loggerConfiguration = new LoggerConfiguration();

            ConfigureLogging(loggerConfiguration);

            Log.Logger = loggerConfiguration.CreateLogger();

            var host = CreateWebHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                Log.Information("Starting web host");
                var context = services.GetRequiredService<ApplicationDbContext>();

                context.Database.Migrate();
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(
                    ex, "Gateway failed start. Host terminated unexpectedly, {ErrorMessage} - {ErrorInnerException} \n {ErrorData}", ex.Message, ex.InnerException, ex.Data);

                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureLogging(LoggerConfiguration loggerConfiguration)
        {
            // load telemetry configurations
            var telemetry = Configuration.GetTelemetrySettings();

            // enable logging to file
            if (telemetry.FileLogs.Enabled)
            {
                loggerConfiguration.AddFileSerilogSink(
                    Configuration,
                    destinationFolder: telemetry.FileLogs.DestinationFolder,
                    nameOfService: telemetry.ServiceName);
            }

            // enable logging to email
            if (telemetry.EmailLogs.Enabled)
            {
                loggerConfiguration.AddEmailSerilogSink(
                    Configuration,
                    nameOfService: telemetry.ServiceName,
                    emailConfiguration: telemetry.EmailLogs);
            }

            // enable logging to elasticsearch node
            if (telemetry.Elasticsearch.Enabled)
            {
                loggerConfiguration.AddElasticsearchSerilogSink(
                    nameOfService: telemetry.ServiceName,
                    urlOfElasticsearchNode: telemetry.Elasticsearch.NodeUrl,
                    formatOfIndex: telemetry.Elasticsearch.IndexFormat,
                    username: telemetry.Elasticsearch.UserName,
                    password: telemetry.Elasticsearch.Password,
                    periodInMinutes: 15);
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();

    }
}