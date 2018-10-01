using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using RulesService.ServiceBus;
using System;
using System.Threading.Tasks;

namespace WebJobInstrumentation.MessageFlow.Pingers
{
    public class ServiceBusQueuePinger : IPinger
    {
        private readonly ILogger logger;

        public ServiceBusQueuePinger(ILogger logger)
        {
            this.logger = logger;
        }

        public bool Ping(string queueName)
        {
            return PingAsync(queueName).Result;
        }

        private async Task<bool> PingAsync(string queueName)
        {
            bool pingable = false;

            try
            {
                var pingMessage = new BrokeredMessage
                {
                    ContentType = "application/vnd.ms-servicebus-ping",
                    TimeToLive = TimeSpan.FromSeconds(1)
                };

                var queueManager = new QueueManager(queueName);
                await queueManager.SendToQueueAsync(pingMessage);

                pingable = true;
            }
            catch (Exception ex)
            {
                // Log exception and continue.
                logger.LogCritical(ex, $"404 - Queue {queueName} is unavailable.");
            }

            return pingable;
        }
    }
}
