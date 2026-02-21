using System.Diagnostics;

namespace POS.Avalonia.Services;

/// <summary>Stub: logs receipt; ESC/POS in Phase 14.</summary>
public sealed class PrintServiceStub : IPrintService
{
    public void PrintReceipt(string receiptText)
    {
        Debug.WriteLine("PrintReceipt (stub):\n" + receiptText);
    }

    public void PrintKitchenTicket(string kitchenTicketText)
    {
        Debug.WriteLine("PrintKitchenTicket (stub):\n" + kitchenTicketText);
    }

    public void OpenCashDrawer() { }
}
