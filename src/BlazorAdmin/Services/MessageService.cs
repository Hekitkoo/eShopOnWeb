using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using BlazorShared;
using Microsoft.eShopWeb.ApplicationCore.Constants;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace BlazorAdmin.Services;

public class MessageService : IMessageService
{
    private readonly ServiceBusSender _serviceBusSender;

    public MessageService(BaseUrlConfiguration options)
    {
        var serviceBusBase = options.ServiceBusBase;
        var client = new ServiceBusClient(serviceBusBase);
        _serviceBusSender = client.CreateSender(AzureConstants.QueueName);
    }

    public async Task SendAsync<T>(T message)
    {
        var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var serializedMessage = JsonSerializer.Serialize(message, serializerOptions);

        await _serviceBusSender.SendMessageAsync(new ServiceBusMessage(serializedMessage));
    }
}
