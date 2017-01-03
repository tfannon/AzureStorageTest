using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types

namespace AzureStorageTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            var table = tableClient.GetTableReference("people");

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

            // Create a new customer entity.
            var customer1 = new CustomerEntity("Harp", "Walter");
            customer1.Email = "Walter@contoso.com";
            customer1.PhoneNumber = "425-555-0101";

            // Create the TableOperation object that inserts the customer entity.
            var insertOperation = TableOperation.Insert(customer1);

            // Execute the insert operation.
            table.Execute(insertOperation);

            // Create the batch operation.
            var batchOperation = new TableBatchOperation();

            // Create a customer entity and add it to the table.
            var customer2 = new CustomerEntity("Smith", "Jeff");
            customer2.Email = "Jeff@contoso.com";
            customer2.PhoneNumber = "425-555-0104";

            // Create another customer entity and add it to the table.
            var customer3 = new CustomerEntity("Smith", "Ben");
            customer3.Email = "Ben@contoso.com";
            customer3.PhoneNumber = "425-555-0102";

            // Add both customer entities to the batch insert operation.
            batchOperation.Insert(customer2);
            batchOperation.Insert(customer3);

            // Execute the batch operation.
            table.ExecuteBatch(batchOperation);

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            var query = new TableQuery<CustomerEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith"));

            // Print the fields for each customer.
            foreach (var entity in table.ExecuteQuery(query))
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                    entity.Email, entity.PhoneNumber);
            }
        }
    }
}
