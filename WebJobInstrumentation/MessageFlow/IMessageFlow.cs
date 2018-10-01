using System.Threading.Tasks;

namespace WebJobInstrumentation.MessageFlow
{
    public interface IMessageFlow
    {
        Task<bool> QueueDisableAsync(string queueName);
        Task<bool> QueueDisableReceiveAsync(string queueName);
        Task<bool> QueueDisableSendAsync(string queueName);
        Task<bool> QueueEnableAsync(string queueName);
        Task<bool> SubscriptionDisableAsync(string topicName, string subsriptionName);
        Task<bool> SubscriptionDisableReceiveAsync(string topicName, string subsriptionName);
        Task<bool> SubscriptionDisableSendAsync(string topicName, string subsriptionName);
        Task<bool> SubscriptionEnableAsync(string topicName, string subsriptionName);
    }
}