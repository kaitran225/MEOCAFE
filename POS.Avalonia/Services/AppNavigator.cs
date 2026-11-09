using System;
using POS.Avalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace POS.Avalonia.Services;

public sealed class AppNavigator : IAppNavigator
{
    private ViewModels.MainViewModel? _main;

    public void Register(ViewModels.MainViewModel mainViewModel) => _main = mainViewModel;

    public void GoHome() => _main?.ShowHome();

    public void OpenModule(Type viewModelType, string title) => _main?.ShowModule(viewModelType, title);

    public void OpenOrderForTable(POS.Core.Models.Table table)
    {
        var pending = Program.Host?.Services.GetService<PendingTableService>();
        pending?.SetPending(table);
        var title = Program.GetLocalizer()["Nav_Order"].Value ?? "Đặt món";
        OpenModule(typeof(OrderSelectionViewModel), title);
    }
}
