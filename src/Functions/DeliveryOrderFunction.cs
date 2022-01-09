using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace Functions
{
    public class DeliveryOrderFunction
    {
        private readonly string _cosmoDbBase;
        private readonly string _containerName= "test";
        private readonly string _databaseName = "test";
        private readonly string _account = "https://cloudxsokurenko.documents.azure.com:443/";
        private readonly string _key = "U3qeOmbf9TzsYloBaDj9BRLvCPM7ieFKCXxQcHoTMj5NT1yXz0yPM6MuHLXmMX9bbYGTzvtNTdf1xOopBzxYQQ==";

        private Container _container;

        public DeliveryOrderFunction()
        {
            CosmosClient client = new CosmosClient(_account, _key);
            _cosmoDbBase = "";
            _container = client.GetContainer(_databaseName, _containerName);
        }
        
        [FunctionName("DeliveryOrderFunction")]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "DeliveryOrder")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("DeliveryOrderFunction processing");

            var toBlobData = await ParseToCosmoDbData(req);
            await SendToCosmoDb(toBlobData);
            
            log.LogInformation("DeliveryOrderFunction are processed");
        }

        private async Task SendToCosmoDb(OrderTest itemToCosmosDb)
        {
            await this._container.CreateItemAsync(itemToCosmosDb);
        }
        
        private async Task<OrderTest> ParseToCosmoDbData(HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var data = JsonConvert.DeserializeObject<OrderTest>(requestBody);

            return data;
        }
    }
}
