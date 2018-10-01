using Microsoft.ServiceBus.Messaging;
using RulesService.Exceptions;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace RulesService.ServiceBus
{
    public class QueueManager : IQueueManager
    {
        private const string serviceBusConnectionStringKey = "RulesEngineServiceBusConnectionString";
        private readonly QueueClient queueClient;
        private readonly string queueName;

        public QueueManager(
            string queueName)
        {
            this.queueName = queueName;
            string serviceBusConnectionString = ConfigurationManager.ConnectionStrings[serviceBusConnectionStringKey].ConnectionString;
            string queueConnectionString = $"{serviceBusConnectionString};EntityPath={queueName}";
            this.queueClient = QueueClient.CreateFromConnectionString(queueConnectionString);
        }

        public QueueManager(
            QueueClient queueClient)
        {
            this.queueClient = queueClient;
        }

        public async Task SendToQueueAsync(
            BrokeredMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            try
            {
                await this.queueClient.SendAsync(message);
            }
            catch (Exception ex)
            {
                // Throw service exception.
                string customErrorMessage = $"Unable to send message {message.MessageId} to queue {this.queueName} after retries.";
                throw new ServiceException(customErrorMessage, ex);
            }
        }
    }
}
