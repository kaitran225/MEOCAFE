using System.Threading;
using System.Threading.Tasks;
using POS.Avalonia.Models;

namespace POS.Avalonia.Services;

public interface IReceiptService
{
    Task<ReceiptData> BuildReceiptFromOrderAsync(int orderId, string paymentMethod = "Cash", CancellationToken cancellationToken = default);
    string FormatReceiptAsText(ReceiptData data);
}
