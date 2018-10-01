using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using RulesService.Model;
using System;
using WebJobInstrumentation.Telemetry;

namespace RulesService
{
    public static class SecondRoute
    {
        private const string serviceBusConnectionStringKey = "RulesEngineServiceBusConnectionString";
        private const string topicName = "eventmatchedtopic";
        private const string subscriptionName = "eventmatchedsub002";

        public static void RouteEvent([ServiceBusTrigger(topicName, subscriptionName, AccessRights.Manage, Connection = serviceBusConnectionStringKey)]
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

                CorrelationManager.SetStopTrace();
                logger.LogInformation($"Received valid {myEvent.EventType} event with ID {myEvent.EventId}.");
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
