using Core;
using Core.Interfaces;
using Core.ServiceBus;
using Flare.Api.Infrastructure;
using Flare.Api.Services.Certificate;
using Flare.Data;
using FluentValidation.AspNetCore;
using Integration.Here;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Azure.ServiceBus;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NodaTime;
using System;
using System.Data;
using System.IO.Abstractions;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Tradefact.Api.Common.Extensions;
using Tradefact.Api.Common.Services.Identity;
using Tradefact.Api.Common.Versioning.Interfaces;
using Tradefact.Api.Common.Versioning.Services;
using Tradefact.Application.Common.Interfaces;
using Tradefact.Services;

namespace Flare.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

#if DEBUG
            TypeAdapterConfig.GlobalSettings.Compiler = exp => exp.CompileWithDebugInfo();
#endif
            TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);
            TypeAdapterConfig.GlobalSettings.Scan(typeof(Startup).Assembly);

            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringEnumConverter());
                settings.ContractResolver = new OrderedContractResolver();
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                settings.NullValueHandling = NullValueHandling.Ignore;
#if DEBUG
                settings.Formatting = Formatting.Indented;
#else
                settings.Formatting = Formatting.None;
#endif
                return settings;
            };
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAzureB2C(Configuration)
                .AddNSwag(Configuration);

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddApplication();

            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowSpecificOrigin",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .WithMethods("PUT", "PATCH", "POST", "OPTIONS", "GET", "DELETE"));
            });

            services.AddPlatformServices(Configuration);

            // SQL Server
            services.AddDbContext<FlareDbContext>(opts =>
            {
                opts.UseSqlServer(Configuration.GetConnectionString("FlareDB"), b => b.MigrationsAssembly("Flare.Api"));
            });

            // ASP.NET Identity Platform
            //services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            //{
            //    options.User.RequireUniqueEmail = false;
            //}).AddEntityFrameworkStores<TradefactDbContext>();

            services.AddTransient<IDbConnection>(db => new SqlConnection(Configuration.GetConnectionString("FlareDB")));

            services.AddEventBus(Configuration.GetValue<string>("AppSettings:ServiceBusConnectionString"));

            services.AddTransient<IBlockBlob, BlockBlob>();
            services.AddSingleton<AzureStorageSettings>(x => new AzureStorageSettings { AzureStorage = Configuration.GetValue<string>("AppSettings:AzureStorageConnectionString") });
            services.AddSingleton<IAzureBlobService, AzureBlobService>();

            services.AddCorrelation();

            services.AddTransient<IFileSystem, FileSystem>();
            services.AddTransient<IVersionService, AssemblyVersionService>();

            services.AddHealthChecksService();

            services.AddMemoryCache();
            services.Configure<PlatformOptions>(Configuration.GetSection("Platform"));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();


            services.AddTransient<IPrincipal>(
                provider => provider.GetService<IHttpContextAccessor>().HttpContext.User);
            services.AddTransient<IClaimsTransformation, ClaimsTransformer>();

            services.AddTransient<KeyVaultCertificateService>(s => new KeyVaultCertificateService(new KeyVaultConfiguration
            {
                TenantId = "bd0b8360-171b-484a-922b-755bb4b1dc37",
                AppClientId = "f69d15f7-d316-465c-ac26-a00308414345",
                AppClientSecret = "n@=Wru/3-ywca4yhs93R.SN4hsvb?tCW",
                Endpoint = "https://dev-tradefact-keyvault.vault.azure.net/",
                CertificateName = "PolicyInviteCertificate"
            }));

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            var redisConnectionString = Configuration.GetConnectionString("RedisConnectionString");

            services.AddSingleton(typeof(IClock), NodaTime.SystemClock.Instance);

            services.AddScoped<IUserResolverService, UserResolverService>();
            services.AddExternalIntegrations(Configuration);

            services.AddControllers(configure =>
            {
                var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
                configure.Filters.Add(new AuthorizeFilter(policy));

                configure.Filters.Add(typeof(ValidatorActionFilter));
            })
            .AddNewtonsoftJson(opts =>
            {
                opts.SerializerSettings.Converters.Add(new StringEnumConverter());
                opts.SerializerSettings.ContractResolver = new OrderedContractResolver();
                opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
#if DEBUG
                opts.SerializerSettings.Formatting = Formatting.Indented;
#else
                opts.SerializerSettings.Formatting = Formatting.None;
#endif
            }
            )
            .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            services.AddAuthorization(options =>
            {
                //options.AddPolicy("Over18", policy =>
                //{
                //    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                //    policy.RequireAuthenticatedUser();
                //    policy.Requirements.Add(new MinimumAgeRequirement());
                //});
            });

            services.AddRouting(option =>
            {
                option.LowercaseUrls = true;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 1073741274;
            });

            // var urlSigningKey = Encoding.ASCII.GetBytes(Configuration.GetValue<string>("UrlSigningKey"));

            // string portal_url = Configuration.GetValue<string>("TradefactPortalURL");
            // services.AddSingleton(new SigningUrlHelper(portal_url, urlSigningKey));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseNSwag();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            // app.UseMiddleware<CorrelationIdMiddleware>();
            // app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHealthChecks("/health");


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddPlatformServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddOptions<CachingOptions>().Bind(configuration.GetSection("Caching")).ValidateDataAnnotations();
            //services.AddSingleton<ITradefactMemoryCache, TradefactMemoryCache>();

            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, string serviceBusConnectionString)
        {
            services.AddSingleton<IServiceBusPersistedConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ServiceBusPersistedConnection>>();

                var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

                return new ServiceBusPersistedConnection(serviceBusConnection, logger);
            });

            services.AddTransient<IServiceBusClient, AzureServiceBusClient>();

            return services;
        }
        public static IServiceCollection AddExternalIntegrations(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("here:enabled"))
            {
                var here_config = new HereOptions();
                configuration.Bind("here", here_config);      //  <--- This
                services.AddSingleton(here_config);

                services.AddHereService();
            }

            return services;
        }
    }

    public class ScopesRequirement : IAuthorizationRequirement
    {
        public readonly string ScopeName;

        public ScopesRequirement(string scopeName)
        {
            ScopeName = scopeName;
        }
    }

    public class ScopesHandler : AuthorizationHandler<ScopesRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                        ScopesRequirement requirement)
        {
            // If there are no scopes, do not process
            if (!context.User.Claims.Any(x => x.Type == ClaimConstants.Scope)
               && !context.User.Claims.Any(y => y.Type == ClaimConstants.Scp))
            {
                return Task.CompletedTask;
            }

            Claim scopeClaim = context?.User?.FindFirst(ClaimConstants.Scp);

            if (scopeClaim == null)
                scopeClaim = context?.User?.FindFirst(ClaimConstants.Scope);

            if (scopeClaim != null && scopeClaim.Value.Equals(requirement.ScopeName, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    //public static class DependencyInjection
    //{
    //    public static IServiceCollection AddApplication(this IServiceCollection services)
    //    {
    //        services.AddMediatR(Assembly.GetExecutingAssembly());
    //        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
    //        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

    //        return services;
    //    }
    //}
}
