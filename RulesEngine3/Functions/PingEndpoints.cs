using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebJobInstrumentation.MessageFlow;
using WebJobInstrumentation.MessageFlow.Pingers;

namespace RulesService.Functions
{
    public static class Ping
    {
        public async static Task PingEndpoints([TimerTrigger("00:00:30", RunOnStartup = true)]TimerInfo myTimer, ILogger logger)
        {
            //// Ping Google endpoint.
            //string endpoint_1 = "google.com";
            //string queueName_1 = "eventqueue";

            //IPinger webAddressPinger = new WebAddressPinger(logger);

            //var messageFlowManager_1 = new MessageFlowManager(webAddressPinger, logger);
            //await messageFlowManager_1.ControlQueueMessageFlowAsync(queueName_1, endpoint_1);

            // Ping eventmatchedtopic endpoint.
            string endpoint_2 = "eventmatchedtopic";
            string queueName_2 = "eventqueue";

            IPinger serviceBusTopicPinger = new ServiceBusTopicPinger(logger);
            var messageFlowManager_2 = new MessageFlowManager(serviceBusTopicPinger, logger);
            await messageFlowManager_2.ControlQueueMessageFlowAsync(queueName_2, endpoint_2);

            // Ping eventqueue endpoint.
            string endpoint_3 = "eventqueue";
            string topicName = "eventmatchedtopic";
            string subscriptionName = "eventmatchedsub001";

            IPinger serviceBusQueuePinger = new ServiceBusQueuePinger(logger);
            var messageFlowManager_3 = new MessageFlowManager(serviceBusQueuePinger, logger);
            await messageFlowManager_3.ControlSubscriptionMessageFlowAsync(topicName, subscriptionName, endpoint_3);
        }
    }
}
