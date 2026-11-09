using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace POS.Avalonia.Controls;

/// <summary>
/// Lightweight rich-text TextBlock.
/// Renders a safe HTML subset (<b>, <strong>, <i>, <em>, <u>, &lt;br&gt;) plus
/// ingredient mentions written as [[Ingredient name]], which render as tappable chips.
/// </summary>
public class RichTextBlock : TextBlock
{
    public static readonly StyledProperty<string> HtmlProperty =
        AvaloniaProperty.Register<RichTextBlock, string>(nameof(Html), string.Empty);

    public static readonly StyledProperty<ICommand?> MentionClickedCommandProperty =
        AvaloniaProperty.Register<RichTextBlock, ICommand?>(nameof(MentionClickedCommand));

    /// <summary>Raised when a [[...]] mention chip is clicked.</summary>
    public event Action<string>? MentionClicked;

    public string Html
    {
        get => GetValue(HtmlProperty);
        set => SetValue(HtmlProperty, value);
    }

    public ICommand? MentionClickedCommand
    {
        get => GetValue(MentionClickedCommandProperty);
        set => SetValue(MentionClickedCommandProperty, value);
    }

    public RichTextBlock()
    {
        TextWrapping = TextWrapping.Wrap;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == HtmlProperty)
            Rebuild(change.GetNewValue<string>() ?? "");
    }

    private void Rebuild(string source)
    {
        var inlines = new InlineCollection();
        Parse(source, inlines);
        Inlines = inlines;
    }

    private void Parse(string source, InlineCollection inlines)
    {
        var pos = 0;
        var bold = false;
        var italic = false;
        var underline = false;

        while (pos < source.Length)
        {
            var nextTag = source.IndexOf('<', pos);
            var nextMention = source.IndexOf("[[", pos);

            var next = -1;
            var kind = (string?)null;
            if (nextTag >= 0 && (nextMention < 0 || nextTag <= nextMention))
            {
                next = nextTag;
                kind = "tag";
            }
            else if (nextMention >= 0)
            {
                next = nextMention;
                kind = "mention";
            }

            if (next < 0)
            {
                AppendText(source.Substring(pos), inlines, bold, italic, underline);
                break;
            }

            if (next > pos)
                AppendText(source.Substring(pos, next - pos), inlines, bold, italic, underline);

            if (kind == "mention")
            {
                var end = source.IndexOf("]]", next + 2);
                if (end < 0)
                {
                    AppendText(source.Substring(next), inlines, bold, italic, underline);
                    break;
                }
                var name = source.Substring(next + 2, end - next - 2).Trim();
                if (name.Length > 0)
                    inlines.Add(MakeMentionChip(name));
                pos = end + 2;
            }
            else
            {
                var end = source.IndexOf('>', next + 1);
                if (end < 0)
                {
                    AppendText(source.Substring(next), inlines, bold, italic, underline);
                    break;
                }
                var tag = source.Substring(next + 1, end - next - 1).Trim().ToLowerInvariant();
                pos = end + 1;
                switch (tag)
                {
                    case "br":
                    case "br/":
                        inlines.Add(new LineBreak());
                        break;
                    case "b":
                    case "strong":
                        bold = true;
                        break;
                    case "i":
                    case "em":
                        italic = true;
                        break;
                    case "u":
                        underline = true;
                        break;
                    case "/b":
                    case "/strong":
                        bold = false;
                        break;
                    case "/i":
                    case "/em":
                        italic = false;
                        break;
                    case "/u":
                        underline = false;
                        break;
                    // Any other tag is dropped from the output.
                }
            }
        }
    }

    private static void AppendText(string text, InlineCollection inlines, bool bold, bool italic, bool underline)
    {
        if (text.Length == 0)
            return;

        var decoded = DecodeEntities(text);
        var run = new Run(decoded);
        if (bold) run.FontWeight = FontWeight.Bold;
        if (italic) run.FontStyle = FontStyle.Italic;
        if (underline) run.TextDecorations = global::Avalonia.Media.TextDecorations.Underline;
        inlines.Add(run);
    }

    private static string DecodeEntities(string text)
        => text
            .Replace("&amp;", "&")
            .Replace("&lt;", "<")
            .Replace("&gt;", ">")
            .Replace("&quot;", "\"")
            .Replace("&#39;", "'");

    private Inline MakeMentionChip(string name)
    {
        var bg = ResolveBrush("PrimaryLightBrush") ?? new SolidColorBrush(Color.Parse("#D4E5D2"));
        var fg = ResolveBrush("PrimaryBrush") ?? new SolidColorBrush(Color.Parse("#6B8E6B"));
        var fontSize = FontSize > 0 ? FontSize : 13d;

        var label = new TextBlock
        {
            Text = name,
            FontSize = fontSize,
            FontWeight = FontWeight.SemiBold,
            Foreground = fg,
            VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center
        };

        var chip = new Border
        {
            Background = bg,
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(6, 1, 6, 2),
            Child = label,
            Cursor = new Cursor(StandardCursorType.Hand)
        };
        ToolTip.SetTip(chip, "View ingredient: " + name);

        var tappedName = name;
        chip.Tapped += (_, _) =>
        {
            MentionClicked?.Invoke(tappedName);
            if (MentionClickedCommand is { } cmd && cmd.CanExecute(tappedName))
                cmd.Execute(tappedName);
        };

        return new InlineUIContainer { Child = chip, BaselineAlignment = BaselineAlignment.Center };
    }

    private IBrush? ResolveBrush(string key)
    {
        if (this.TryFindResource(key, ActualThemeVariant, out var value) && value is IBrush b)
            return b;
        if (Application.Current?.TryGetResource(key, ActualThemeVariant, out var appValue) == true && appValue is IBrush ab)
            return ab;
        return null;
    }
}
