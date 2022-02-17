using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Functions
{
    public class DeliveryOrderFunction
    {
        [FunctionName("DeliveryOrderFunction")]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "DeliveryOrder")] HttpRequest req,
            [CosmosDB(databaseName: "DeliveryOrderProcessing", collectionName: "DeliveryOrderProcessing", ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<OrderDetailsDto> orderDetails,
            ILogger log)
        {
            log.LogInformation("DeliveryOrderFunction processing");

            var toBlobData = await ParseToCosmoDbData(req);
            await orderDetails.AddAsync(toBlobData);
            
            log.LogInformation("DeliveryOrderFunction are processed");
        }

        private async Task<OrderDetailsDto> ParseToCosmoDbData(HttpRequest req)
        {
            var data = await JsonSerializer.DeserializeAsync<OrderDetailsDto>(
                req.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data;
        }
    }

    public class OrderDetailsDto
    {
        public string BuyerId { get; set; }

        public DateTime OrderDate { get; set; }

        public IEnumerable<OrderItemDto> OrderItems { get; set; }

        public ShopToAddressDto ShopToAddress { get; set; }

        public decimal Total { get; set; }
    }

    public abstract class OrderItemDto
    {
        public double UnitPrice { get; set; }

        public int Units { get; set; }
    }

    public abstract class ShopToAddressDto
    {
        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }
    }
}
