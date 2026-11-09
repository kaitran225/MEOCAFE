using System;
using Microsoft.Extensions.DependencyInjection;

namespace POS.Avalonia.Services;

public sealed class ViewModelResolver : IViewModelResolver
{
    private readonly IServiceProvider _provider;

    public ViewModelResolver(IServiceProvider provider) => _provider = provider;

    public ViewModels.ViewModelBase? Resolve(Type viewModelType) =>
        _provider.GetService(viewModelType) as ViewModels.ViewModelBase;
}
