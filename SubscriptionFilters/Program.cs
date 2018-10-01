using System;
using System.Threading.Tasks;

namespace SubscriptionFilters
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        { 
            string serviceBusConnectionString = "Endpoint=sb://dev-mobile-sb-w2-01.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=QHG8rei3cx9qVVULFG9mDSbIBDdkdCZOCDuDiM/npJM=";
            string topicName = "eventmatchedtopic";
            string subscriptionName = "eventmatchedsub001";
            string propertyName = "EventType";
            string propertyValue = "Claim";

            await AddCorrelationFilter(serviceBusConnectionString, topicName, subscriptionName, propertyName, propertyValue);

            subscriptionName = "eventmatchedsub002";
            propertyValue = "EnrichedClaim";

            await AddCorrelationFilter(serviceBusConnectionString, topicName, subscriptionName, propertyName, propertyValue);

            Console.Read();
        }

        private static async Task AddCorrelationFilter(
            string serviceBusConnectionString,
            string topicName,
            string subscriptionName,
            string propertyName,
            string propertyValue)
        {
            var subscriptionManager = new SubscriptionManager(
                serviceBusConnectionString,
                topicName,
                subscriptionName);

            var subscriptionRules = await subscriptionManager.GetSubscriptionRules();
            var enumerator = subscriptionRules.GetEnumerator();
            while (enumerator.MoveNext())
            {
                await subscriptionManager.RemoveCorrelationFilter(enumerator.Current.Name);
            }

            await subscriptionManager.AddCorrelationFilter(propertyName, propertyValue);

            subscriptionRules = await subscriptionManager.GetSubscriptionRules();
            enumerator = subscriptionRules.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine($"{subscriptionName} {enumerator.Current.Name} {enumerator.Current.Filter.ToString()}");
            }
        }
    }
}
