using Microsoft.Azure.Cosmos;

record Item(string id, string pk, string name, string type);

class Program
{
    // TODO: Insert your Cosmos DB details here:
    private const string EndpointUri = "x";
    private const string Key = "x";

    // TODO: Set your names here (or leave these defaults)
    private const string DatabaseId = "shop";
    private const string MonitoredContainerId = "items";       // The data you're watching
    private const string LeaseContainerId = "items-leases";    // The processor's internal tracking container

    static async Task Main()
    {
        var client = new CosmosClient(EndpointUri, Key, new CosmosClientOptions
        {
            ApplicationName = "ChangeFeedDemoApp"
        });

        // Ensure database exists
        var dbResponse = await client.CreateDatabaseIfNotExistsAsync(DatabaseId);
        var database = dbResponse.Database;

        // Ensure the monitored data container exists
        await database.CreateContainerIfNotExistsAsync(
            new ContainerProperties(MonitoredContainerId, "/category"),
            throughput: 400);

        // Ensure the lease container exists (important!)
        await database.CreateContainerIfNotExistsAsync(
            new ContainerProperties(LeaseContainerId, "/id"),
            throughput: 400);

        var monitored = client.GetContainer(DatabaseId, MonitoredContainerId);
        var lease = client.GetContainer(DatabaseId, LeaseContainerId);

        // Build the change feed processor
        var processor = monitored
            .GetChangeFeedProcessorBuilder<Item>(
                processorName: "items-change-processor",
                onChangesDelegate: HandleChangesAsync)
            .WithInstanceName(Environment.MachineName)
            .WithLeaseContainer(lease)
            .Build();

        Console.WriteLine("Starting change feed processor...");
        await processor.StartAsync();

        Console.WriteLine("Listening for changes. Insert/update data to see output."); 
        Console.WriteLine("Press ENTER to stop.");
        Console.ReadLine();

        Console.WriteLine("Stopping...");
        await processor.StopAsync();
        Console.WriteLine("Stopped.");
    }

    private static async Task HandleChangesAsync(
        ChangeFeedProcessorContext context,
        IReadOnlyCollection<Item> changes,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Partition {context.LeaseToken} received {changes.Count} changes:");

        foreach (var item in changes)
        {
            Console.WriteLine($"  → id={item}");
        }

        await Task.CompletedTask;
    }
}
