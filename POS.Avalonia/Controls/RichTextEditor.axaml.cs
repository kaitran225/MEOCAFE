using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;

namespace POS.Avalonia.Controls;

/// <summary>
/// Lightweight rich-text step editor: formatting toolbar + source box + live preview.
/// Formatting is stored as safe HTML (&lt;b&gt;, &lt;i&gt;, &lt;u&gt;, &lt;br&gt;) and ingredient
/// mentions are inserted as [[Ingredient name]] so they render as clickable chips in RichTextBlock.
/// </summary>
public partial class RichTextEditor : UserControl
{
    public static readonly StyledProperty<string> HtmlProperty =
        AvaloniaProperty.Register<RichTextEditor, string>(nameof(Html), string.Empty, true);

    public static readonly StyledProperty<IEnumerable<string>?> MentionsProperty =
        AvaloniaProperty.Register<RichTextEditor, IEnumerable<string>?>(nameof(Mentions));

    public static readonly StyledProperty<string?> SelectedMentionProperty =
        AvaloniaProperty.Register<RichTextEditor, string?>(nameof(SelectedMention));

    public string Html
    {
        get => GetValue(HtmlProperty);
        set => SetValue(HtmlProperty, value);
    }

    public IEnumerable<string>? Mentions
    {
        get => GetValue(MentionsProperty);
        set => SetValue(MentionsProperty, value);
    }

    public string? SelectedMention
    {
        get => GetValue(SelectedMentionProperty);
        set => SetValue(SelectedMentionProperty, value);
    }

    public ICommand BoldCommand { get; }
    public ICommand ItalicCommand { get; }
    public ICommand UnderlineCommand { get; }
    public ICommand BulletCommand { get; }
    public ICommand InsertMentionCommand { get; }

    public RichTextEditor()
    {
        InitializeComponent();
        BoldCommand = new RelayCommand(() => WrapSelection("<b>", "</b>"));
        ItalicCommand = new RelayCommand(() => WrapSelection("<i>", "</i>"));
        UnderlineCommand = new RelayCommand(() => WrapSelection("<u>", "</u>"));
        BulletCommand = new RelayCommand(ToggleBullets);
        InsertMentionCommand = new RelayCommand(InsertMention);
    }

    private void WrapSelection(string open, string close)
    {
        if (EditorBox is null) return;
        var start = EditorBox.SelectionStart;
        var end = EditorBox.SelectionEnd;
        var len = Math.Abs(end - start);
        start = Math.Min(start, end);
        var text = EditorBox.Text ?? "";
        var selected = len > 0 && start + len <= text.Length ? text.Substring(start, len) : "";
        ApplyEdit(text, start, len, open + selected + close, start + open.Length + selected.Length);
    }

    private void InsertMention()
    {
        var mention = SelectedMention?.Trim();
        if (string.IsNullOrEmpty(mention) || EditorBox is null) return;
        var text = EditorBox.Text ?? "";
        var start = EditorBox.SelectionStart;
        var end = EditorBox.SelectionEnd;
        var len = Math.Abs(end - start);
        start = Math.Min(start, end);
        var token = "[[" + mention + "]]";
        var caret = start + token.Length;
        ApplyEdit(text, start, len, token, caret);
    }

    private void ToggleBullets()
    {
        if (EditorBox is null) return;
        var selStart = EditorBox.SelectionStart;
        var selEnd = EditorBox.SelectionEnd;
        var start = Math.Min(selStart, selEnd);
        var len = Math.Abs(selEnd - selStart);
        var text = EditorBox.Text ?? "";
        if (len <= 0) return;

        // Expand selection to whole lines.
        var lineStart = text.LastIndexOf('\n', Math.Max(0, start - 1)) + 1;
        var end = start + len;
        var lineEnd = text.IndexOf('\n', end);
        if (lineEnd < 0) lineEnd = text.Length;
        var block = text.Substring(lineStart, lineEnd - lineStart);
        var lines = block.Split('\n').ToList();

        var allBulleted = lines.Count > 0 && lines.All(l => l.TrimStart().StartsWith("• ", StringComparison.Ordinal));
        var rebuilt = allBulleted
            ? lines.Select(l => l.TrimStart().StartsWith("• ", StringComparison.Ordinal) ? l.Remove(l.TrimStart().IndexOf("• ", StringComparison.Ordinal) + (l.Length - l.TrimStart().Length)).TrimEnd() : l)
                .Where(l => l.Length > 0)
                .ToList()
            : lines.Select(l => "• " + l).ToList();

        var newBlock = string.Join("\n", rebuilt);
        var newText = text.Substring(0, lineStart) + newBlock + text.Substring(lineEnd);
        Html = newText;
        if (EditorBox is not null)
            EditorBox.CaretIndex = Math.Min(lineStart + newBlock.Length, newText.Length);
    }

    private void ApplyEdit(string text, int start, int len, string replacement, int caret)
    {
        if (EditorBox is null) return;
        var newText = text.Substring(0, start) + replacement + text.Substring(start + len);
        Html = newText;
        EditorBox.CaretIndex = Math.Min(caret, newText.Length);
        EditorBox.Focus();
    }
}
