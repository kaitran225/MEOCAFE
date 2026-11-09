using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class AiChatViewModel : ViewModelBase
{
    [ObservableProperty] private string _inputText = "";

    public ObservableCollection<ChatMessage> Messages { get; } = new()
    {
        new("Xin chào! Tôi là trợ lý HienXanh. Tôi có thể gợi ý menu, phân tích doanh số, hoặc mẹo vận hành quán. Hãy hỏi tôi bất cứ điều gì!", false, DateTime.Now.AddMinutes(-1)),
    };

    [RelayCommand]
    private async Task SendAsync()
    {
        var text = InputText?.Trim();
        if (string.IsNullOrEmpty(text)) return;

        Messages.Add(new ChatMessage(text, true, DateTime.Now));
        InputText = "";

        await Task.Delay(600);

        var reply = GenerateReply(text);
        Messages.Add(new ChatMessage(reply, false, DateTime.Now));
    }

    private static string GenerateReply(string input)
    {
        var lower = input.ToLowerInvariant();

        if (lower.Contains("menu") || lower.Contains("drink"))
            return "Dựa trên dữ liệu bán hàng gần đây, những món bán chạy nhất là Cà phê sữa đá, Trà đào cam sả, và Matcha Latte. Bạn có muốn tôi gợi ý thêm combo không?";

        if (lower.Contains("sale") || lower.Contains("revenue") || lower.Contains("doanh"))
            return "Doanh số hôm nay tăng 12% so với hôm qua. Khung giờ cao điểm là 10h-12h và 14h-16h. Tôi khuyên bạn nên tăng cường nhân viên vào những giờ này.";

        if (lower.Contains("inventory") || lower.Contains("stock") || lower.Contains("kho"))
            return "Tôi nhận thấy nguyên liệu sữa tươi và syrup caramel sắp hết. Bạn nên đặt hàng thêm trước cuối tuần để tránh thiếu hàng.";

        if (lower.Contains("customer") || lower.Contains("khách"))
            return "Khách hàng thân thiết chiếm 35% doanh số. Bạn có thể tạo chương trình ưu đãi cho thành viên để tăng tỷ lệ quay lại.";

        return "Cảm ơn bạn đã hỏi! Tôi có thể giúp bạn phân tích doanh số, gợi ý menu, quản lý kho, hoặc tối ưu vận hành. Hãy cho tôi biết bạn cần hỗ trợ gì cụ thể nhé!";
    }
}

public class ChatMessage
{
    public string Text { get; }
    public bool IsUser { get; }
    public DateTime Timestamp { get; }
    public bool IsAi => !IsUser;

    public ChatMessage(string text, bool isUser, DateTime timestamp)
    {
        Text = text;
        IsUser = isUser;
        Timestamp = timestamp;
    }
}
