using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;
using Serilog.Sinks.SystemConsole.Themes;

namespace Tradefact.Api.Common.Extensions
{
    public static class SerilogConfigurationSetup
    {

        public static void AddTradefactConfiguration(
            this LoggerConfiguration loggerConfiguration,
            IConfiguration configuration)
        {
            _ = loggerConfiguration ?? throw new ArgumentNullException(nameof(loggerConfiguration));
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;

            loggerConfiguration
               .ReadFrom.Configuration(configuration, "Tradefact:Settings:Api:Serilog")
               .Enrich.FromLogContext()
               .Enrich.WithProperty("Assembly", assemblyName)
               .Enrich.FromLogContext()
               .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
               .WriteTo.Logger(lc => lc.Filter.ByExcluding(Matching.WithProperty<bool>("Security", p => p)));

            if (configuration.GetValue<bool>("Tradefact:Settings:Api:ApplicationInsights:Enabled"))
            {
                string instrumentationKey = configuration.GetValue<string>("Tradefact:Settings:Api:ApplicationInsights:InstrumentationKey");
                loggerConfiguration.WriteTo.ApplicationInsights(instrumentationKey, new CustomApplicationInsightsTelemetryConverter());
            }
        }

        private class CustomApplicationInsightsTelemetryConverter : TraceTelemetryConverter
        {
            public override IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
            {
                foreach (ITelemetry telemetry in base.Convert(logEvent, formatProvider))
                {
                    if (logEvent.Properties.ContainsKey("ErrorId"))
                    {
                        telemetry.Context.Operation.Id = logEvent.Properties["ErrorId"].ToString();
                    }

                    ISupportProperties propTelematry = (ISupportProperties)telemetry;

                    var removeProps = new[] { "MessageTemplate", "ErrorId", "ErrorMessage" };
                    removeProps = removeProps.Where(prop => propTelematry.Properties.ContainsKey(prop)).ToArray();

                    foreach (var prop in removeProps)
                    {
                        propTelematry.Properties.Remove(prop);
                    }

                    yield return telemetry;
                }
            }
        }
    }
}

