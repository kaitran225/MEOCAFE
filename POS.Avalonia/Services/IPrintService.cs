namespace POS.Avalonia.Services;

public interface IPrintService
{
    void PrintReceipt(string receiptText);
    void PrintKitchenTicket(string kitchenTicketText);
}
