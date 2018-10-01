using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using WebJobInstrumentation.Telemetry;
using WebJobInstrumentation.Telemetry.Initializers;
using System.Configuration;
using System.Diagnostics;
using Microsoft.ServiceBus;

namespace RulesService
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            // Add telemetry initializers.
            TelemetryConfiguration.Active.TelemetryInitializers
                .Add(new CorrelationTelemetryInitializer());

            TelemetryConfiguration.Active.TelemetryInitializers
                .Add(new CloudRoleNameInitializer(ConfigurationManager.AppSettings["ComponentName"]));

            // Set up logging.
            using (var loggerFactory = new LoggerFactory())
            {
                var config = new JobHostConfiguration();

                // If this variable exists, build up a LoggerFactory with requested loggers.
                string instrumentationKey = ConfigurationManager.AppSettings["AppInsightsInstrumentationKey"];
                if (!string.IsNullOrEmpty(instrumentationKey))
                {
                    // Set default LogCategoryFilter.
                    var filter = new LogCategoryFilter
                    {
                        DefaultLevel = LogLevel.Trace
                    };

                    // Wire up LoggerFactory with default filter.
                    config.LoggerFactory = loggerFactory
                        //.AddApplicationInsights(instrumentationKey, filter.Filter)
                        .AddConsole(filter.Filter);

                    config.Tracing.ConsoleLevel = TraceLevel.Verbose;

                    // Add custom TraceLogger for Application Insights.
                    var traceLoggerProvider = new TraceLoggerProvider(instrumentationKey);
                    config.LoggerFactory.AddProvider(traceLoggerProvider);
                }

                // Add function filters.
                var extensions = config.GetService<IExtensionRegistry>();
                extensions.RegisterExtension<IFunctionInvocationFilter>(new OperationIdFilter());

                // Add servicebus configuration.
                ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Https;

                var serviceBusConfiguration = new ServiceBusConfiguration
                {
                    MessageOptions = new OnMessageOptions
                    {
                        MaxConcurrentCalls = 12,
                        AutoComplete = true
                    }
                };

                config.UseServiceBus(serviceBusConfiguration);

                // Add function trigger services.
                config.UseTimers();
                config.UseHttp();

                if (config.IsDevelopment)
                {
                    config.UseDevelopmentSettings();
                }

                // Initialize WebJob host.
                var host = new JobHost(config);

                // Run WebJob.
                host.RunAndBlock();
            }
        }
    }
}
