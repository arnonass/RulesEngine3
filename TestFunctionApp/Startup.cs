using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using WebJobInstrumentation.Telemetry;
using WebJobInstrumentation.Telemetry.Initializers;

namespace TestFunctionApp
{
    public class Startup : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            // Add telemetry initializers.
            TelemetryConfiguration.Active.TelemetryInitializers
                .Add(new CorrelationTelemetryInitializer());

            TelemetryConfiguration.Active.TelemetryInitializers
                .Add(new CloudRoleNameInitializer("Function1"));

            //// Set up logging.
            //var loggerFactory = new LoggerFactory();
            //var config = new JobHostConfiguration();

            //// If this variable exists, build up a LoggerFactory with requested loggers.
            //string instrumentationKey = "1b3637b7-d23a-4cdc-bc19-12f22a2cb1b8";
            //if (!string.IsNullOrEmpty(instrumentationKey))
            //{
            //    // Set default LogCategoryFilter.
            //    var filter = new LogCategoryFilter
            //    {
            //        DefaultLevel = LogLevel.Trace
            //    };

            //    // Add custom TraceLogger for Application Insights.
            //    var traceLoggerProvider = new TraceLoggerProvider(instrumentationKey);
            //    config.LoggerFactory.AddProvider(traceLoggerProvider);
            //}

            //// Add function filters.
            //var extensions = config.GetService<IExtensionRegistry>();
            //extensions.RegisterExtension<IFunctionInvocationFilter>(new OperationIdFilter());

            //// Add function trigger services.
            //config.UseTimers();
            //config.UseHttp();
            //config.UseServiceBus(
            //    new ServiceBusConfiguration
            //    {
            //        MessageOptions = new OnMessageOptions
            //        {
            //            MaxConcurrentCalls = 12
            //        }
            //    });

            //if (config.IsDevelopment)
            //{
            //    config.UseDevelopmentSettings();
            //}

            //// Initialize WebJob host.
            //var host = new JobHost(config);

            //// Run WebJob.
            //host.RunAndBlock();
        }
    }
}
