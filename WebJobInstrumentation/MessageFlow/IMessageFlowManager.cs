using System.Threading.Tasks;

namespace WebJobInstrumentation.MessageFlow
{
    public interface IMessageFlowManager
    {
        Task ControlQueueMessageFlowAsync(string queueName, string endpoint);
        Task ControlSubscriptionMessageFlowAsync(string topicName, string subscriptionName, string endpoint);
    }
}