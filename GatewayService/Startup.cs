using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using GatewayService.Context;
using GatewayService.DTOs;
using GatewayService.Extensions;
using GatewayService.HangFire;
using GatewayService.Helpers;
using GatewayService.Hubs.Implementations;
using GatewayService.Services;
using Hangfire;
using Hangfire.SqlServer;
using Laboremus.Extensions.Telemetry.Elasticsearch;
using Laboremus.Extensions.Telemetry.HealthChecks;
using Laboremus.Extensions.Telemetry.Helpers;
using Laboremus.Extensions.Telemetry.Logs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.ReDoc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace GatewayService
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            _logger = logger;
            Configuration = configuration;
            Environment = env;
            _logger.LogInformation("Application started... Env: {EnvironmentName}", env.EnvironmentName);

            // load telemetry configurations
            Telemetry = Configuration.GetTelemetrySettings();
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        private TelemetryConfiguration Telemetry { get; }

        private readonly string _clientPermissions = "ClientPermissions";
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var dbSettings = Configuration.GetDbSettings();
            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.EnableSensitiveDataLogging()
                    .UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: dbSettings.RetryCount,
                        maxRetryDelay: TimeSpan.FromSeconds(Math.Pow(2, dbSettings.RetryCount)),
                        errorNumbersToAdd: null);
                    });
            }, poolSize: dbSettings.MaxPoolSize);

            // enable cache
            services.AddMemoryCache();

            // Add Hangfire services...
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            var hangfireConfig = Configuration.GetHangfireConfig();

            // Add the processing server as IHostedService
            services.AddHangfireServer(options =>
            {
                if (Environment.IsDevelopment())
                {
                    // This is the default value
                    options.WorkerCount = System.Environment.ProcessorCount * 5; 
                }
                else
                {
                    options.WorkerCount = hangfireConfig.WorkerCount;
                }
            });

            services.AddMvcCore()
                .AddApiExplorer()
                .AddAuthorization()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services.AddHealthChecks();

            services.ConfigureDependencyInjection(GetType().Assembly);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<ITokenUtil, TokenUtil>();

            var auth = Configuration.GetAuthServiceSettings();
            services.AddAuthentication("Bearer")
                .AddJwtBearer(authenticationScheme: "Bearer", configureOptions: options => 
                {
                    options.Authority = auth.Authority;
                    options.Audience = auth.ApiName;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidTypes = new[] { "at+jwt" },
                        ValidateAudience = true
                    };
                });

            services.AddCors(options =>
            {
                options.AddPolicy(_clientPermissions, policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins(Configuration.GetAllowedOrigins())
                        .AllowCredentials();
                });
            });

            var globalPolicy
                = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

            services.AddMvc(options =>
                {
                    options.Filters.Add(new ValidateModelAttribute());
                    //options.Filters.Add(new AuthorizeFilter(globalPolicy));
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy(),
                    };
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddFluentValidation(s =>
                {
                    s.RegisterValidatorsFromAssemblyContaining<Startup>();
                    s.DisableDataAnnotationsValidation = true;
                });

            services.AddSignalR()
                    .AddJsonProtocol(options =>
                    {
                        options.PayloadSerializerOptions.Converters
                           .Add(new JsonStringEnumConverter());
                    });

            services.Configure<AuthServiceSettings>(options => Configuration
                .GetSection(AuthServiceSettings.ConfigurationName)
                .Bind(options));

            services.Configure<ExportSettings>(options => Configuration
                .GetSection(ExportSettings.ConfigurationName)
                .Bind(options));
            
            services.Configure<NiraSettings>(options => Configuration
                .GetSection(NiraSettings.ConfigurationName)
                .Bind(options));

            services.Configure<EncryptionOptions>(options => Configuration
                .GetSection(EncryptionOptions.SectionName)
                .Bind(options));

            services.Configure<ClientCredentialsSettings>(options => Configuration
                .GetSection(ClientCredentialsSettings.SectionName)
                .Bind(options));
            
            services.Configure<VerificationSettings>(options => Configuration
                .GetSection(VerificationSettings.ConfigurationName)
                .Bind(options));
            
            services.Configure<NitaSettings>(options => Configuration
                .GetSection(NitaSettings.ConfigurationName)
                .Bind(options));
            
            services.Configure<SubscriptionSettings>(options => Configuration
                .GetSection(SubscriptionSettings.ConfigurationName)
                .Bind(options));

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = new QueryStringApiVersionReader(new[] { "api-version", "v" });
            });

            var swagger = Configuration.GetSwaggerConfig();
            var clientCredentialsOptions = Configuration.GetClientCredentialsSettings();
            if (swagger.Enabled)
            {
                services.AddSwaggerGenNewtonsoftSupport();
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("1.0", new OpenApiInfo
                    {
                        Title = Documentation.Title,
                        Version = Documentation.Version,
                        Description = Documentation.Description,
                        Contact = new OpenApiContact
                        {
                            Name = swagger.ContactName,
                            Email = swagger.ContactEmail,
                            Url = new Uri(swagger.ContactUrl),
                        }
                    });

                    c.AddSecurityDefinition(clientCredentialsOptions.ClientDefinition, new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Description = clientCredentialsOptions.Description,
                        In = ParameterLocation.Header,
                        Name = clientCredentialsOptions.HeaderName,
                        Flows = new OpenApiOAuthFlows
                        {
                            ClientCredentials = new OpenApiOAuthFlow
                            {
                                TokenUrl = new Uri(clientCredentialsOptions.TokenUrl),
                            }
                        },
                    });

                    c.AddSecurityDefinition(auth.ImplicitDefinition, new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Description = auth.ImplicitDescription,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                TokenUrl = new Uri($"{auth.Authority}/connect/token"),
                                AuthorizationUrl = new Uri($"{auth.Authority}/connect/authorize")
                            }
                        }
                    });


                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "client_credentials"
                                }
                            },
                            new List<string>()
                        }
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference 
                                { 
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "implicit_oauth2" 
                                }
                            },
                            new List<string>()
                        }
                    });

                    c.DocInclusionPredicate((docName, apiDesc) =>
                    {
                        if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                        if (methodInfo.DeclaringType != null)
                        {
                            var versions = methodInfo.DeclaringType
                                .GetCustomAttributes(true)
                                .OfType<ApiVersionAttribute>()
                                .SelectMany(attr => attr.Versions);

                            return versions.Any(v => $"{v}" == docName);
                        }

                        return false;
                    });

                    c.EnableAnnotations(true);

                    c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.DisplayName}_{apiDesc.HttpMethod}");

                    c.OperationFilter<CustomHeaderSwaggerAttribute>();
                    
                    c.OperationFilter<AddResponseHeadersFilter>();

                    c.DocumentFilter<TagReOrderDocumentFilter>();

                    var basePath = Environment.WebRootPath;
                    var xmlPath = Path.Combine(basePath, "Documentation", "api-spec.xml");
                    c.IncludeXmlComments(xmlPath);

                    c.AddEnumsWithValuesFixFilters();
                });
            }

            services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

            var nitaConfig = Configuration.GetNitaSettings();
            var workerCount = System.Environment.ProcessorCount * 5;
            Console.WriteLine($"########################## WorkerCount: {workerCount} ############################");

            var maxQueueActions = workerCount > nitaConfig.RateLimit ? 12 : workerCount;
            var bulkheadPolicy =
                Policy.BulkheadAsync<HttpResponseMessage>(maxParallelization: 6, maxQueuingActions: maxQueueActions);

            services.AddHttpClient("nitahub", c =>
            {
                c.BaseAddress = new Uri(nitaConfig.Host);
                c.Timeout = TimeSpan.FromMinutes(2);
            })
                .AddPolicyHandler(bulkheadPolicy)
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

            // add elasticsearch, external services healthchecks plus drives and memory healthchecks
            if (Telemetry.AddHealthChecks)
            {
                var niraSettings = Configuration.GetNiraSettings();
                var nitaSettings = Configuration.GetNitaSettings();
                var verificationSettings = Configuration.GetVerificationSettings();
                var proxySettings = Configuration.GetProxySettings();

                var healthCheckProxy = new Laboremus.Extensions.Telemetry.Helpers.ProxySettings
                {
                    BypassLocal = proxySettings.BypassLocal,
                    Enabled = proxySettings.Enabled,
                    Url = proxySettings.Url
                };

                var externalServices = new List<ExternalService>();

                if (verificationSettings.ConnectionType == ConnectionType.Nira)
                {
                    externalServices.Add(new ExternalService
                    {
                        Tag = "NIRA",
                        Url = niraSettings.Url
                    });
                }
                
                if (verificationSettings.ConnectionType == ConnectionType.Nita)
                {
                    externalServices.Add(new ExternalService
                    {
                        Tag = "NITA",
                        Url = nitaSettings.Host
                    });
                }

                services.AddExternalServiceMetrics(externalServices, healthCheckProxy, niraSettings.MaxRetries);

                if (Telemetry.Elasticsearch.Enabled)
                {
                    services.AddMetrics(
                        urlOfElasticsearchNode: Telemetry.Elasticsearch.NodeUrl);

                    // add healthchecks elasticsearch publisher
                    services.AddElasticsearchHealthChecksPublisher(
                        options => 
                        {
                            options.NameOfIndex = Telemetry.Elasticsearch.HealthIndexName;
                            options.UrlOfElasticsearchNode = Telemetry.Elasticsearch.NodeUrl;
                            options.NameOfService = Telemetry.ServiceName;
                            options.ElasticsearchUsername = Telemetry.Elasticsearch.UserName;
                            options.ElasticsearchPassword = Telemetry.Elasticsearch.Password;
                        },
                        delayInSeconds: 5,
                        periodInMinutes: 30);
                }
                else
                {
                    services.AddMetrics();
                }
            }

            services.AddMediatR(Assembly.GetExecutingAssembly());

            IdentityModelEventSource.ShowPII = true;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseCors(_clientPermissions);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Pragma", "no-cache");
                context.Response.Headers.Add("Cache-Control", "no-store, max-age=0");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                await next();
            });

            app.UseFileServer();
            // app.UseStaticFiles();

            app.UseCustomErrorHandling();

            // use custom exception logging middleware/interceptor
            if (Telemetry.UseExceptionHandler)
            {
                app.UseExceptionLogHandler();
            }

            app.UseRouting();

            app.UseWebSocketsHandling();

            app.UseAuthentication();
            app.UseAuthorization();

            // use healthchecks endpoint
            if (Telemetry.AddHealthChecks)
            {
                app.UseMetrics(urlOfhealthChecks: "/health");
            }

            var auth = Configuration.GetAuthServiceSettings();

            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                Authorization = new[]
                {
                    new HangFireAuthorizationFilter( new TokenValidationParameters
                    {
                        RequireSignedTokens = false,
                        ValidateIssuerSigningKey = false,
                        ValidateLifetime = false,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = auth.Authority,
                        ValidAudience = auth.ApiName,
                    }, auth.AuthClaims.AdminRole),
                }
            });

            RecurringJob.AddOrUpdate("Delete Old export files",
                () => serviceProvider.GetService<IZipService>()!
                        .DeleteRequestExportAsync(),
                            Cron.Daily());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Documentation}/{action=Index}/{id?}");
                endpoints.MapHub<CredentialHub>("/hubs/credentialhub");
                endpoints.MapHub<RequestHub>("/hubs/requestHub");
            });

            var swagger = Configuration.GetSwaggerConfig();
            if (swagger.Enabled)
            {
                app.UseSwagger(c => {});

                app.UseReDoc(options =>
                {
                    options.SpecUrl = "/swagger/1.0/swagger.json";
                    options.DocumentTitle = swagger.Title;
                    options.RoutePrefix = "spec";
                    options.InjectStylesheet("/custom/styles.css");
                    options.ConfigObject = new ConfigObject
                    {
                        ExpandResponses = "200,201,202",
                        PathInMiddlePanel = false,
                        NativeScrollbars = true,
                        DisableSearch = true,
                        HideDownloadButton = true,
                        RequiredPropsFirst = true,
                        OnlyRequiredInSamples = false,
                        HideHostname = true,
                    };
                });
            }
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromMinutes(1));
        }

        /// <summary>
        ///  The circuit breaker policy configured so it breaks or opens the circuit when there have been 3 consecutive faults when retrying the Http requests.
        /// </summary>
        /// <returns></returns>
        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
        }
    }
}