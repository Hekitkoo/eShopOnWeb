using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IMessageService
{
    Task SendAsync<T>(T message);
}
