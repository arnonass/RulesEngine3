using Microsoft.ServiceBus.Messaging;
using RulesService.Exceptions;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace RulesService.ServiceBus
{
    public class TopicManager : ITopicManager
    {
        private const string serviceBusConnectionStringKey = "RulesEngineServiceBusConnectionString";
        private readonly TopicClient topicClient;
        private readonly string topicName;

        public TopicManager(
            string topicName)
        {
            this.topicName = topicName;
            string serviceBusConnectionString = ConfigurationManager.ConnectionStrings[serviceBusConnectionStringKey].ConnectionString;
            string topicConnectionString = $"{serviceBusConnectionString};EntityPath={topicName}";
            this.topicClient = TopicClient.CreateFromConnectionString(topicConnectionString);
        }

        public TopicManager(
            TopicClient topicClient)
        {
            this.topicClient = topicClient;
        }

        public async Task SendToTopicAsync(
            BrokeredMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            try
            {
                await this.topicClient.SendAsync(message);
            }
            catch (Exception ex)
            {
                // Throw service exception.
                string customErrorMessage = $"Unable to send message {message.MessageId} to queue {this.topicName} after retries.";
                throw new ServiceException(customErrorMessage, ex);
            }
        }
    }
}
