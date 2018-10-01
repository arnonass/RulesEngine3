using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace RulesService.Storage
{
    public interface ITable
    {
        Task<bool> CreateIfNotExistsAsync();

        Task<TableQuerySegment<DynamicTableEntity>> ExecuteQuerySegmentedAsync(
            TableQuery query,
            TableContinuationToken token);

        Task<TableResult> ExecuteAsync(
            TableOperation operation);
    }
}