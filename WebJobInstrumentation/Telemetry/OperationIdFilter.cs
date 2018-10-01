using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace WebJobInstrumentation.Telemetry
{
    public class OperationIdFilter : IFunctionInvocationFilter
    {
        private readonly string messageNameKey;
        private readonly string sourceBatchIdKey;
        private readonly string operationParentIdKey;
        private readonly string operationIdKey;

        public OperationIdFilter()
        {
            this.messageNameKey = ConfigurationManager.AppSettings["MessageNameKey"].ToString();
            this.sourceBatchIdKey = ConfigurationManager.AppSettings["SourceBatchIdKey"].ToString();
            this.operationParentIdKey = ConfigurationManager.AppSettings["OperationParentIdKey"].ToString();
            this.operationIdKey = ConfigurationManager.AppSettings["OperationIdKey"].ToString();
        }

        public Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            // Set operation name to function name.
            CorrelationManager.SetOperationName(executingContext.FunctionName);

            // Set request ID.
            string requestId = Guid.NewGuid().ToString();
            CorrelationManager.SetRequestId(requestId);
            string logMessage = $"Initialized Request ID {requestId}. ";

            if (executingContext.Arguments.ContainsKey(this.messageNameKey) && 
                executingContext.Arguments[this.messageNameKey] is BrokeredMessage eventMessage)
            {
                // Set Source Batch ID for batch tracing.
                string sourceBatchId = Guid.NewGuid().ToString();
                if (eventMessage.Properties.ContainsKey(this.sourceBatchIdKey))
                {
                    sourceBatchId = eventMessage.Properties[this.sourceBatchIdKey].ToString();
                }
                else
                {
                    logMessage += $"Initialized Source Batch ID {sourceBatchId}. ";
                }

                CorrelationManager.SetSourceBatchId(sourceBatchId);

                // Set Operation ID for end-to-end tracing.
                string operationId = Guid.NewGuid().ToString();
                if (eventMessage.Properties.ContainsKey(this.operationIdKey))
                {
                    operationId = eventMessage.Properties[this.operationIdKey].ToString();
                }
                else
                {
                    logMessage += $"Initialized Operation ID {operationId}. ";
                }

                CorrelationManager.SetOperationId(operationId);

                // Set Operation Parent ID for parent tracing.
                string operationParentId = operationId;
                if (eventMessage.Properties.ContainsKey(this.operationParentIdKey))
                {
                    operationParentId = eventMessage.Properties[this.operationParentIdKey].ToString();
                }
                else
                {
                    logMessage += $"Initialized Operation Parent ID {operationParentId}. ";
                }

                CorrelationManager.SetOperationParentId(operationParentId);
            }

            // Log tracing updates.
            if (!string.IsNullOrEmpty(logMessage))
            {
                executingContext.Logger.LogTrace(logMessage);
            }

            return Task.CompletedTask;
        }
    }
}
