using System;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;

namespace TextFeaturesShowcase;

public sealed partial class MainPage : Page
{
	public MainPage()
	{
		this.InitializeComponent();
		Loaded += OnLoaded;
	}

	private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		// Seed the initial highlight.
		ApplyHighlight(SearchBox.Text);
	}

	private void OnSearchChanged(object sender, TextChangedEventArgs e) => ApplyHighlight(SearchBox.Text);

	private void ApplyHighlight(string term)
	{
		if (HighlightTarget is null)
		{
			return;
		}

		HighlightTarget.TextHighlighters.Clear();

		if (string.IsNullOrEmpty(term))
		{
			return;
		}

		var highlighter = new TextHighlighter
		{
			Background = new SolidColorBrush(Colors.Gold),
			Foreground = new SolidColorBrush(Colors.Black)
		};

		var text = HighlightTarget.Text;
		var index = text.IndexOf(term, StringComparison.OrdinalIgnoreCase);
		while (index >= 0)
		{
			highlighter.Ranges.Add(new TextRange { StartIndex = index, Length = term.Length });
			index = text.IndexOf(term, index + term.Length, StringComparison.OrdinalIgnoreCase);
		}

		if (highlighter.Ranges.Count > 0)
		{
			HighlightTarget.TextHighlighters.Add(highlighter);
		}
	}
}
