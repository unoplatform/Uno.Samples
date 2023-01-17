using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Microsoft.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AutoSuggestSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        string[] _colors;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suggestions = new List<string>();

                if (sender.Text.Length >= 3)
                {
                    foreach (var color in GetColors())
                    {
                        if (color.IndexOf(sender.Text, StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            suggestions.Add(color);
                        }
                    }

                    if (suggestions.Count > 0)
                    {
                        suggestBox.ItemsSource = suggestions.OrderBy(s => s).Take(10).ToList();
                    }
                    else
                    {
                        suggestBox.ItemsSource = new string[] { "No results found" };
                    }
                }
                else
                {
                    suggestBox.ItemsSource = new string[0];
                }
            }
        }

        private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            query.Text = args.QueryText;
        }

        private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            query.Text = args.SelectedItem.ToString();
        }

        private string[] GetColors()
        {
            if (_colors == null)
            {
                var fields = typeof(Colors).GetTypeInfo().DeclaredFields.Select(f => f.Name);
                var properties = typeof(Colors).GetTypeInfo().DeclaredProperties.Select(f => f.Name);

                _colors = properties.Concat(fields).ToArray();
            }

            return _colors;
        }
    }
}
