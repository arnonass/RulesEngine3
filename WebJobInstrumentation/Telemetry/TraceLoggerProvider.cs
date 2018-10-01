using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using WebJobInstrumentation.Logging;

namespace WebJobInstrumentation.Telemetry
{
    /// <summary>
    /// TraceLoggerProvider for TraceLogger logging to Application Insights.
    /// </summary>
    public class TraceLoggerProvider : ILoggerProvider
    {
        private readonly string instrumentationKey;

        public TraceLoggerProvider(string instrumentationKey)
        {
            this.instrumentationKey = instrumentationKey;
        }

        public TraceLoggerProvider(EventHandler<TraceLoggerProviderEventArgs> onCreateLogger)
        {
            OnCreateLogger = onCreateLogger;
        }

        public ConcurrentDictionary<string, TraceLogger> Loggers { get; set; } = new ConcurrentDictionary<string, TraceLogger>();

        public ILogger CreateLogger(string categoryName)
        {
            var traceLogger = Loggers.GetOrAdd(categoryName, new TraceLogger(this.instrumentationKey));
            OnCreateLogger?.Invoke(this, new TraceLoggerProviderEventArgs(traceLogger));
            return traceLogger;
        }

        public void Dispose() { }

        public event EventHandler<TraceLoggerProviderEventArgs> OnCreateLogger = delegate { };
    }

    public class TraceLoggerProviderEventArgs
    {
        public TraceLogger TraceLogger { get; }

        public TraceLoggerProviderEventArgs(TraceLogger logger)
        {
            TraceLogger = logger;
        }
    }
}
