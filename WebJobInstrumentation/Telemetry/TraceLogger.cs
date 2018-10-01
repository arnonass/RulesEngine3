using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using System;

namespace WebJobInstrumentation.Logging
{
    /// <summary>
    /// TraceLogger for logging to Application Insights.
    /// </summary>
    public class TraceLogger : ILogger
    {
        private readonly TelemetryClient telemetryClient;

        public TraceLogger(
            string instrumentationKey)
        {
            this.telemetryClient = new TelemetryClient
            {
                InstrumentationKey = instrumentationKey
            };
        }

        #region Start/stop operation

        public IOperationHolder<DependencyTelemetry> StartOperation(string operationName)
        {
            return this.telemetryClient.StartOperation<DependencyTelemetry>(operationName);
        }

        public IOperationHolder<DependencyTelemetry> StartOperation(DependencyTelemetry operationTelemetry)
        {
            return this.telemetryClient.StartOperation<DependencyTelemetry>(operationTelemetry);
        }

        public void StopOperation(IOperationHolder<DependencyTelemetry> operation)
        {
            this.telemetryClient.StopOperation<DependencyTelemetry>(operation);
        }

        #endregion

        #region Log by severity level

        public void LogInformation(string message)
        {
            telemetryClient.TrackTrace(message, SeverityLevel.Information);
        }

        public void LogInformation(string fmt, params object[] vars)
        {
            telemetryClient.TrackTrace(string.Format(fmt, vars), SeverityLevel.Information);
        }

        public void LogInformation(Exception exception, string fmt, params object[] vars)
        {
            var telemetry = new TraceTelemetry(string.Format(fmt, vars), SeverityLevel.Information);
            //telemetry.Properties.Add("Exception", ExceptionUtilities.FormatException(exception, includeContext: true));

            telemetryClient.TrackTrace(telemetry);
        }

        public void LogWarning(string message)
        {
            telemetryClient.TrackTrace(message, SeverityLevel.Warning);
        }

        public void LogWarning(string fmt, params object[] vars)
        {
            telemetryClient.TrackTrace(string.Format(fmt, vars), SeverityLevel.Warning);
        }

        public void LogWarning(Exception exception, string fmt, params object[] vars)
        {
            var telemetry = new TraceTelemetry(string.Format(fmt, vars), SeverityLevel.Warning);
            //telemetry.Properties.Add("Exception", ExceptionUtilities.FormatException(exception, includeContext: true));

            telemetryClient.TrackTrace(telemetry);
        }

        public void LogError(string message)
        {
            telemetryClient.TrackTrace(message, SeverityLevel.Error);
        }

        public void LogError(string fmt, params object[] vars)
        {
            telemetryClient.TrackTrace(string.Format(fmt, vars), SeverityLevel.Error);
        }

        public void LogError(Exception exception, string fmt, params object[] vars)
        {
            var telemetry = new ExceptionTelemetry(exception)
            {
                SeverityLevel = SeverityLevel.Error
            };
            telemetry.Properties.Add("message", string.Format(fmt, vars));

            telemetryClient.TrackException(telemetry);
        }

        public void LogCritical(string message)
        {
            telemetryClient.TrackTrace(message, SeverityLevel.Critical);
        }

        public void LogCritical(string fmt, params object[] vars)
        {
            telemetryClient.TrackTrace(string.Format(fmt, vars), SeverityLevel.Critical);
        }

        public void LogCritical(Exception exception, string fmt, params object[] vars)
        {
            var telemetry = new ExceptionTelemetry(exception)
            {
                SeverityLevel = SeverityLevel.Critical
            };
            telemetry.Properties.Add("message", string.Format(fmt, vars));

            telemetryClient.TrackException(telemetry);
        }

        #endregion

        #region Tracing

        public void TraceApi(string componentName, string method, TimeSpan timespan)
        {
            TraceApi(componentName, method, timespan, string.Empty);
        }

        public void TraceApi(string componentName, string method, TimeSpan timespan, string properties)
        {
            var telemetry = new TraceTelemetry("Trace component call", SeverityLevel.Verbose);
            telemetry.Properties.Add("component", componentName);
            telemetry.Properties.Add("method", method);
            telemetry.Properties.Add("timespan", timespan.ToString());

            if (!string.IsNullOrWhiteSpace(properties))
            {
                telemetry.Properties.Add("properties", properties);
            }

            telemetryClient.TrackTrace(telemetry);
        }

        public void TraceApi(string componentName, string method, TimeSpan timespan, string fmt, params object[] vars)
        {
            TraceApi(componentName, method, timespan, string.Format(fmt, vars));
        }

        #endregion

        #region Default ILogger implementation

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel != LogLevel.None)
            {
                var severityLevel = SeverityLevel.Verbose;
                switch (logLevel)
                {
                    case LogLevel.Critical:
                        severityLevel = SeverityLevel.Critical;
                        break;
                    case LogLevel.Error:
                        severityLevel = SeverityLevel.Error;
                        break;
                    case LogLevel.Warning:
                        severityLevel = SeverityLevel.Warning;
                        break;
                    case LogLevel.Debug:
                    case LogLevel.Information:
                        severityLevel = SeverityLevel.Information;
                        break;
                    case LogLevel.Trace:
                    default:
                        severityLevel = SeverityLevel.Verbose;
                        break;
                }

                if (formatter != null)
                {
                    string message = formatter(state, exception);

                    telemetryClient.TrackTrace(message, severityLevel);
                }
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        #endregion
    }
}
