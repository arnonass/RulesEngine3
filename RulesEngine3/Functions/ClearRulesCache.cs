using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using RulesService.Caching;
using RulesService.Model;
using System;
using System.Collections.Generic;
using WebJobInstrumentation.Telemetry;

namespace RulesService
{
    public static class RulesCache
    {
        private const string serviceBusConnectionStringKey = "RulesEngineServiceBusConnectionString";
        private const string queueName = "clearcachequeue";

        public static void ClearCache([ServiceBusTrigger(queueName, AccessRights.Manage, Connection = serviceBusConnectionStringKey)]
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

                if (eventMessage.Properties.ContainsKey("EventType"))
                {
                    string eventType = eventMessage.Properties["EventType"].ToString();

                    var cache = new Cache<List<Rule<Event>>>();
                    cache.Clear(eventType);

                    CorrelationManager.SetStopTrace();
                    logger.LogTrace($"ClearCache request successfully executed. Cache cleared for {eventType} rules.");
                }
                else
                {
                    CorrelationManager.SetStopTrace();
                    logger.LogError($"Unable to  to execute ClearCache request. Property EventType is missing.");
                }
            }
            catch (Exception ex)
            {
                // Log critical error and continue.
                CorrelationManager.SetStopTrace();
                logger.LogCritical($"Failed to execute ClearCache request. {ex.Message}");
            }
        }
    }
}
