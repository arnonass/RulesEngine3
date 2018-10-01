using Microsoft.ServiceBus.Messaging;
using System.Threading.Tasks;

namespace RulesService.ServiceBus
{
    public interface ITopicManager
    {
        Task SendToTopicAsync(BrokeredMessage message);
    }
}