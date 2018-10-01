using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace RulesService.Storage
{
    public class StorageManager<T> : IStorageManager<T>
    {
        private readonly ITableClient tableClient;
        private readonly ITable table;

        private const string PartitionKey = "PartitionKey";

        // Default constructor.
        public StorageManager(string tableName)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["RulesEngineStorageAccountConnectionString"].ConnectionString);
            this.tableClient = new TableClient(storageAccount);
            this.table = this.tableClient.GetTableReference(tableName);
        }

        // Constructor for testing purpose.
        public StorageManager(
            ITableClient tableClient, 
            ITable table)
        {
            this.tableClient = tableClient;
            this.table = table;
        }

        /// <summary>
        /// Returns table entities by partition key.
        /// </summary>
        /// <param name="partitionKey">Partition key</param>
        /// <returns>Table entities</returns>
        public async virtual Task<List<DynamicTableEntity>> GetByPartitionKeyAsync(
            string partitionKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentNullException("partitionKey");
            }

            var tableEntities = new List<DynamicTableEntity>();
        
            await this.table.CreateIfNotExistsAsync();

            var query = new TableQuery().Where(TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, partitionKey));

            TableContinuationToken token = null;
            do
            {
                var resultSegment = await this.table.ExecuteQuerySegmentedAsync(query, token);
                if (resultSegment != null)
                {
                    token = resultSegment.ContinuationToken;
                    tableEntities.AddRange(resultSegment.Results);
                }
            } while (token != null);

            return tableEntities;
        }

        /// <summary>
        /// Adds or updates entity in table.
        /// </summary>
        /// <param name="tableEntity">Table entity</param>
        /// <returns>Table result</returns>
        public async virtual Task<TableResult> AddOrUpdateAsync(
            T tableEntity)
        {
            if (tableEntity == null)
            {
                throw new ArgumentNullException("tableEntity");
            }

            await this.table.CreateIfNotExistsAsync();

            var insertOperation = TableOperation.InsertOrReplace((ITableEntity)tableEntity);

            return await this.table.ExecuteAsync(insertOperation);
        }

        /// <summary>
        /// Removes entity from table by keys.
        /// </summary>
        /// <param name="partitionKey">Partition key</param>
        /// <param name="rowKey">Row key</param>
        /// <returns>Table result</returns>
        public async virtual Task<TableResult> DeleteAsync(
            string partitionKey,
            string rowKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentNullException("partitionKey");
            }

            if (string.IsNullOrWhiteSpace(rowKey))
            {
                throw new ArgumentNullException("rowKey");
            }

            await this.table.CreateIfNotExistsAsync();

            var retrieveOperation = TableOperation.Retrieve<TableEntity>(partitionKey, rowKey);
            var result = await this.table.ExecuteAsync(retrieveOperation);
            var deleteEntity = (TableEntity)result.Result;
            if (deleteEntity != null)
            {
                var deleteOperation = TableOperation.Delete(deleteEntity);
                return await this.table.ExecuteAsync(deleteOperation);
            }

            return null;
        }
    }
}
