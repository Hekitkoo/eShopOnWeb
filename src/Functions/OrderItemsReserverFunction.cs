using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace Functions
{
    public class OrderItemsReserverFunction
    {
        private readonly string _blobBase;
        private readonly string _containerName;
        private readonly string _blobName;
        private const string JsonFormat = ".json";
        private const string JsonContentType = "application/json";
        

        public OrderItemsReserverFunction()
        {
            _blobBase = "DefaultEndpointsProtocol=https;AccountName=cloudxtestms;AccountKey=UgR5WogKEooTJm3soBGzDIbfeit9+F/DJYNGgA8+8oxma3JdLYSdcdWrWhuFb0bUvYOXIRQV+A6IxCgSkFdmQw==;EndpointSuffix=core.windows.net";
            _containerName = "test";
            _blobName = "test";
        }
        
        [FunctionName("OrderItemsReserverFunction")]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "OrderReserve")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Order Reserve processing");

            var toBlobData = await ParseToItemToBlobData(req);
            await SendToBlob(toBlobData);
            
            log.LogInformation("Order Reserve are processed");
        }

        private async Task SendToBlob(IEnumerable<OrderItemsToBlobData> itemsToBlobData)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(_blobBase);
            var client = cloudStorageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(_containerName);
            var date = DateTime.UtcNow;
            var blobName = _blobName + date.ToString("MM-dd-yy") + date.Millisecond + JsonFormat;
            var blob = container.GetBlockBlobReference(blobName);
            blob.Properties.ContentType = JsonContentType;
            await using MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(itemsToBlobData)));
            await blob.UploadFromStreamAsync(ms);
        }
        
        private async Task<IEnumerable<OrderItemsToBlobData>> ParseToItemToBlobData(HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            Dictionary<string, int> data = JsonConvert.DeserializeObject<Dictionary<string, int>>(requestBody);

            return data?.Select(x => new OrderItemsToBlobData(x.Key, x.Value));
        }
    }

    public class OrderItemsToBlobData
    {
        public string ItemId { get; }
        public int Quantity { get; }

        public OrderItemsToBlobData(string itemId, int quantity)
        {
            this.ItemId = itemId;
            this.Quantity = quantity;
        }
    }
}
