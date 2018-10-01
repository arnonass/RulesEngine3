using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RulesService.Exceptions;
using RulesService.Model;
using System;
using System.Collections.Generic;

namespace RulesService.Test
{
    [TestClass]
    public class EventEvaluatorUnitTest
    {
        private ILogger logger;

        [TestInitialize]
        public void Init()
        {
            logger = new Mock<ILogger>().Object;
        }

        [TestMethod, TestCategory("Unit")]
        public void EventEvaluatorShouldReturnEvaluationTrue()
        {
            // Arrange

            const string eventType = "MyEventType";

            var rules = new List<Rule<Event>>
            {
                new Rule<Event>
                {
                    RuleId = 1,
                    RuleName = "Rule001",
                    EventType = eventType,
                    MatchExpression = "p.Priority == 1",
                    Rank = 1,
                    Enabled = true
                },
                new Rule<Event>
                {
                    RuleId = 2,
                    RuleName = "Rule002",
                    EventType = eventType,
                    MatchExpression = "p.EventName == \"MyEvent100\"",
                    Rank = 2,
                    Enabled = true
                },
                new Rule<Event>
                {
                    RuleId = 3,
                    RuleName = "Rule003",
                    EventType = eventType,
                    MatchExpression = "p.Properties[\"prop1\"] == \"value1\"",
                    Rank = 3,
                    Enabled = true
                },
                new Rule<Event>
                {
                    RuleId = 4,
                    RuleName = "Rule004",
                    EventType = eventType,
                    MatchExpression = "p.Properties[\"prop2\"] == \"value2\"",
                    Rank = 4,
                    Enabled = true
                },
            };

            var myEvent = new Event
            {
                EventId = 100,
                EventName = "MyEvent100",
                EventType = eventType,
                Priority = 1,
                CreatedDate = DateTime.Now,
                Properties = new Dictionary<string, string>
                {
                    { "prop1", "value1" },
                    { "prop2", "value2" }
                }
            };

            // Act

            var eventEvaluator = new EventEvaluator(logger);
            var result = eventEvaluator.EvaluateEvent(myEvent, rules);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod, TestCategory("Unit")]
        public void EventEvaluatorShouldReturnEvaluationFalse()
        {
            // Arrange

            const string eventType = "MyEventType";

            var rules = new List<Rule<Event>>
            {
                new Rule<Event>
                {
                    RuleId = 1,
                    RuleName = "Rule001",
                    EventType = eventType,
                    MatchExpression = "p.Priority == 1",
                    Rank = 1,
                    Enabled = true
                },
                new Rule<Event>
                {
                    RuleId = 2,
                    RuleName = "Rule002",
                    EventType = eventType,
                    MatchExpression = "p.EventName == \"MyEvent100\"",
                    Rank = 2,
                    Enabled = true
                },
                new Rule<Event>
                {
                    RuleId = 3,
                    RuleName = "Rule003",
                    EventType = eventType,
                    MatchExpression = "p.Properties[\"prop1\"] == \"value10\"", // false
                    Rank = 3,
                    Enabled = true
                },
                new Rule<Event>
                {
                    RuleId = 4,
                    RuleName = "Rule004",
                    EventType = eventType,
                    MatchExpression = "p.Properties[\"prop2\"] == \"value2\"",
                    Rank = 4,
                    Enabled = true
                },
            };

            var myEvent = new Event
            {
                EventId = 100,
                EventName = "MyEvent100",
                EventType = eventType,
                Priority = 1,
                CreatedDate = DateTime.Now,
                Properties = new Dictionary<string, string>
                {
                    { "prop1", "value1" },
                    { "prop2", "value2" }
                }
            };

            // Act

            var eventEvaluator = new EventEvaluator(logger);
            var result = eventEvaluator.EvaluateEvent(myEvent, rules);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod, TestCategory("Unit")]
        public void EventEvaluatorInvalidRuleShouldReturnEvaluationFalse()
        {
            // Arrange

            const string eventType = "MyEventType";

            var rules = new List<Rule<Event>>
            {
                new Rule<Event>
                {
                    RuleId = 1,
                    RuleName = "Rule001",
                    EventType = eventType,
                    MatchExpression = "bla", // Invalid rule
                    Rank = 1,
                    Enabled = true
                }
            };

            var myEvent = new Event
            {
                EventId = 100,
                EventName = "MyEvent100",
                EventType = eventType,
                Priority = 1,
                CreatedDate = DateTime.Now,
                Properties = new Dictionary<string, string>
                {
                    { "prop1", "value1" },
                    { "prop2", "value2" }
                }
            };

            // Act

            var eventEvaluator = new EventEvaluator(logger);
            var result = eventEvaluator.EvaluateEvent(myEvent, rules);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod, TestCategory("Unit")]
        public void EventEvaluatorMissingEventPropertyShouldReturnEvaluationFalse()
        {
            // Arrange

            const string eventType = "MyEventType";

            var rules = new List<Rule<Event>>
            {
                new Rule<Event>
                {
                    RuleId = 1,
                    RuleName = "Rule001",
                    EventType = eventType,
                    MatchExpression = "p.Properties[\"prop2\"] == \"value2\"",
                    Rank = 1,
                    Enabled = true
                }
            };

            var myEvent = new Event
            {
                EventId = 100,
                EventName = "MyEvent100",
                EventType = eventType,
                Priority = 1,
                CreatedDate = DateTime.Now,
                Properties = new Dictionary<string, string>
                {
                    { "prop1", "value1" },
                    //{ "prop2", "value2" } Missing property
                }
            };

            // Act

            var eventEvaluator = new EventEvaluator(logger);
            var result = eventEvaluator.EvaluateEvent(myEvent, rules);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod, TestCategory("Unit")]
        public void EventEvaluatorEventNullShouldThrowInvalidInputException()
        {
            // Arrange

            const string eventType = "MyEventType";

            var rules = new List<Rule<Event>>
            {
                new Rule<Event>
                {
                    RuleId = 1,
                    RuleName = "Rule001",
                    EventType = eventType,
                    MatchExpression = "p.Properties[\"prop2\"] == \"value2\"",
                    Rank = 1,
                    Enabled = true
                }
            };

            Event myEvent = null; // myEvent is null

            // Act

            var eventEvaluator = new EventEvaluator(logger);

            try
            {
                var result = eventEvaluator.EvaluateEvent(myEvent, rules);
            }
            catch (ArgumentNullException ex)
            {
                // Assert

                Assert.AreEqual("Value cannot be null.\r\nParameter name: myEvent", ex.Message);
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void EventEvaluatorRulesNullShouldThrowInvalidInputException()
        {
            // Arrange

            const string eventType = "MyEventType";

            List<Rule<Event>> rules = null; // rules is null

            var myEvent = new Event
            {
                EventId = 100,
                EventName = "MyEvent100",
                EventType = eventType,
                Priority = 1,
                CreatedDate = DateTime.Now,
                Properties = new Dictionary<string, string>
                {
                    { "prop1", "value1" },
                    { "prop2", "value2" }
                }
            };

            // Act

            var eventEvaluator = new EventEvaluator(logger);

            try
            {
                var result = eventEvaluator.EvaluateEvent(myEvent, rules);
            }
            catch (ArgumentNullException ex)
            {
                // Assert

                Assert.AreEqual("Value cannot be null.\r\nParameter name: rules", ex.Message);
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void EventEvaluatorNoRulesShouldReturnEvaluationTrue()
        {
            // Arrange

            const string eventType = "MyEventType";

            var rules = new List<Rule<Event>>(); // No rules

            var myEvent = new Event
            {
                EventId = 100,
                EventName = "MyEvent100",
                EventType = eventType,
                Priority = 1,
                CreatedDate = DateTime.Now,
                Properties = new Dictionary<string, string>
                {
                    { "prop1", "value1" },
                    { "prop2", "value2" }
                }
            };

            // Act

            var eventEvaluator = new EventEvaluator(logger);
            var result = eventEvaluator.EvaluateEvent(myEvent, rules);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod, TestCategory("Unit")]
        public void EventEvaluatorRuleWithEmptyMatchExpressionShouldReturnEvaluationTrue()
        {
            // Arrange

            const string eventType = "MyEventType";

            var rules = new List<Rule<Event>>
            {
                new Rule<Event>
                {
                    RuleId = 1,
                    RuleName = "Rule001",
                    EventType = eventType,
                    MatchExpression = "",
                    Rank = 1,
                    Enabled = true
                }
            };

            var myEvent = new Event
            {
                EventId = 100,
                EventName = "MyEvent100",
                EventType = eventType,
                Priority = 1,
                CreatedDate = DateTime.Now,
                Properties = new Dictionary<string, string>
                {
                    { "prop1", "value1" },
                    { "prop2", "value2" }
                }
            };

            // Act

            var eventEvaluator = new EventEvaluator(logger);
            var result = eventEvaluator.EvaluateEvent(myEvent, rules);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod, TestCategory("Unit")]
        public void EventEvaluatorRuleMatchExpressionNullShouldReturnEvaluationTrue()
        {
            // Arrange

            const string eventType = "MyEventType";

            var rules = new List<Rule<Event>>
            {
                new Rule<Event>
                {
                    RuleId = 1,
                    RuleName = "Rule001",
                    EventType = eventType,
                    MatchExpression = null,
                    Rank = 1,
                    Enabled = true
                }
            };

            var myEvent = new Event
            {
                EventId = 100,
                EventName = "MyEvent100",
                EventType = eventType,
                Priority = 1,
                CreatedDate = DateTime.Now,
                Properties = new Dictionary<string, string>
                {
                    { "prop1", "value1" },
                    { "prop2", "value2" }
                }
            };

            // Act

            var eventEvaluator = new EventEvaluator(logger);
            var result = eventEvaluator.EvaluateEvent(myEvent, rules);

            // Assert

            Assert.IsTrue(result);
        }
    }
}
