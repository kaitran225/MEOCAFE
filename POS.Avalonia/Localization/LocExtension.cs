using System;
using Avalonia.Markup.Xaml;

namespace POS.Avalonia.Localization;

public sealed class LocExtension : MarkupExtension
{
    public string Key { get; set; } = "";

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrEmpty(Key))
            return "";
        try
        {
            return Program.GetLocalizer()[Key].Value ?? Key;
        }
        catch
        {
            return Key;
        }
    }
}
