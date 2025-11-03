using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

class Program
{
 
    static async Task Main()
    {
        // 1️⃣ Connect

        var endpoint = "https://YYYYY.documents.azure.com:443/";
        var key = "XXXX";

        CosmosClient client = new CosmosClient(endpoint, key);


       
       // CosmosClient client = new CosmosClient(connectionString);

        // 2️⃣ Get or create database & container
        Database db = await client.CreateDatabaseIfNotExistsAsync("shop");
        Container container = await db.CreateContainerIfNotExistsAsync("items", "/category");

        // 3️⃣ Data to send to the stored procedure
        var docs = new dynamic[]
        {
            new { id = "10", category = "books", name = "Dune" },
            new { id = "11", category = "books", name = "1984" }
        };

        // 4️⃣ Execute the stored procedure
        var result = await container.Scripts.ExecuteStoredProcedureAsync<string>(
            "spBulkInsert",                     // stored proc Id
            new PartitionKey("books"),          // partition key value
            new object[] { docs }               // parameters (body)
        );

        Console.WriteLine($"Stored procedure result: {result.Resource}");
    }
}
