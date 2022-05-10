using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.AspnetcoreHttpcontext;
using Tradefact.Api.Common.Extensions;

namespace Tradefact.Api
{
    public static class Program
    {
        public static string Namespace = typeof(Startup).Namespace;
        public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
                                     .SetBasePath(Environment.CurrentDirectory)
                                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                     .AddJsonFile("appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                                     .AddEnvironmentVariables()
                                     .Build();

        public static void Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();

                Log.Information("Starting host...");
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
                    {
                        loggerConfiguration.AddTradefactConfiguration(hostingContext.Configuration);
                    });
                });
    }
}
