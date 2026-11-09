using System;

namespace POS.Avalonia.Services;

public interface IAppNavigator
{
    void Register(ViewModels.MainViewModel mainViewModel);
    void GoHome();
    void OpenModule(Type viewModelType, string title);
    void OpenOrderForTable(POS.Core.Models.Table table);
}
