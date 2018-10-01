using Microsoft.ServiceBus.Messaging;
using System.Threading.Tasks;

namespace RulesService.ServiceBus
{
    public interface IQueueManager
    {
        Task SendToQueueAsync(BrokeredMessage message);
    }
}