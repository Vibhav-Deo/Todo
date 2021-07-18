using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Todo.Database.Cosmos
{
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
            CosmosClient = new CosmosClient(endpointUrl, key, new CosmosClientOptions() { ApplicationName = "Todo" });
            DatabaseId = "Event";
            Task.Run(async () => await CreateCosmosDbAsync()).GetAwaiter().GetResult();
            Task.Run(async () => await CreateContainerAsync()).GetAwaiter().GetResult();
        }

        public async Task CreateCosmosDbAsync()
        {
            // Create a new database
            Database = await CosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId);
        }

        public  async Task CreateContainerAsync()
        {
            // Create a new container
            Container = await Database.CreateContainerIfNotExistsAsync(ContainerId, "/LastName", 400);
        }

        public async Task<ItemResponse<string>> CreateItemAsync<T>(T data)
        {
            var dataToWrite = JsonConvert.SerializeObject(data);
            var partitionKey = JObject.Parse(dataToWrite)["EntityId"].ToString();
            return await Container.CreateItemAsync(dataToWrite, new PartitionKey(partitionKey));
        }

        public async Task<ItemResponse<T>> ReadItemAsync<T>(string id, string partitionKey)
        {
            return await Container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
        }
    }
}
