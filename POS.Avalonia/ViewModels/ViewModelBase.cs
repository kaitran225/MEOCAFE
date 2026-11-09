using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Data.Sqlite;

namespace POS.Avalonia.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty] private string _formError = "";
    [ObservableProperty] private string _statusMessage = "";

    protected void ClearMessages()
    {
        FormError = "";
        StatusMessage = "";
    }

    protected async Task<bool> RunCrudAsync(Func<Task> action, Action? onSuccess = null)
    {
        FormError = "";
        try
        {
            await action().ConfigureAwait(true);
            onSuccess?.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            FormError = MapCrudError(ex);
            return false;
        }
    }

    protected static string MapCrudError(Exception ex)
    {
        if (ex is SqliteException sqlite)
        {
            return sqlite.SqliteErrorCode switch
            {
                19 => "Dữ liệu không hợp lệ hoặc bị trùng (ràng buộc CSDL).",
                787 or 1811 => "Không xóa được: còn bản ghi liên quan.",
                _ => string.IsNullOrWhiteSpace(sqlite.Message) ? "Lỗi CSDL." : sqlite.Message
            };
        }

        return string.IsNullOrWhiteSpace(ex.Message) ? "Đã xảy ra lỗi." : ex.Message;
    }
}
