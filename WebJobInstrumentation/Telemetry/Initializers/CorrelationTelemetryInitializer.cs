using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using System.Configuration;

namespace WebJobInstrumentation.Telemetry.Initializers
{
    public class CorrelationTelemetryInitializer : ITelemetryInitializer
    {
        private readonly string sourceBatchIdKey;

        public CorrelationTelemetryInitializer()
        {
            this.sourceBatchIdKey = ConfigurationManager.AppSettings["SourceBatchIdKey"].ToString();
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Operation.Name = CorrelationManager.GetOperationName();
            telemetry.Context.Operation.ParentId = CorrelationManager.GetOperationParentId();
            telemetry.Context.Operation.Id = CorrelationManager.GetOperationId();
            telemetry.Context.Properties["StartTrace"] = CorrelationManager.GetStartTrace();
            telemetry.Context.Properties["StopTrace"] = CorrelationManager.GetStopTrace();
            telemetry.Context.Properties[this.sourceBatchIdKey] = CorrelationManager.GetSourceBatchId();
        }
    }
}
