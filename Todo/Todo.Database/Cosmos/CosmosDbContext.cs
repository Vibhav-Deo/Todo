namespace Todo.Database.Cosmos;

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CosmosDbContext
{

    // The Cosmos client instance
    private CosmosClient CosmosClient;

    // The database we will create
    private Microsoft.Azure.Cosmos.Database Database;

    // The container we will create.
    private Container Container;

    // The name of the database and container we will create
    private readonly string DatabaseId;
    private readonly string ContainerId = "items";

    public CosmosDbContext(string key, string endpointUrl)
    {
        var options = new CosmosClientOptions()
        {
            ApplicationName = "Todo",
            SerializerOptions = new CosmosSerializationOptions()
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            },
            EnableContentResponseOnWrite = true
        };
        CosmosClient = new CosmosClient(endpointUrl, key, options);
        DatabaseId = "Event";
        Task.Run(async () => await CreateCosmosDbAsync()).GetAwaiter().GetResult();
        Task.Run(async () => await CreateContainerAsync()).GetAwaiter().GetResult();
    }

    public async Task CreateCosmosDbAsync()
    {
        // Create a new database
        Database = await CosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId);
    }

    public async Task CreateContainerAsync()
    {
        // Create a new container
        Container = await Database.CreateContainerIfNotExistsAsync(ContainerId, "/correlationId", 400);
    }

    public async Task<ItemResponse<T>> CreateItemAsync<T>(T data)
    {
        try
        {
            var correlationId = JObject.Parse(JsonConvert.SerializeObject(data))["CorrelationId"];
            if (correlationId.Equals(Guid.Empty))
            {
                throw new NullReferenceException("Id cannot be null");
            }
            var itemResponse = await Container.CreateItemAsync(data, new PartitionKey(correlationId.ToString()));
            return itemResponse;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ItemResponse<T>> ReadItemAsync<T>(string id, string partitionKey)
    {
        return await Container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
    }
}