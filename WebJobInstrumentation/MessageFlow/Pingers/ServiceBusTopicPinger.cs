using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using RulesService.ServiceBus;
using System;
using System.Threading.Tasks;

namespace WebJobInstrumentation.MessageFlow.Pingers
{
    public class ServiceBusTopicPinger : IPinger
    {
        private readonly ILogger logger;

        public ServiceBusTopicPinger(ILogger logger)
        {
            this.logger = logger;
        }

        public bool Ping(string topicName)
        {
            return PingAsync(topicName).Result;
        }

        private async Task<bool> PingAsync(string topicName)
        {
            bool pingable = false;

            try
            {
                var pingMessage = new BrokeredMessage
                {
                    ContentType = "application/vnd.ms-servicebus-ping",
                    TimeToLive = TimeSpan.FromSeconds(1)
                };

                var topicManager = new TopicManager(topicName);
                await topicManager.SendToTopicAsync(pingMessage);

                pingable = true;
            }
            catch (Exception ex)
            {
                // Log exception and continue.
                logger.LogCritical(ex, $"404 - Topic {topicName} is unavailable.");
            }

            return pingable;
        }
    }
}
