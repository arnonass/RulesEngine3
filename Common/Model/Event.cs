using System;
using System.Collections.Generic;

namespace RulesService.Model
{
    public class Event
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        public string EventType { get; set; }

        public int Priority { get; set; }

        public Dictionary<string, string> Properties { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
