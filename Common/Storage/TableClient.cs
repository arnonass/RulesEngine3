using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace RulesService.Storage
{
    public class TableClient : ITableClient
    {
        private readonly CloudTableClient tableClient;

        public TableClient(CloudTableClient tableClient)
        {
            this.tableClient = tableClient;
        }

        public TableClient(CloudStorageAccount storageAccount)
        {
            this.tableClient = storageAccount.CreateCloudTableClient();
        }

        public virtual Table GetTableReference(string tableName)
        {
            var cloudTable = this.tableClient.GetTableReference(tableName);
            return new Table(cloudTable);
        }
    }
}
