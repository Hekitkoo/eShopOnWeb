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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "order-details")] HttpRequest req,
            [CosmosDB(databaseName: "OrdersProcessing", collectionName: "OrdersProcessing", ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<OrderDetailsDto> orderDetails,
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

    internal class OrderDetailsDto
    {
        public string id { get; set; }
        public string BuyerId { get; set; }
        public DateTime OrderDate { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
        public ShopToAddress ShopToAddress { get; set; }
        public double Total { get => OrderItems.Sum(oi => oi.Units * oi.UnitPrice); }
    }

    internal class OrderItem
    {
        public double UnitPrice { get; set; }

        public int Units { get; set; }
    }

    internal class ShopToAddress
    {
        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }
    }
}
