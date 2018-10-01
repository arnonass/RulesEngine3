using Microsoft.WindowsAzure.Storage.Table;
using RulesService.Exceptions;
using System;
using System.Threading.Tasks;

namespace RulesService.Storage
{
    public class Table : ITable
    {
        private readonly CloudTable cloudTable;
        private readonly string tableName;

        public Table() { }

        public Table(CloudTable cloudTable)
        {
            this.cloudTable = cloudTable;
            this.tableName = cloudTable.Name;
        }

        public async Task<bool> CreateIfNotExistsAsync()
        {
            try
            {
                return await this.cloudTable.CreateIfNotExistsAsync();
            }
            catch (Exception ex)
            {
                // Throw service exception.
                string customErrorMessage = $"Unable to create cloud table {this.tableName} if not exist.";
                throw new ServiceException(customErrorMessage, ex);
            }
        }

        public async Task<TableQuerySegment<DynamicTableEntity>> ExecuteQuerySegmentedAsync(
            TableQuery tableQuery, 
            TableContinuationToken token)
        {
            if (tableQuery == null)
            {
                throw new ArgumentNullException("tableQuery");
            }

            try
            {
                return await this.cloudTable.ExecuteQuerySegmentedAsync(tableQuery, token);
            }
            catch (Exception ex)
            {
                // Throw service exception.
                string customErrorMessage = $"Unable to execute segmented query against cloud table {this.tableName}.";
                throw new ServiceException(customErrorMessage, ex);
            }
        }

        public async Task<TableResult> ExecuteAsync(
            TableOperation tableOperation)
        {
            if (tableOperation == null)
            {
                throw new ArgumentNullException("tableOperation");
            }

            try
            {
                return await this.cloudTable.ExecuteAsync(tableOperation);
            }
            catch (Exception ex)
            {
                // Throw service exception.
                string customErrorMessage = $"Unable to execute table operation {tableOperation.OperationType.ToString()} against cloud table {this.tableName}.";
                throw new ServiceException(customErrorMessage, ex);
            }
        }
    }
}
