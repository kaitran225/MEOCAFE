using POS.Avalonia.Models;
using System.Threading;
using System.Threading.Tasks;

namespace POS.Avalonia.Services;

public interface IKitchenDisplayService
{
    Task<KitchenOrderDisplay?> GetOrderDisplayAsync(int orderId, CancellationToken cancellationToken);
    string FormatKitchenOrderAsText(KitchenOrderDisplay display);
}
