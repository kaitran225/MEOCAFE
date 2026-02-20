using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using POS.Models;
using POS.ViewModels;

namespace POS.Helpers;

public class MenuItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate RegularTemplate { get; set; }
    public DataTemplate AddItemTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        var result = item is ItemViewModel { IsPlaceholder: true };
        Debug.WriteLine(result);
        return result ? AddItemTemplate : RegularTemplate;
    }
}