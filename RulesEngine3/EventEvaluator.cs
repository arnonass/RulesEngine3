using Microsoft.Extensions.Logging;
using RulesService.Model;
using System;
using System.Collections.Generic;

namespace RulesService
{
    public class EventEvaluator
    {
        private readonly ILogger logger;

        public EventEvaluator(ILogger logger)
        {
            this.logger = logger;
        }

        public bool EvaluateEvent(
             Event myEvent,
             IEnumerable<Rule<Event>> rules)
        {
            if (myEvent == null)
            {
                throw new ArgumentNullException("myEvent");
            }

            if (rules == null)
            {
                throw new ArgumentNullException("rules");
            }

            // Evaluate event.
            bool isValid = true;
            foreach (var rule in rules)
            {
                try
                {
                    if (!rule.Match(myEvent))
                    {
                        isValid = false;
                        this.logger.LogTrace($"{myEvent.EventType} event with ID {myEvent.EventId} failed rule with ID {rule.RuleId}; {rule.MatchExpression}.");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("The given key was not present in the dictionary."))
                    {
                        // Log warning for missing property on the event and continue with false.
                        this.logger.LogWarning($"{myEvent.EventType} event with ID {myEvent.EventId} is missing at least one property therefore fails validation by rule with ID {rule.RuleId}; {rule.MatchExpression}.");
                    }
                    else if (ex.Message.Contains("Rule invalid with ID"))
                    {
                        // Log critical error for an invalid rule and continue with false.
                        this.logger.LogCritical($"{ex.Message} {myEvent.EventType} events cannot be evaluated until this rule is fixed.");
                    }
                    else
                    {
                        // Log any other error as critical and continue with false.
                        this.logger.LogCritical($"{myEvent.EventType} event with ID {myEvent.EventId} cannot be evaluated. {ex.Message}");
                    }

                    isValid = false;
                    break;
                }
            }

            return isValid;
        }
    }
}
