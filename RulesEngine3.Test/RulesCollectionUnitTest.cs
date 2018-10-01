using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using RulesService.Caching;
using RulesService.Model;
using RulesService.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RulesService.Test
{
    [TestClass]
    public class RulesCollectionUnitTest
    {
        private ILogger mockLogger;

        [TestInitialize]
        public void Setup()
        {
            mockLogger = new Mock<ILogger>().Object;
        }

        [TestMethod, TestCategory("Unit")]
        public void RulesCollectionShouldReturnRulesCollectionByEventTypeFromCache()
        {
            // Arrange

            const string eventType = "MyEventType";

            var rulesFromCache = new List<Rule<Event>>
            {
                new Rule<Event>
                {
                    RuleId = 1,
                    RuleName = "Rule1",
                    EventType = eventType,
                    MatchExpression = "MatchExpression1",
                    Rank = 1,
                    Enabled = true
                },
                new Rule<Event>
                {
                    RuleId = 2,
                    RuleName = "Rule2",
                    EventType = eventType,
                    MatchExpression = "MatchExpression2",
                    Rank = 2,
                    Enabled = true
                }
            };

            var mockCache = new Mock<ICache<List<Rule<Event>>>>();
            mockCache.Setup(c => c.GetCachedItem(eventType))
                .Returns(rulesFromCache);
            mockCache.Setup(c => c.Add(eventType, It.IsAny<List<Rule<Event>>>()));

            var mockStorageManager = new Mock<IStorageManager<RuleBase>>();
            mockStorageManager.Setup(r => r.GetByPartitionKeyAsync(eventType))
                .Returns(Task.FromResult(Mock.Of<List<DynamicTableEntity>>()));

            // Act

            var rulesCollection = new RulesCollection(mockLogger, mockCache.Object, mockStorageManager.Object);
            var rules = rulesCollection.GetRulesCollection(eventType);

            // Assert

            Assert.IsInstanceOfType(rules, typeof(IEnumerable<Rule<Event>>));
            int i = 0;
            foreach (var rule in rules)
            {
                Assert.AreEqual("MyEventType", rule.EventType);
                i++;
            }
            Assert.AreEqual(2, i);
        }

        [TestMethod, TestCategory("Unit")]
        public void RulesCollectionShouldReturnRulesCollectionByEventTypeFromStorage()
        {
            // Arrange

            const string eventType = "MyEventType";

            var rulesFromStorage = new List<DynamicTableEntity>
            {
                new DynamicTableEntity
                {
                    PartitionKey = eventType,
                    RowKey = "1001",
                    Properties = new Dictionary<string, EntityProperty>
                    {
                        { "RuleId",  new EntityProperty(1) },
                        { "RuleName",  new EntityProperty("Rule100") },
                        { "MatchExpression",  new EntityProperty("MatchExpression1") },
                        { "Rank",  new EntityProperty(1) },
                        { "Enabled", new EntityProperty(true) }
                    }
                },
                new DynamicTableEntity
                {
                    PartitionKey = eventType,
                    RowKey = "1002",
                    Properties = new Dictionary<string, EntityProperty>
                    {
                        { "RuleId",  new EntityProperty(2) },
                        { "RuleName",  new EntityProperty("Rule101") },
                        { "MatchExpression",  new EntityProperty("MatchExpression2") },
                        { "Rank",  new EntityProperty(2) },
                        { "Enabled", new EntityProperty(true) }
                    }
                }
            };

            var mockCache = new Mock<ICache<List<Rule<Event>>>>();
            mockCache.Setup(c => c.GetCachedItem(eventType))
                .Returns(null);
            mockCache.Setup(c => c.Add(eventType, It.IsAny<List<Rule<Event>>>()));

            var mockStorageManager = new Mock<IStorageManager<RuleBase>>();
            mockStorageManager.Setup(r => r.GetByPartitionKeyAsync(eventType))
                .Returns(Task.FromResult(rulesFromStorage));

            // Act

            var rulesCollection = new RulesCollection(mockLogger, mockCache.Object, mockStorageManager.Object);
            var rules = rulesCollection.GetRulesCollection(eventType);

            // Assert

            Assert.IsInstanceOfType(rules, typeof(IEnumerable<Rule<Event>>));
            int i = 0;
            foreach (var rule in rules)
            {
                Assert.AreEqual("MyEventType", rule.EventType);
                i++;
            }
            Assert.AreEqual(2, i);
        }

        [TestMethod, TestCategory("Unit")]
        public void RulesCollectionShouldNotReturnDisabledRulesFromStorage()
        {
            // Arrange

            const string eventType = "MyEventType";

            var rulesFromStorage = new List<DynamicTableEntity>
            {
                new DynamicTableEntity
                {
                    PartitionKey = eventType,
                    RowKey = "1001",
                    Properties = new Dictionary<string, EntityProperty>
                    {
                        { "RuleId",  new EntityProperty(1) },
                        { "RuleName",  new EntityProperty("Rule100") },
                        { "MatchExpression",  new EntityProperty("MatchExpression1") },
                        { "Rank",  new EntityProperty(1) },
                        { "Enabled", new EntityProperty(false) } // Disabled
                    }
                },
                new DynamicTableEntity
                {
                    PartitionKey = eventType,
                    RowKey = "1002",
                    Properties = new Dictionary<string, EntityProperty>
                    {
                        { "RuleId",  new EntityProperty(2) },
                        { "RuleName",  new EntityProperty("Rule101") },
                        { "MatchExpression",  new EntityProperty("MatchExpression2") },
                        { "Rank",  new EntityProperty(2) },
                        { "Enabled", new EntityProperty(false) } // Disabled
                    }
                }
            };

            var mockCache = new Mock<ICache<List<Rule<Event>>>>();
            mockCache.Setup(c => c.GetCachedItem(eventType))
                .Returns(null);
            mockCache.Setup(c => c.Add(eventType, It.IsAny<List<Rule<Event>>>()));

            var mockStorageManager = new Mock<IStorageManager<RuleBase>>();
            mockStorageManager.Setup(r => r.GetByPartitionKeyAsync(eventType))
                .Returns(Task.FromResult(rulesFromStorage));

            // Act

            var rulesCollection = new RulesCollection(mockLogger, mockCache.Object, mockStorageManager.Object);
            var rules = rulesCollection.GetRulesCollection(eventType);

            // Assert

            Assert.IsInstanceOfType(rules, typeof(IEnumerable<Rule<Event>>));
            int i = 0;
            foreach (var rule in rules)
            {
                i++;
            }
            Assert.AreEqual(0, i); // 0 rules are enabled
        }
    }
}
