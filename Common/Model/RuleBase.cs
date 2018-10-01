namespace RulesService.Model
{
    public class RuleBase
    {
        public int RuleId { get; set; }

        public string RuleName { get; set; }

        public string EventType { get; set; }

        public int Rank { get; set; }

        public string MatchExpression { get; set; }

        public bool Enabled { get; set; }
    }
}
