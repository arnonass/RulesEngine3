namespace RulesService.Storage
{
    public interface ITableClient
    {
        Table GetTableReference(string tableName);
    }
}
