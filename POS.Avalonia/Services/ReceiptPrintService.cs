using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using POS.Core.Contracts;

namespace POS.Avalonia.Services;

/// <summary>Sends receipt and kitchen ticket text to a configured network printer (host:port) or logs when not configured.</summary>
public sealed class ReceiptPrintService : IPrintService
{
    private const int DefaultPort = 9100;
    private const string EscInit = "\x1B@"; // ESC @ = initialize printer

    private readonly IConfig _config;
    private readonly ISettingsStore _store;

    public ReceiptPrintService(IConfig config, ISettingsStore store)
    {
        _config = config;
        _store = store;
    }

    private string? GetPrinterTarget(string key) => _store.Get("Printer:" + key) ?? _config.GetSetting("Printer:" + key) ?? _config.GetSetting("Printer:Default");

    public void PrintReceipt(string receiptText)
    {
        if (!TrySendToPrinter(GetPrinterTarget("Receipt"), receiptText))
            Debug.WriteLine("PrintReceipt:\n" + receiptText);
    }

    public void PrintKitchenTicket(string kitchenTicketText)
    {
        if (!TrySendToPrinter(GetPrinterTarget("Kitchen"), kitchenTicketText))
            Debug.WriteLine("PrintKitchenTicket:\n" + kitchenTicketText);
    }

    public void OpenCashDrawer()
    {
        var target = GetPrinterTarget("Receipt");
        if (string.IsNullOrWhiteSpace(target) || string.Equals(target, "None", StringComparison.OrdinalIgnoreCase)) return;
        try
        {
            var (host, port) = ParseHostPort(target.Trim());
            var cmd = new byte[] { 0x1B, 0x70, 0x00, 0x19, 0xFA }; // ESC p m t1 t2 - drawer kick
            using var client = new TcpClient();
            client.ConnectAsync(host, port).GetAwaiter().GetResult();
            using var stream = client.GetStream();
            stream.Write(cmd, 0, cmd.Length);
            stream.Flush();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("OpenCashDrawer: " + ex.Message);
        }
    }

    private static bool TrySendToPrinter(string? target, string text)
    {
        if (string.IsNullOrWhiteSpace(target) || string.Equals(target, "None", StringComparison.OrdinalIgnoreCase))
            return false;
        try
        {
            var (host, port) = ParseHostPort(target);
            var payload = Encoding.UTF8.GetBytes(EscInit + text + "\n\n\n");
            using var client = new TcpClient();
            client.ConnectAsync(host, port).GetAwaiter().GetResult();
            using var stream = client.GetStream();
            stream.Write(payload, 0, payload.Length);
            stream.Flush();
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ReceiptPrintService: " + ex.Message);
            return false;
        }
    }

    private static (string host, int port) ParseHostPort(string value)
    {
        value = value.Trim();
        var colon = value.LastIndexOf(':');
        if (colon > 0 && int.TryParse(value.AsSpan(colon + 1), out var p))
            return (value[..colon].Trim(), p);
        return (value, DefaultPort);
    }
}
