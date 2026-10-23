using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Documents;
using Windows.UI;

// The ITextRange/ITextCharacterFormat/ITextParagraphFormat/ITextSelection interface stubs are still
// generated with [Uno.NotImplemented] on Skia although they are implemented (stale flags, see the uno issue).
#pragma warning disable Uno0001

namespace RichTextEditor;

public sealed partial class MainPage : Page
{
    private const string HighlightedPhrase = "typography and geometry";

    private bool _syncing;

    public MainPage()
    {
        this.InitializeComponent();

        foreach (int size in new[] { 11, 12, 14, 16, 18, 20, 24, 28, 36 })
        {
            FontSizeBox.Items.Add(size);
        }

        FontSizeBox.SelectedItem = 16;

        BuildColorMenu();
        LoadArticle();
        BuildColumns();
        UpdateStatus();
        UpdateToolbar();
    }

    private void OnTabChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        bool editor = sender.SelectedItem == EditorTab;
        EditorPage.Visibility = editor ? Visibility.Visible : Visibility.Collapsed;
        ColumnsPage.Visibility = editor ? Visibility.Collapsed : Visibility.Visible;
    }

    // ---- Editor ----

    private void LoadArticle()
    {
        const string rtf = @"{\rtf1\ansi\deff0
{\fonttbl{\f0\fnil Segoe UI;}}
{\colortbl;\red0\green103\blue192;\red196\green43\blue28;\red16\green124\blue16;}
\f0\fs26
\pard\sa160\fs52\b Designing for quiet interfaces\b0\fs26\par
\pard\sa200\i A short guide to apps that stay out of the way.\i0\par
\pard\sa200 The best tools disappear. When you write, you want to think about the \b sentence\b0 , not about the toolbar. A rich text editor earns its place by being \i predictable\i0 : select, format, move on. Nothing should shift under your cursor, and nothing should ask for attention it has not earned.\par
\pard\sa200\cf1\b Three habits worth keeping\cf0\b0\par
\pard\sa80\li360\fi-240 \u8226?  Keep formatting reversible, so every change can be \ul undone\ulnone .\par
\pard\sa80\li360\fi-240 \u8226?  Show the current state in the toolbar, not in a dialog.\par
\pard\sa200\li360\fi-240 \u8226?  Prefer \cf3\b one clear default\cf0\b0  over ten clever options.\par
\pard\sa200 Even small touches matter: a \cf2 red\cf0  warning should look urgent, a \strike strikethrough\strike0  should look final, and a link such as {\field{\*\fldinst{HYPERLINK ""https://platform.uno""}}{\fldrslt{\ul\cf1 platform.uno}}} should look clickable. Try the toolbar above: every button works on the current selection.\par
}";

        Editor.Document.SetText(TextSetOptions.FormatRtf, rtf);

        Editor.Document.Selection.SetRange(0, 0);
        Editor.Document.ApplyDisplayUpdates();
    }

    private void BuildColorMenu()
    {
        (string Name, Color Color)[] colors =
        {
            ("Default", Colors.Transparent),
            ("Blue", Color.FromArgb(255, 0, 103, 192)),
            ("Red", Color.FromArgb(255, 196, 43, 28)),
            ("Green", Color.FromArgb(255, 16, 124, 16)),
            ("Purple", Color.FromArgb(255, 136, 23, 152)),
            ("Orange", Color.FromArgb(255, 218, 99, 10)),
        };

        foreach (var (name, color) in colors)
        {
            MenuFlyoutItem item = new() { Text = name, Tag = color };
            item.Icon = color == Colors.Transparent
                ? new FontIcon { Glyph = "" }
                : new FontIcon { Glyph = "", Foreground = new SolidColorBrush(color) };
            AutomationProperties.SetAutomationId(item, "Color" + name);
            item.Click += OnColorPicked;
            ColorMenu.Items.Add(item);
        }
    }

    private ITextSelection Selection => Editor.Document.Selection;

    private static FormatEffect Effect(bool on) => on ? FormatEffect.On : FormatEffect.Off;

    private void OnBold(object sender, RoutedEventArgs e) => Selection.CharacterFormat.Bold = Effect(BoldButton.IsChecked == true);

    private void OnItalic(object sender, RoutedEventArgs e) => Selection.CharacterFormat.Italic = Effect(ItalicButton.IsChecked == true);

    private void OnUnderline(object sender, RoutedEventArgs e) =>
        Selection.CharacterFormat.Underline = UnderlineButton.IsChecked == true ? UnderlineType.Single : UnderlineType.None;

    private void OnStrike(object sender, RoutedEventArgs e) => Selection.CharacterFormat.Strikethrough = Effect(StrikeButton.IsChecked == true);

    private void OnFontSizeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_syncing && FontSizeBox.SelectedItem is int size)
        {
            Selection.CharacterFormat.Size = size;
        }
    }

    private void OnColorPicked(object sender, RoutedEventArgs e)
    {
        var color = (Color)((MenuFlyoutItem)sender).Tag;
        if (color == Colors.Transparent)
        {
            Selection.CharacterFormat.ForegroundColor = (Editor.Foreground as SolidColorBrush)?.Color ?? Colors.Black;
        }
        else
        {
            Selection.CharacterFormat.ForegroundColor = color;
        }
    }

    private void OnAlignLeft(object sender, RoutedEventArgs e) => Selection.ParagraphFormat.Alignment = ParagraphAlignment.Left;

    private void OnAlignCenter(object sender, RoutedEventArgs e) => Selection.ParagraphFormat.Alignment = ParagraphAlignment.Center;

    private void OnAlignRight(object sender, RoutedEventArgs e) => Selection.ParagraphFormat.Alignment = ParagraphAlignment.Right;

    private void OnUndo(object sender, RoutedEventArgs e) => Editor.Document.Undo();

    private void OnRedo(object sender, RoutedEventArgs e) => Editor.Document.Redo();

    private void OnEditorTextChanged(object sender, RoutedEventArgs e) => UpdateStatus();

    private void OnEditorSelectionChanged(object sender, RoutedEventArgs e) => UpdateToolbar();

    private void UpdateStatus()
    {
        Editor.Document.GetText(TextGetOptions.None, out string text);
        text = text.TrimEnd('\r', '\n');
        int words = text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
        StatusText.Text = $"{words} words  |  {text.Length} characters";
    }

    private void UpdateToolbar()
    {
        if (Editor is null)
        {
            return;
        }

        _syncing = true;
        try
        {
            ITextCharacterFormat cf = Selection.CharacterFormat;
            ITextParagraphFormat pf = Selection.ParagraphFormat;

            BoldButton.IsChecked = cf.Bold == FormatEffect.On;
            ItalicButton.IsChecked = cf.Italic == FormatEffect.On;
            UnderlineButton.IsChecked = cf.Underline != UnderlineType.None && cf.Underline != UnderlineType.Undefined;
            StrikeButton.IsChecked = cf.Strikethrough == FormatEffect.On;
            AlignLeftButton.IsChecked = pf.Alignment == ParagraphAlignment.Left;
            AlignCenterButton.IsChecked = pf.Alignment == ParagraphAlignment.Center;
            AlignRightButton.IsChecked = pf.Alignment == ParagraphAlignment.Right;

            int size = (int)Math.Round(cf.Size);
            if (FontSizeBox.Items.Contains(size))
            {
                FontSizeBox.SelectedItem = size;
            }

            UndoButton.IsEnabled = Editor.Document.CanUndo();
            RedoButton.IsEnabled = Editor.Document.CanRedo();

            int length = Math.Abs(Selection.EndPosition - Selection.StartPosition);
            SelectionText.Text = length == 0
                ? $"Caret at {Selection.StartPosition}"
                : $"{length} characters selected";
        }
        finally
        {
            _syncing = false;
        }
    }

    // ---- Columns ----

    private static readonly (string Text, bool Heading)[] Article =
    {
        ("Why text deserves a real engine", true),
        ("Reading on a screen is a negotiation between typography and geometry. A paragraph has to wrap, a line has to break at the right place, and a selection has to follow the pointer without flicker. When those three things work, nobody notices. When they do not, everybody does.", false),
        ("Magazines solved this long ago with columns. A narrow measure keeps the eye moving, and a story can continue from one column to the next without losing its place. The same idea works in an app: give a block of rich text a fixed height, and let the overflow flow into the next container.", false),
        ("Building that is harder than it sounds", true),
        ("A column layout needs the whole text stack to agree. Measuring, line breaking, hit testing and selection all have to understand that a paragraph may start in one container and end in another. The columns on this page are one RichTextBlock chained to two RichTextBlockOverflow elements, all backed by the same text stack.", false),
        ("Try dragging a selection from the first column into the third. The highlight crosses the gutters, the clipboard receives a single continuous string, and the highlighted phrase in the first paragraph stays put while everything around it reflows.", false),
        ("Formatting travels with the text", true),
        ("Bold, italic and colored runs, hyperlinks and inline spans all survive the trip. Resize the window and the paragraphs find new break points; a sentence that ended a column a moment ago may now open the next one. Nothing is cached by hand, because the layout is simply recomputed whenever the available space changes.", false),
        ("Selection is a feature too", true),
        ("People select text to copy it, to quote it, or simply to keep their place. A good reading surface treats selection as a first class citizen: it starts under the pointer, extends with the keyboard, and survives a change of theme or a resize without jumping to a different sentence.", false),
        ("Highlights are the quiet cousin of selection. They mark a phrase for the reader, such as a search match or a note from an editor, and they should never get in the way of the text underneath.", false),
        ("Small details add up", true),
        ("Good defaults make this feel effortless: generous line height for long reading, a measure of roughly fifty to seventy characters, and a gutter wide enough to separate columns without wasting the page. Add readable contrast in both light and dark themes, and long articles become a pleasure instead of a chore.", false),
        ("The result is a surface that behaves the same everywhere. Desktop or browser, the words land in the same places, and the person reading never has to think about how they got there.", false),
    };

    private void BuildColumns()
    {
        foreach (var (text, heading) in Article)
        {
            Paragraph paragraph = new() { Margin = new Thickness(0, heading ? 12 : 0, 0, heading ? 4 : 10) };
            if (heading)
            {
                paragraph.Inlines.Add(new Run
                {
                    Text = text,
                    FontSize = 22,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = (Brush)Application.Current.Resources["AccentTextFillColorPrimaryBrush"],
                });
            }
            else
            {
                AddBody(paragraph, text);
            }

            Column1.Blocks.Add(paragraph);
        }

        Column1.TextHighlighters.Add(CreateHighlighter());
    }

    private static void AddBody(Paragraph paragraph, string text)
    {
        int index = text.IndexOf(HighlightedPhrase, StringComparison.Ordinal);
        if (index < 0)
        {
            paragraph.Inlines.Add(new Run { Text = text });
            return;
        }

        paragraph.Inlines.Add(new Run { Text = text[..index] });
        paragraph.Inlines.Add(new Run { Text = HighlightedPhrase });
        paragraph.Inlines.Add(new Run { Text = text[(index + HighlightedPhrase.Length)..] });
    }

    private TextHighlighter CreateHighlighter()
    {
        int offset = 0;
        int start = -1;
        foreach (var (text, _) in Article)
        {
            int index = text.IndexOf(HighlightedPhrase, StringComparison.Ordinal);
            if (index >= 0)
            {
                start = offset + index;
                break;
            }

            offset += text.Length + 2;
        }

        TextHighlighter highlighter = new()
        {
            Background = new SolidColorBrush(Color.FromArgb(120, 255, 200, 0)),
            Foreground = new SolidColorBrush(Colors.Black),
        };
        highlighter.Ranges.Add(new TextRange { StartIndex = start, Length = HighlightedPhrase.Length });
        return highlighter;
    }

    private void OnSelectAcross(object sender, RoutedEventArgs e)
    {
        TextPointer? start = Column1.ContentStart?.GetPositionAtOffset(700, LogicalDirection.Forward);
        TextPointer? end = Column1.ContentStart?.GetPositionAtOffset(1900, LogicalDirection.Forward);
        if (start is null || end is null)
        {
            return;
        }

        Column1.Focus(FocusState.Programmatic);
        Column1.Select(start, end);
    }

    private void OnClearSelection(object sender, RoutedEventArgs e)
    {
        if (Column1.ContentStart is { } origin)
        {
            Column1.Select(origin, origin);
        }
    }
}
