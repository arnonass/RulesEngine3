using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using RulesService.Model;
using RulesService.ServiceBus;
using WebJobInstrumentation.Telemetry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using WebJobInstrumentation.MessageFlow;

namespace RulesService
{
    public static class Initialize
    {
        private const string queueName = "eventqueue";
        private const string clearCacheQueueName = "clearcachequeue";
        private static readonly QueueManager queueManager;
        private static readonly QueueManager clearCacheQueueManager;
        private static readonly string sourceBatchIdKey;
        private static readonly string operationParentIdKey;
        private static readonly string operationIdKey;

        static Initialize()
        {
            queueManager = new QueueManager(queueName);
            clearCacheQueueManager = new QueueManager(clearCacheQueueName);
            sourceBatchIdKey = ConfigurationManager.AppSettings["SourceBatchIdKey"].ToString();
            operationParentIdKey = ConfigurationManager.AppSettings["OperationParentIdKey"].ToString();
            operationIdKey = ConfigurationManager.AppSettings["OperationIdKey"].ToString();
        }

        public async static Task PublishEvent([TimerTrigger("00:00:03", RunOnStartup = true)]TimerInfo myTimer, ILogger logger)
        {
            CorrelationManager.SetStartTrace();

            string requestId = Guid.NewGuid().ToString();
            CorrelationManager.SetRequestId(requestId);

            string operationId = Guid.NewGuid().ToString();
            CorrelationManager.SetOperationId(operationId);

            string operationParentId = Guid.NewGuid().ToString();
            CorrelationManager.SetOperationParentId(operationParentId);

            string sourceBatchId = Guid.NewGuid().ToString();
            CorrelationManager.SetSourceBatchId(sourceBatchId);

            try
            {
                //var myEvent = new Event
                //{
                //    EventId = 1000,
                //    EventName = "Event1000",
                //    EventType = "Claim",
                //    Priority = 1,
                //    Properties = new Dictionary<string, string>
                //    {
                //        { "prop1", "value1" },
                //        { "prop2", "value2" }
                //    }
                //};

                var events = GetEventsCollection();
                Random random = new Random();
                int index = random.Next(0, 11);
                var myEvent = events[index];

                var eventMessage = new BrokeredMessage(myEvent);
                eventMessage.Properties["EventType"] = myEvent.EventType;
                CorrelationManager.SetCorrelation(eventMessage);

                if (myEvent.EventName == "ClearCache")
                {
                    await clearCacheQueueManager.SendToQueueAsync(eventMessage);
                }
                else
                {
                    await queueManager.SendToQueueAsync(eventMessage);
                }

                logger.LogInformation($"Sent {myEvent.EventType} event with ID {myEvent.EventId}.");
            }
            catch (Exception ex)
            {
                logger.LogCritical($"Failed to send event! {ex.Message}");
            }
        }

        private static List<Event> GetEventsCollection()
        {
            var events = new List<Event>
            {
                new Event
                {
                    EventId = 100,
                    EventName = "ClearCache",
                    EventType = "Other"
                },
                new Event
                {
                    EventId = 101,
                    EventName = "ClearCache",
                    EventType = "Claim"
                },
                new Event
                {
                    EventId = 998,
                    EventName = "Event998",
                    EventType = "Bar",
                    Priority = 1,
                    Properties = new Dictionary<string, string>
                    {
                        { "prop10", "value10" },
                        { "prop20", "value20" }
                    }
                },
                new Event
                {
                    EventId = 999,
                    EventName = "Event999",
                    EventType = "Foo",
                    Priority = 1,
                    Properties = new Dictionary<string, string>
                    {
                        { "prop10", "value10" },
                        { "prop20", "value20" }
                    }
                },
                new Event
                {
                    EventId = 1000,
                    EventName = "Event1000",
                    EventType = "Claim",
                    Priority = 1,
                    Properties = new Dictionary<string, string>
                    {
                        { "prop1", "value1" },
                        { "prop2", "value2" }
                    }
                },
                new Event
                {
                    EventId = 1001,
                    EventName = "Event1001",
                    EventType = "Claim",
                    Priority = 1,
                    Properties = new Dictionary<string, string>
                    {
                        { "prop1", "value10" },
                        { "prop2", "value2" }
                    }
                },
                new Event
                {
                    EventId = 1002,
                    EventName = "Event1002",
                    EventType = "Claim",
                    Priority = 2,
                    Properties = new Dictionary<string, string>
                    {
                        { "prop10", "value1" },
                        { "prop2", "value2" }
                    }
                },
                new Event
                {
                    EventId = 1003,
                    EventName = "Event1003",
                    EventType = "Claim",
                    Priority = 1,
                    Properties = new Dictionary<string, string>
                    {
                        { "prop1", "value1" },
                        { "prop2", "value2" }
                    }
                },
                new Event
                {
                    EventId = 1004,
                    EventName = "Event1004",
                    EventType = "Other",
                    Priority = 1,
                    Properties = new Dictionary<string, string>
                    {
                        { "prop10", "value10" },
                        { "prop20", "value20" }
                    }
                },
                new Event
                {
                    EventId = 1005,
                    EventName = "Event1005",
                    EventType = "Other",
                    Priority = 3,
                    Properties = new Dictionary<string, string>
                    {
                        { "prop10", "value10" },
                        { "prop20", "value20" }
                    }
                },
                new Event
                {
                    EventId = 1006,
                    EventName = "Event1006",
                    EventType = "Other",
                    Priority = 1,
                    Properties = new Dictionary<string, string>
                    {
                        { "prop1", "value1" },
                        { "prop20", "value20" }
                    }
                },
                new Event
                {
                    EventId = 1007,
                    EventName = "Event1007",
                    EventType = "Other",
                    Priority = 1,
                    Properties = new Dictionary<string, string>
                    {
                        { "prop10", "value10" },
                        { "prop2", "value20" }
                    }
                },
            };

            return events;
        }
    }
}
