using System.Collections.Generic;
using RulesService.Model;

namespace RulesService
{
    public interface IRulesCollection
    {
        IEnumerable<Rule<Event>> GetRulesCollection(string eventType);
    }
}