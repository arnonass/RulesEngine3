using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Threading.Tasks;
using System.Timers;
using WebJobInstrumentation.MessageFlow.Pingers;

namespace WebJobInstrumentation.MessageFlow
{
    public class MessageFlowManager : IMessageFlowManager
    {
        private readonly int maxOutageInSeconds;
        private static readonly ConcurrentDictionary<string, Timer> outageTimers;
        private readonly MessageFlow messageFlow;
        private readonly IPinger pinger;
        private readonly ILogger logger;

        static MessageFlowManager()
        {
            outageTimers = new ConcurrentDictionary<string, Timer>();
        }

        public MessageFlowManager(
            IPinger pinger,
            ILogger logger)
        {
            this.logger = logger;
            this.pinger = pinger;
            this.messageFlow = new MessageFlow(logger);
            this.maxOutageInSeconds = 60;
            if (!Int32.TryParse(ConfigurationManager.AppSettings["MaxOutageInSeconds"], out maxOutageInSeconds))
            {
                logger.LogCritical($"Invalid configuration setting for MaxOutageInSeconds. Default value 60 applies.");
            }
        }

        public async Task ControlQueueMessageFlowAsync(
            string queueName,
            string endpoint)
        {
            try
            {
                if (pinger.Ping(endpoint))
                {
                    if (await messageFlow.QueueEnableAsync(queueName) &&
                        outageTimers.ContainsKey(queueName) &&
                        outageTimers.TryRemove(queueName, out Timer outageTimer))
                    {
                        outageTimer.Dispose();
                    }
                }
                else
                {
                    if (!outageTimers.ContainsKey(queueName))
                    {
                        int maxOutageInMilliSeconds = maxOutageInSeconds * 1000;

                        var outageTimer = new Timer(maxOutageInMilliSeconds);
                        outageTimer.Elapsed += async (sender, e) => await OnTimedOutageQueueAsync(queueName, pinger, endpoint);
                        outageTimer.Start();
                        outageTimers[queueName] = outageTimer;

                        logger.LogCritical($"404 - Endpoint {endpoint} is unavailable. If this lasts longer than {maxOutageInMilliSeconds} seconds, queue {queueName} will postpone sending messages.");
                    }
                    else
                    {
                        logger.LogCritical($"404 - Endpoint {endpoint} is unavailable. Queue {queueName} has postponed sending messages.");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, $"Failed to validate message flow is healthy for queue {queueName}.");
            }
        }

        private async Task OnTimedOutageQueueAsync(
            string queueName,
            IPinger pinger,
            string endpoint)
        {
            if (!pinger.Ping(endpoint) && 
                await messageFlow.QueueDisableSendAsync(queueName) && 
                outageTimers.TryRemove(queueName, out Timer outageTimer))
            {
                outageTimer.Dispose();
            }
        }

        public async Task ControlSubscriptionMessageFlowAsync(
            string topicName,
            string subscriptionName,
            string endpoint)
        {
            try
            {
                string topicSubscriptionName = $"{topicName}_{subscriptionName}";

                if (pinger.Ping(endpoint))
                {
                    if (await messageFlow.SubscriptionEnableAsync(topicName, subscriptionName) &&
                        outageTimers.ContainsKey(topicSubscriptionName) &&
                        outageTimers.TryRemove(topicSubscriptionName, out Timer outageTimer))
                    {
                        outageTimer.Dispose();
                    }
                }
                else
                {
                    if (!outageTimers.ContainsKey(topicSubscriptionName))
                    {
                        int maxOutageInMilliSeconds = maxOutageInSeconds * 1000;

                        var outageTimer = new Timer(maxOutageInMilliSeconds);
                        outageTimer.Elapsed += async (sender, e) => await OnTimedOutageSubscriptionAsync(topicName, subscriptionName, pinger, endpoint);
                        outageTimer.Start();
                        outageTimers[topicSubscriptionName] = outageTimer;

                        logger.LogCritical($"404 - Endpoint {endpoint} is unavailable. If this lasts longer than {maxOutageInMilliSeconds} seconds, subscription {subscriptionName} for topic {topicName} will postpone sending messages.");
                    }
                    else
                    {
                        logger.LogCritical($"404 - Endpoint {endpoint} is unavailable. Subscription {subscriptionName} for topic {topicName} has postponed sending messages.");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, $"Failed to validate message flow is healthy for subscription {subscriptionName} for topic {topicName}.");
            }
        }

        private async Task OnTimedOutageSubscriptionAsync(
            string topicName, 
            string subscriptionName,
            IPinger pinger,
            string endpoint)
        {
            if (!pinger.Ping(endpoint) &&
                await messageFlow.SubscriptionDisableSendAsync(topicName, subscriptionName) &&
                outageTimers.TryRemove($"{topicName}_{subscriptionName}", out Timer outageTimer))
            {
                outageTimer.Dispose();
            }
        }
    }
}
