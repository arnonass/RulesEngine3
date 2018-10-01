using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RulesService.Storage
{
    public interface IStorageManager<T>
    {
        Task<TableResult> AddOrUpdateAsync(T tableEntity);
        Task<TableResult> DeleteAsync(string partitionKey, string rowKey);
        Task<List<DynamicTableEntity>> GetByPartitionKeyAsync(string partitionKey);
    }
}