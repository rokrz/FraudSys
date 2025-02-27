
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FraudSys.Model;
using System.Diagnostics;

namespace FraudSys.Helpers
{
    public class DatabaseManager
    {
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        private static readonly string table_name = "Banco_KRT";
        public static void CreateTable(bool criarTabela)
        {
            if (!criarTabela){
                return;
            }
            var checkForTables = client.ListTablesAsync();
            if (checkForTables.Result.TableNames.Contains(table_name))
            {
                return;
            }
            var request = new CreateTableRequest
            {
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "NumeroAgencia",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "CPF",
                        AttributeType = "S"
                    },
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "NumeroAgencia",
                        KeyType = "HASH" //Partition key
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "CPF",
                        KeyType = "RANGE" //Partition key
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 6
                },
                TableName = table_name
            };
            var response = client.CreateTableAsync(request);

            WaitUntilTableReady(table_name);
        }
        private static void WaitUntilTableReady(string tableName)
        {
            string status = null;
            // Let us wait until table is created. Call DescribeTable.
            do
            {
                System.Threading.Thread.Sleep(5000); // Wait 5 seconds.
                try
                {
                    var res = client.DescribeTableAsync(new DescribeTableRequest
                    {
                        TableName = tableName
                    });
                    status = res.Result.Table.TableStatus;
                }
                catch (ResourceNotFoundException)
                {
                    // DescribeTable is eventually consistent. So you might
                    // get resource not found. So we handle the potential exception.
                }
            } while (status != "ACTIVE");
        }
    }
}
