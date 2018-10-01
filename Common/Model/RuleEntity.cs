using Microsoft.WindowsAzure.Storage.Table;

namespace RulesService.Model
{
    public class RuleEntity : TableEntity
    {
        public RuleEntity(
            string eventType, int rank)
        {
            PartitionKey = eventType;
            RowKey = (rank + 1000).ToString(); ;
        }

        public int RuleId { get; set; }
        public string RuleName { get; set; }
        public int Rank { get; set; }
        public string MatchExpression { get; set; }
        public bool Enabled { get; set; }
    }
}
