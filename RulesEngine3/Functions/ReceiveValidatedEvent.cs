using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using RulesService.Model;
using RulesService.ServiceBus;
using System;
using System.Configuration;
using System.Threading.Tasks;
using WebJobInstrumentation.Telemetry;

namespace RulesService
{
    public static class FirstRoute
    {
        private const string serviceBusConnectionStringKey = "RulesEngineServiceBusConnectionString";
        private const string queueName = "eventqueue";
        private const string topicName = "eventmatchedtopic";
        private const string subscriptionName = "eventmatchedsub001";
        private static readonly QueueManager queueManager;
        private static readonly string sourceBatchIdKey;
        private static readonly string operationParentIdKey;

        static FirstRoute()
        {
            queueManager = new QueueManager(queueName);
            sourceBatchIdKey = ConfigurationManager.AppSettings["SourceBatchIdKey"].ToString();
            operationParentIdKey = ConfigurationManager.AppSettings["OperationParentIdKey"].ToString();
        }

        public async static Task RouteEvent([ServiceBusTrigger(topicName, subscriptionName, AccessRights.Manage, Connection = serviceBusConnectionStringKey)]
            BrokeredMessage eventMessage, ILogger logger)
        {
            try
            {
                if (eventMessage == null)
                {
                    throw new ArgumentNullException("eventMessage");
                }

                if (logger == null)
                {
                    throw new ArgumentNullException("logger");
                }

                var myEvent = eventMessage.GetBody<Event>();
                string eventType = $"Enriched{myEvent.EventType}";
                myEvent.EventType = eventType;

                var eventEnrichedMessage = new BrokeredMessage(myEvent);
                eventEnrichedMessage.Properties["EventType"] = eventType;
                CorrelationManager.SetCorrelation(eventEnrichedMessage);

                await queueManager.SendToQueueAsync(eventEnrichedMessage);

                logger.LogInformation($"Received valid {eventType} event with ID {myEvent.EventId}.");
            }
            catch (Exception ex)
            {
                // Log critical error and continue.
                CorrelationManager.SetStopTrace();
                logger.LogCritical($"Failed to receive evaluated event for message with Message ID: {eventMessage.MessageId}. {ex.Message}");
            }
        }
    }
}
