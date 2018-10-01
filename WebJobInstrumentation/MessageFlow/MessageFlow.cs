using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace WebJobInstrumentation.MessageFlow
{
    public class MessageFlow : IMessageFlow
    {
        private const string serviceBusConnectionStringKey = "RulesEngineServiceBusConnectionString";
        private readonly NamespaceManager namespaceManager;
        private readonly ILogger logger;

        public MessageFlow(ILogger logger)
        {
            this.logger = logger;
            string serviceBusConnectionString = ConfigurationManager.ConnectionStrings[serviceBusConnectionStringKey].ConnectionString;
            this.namespaceManager = NamespaceManager.CreateFromConnectionString(serviceBusConnectionString);
        }

        public async Task<bool> QueueEnableAsync(string queueName)
        {
            return await QueueUpdateEntityStatusAsync(queueName, EntityStatus.Active);
        }

        public async Task<bool> QueueDisableAsync(string queueName)
        {
            return await QueueUpdateEntityStatusAsync(queueName, EntityStatus.Disabled);
        }

        public async Task<bool> QueueDisableSendAsync(string queueName)
        {
            return await QueueUpdateEntityStatusAsync(queueName, EntityStatus.SendDisabled);
        }

        public async Task<bool> QueueDisableReceiveAsync(string queueName)
        {
            return await QueueUpdateEntityStatusAsync(queueName, EntityStatus.ReceiveDisabled);
        }

        private async Task<bool> QueueUpdateEntityStatusAsync(
            string queueName,
            EntityStatus entityStatus)
        {
            bool statusChanged = false;

            try
            {
                if (namespaceManager.QueueExists(queueName))
                {
                    var queue = await namespaceManager.GetQueueAsync(queueName);
                    var queueStatus = queue.Status;
                    if (queueStatus != entityStatus)
                    {
                        var queueDescription = new QueueDescription(queueName)
                        {
                            Status = entityStatus
                        };
                        await namespaceManager.UpdateQueueAsync(queueDescription);

                        statusChanged = true;

                        logger.LogInformation($"Queue {queueName} changed status from {queueStatus} to {entityStatus}.");
                    }
                }
                else
                {
                    logger.LogCritical($"Failed to change status for queue {queueName} to {entityStatus}. Queue does not exist.");
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, $"Failed to change status for queue {queueName} to {entityStatus}.");
            }

            return statusChanged;
        }

        public async Task<bool> SubscriptionEnableAsync(
            string topicName,
            string subsriptionName)
        {
            return await SubscriptionUpdateEntityStatusAsync(topicName, subsriptionName, EntityStatus.Active);
        }

        public async Task<bool> SubscriptionDisableAsync(
            string topicName,
            string subsriptionName)
        {
            return await SubscriptionUpdateEntityStatusAsync(topicName, subsriptionName, EntityStatus.Disabled);
        }

        public async Task<bool> SubscriptionDisableSendAsync(
            string topicName,
            string subsriptionName)
        {
            return await SubscriptionUpdateEntityStatusAsync(topicName, subsriptionName, EntityStatus.SendDisabled);
        }

        public async Task<bool> SubscriptionDisableReceiveAsync(
            string topicName,
            string subsriptionName)
        {
            return await SubscriptionUpdateEntityStatusAsync(topicName, subsriptionName, EntityStatus.ReceiveDisabled);
        }

        private async Task<bool> SubscriptionUpdateEntityStatusAsync(
            string topicName,
            string subsriptionName,
            EntityStatus entityStatus)
        {
            bool statusChanged = false;

            try
            {
                if (namespaceManager.SubscriptionExists(topicName, subsriptionName))
                {
                    var subscription = await namespaceManager.GetSubscriptionAsync(topicName, subsriptionName);
                    var subscriptionStatus = subscription.Status;
                    if (subscriptionStatus != entityStatus)
                    {
                        var subscriptionDescription = new SubscriptionDescription(topicName, subsriptionName)
                        {
                            Status = entityStatus
                        };
                        await namespaceManager.UpdateSubscriptionAsync(subscriptionDescription);

                        statusChanged = true;

                        logger.LogInformation($"Subscription {subsriptionName} for topic {topicName} changed status from {subscriptionStatus} to {entityStatus}.");
                    }
                }
                else
                {
                    logger.LogCritical($"Failed to change status for subscription {subsriptionName} for topic {topicName} to {entityStatus}. Subscription does not exist.");
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, $"Failed to change status for subscription {subsriptionName} for topic {topicName} to {entityStatus}.");
            }

            return statusChanged;
        }
    }
}
