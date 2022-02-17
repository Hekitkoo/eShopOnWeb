using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Functions
{
    public class OrderItemsReserverFunction
    {
        [FunctionName("OrderItemsReserverFunction")]
        public static async Task Run(
            [ServiceBusTrigger("orderitemsreserverqueue", Connection = "ServiceBusConnection")] string myQueueItem,
            [Blob("orders/{sys.randguid}", FileAccess.Write), StorageAccount("AzureWebJobsStorage")] Stream blobStream,
            ILogger log)
        {
            log.LogInformation("Order Reserve processing");

            await blobStream.WriteAsync(Encoding.UTF8.GetBytes(myQueueItem));

            log.LogInformation("Order Reserve are processed");
        }
    }
}
