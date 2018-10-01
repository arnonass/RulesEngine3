using Microsoft.Azure.ServiceBus;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubscriptionFilters
{
    public class SubscriptionManager
    {
        private readonly SubscriptionClient subscriptionClient;

        public SubscriptionManager(
            string serviceBusConnectionString,
            string topicName, 
            string subscriptionName)
        {
            this.subscriptionClient = new SubscriptionClient(serviceBusConnectionString, topicName, subscriptionName);
        }

        public async Task RemoveCorrelationFilter(string name)
        {
            await this.subscriptionClient.RemoveRuleAsync(name);
        }

        public async Task AddCorrelationFilter(
            string propertyName,
            string propertyValue)
        {
            var customerPropertyFilter = new CorrelationFilter();
            customerPropertyFilter.Properties.Add(propertyName, propertyValue);

            await this.subscriptionClient.AddRuleAsync(new RuleDescription
            {
                Filter = customerPropertyFilter,
                Name = $"{propertyName}Filter"
            });
        }

        public async Task<IEnumerable<RuleDescription>> GetSubscriptionRules()
        {
            return await this.subscriptionClient.GetRulesAsync();
        }
    }
}
