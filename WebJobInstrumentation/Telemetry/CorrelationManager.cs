using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;
using System.Runtime.Remoting.Messaging;

namespace WebJobInstrumentation.Telemetry
{
    public static class CorrelationManager
    {
        private static readonly string operationNameKey;
        private static readonly string requestIdKey;
        private static readonly string operationIdKey;
        private static readonly string operationParentIdKey;
        private static readonly string sourceBatchIdKey;

        static CorrelationManager()
        {
            operationNameKey = ConfigurationManager.AppSettings["OperationNameKey"].ToString();
            requestIdKey = ConfigurationManager.AppSettings["RequestIdKey"].ToString();
            operationIdKey = ConfigurationManager.AppSettings["OperationIdKey"].ToString();
            operationParentIdKey = ConfigurationManager.AppSettings["OperationParentIdKey"].ToString();
            sourceBatchIdKey = ConfigurationManager.AppSettings["SourceBatchIdKey"].ToString();
        }

        public static void SetStartTrace()
        {
            CallContext.LogicalSetData("StartTrace", "StartTrace");
        }

        public static string GetStartTrace()
        {
            string startTrace = null;
            if (CallContext.LogicalGetData("StartTrace") != null)
            {
                startTrace = GetOperationId();
                CallContext.LogicalSetData("StartTrace", null);
            }
            return startTrace;
        }

        public static void SetStopTrace()
        {
            CallContext.LogicalSetData("StopTrace", "StopTrace");
        }

        public static string GetStopTrace()
        {
            string stopTrace = null;
            if (CallContext.LogicalGetData("StopTrace") != null)
            {
                stopTrace = GetOperationId();
                CallContext.LogicalSetData("StopTrace", null);
            }
            return stopTrace;
        }

        public static void SetOperationName(string operationName)
        {
            CallContext.LogicalSetData(operationNameKey, operationName);
        }

        public static string GetOperationName()
        {
            return CallContext.LogicalGetData(operationNameKey) as string;
        }

        public static void SetRequestId(string requestId)
        {
            CallContext.LogicalSetData(requestIdKey, requestId);
        }

        public static string GetRequestId()
        {
            var requestId = CallContext.LogicalGetData(requestIdKey) as string;
            return requestId ?? Guid.NewGuid().ToString();
        }

        public static void SetOperationId(string operationId)
        {
            CallContext.LogicalSetData(operationIdKey, operationId);
        }

        public static string GetOperationId()
        {
            var operationId = CallContext.LogicalGetData(operationIdKey) as string;
            return operationId ?? Guid.NewGuid().ToString();
        }

        public static void SetOperationParentId(string operationParentId)
        {
            CallContext.LogicalSetData(operationParentIdKey, operationParentId);
        }

        public static string GetOperationParentId()
        {
            var operationParentId = CallContext.LogicalGetData(operationParentIdKey) as string;
            return operationParentId ?? Guid.NewGuid().ToString();
        }

        public static void SetSourceBatchId(string sourceBatchId)
        {
            CallContext.LogicalSetData(sourceBatchIdKey, sourceBatchId);
        }

        public static string GetSourceBatchId()
        {
            var sourceBatchId = CallContext.LogicalGetData(sourceBatchIdKey) as string;
            return sourceBatchId ?? Guid.NewGuid().ToString();
        }

        public static void SetCorrelation(BrokeredMessage message)
        {
            message.Properties[operationIdKey] = GetOperationId();
            message.Properties[operationParentIdKey] = GetRequestId();
            message.Properties[sourceBatchIdKey] = GetSourceBatchId();
        }
    }
}
