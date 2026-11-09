using System.Threading;
using System.Threading.Tasks;
using POS.Avalonia.Models;
using POS.Avalonia.ViewModels;

namespace POS.Avalonia.Services;

public interface IReceiptService
{
    Task<ReceiptData> BuildReceiptFromOrderAsync(int orderId, string paymentMethod = "Cash", CancellationToken cancellationToken = default);
    ReceiptData BuildReceiptFromBill(BillViewModel bill);
    string FormatReceiptAsText(ReceiptData data);
}
