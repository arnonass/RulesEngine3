using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using RulesService.Model;
using RulesService.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RulesService.Test
{
    [TestClass]
    public class StorageManagerUnitTest
    {
        [TestInitialize]
        public void Init()
        {

        }

        [TestMethod, TestCategory("Unit")]
        public void StorageManagerShouldReturnRulesCollectionByEventType()
        {
            // Arrange

            string tableName = "MyTable";
            string partitionKey = "P1";

            var mockTable = new Mock<ITable>();
            mockTable.Setup(x => x.ExecuteQuerySegmentedAsync(It.IsAny<TableQuery>(), null))
                .Returns(Task.FromResult(default(TableQuerySegment<DynamicTableEntity>)));
            mockTable.Setup(t => t.CreateIfNotExistsAsync())
                .ReturnsAsync(true);

            var mockCloudTable = new Mock<CloudTable>();

            var mockTableClient = new Mock<ITableClient>();
            mockTableClient.Setup(x => x.GetTableReference(tableName))
                .Returns(Mock.Of<Table>());

            // Act

            var storageManager = new StorageManager<RuleEntity>(mockTableClient.Object, mockTable.Object);

            List<DynamicTableEntity> tableEntities = storageManager.GetByPartitionKeyAsync(partitionKey).Result;

            // Assert

            Assert.IsNotNull(tableEntities);
            Assert.IsInstanceOfType(tableEntities, typeof(List<DynamicTableEntity>));
        }

        [TestMethod, TestCategory("Unit")]
        public async Task StorageManagerShouldAddRuleToRulesCollection()
        {
            // Arrange

            string tableName = "MyTable";

            var rule = new RuleEntity(
                "myEventType",
                1)
            {
                RuleId = 1,
                RuleName = "Rule001",
                MatchExpression = "myExpresion",
                Rank = 1,
                Enabled = true
            };

            var mockTable = new Mock<ITable>();
            mockTable.Setup(t => t.CreateIfNotExistsAsync())
                .ReturnsAsync(true);
            mockTable.Setup(t => t.ExecuteAsync(It.IsAny<TableOperation>()))
                .Returns(Task.FromResult(default(TableResult)));

            var mockCloudTable = new Mock<CloudTable>();

            var mockTableClient = new Mock<ITableClient>();
            mockTableClient.Setup(x => x.GetTableReference(tableName))
                .Returns(Mock.Of<Table>());

            // Act

            var storageManager = new StorageManager<RuleEntity>(mockTableClient.Object, mockTable.Object);

            var result = await storageManager.AddOrUpdateAsync(rule);

            // Assert

            Assert.IsNull(result);
        }

        //[TestMethod, TestCategory("Unit")]
        //public async Task StorageManagerShouldDeleteRuleFromRulesCollection()
        //{
        //    // Arrange

        //    string tableName = "MyTable";
        //    string partitionKey = "myPartitionKey";
        //    string rowKey = "myRowKey";

        //    var mockTable = new Mock<ITable>();
        //    mockTable.Setup(t => t.CreateIfNotExistsAsync())
        //        .ReturnsAsync(true);
        //    mockTable.Setup(t => t.ExecuteAsync(It.IsAny<TableOperation>()))
        //        .Returns(Task.FromResult(default(TableResult)));

        //    var mockCloudTable = new Mock<CloudTable>();

        //    var mockTableClient = new Mock<ITableClient>();
        //    mockTableClient.Setup(x => x.GetTableReference(tableName))
        //        .Returns(Mock.Of<Table>());

        //    // Act

        //    var storageManager = new StorageManager<RuleEntity>(mockTableClient.Object, mockTable.Object);

        //    var result = await storageManager.DeleteAsync(partitionKey, rowKey);

        //    // Assert

        //    Assert.IsNull(result);
        //}
    }
}
