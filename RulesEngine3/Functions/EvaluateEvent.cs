using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using RulesService.Exceptions;
using RulesService.Model;
using RulesService.ServiceBus;
using RulesService.Storage;
using System;
using System.Configuration;
using System.Threading.Tasks;
using WebJobInstrumentation.Telemetry;

namespace RulesService
{
    public class Process
    {
        private const string serviceBusConnectionStringKey = "RulesEngineServiceBusConnectionString";
        private const string queueName = "eventqueue";
        private const string topicName = "eventmatchedtopic";
        private static TopicManager topicManager;
        private static readonly StorageManager<RuleBase> storageManager;
        private const string rulesTable = "RxTestRules";
        private static readonly string sourceBatchIdKey;
        private static readonly string operationParentIdKey;
        private static readonly string operationIdKey;

        static Process()
        {
            topicManager = new TopicManager(topicName);
            storageManager = new StorageManager<RuleBase>(rulesTable);
            sourceBatchIdKey = ConfigurationManager.AppSettings["SourceBatchIdKey"].ToString();
            operationParentIdKey = ConfigurationManager.AppSettings["OperationParentIdKey"].ToString();
            operationIdKey = ConfigurationManager.AppSettings["OperationIdKey"].ToString();
        }

        [Singleton(Mode = SingletonMode.Function)]
        public static async Task EvaluateEvent([ServiceBusTrigger(queueName, AccessRights.Manage, Connection = serviceBusConnectionStringKey)]
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

                // Retrieve event and event type from the message.
                var myEvent = eventMessage.GetBody<Event>();
                string eventType = myEvent.EventType;

                // Load rules.
                var rulesCollection = new RulesCollection(logger, storageManager);
                var rules = rulesCollection.GetRulesCollection(eventType);

                // Evaluate event by rules.
                var eventEvaluator = new EventEvaluator(logger);
                bool isValid = eventEvaluator.EvaluateEvent(myEvent, rules);

                // If true, send to Event Matched Topic.
                if (isValid)
                {
                    var eventEvaluatedMessage = eventMessage.Clone();
                    eventEvaluatedMessage.Properties["EventType"] = eventType;
                    CorrelationManager.SetCorrelation(eventEvaluatedMessage);

                    await topicManager.SendToTopicAsync(eventEvaluatedMessage);

                    logger.LogInformation($"Approved {eventType} event with ID {myEvent.EventId}.");
                }
                else
                {
                    // Log exit trace.
                    CorrelationManager.SetStopTrace();
                    logger.LogInformation($"Rejected {eventType} event with ID {myEvent.EventId}.");
                }
            }
            catch (ArgumentNullException ex)
            {
                // Log critical error and continue.
                CorrelationManager.SetStopTrace();
                logger.LogCritical($"Failed to evaluate event for message with Message ID: {eventMessage.MessageId}. {ex.Message}");
            }
            catch (ServiceException ex)
            {
                // Log critical error and continue.
                CorrelationManager.SetStopTrace();
                logger.LogCritical($"Failed to evaluate event for message with Message ID: {eventMessage.MessageId}. {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log critical error and continue.
                CorrelationManager.SetStopTrace();
                logger.LogCritical($"Failed to evaluate event for message with Message ID: {eventMessage.MessageId}. {ex.Message}");
            }
        }
    }
}
