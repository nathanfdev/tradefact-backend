using Core.Interfaces;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Tradefact.AADB2C.Api.Services;
using Tradefact.AADB2C.Api.Services.Certificate;
using Tradefact.Api.Common.Configuration;
using Tradefact.Api.Common.Extensions;
using Tradefact.Data;

namespace Tradefact.AADB2C.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        private HostConfig _hostConfig { get; set; }

        const string APP_CLIENT_ID = "f69d15f7-d316-465c-ac26-a00308414345";

        // This is the client secret from the app registration process.
        const string APP_CLIENT_SECRET = "3Fy06@_3KzMJjvmAdE-A[R:r_l@UB?mT";

        // This is available as "DNS Name" from the overview page of the Key Vault.
        const string KEYVAULT_BASE_URI = "https://dev-tradefact-keyvault.vault.azure.net/";


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ApplicationConfiguration(services);
            CorsConfiguration(services);

            services
                .AddConfiguration(Configuration)
                //.AddAuthenticationWithAuthorizationSupport(Configuration)
                .AddNSwag(Configuration);

            services.AddDbContext<TradefactDbContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("TradefactDB"), b => b.MigrationsAssembly("Tradefact.Api")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = false;
            }).AddEntityFrameworkStores<TradefactDbContext>();

            services.AddTransient<IUserResolverService, UserService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddTransient<KeyVaultCertificateService>(s => new KeyVaultCertificateService(new KeyVaultConfiguration
            {
                TenantId = "bd0b8360-171b-484a-922b-755bb4b1dc37",
                AppClientId = "f69d15f7-d316-465c-ac26-a00308414345",
                AppClientSecret = "n@=Wru/3-ywca4yhs93R.SN4hsvb?tCW",
                Endpoint = "https://dev-tradefact-keyvault.vault.azure.net/",
                CertificateName = "PolicyInviteCertificate"

            }));

            // Sample: Load the app settings section and bind to AppSettingsModel object graph
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddControllers()
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());


            //services.AddOpenApiDocument(config => {
            //    config.PostProcess = document =>
            //    {
            //        document.Info.Version = "v1";
            //        document.Info.Title = "Tradefact API";
            //        document.Info.Description = "Tradefact API - Next generation customer execution supply chain platform that turns information into action. Tradefact connects the systems, information and people needed to determine, measure, and resolve logistics and related supply chain issues in more real-time, with greater predictability, and with less effort.";
            //        document.Info.TermsOfService = "None";
            //        document.Info.Contact = new NSwag.OpenApiContact
            //        {
            //            Name = "Tradefact",
            //            Email = string.Empty,
            //            Url = "https://www.tradefact.com/"
            //        };
            //        document.Info.License = new NSwag.OpenApiLicense
            //        {
            //            Name = "Use under LICX",
            //            Url = "https://example.com/license"
            //        };
            //    };
            //});
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void CorsConfiguration(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowSpecificOrigin",
                    builder => builder.AllowAnyOrigin() // TODO: Replace with FE Service Host as appropriate to constrain clients
                        .AllowAnyHeader()
                        .WithMethods("PUT", "POST", "OPTIONS", "GET", "DELETE"));
            });
        }

        private void ApplicationConfiguration(IServiceCollection services)
        {
            services.AddSingleton(_ => _configuration);
            services.AddSingleton(_configuration.GetSection(Assembly.GetExecutingAssembly().GetName().Name).Get<HostConfig>());

            var serviceProvider = services.BuildServiceProvider();
            _hostConfig = serviceProvider.GetService<HostConfig>();
        }
    }



}
