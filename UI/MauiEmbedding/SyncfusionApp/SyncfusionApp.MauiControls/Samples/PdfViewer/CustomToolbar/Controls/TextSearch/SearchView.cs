#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Maui.PdfViewer;
using System.ComponentModel;

namespace SyncfusionApp.MauiControls.Samples.PdfViewer.SfPdfViewer
{
    /// <summary>
    /// Represents the search view.
    /// </summary>
    public class SearchView : ContentView
    {
        #region UI
        public Syncfusion.Maui.PdfViewer.SfPdfViewer? SearchHelper { get; set; }
        public ActivityIndicator? SearchBusyIndicator { get; set; }
        public CustomEntry? SearchInputEntry { get; set; }
        public Label? SearchStatusLabel { get; set; }
        string currentSearchText = "";
        bool isMatchCase = false;

        #endregion

        /// <summary>
        /// Gets or sets whether match case is enabled.
        /// </summary>
        public bool IsMatchCase 
        { 
            get
            {
                return isMatchCase;
            }
            set
            {
                if (isMatchCase != value)
                {
                    isMatchCase = value;
                    currentSearchText = "";
                    if (SearchInputEntry != null)
                        SearchText(SearchInputEntry.Text);
                }
                OnPropertyChanged(nameof(IsMatchCase));
            }
        }

        /// <summary>
        /// Stores the result of text search.
        /// </summary>
        TextSearchResult? SearchResult;

        /// <summary>
        /// Occurs when no matches are found.
        /// </summary>
        internal event EventHandler? NoMatchesFound;
        public Command SearchCommand { get; set; }
        public Command ClearTextCommand { get; set; }
        public Command GoToNextMatchCommand { get; set; }
        public Command GoToPreviousMatchCommand { get; set; }
        public Command CloseCommand { get; set; }

        public SearchView()
        {
            SearchCommand = new Command(execute: SearchText);
            ClearTextCommand = new Command(execute: ClearText);
            GoToNextMatchCommand = new Command(execute: GoToNextMatch,
                canExecute: () => { return ((SearchResult != null) && (SearchResult.TotalMatchesCount > 1)); });
            GoToPreviousMatchCommand = new Command(execute: GoToPreviousMatch,
                canExecute: () => { return ((SearchResult != null) && (SearchResult.TotalMatchesCount > 1)); });
            CloseCommand = new Command(execute: Close);
        }

        /// <summary>
        /// Searches the text.
        /// </summary>
        /// <param name="parameter">The text to be searched.</param>
        void SearchText(object parameter)
        {
            if (parameter is string text)
                SearchText(text);
        }

        void ClearText()
        {
            if (SearchInputEntry != null)
                SearchInputEntry.Text = "";
        }

        void GoToNextMatch()
        {
            SearchResult?.GoToNextMatch();
            UpdateStatus();
        }

        void GoToPreviousMatch()
        {
            SearchResult?.GoToPreviousMatch();
            UpdateStatus();
        }

        /// <summary>
        /// The functionality can be called when the view is initialized.
        /// </summary>
        public void OnInitialized()
        {
            if (SearchInputEntry != null)
            {
                SearchInputEntry.TextChanged += SearchInputEntryTextChanged;
                SearchInputEntry.Completed += SearchInputEntryCompleted;
            }
        }

        private void SearchInputEntryCompleted(object? sender, EventArgs e)
        {
            if (sender is CustomEntry entry)
                SearchText(entry.Text);
        }

        private void SearchInputEntryTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.OldTextValue) == false &&
                string.IsNullOrEmpty(e.NewTextValue))
            {
                Clear();
            }
        }

        void SearchText(string text)
        {
            if (string.IsNullOrEmpty(text)==false)
            {
                if (currentSearchText != text)
                {
                    Search(text);
                    currentSearchText = text;
                }
                else
                    GoToNextMatch();
            }
        }

        async void Search(string text)
        {
            RefreshNavigation();
            this.Dispatcher.Dispatch(() =>
            {
                SearchInputEntry?.HideKeyboard();
                ShowBusyIndicator();
                ShowStatus();
            });
            TextSearchOptions searchOptions = TextSearchOptions.None;
            if (IsMatchCase)
                searchOptions = TextSearchOptions.CaseSensitive;
            if (SearchHelper != null)
            {
                SearchHelper.TextSearchProgress += SearchHelperTextSearchProgress;
                await SearchHelper.SearchTextAsync(text, searchOptions);
            }
            this.Dispatcher.Dispatch(HideBusyIndicator);
        }

        private void SearchHelperTextSearchProgress(object? sender, TextSearchProgressEventArgs? e)
        {
            if (e == null)
                return;
            int totalMatches = e.SearchResult.TotalMatchesCount;
            if (e.TotalPagesSearched == 1)
            {
                SearchResult = e.SearchResult;
            }
            if (SearchHelper != null && e.TotalPagesSearched == SearchHelper.PageCount)
            {
                SearchHelper.TextSearchProgress -= SearchHelperTextSearchProgress;
                if (totalMatches == 0)
                    NoMatchesFound?.Invoke(this, EventArgs.Empty);
            }
            UpdateStatus();

            if (totalMatches > 1)
                RefreshNavigation();
        }

        void UpdateStatus()
        {
            if (SearchResult != null)
            {
                int currentMatch = 0;
                if (SearchResult.TotalMatchesCount > 0)
                    currentMatch = SearchResult.CurrentMatchIndex + 1;
                int totalMatches = SearchResult.TotalMatchesCount;
                this.Dispatcher.Dispatch(() =>
                {
                    if (SearchStatusLabel != null)
                    {
                        SearchStatusLabel.Text = currentMatch + " / " + totalMatches;
                    }
                });
            }
        }

        void Clear()
        {
            if (SearchHelper != null)
                SearchHelper.TextSearchProgress -= SearchHelperTextSearchProgress;
            SearchResult?.Clear();
            RefreshNavigation();
            ClearStatus();
            currentSearchText = "";
        }

        /// <summary>
        /// Makes the search view to appear.
        /// </summary>
        public void Open()
        {
            this.IsVisible = true;
            SearchInputEntry?.Focus();
        }

        /// <summary>
        /// Hides the search view.
        /// </summary>
        public virtual void Close()
        {
            Clear();
            Reset();
            this.IsVisible = false;
        }

        void ShowBusyIndicator()
        {
            if (SearchBusyIndicator != null)
            {
                SearchBusyIndicator.IsVisible = true;
                SearchBusyIndicator.IsRunning = true;
            }
        }

        void HideBusyIndicator()
        {
            if (SearchBusyIndicator != null)
            {
                SearchBusyIndicator.IsRunning = false;
                SearchBusyIndicator.IsVisible = false;
            }
        }

        void ShowStatus()
        {
            if (SearchStatusLabel != null)
            {
                SearchStatusLabel.IsVisible = true;
            }
        }

        void ClearStatus()
        {
            if (SearchStatusLabel != null)
            {
                SearchStatusLabel.Text = "0 / 0";
                SearchStatusLabel.IsVisible = false;
            }
        }

        void Reset()
        {
            HideBusyIndicator();
            if (SearchInputEntry != null)
            {
                SearchInputEntry.Text = "";
                SearchInputEntry.HideKeyboard();
            }
        }
        
        void RefreshNavigation()
        {
            this.Dispatcher.Dispatch(() =>
            {
                GoToNextMatchCommand.ChangeCanExecute();
                GoToPreviousMatchCommand.ChangeCanExecute();
            });
        }
    }
}