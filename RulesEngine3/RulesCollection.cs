using Microsoft.Extensions.Logging;
using RulesService.Caching;
using RulesService.Model;
using RulesService.Storage;
using System;
using System.Collections.Generic;

namespace RulesService
{
    public class RulesCollection : IRulesCollection
    {
        private readonly IStorageManager<RuleBase> storageManager;
        private readonly ICache<List<Rule<Event>>> cache;
        private readonly ILogger logger;

        private const string RuleId = "RuleId";
        private const string RuleName = "RuleName";
        private const string Rank = "Rank";
        private const string MatchExpression = "MatchExpression";
        private const string Enabled = "Enabled";
        private const string EventRulesCollection = "EventRulesCollection";

        public RulesCollection(
            ILogger logger,
            IStorageManager<RuleBase> storageManager)
        {
            this.logger = logger;
            this.cache = new Cache<List<Rule<Event>>>();
            this.storageManager = storageManager;
        }

        public RulesCollection(
            ILogger logger,
            ICache<List<Rule<Event>>> cache, 
            IStorageManager<RuleBase> storageManager)
        {
            this.logger = logger;
            this.cache = cache;
            this.storageManager = storageManager;
        }

        /// <summary>
        /// Returns rules collection either from cache or table storage if cache is empty.
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <returns>Rules collection</returns>
        public IEnumerable<Rule<Event>> GetRulesCollection(
            string eventType)
        {
            if (string.IsNullOrWhiteSpace(eventType))
            {
                throw new ArgumentNullException("eventType");
            }

            if (!(this.cache.GetCachedItem(eventType) is List<Rule<Event>> rules))
            {
                rules = new List<Rule<Event>>();

                var tableEntities = this.storageManager.GetByPartitionKeyAsync(eventType).Result;

                foreach (var tableEntity in tableEntities)
                {
                    bool enabled = (bool)tableEntity.Properties[Enabled].BooleanValue;
                    if (enabled)
                    {
                        var rule = new Rule<Event>()
                        {
                            EventType = tableEntity.PartitionKey,
                            RuleId = tableEntity.Properties.ContainsKey(RuleId) ? (int)tableEntity.Properties[RuleId].Int32Value : 0,
                            RuleName = tableEntity.Properties.ContainsKey(RuleName) ? tableEntity.Properties[RuleName].StringValue : string.Empty,
                            Rank = tableEntity.Properties.ContainsKey(Rank) ? (int)tableEntity.Properties[Rank].Int32Value : 0,
                            MatchExpression = tableEntity.Properties.ContainsKey(MatchExpression) ? tableEntity.Properties[MatchExpression].StringValue : string.Empty,
                            Enabled = enabled
                        };

                        rules.Add(rule);
                    }
                }

                this.cache.Add(eventType, rules);

                logger.LogTrace($"{rules.Count} rules for {eventType} event retrieved from storage.");
            }
            else
            {
                logger.LogTrace($"{rules.Count} rules for {eventType} event retrieved from cache.");
            }

            return rules;
        }
    }
}
