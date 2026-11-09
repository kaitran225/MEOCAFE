using System;

namespace POS.Avalonia.Services;

public interface IViewModelResolver
{
    ViewModels.ViewModelBase? Resolve(Type viewModelType);
}
