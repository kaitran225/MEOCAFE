using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace POS.Avalonia.Services;

/// <summary>Resolves view models once per app session so POS state survives navigation.</summary>
public sealed class ViewModelCache : IViewModelResolver
{
    private readonly IServiceProvider _provider;
    private readonly Dictionary<Type, ViewModels.ViewModelBase> _cache = new();

    public ViewModelCache(IServiceProvider provider) => _provider = provider;

    public ViewModels.ViewModelBase? Resolve(Type viewModelType)
    {
        if (_cache.TryGetValue(viewModelType, out var cached))
            return cached;

        if (_provider.GetService(viewModelType) is not ViewModels.ViewModelBase vm)
            return null;

        _cache[viewModelType] = vm;
        return vm;
    }
}
