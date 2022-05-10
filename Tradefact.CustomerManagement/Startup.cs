using Core.Interfaces;
using Core.ServiceBus;
using Mapster;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.ServiceBus;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Data;
using System.IO;
using Tradefact.Application;
using Tradefact.Application.Common.Interfaces;
using Tradefact.Portal.Infrastructure.Middleware;
using Tradefact.Portal.Models;
using Tradefact.Portal.Service;
using Tradefact.Data;
using Tradefact.Services;
using System.Text;
using Tradefact.Portal.Infrastructure.Helpers;
using Microsoft.Extensions.FileProviders;

namespace Tradefact.Portal
{
    public class Startup
    {
        public IWebHostEnvironment _env { get; }


        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;

            TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);
            TypeAdapterConfig.GlobalSettings.Scan(typeof(Startup).Assembly);
            // Tradefact.Application
            TypeAdapterConfig.GlobalSettings.Scan(typeof(Tradefact.Application.Models.MappingRegister).Assembly);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                // Handling SameSite cookie according to https://docs.microsoft.com/en-us/aspnet/core/security/samesite?view=aspnetcore-3.1
                options.HandleSameSiteCookieCompatibility();
            });

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddApplication();

            // Configuration to sign-in users with Azure AD B2C
            services.AddMicrosoftIdentityWebAppAuthentication(Configuration, "AzureAdB2C");

            services.AddControllersWithViews()
                .AddMicrosoftIdentityUI();


            if (_env.IsDevelopment())
            {
                services.AddRazorPages().AddRazorRuntimeCompilation();
            } else
            {
                services.AddRazorPages();
            }

            //Configuring appsettings section AzureAdB2C, into IOptions
            services.AddOptions();
            services.Configure<OpenIdConnectOptions>(Configuration.GetSection("AzureAdB2C"));


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IUserResolverService, Tradefact.Portal.Infrastructure.Services.UserResolverService>();

            services.AddDbContext<TradefactDbContext>(opts =>
            {
                opts.UseSqlServer(Configuration.GetConnectionString("TradefactDB"), b => b.MigrationsAssembly("Tradefact.CustomerManagement"));
            });

            services.AddTransient<IDbConnection>(db => new SqlConnection(
                                Configuration.GetConnectionString("TradefactDB")));

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = false;
            }).AddEntityFrameworkStores<TradefactDbContext>();

            services.AddTransient<UserManager<ApplicationUser>>();

            services.AddEventBus(Configuration.GetValue<string>("AppSettings:ServiceBusConnectionString"));

            services.AddTransient<IAISTrackingService, AISTrackingDisabledService>();

            var urlSigningKey = Encoding.ASCII.GetBytes(Configuration.GetValue<string>("UrlSigningKey"));
            services.AddSingleton(new SigningUrlHelper(urlSigningKey));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SigningUrlHelper _signingUrlHelper)
        {
            // app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseMiddleware<ValidateSharedURLSignatureMiddleware>(_signingUrlHelper);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/html";

                        await context.Response.WriteAsync("<html lang=\"en\"><body>\r\n");
                        await context.Response.WriteAsync("ERROR!<br><br>\r\n");

                        var exceptionHandlerPathFeature =
                            context.Features.Get<IExceptionHandlerPathFeature>();

                        if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
                        {
                            await context.Response.WriteAsync(
                                                      "File error thrown!<br><br>\r\n");
                        }

                        await context.Response.WriteAsync(
                                                      "<a href=\"/\">Home</a><br>\r\n");
                        await context.Response.WriteAsync("</body></html>\r\n");
                        await context.Response.WriteAsync(new string(' ', 512));
                    });
                });
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"web-components/dist/packing-list")),
            //    RequestPath = new PathString("/components/js")
            //});
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }

    static class CustomExtensionsMethods
    {
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
    }
}
