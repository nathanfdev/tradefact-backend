using Core.Interfaces;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Tradefact.Api.Common.Extensions;
using Tradefact.Api.Common.Middleware.ExceptionHandling;
using Tradefact.Data;
using Tradefact.External.API.Helpers;

namespace Tradefact.External.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMapster(options =>
            {
                options.Default.IgnoreNonMapped(true); // Does not work.
                TypeAdapterConfig.GlobalSettings.Default.IgnoreNonMapped(true); // Does not work.
            });

            services.AddNSwag(Configuration);

            services.AddControllers(
                options => options.SuppressAsyncSuffixInActionNames = false
            );

            services.AddDbContext<TradefactDbContext>(opts =>
            {
                opts.UseSqlServer(Configuration.GetConnectionString("TradefactDB"), b => b.MigrationsAssembly("Tradefact.Api"));
            });
            services.AddTransient<IDbConnection>(db => new SqlConnection(Configuration.GetConnectionString("TradefactDB")));
            services.AddScoped<IUserResolverService, UserResolverService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseNSwag();
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


    }

    static class CustomExtensionsMethods
    {
        // Extension method
        public static IServiceCollection AddMapster(this IServiceCollection services, Action<TypeAdapterConfig> options = null)
        {
            var typeAdapterConfig = new TypeAdapterConfig();
            // scans the assembly and gets the IRegister
            var mappingRegistrations = TypeAdapterConfig.GlobalSettings.Scan(typeof(Tradefact.External.API.Helpers.MappingRegister).Assembly);
            // adds the registration to the TypeAdapterConfig
            mappingRegistrations.ToList().ForEach(register => register.Register(typeAdapterConfig));

            // register the mapper as Singleton service for my application
            var mapperConfig = new Mapper(typeAdapterConfig);
            services.AddSingleton<IMapper>(mapperConfig);

            return services;
        }
    }
}