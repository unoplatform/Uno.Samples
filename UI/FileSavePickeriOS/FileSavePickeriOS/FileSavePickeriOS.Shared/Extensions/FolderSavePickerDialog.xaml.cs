using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FileSavePickeriOS.Extensions
{
    public sealed partial class FolderSavePickerDialog : ContentDialog
    {
        private const string SaveResource = "FolderSavePickerSave";
        private const string CancelResource = "FolderSavePickerCancel";
        private const string TitleResource = "FolderSavePickerTitle";
        private const string FileNameResource = "FolderSavePickerFileName";
        private const string PickFolderResource = "FolderSavePickerPickFolder";
        private const string BrowseResource = "FolderSavePickerBrowse";
        private const string FileTypeResource = "FolderSavePickerFileType";

        private static Lazy<ResourceLoader> _resourceLoader = new Lazy<ResourceLoader>(() => ResourceLoader.GetForViewIndependentUse());
        private StorageFolder _pickedFolder;
        private string[] _existingFileNames = Array.Empty<string>();

        public FolderSavePickerDialog()
        {
            this.InitializeComponent();
            Title = _resourceLoader.Value.GetString(TitleResource);
            DefaultButton = ContentDialogButton.Primary;
            PrimaryButtonText = _resourceLoader.Value.GetString(SaveResource);
            SecondaryButtonText = _resourceLoader.Value.GetString(CancelResource);
            FileNameSuggestBox.Header = _resourceLoader.Value.GetString(FileNameResource);
            PickButton.Content = _resourceLoader.Value.GetString(BrowseResource);
            PickFolderLabel.Text = _resourceLoader.Value.GetString(PickFolderResource);
            FileTypeComboBox.Header = _resourceLoader.Value.GetString(FileTypeResource);

            Loaded += FolderSavePickerDialog_Loaded;
        }

        private void FolderSavePickerDialog_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePickedFolderDisplay();
        }

        public string PickedFileName { get; private set; }

        public StorageFolder PickedFolder
        {
            get => _pickedFolder;
            private set
            {
                _pickedFolder = value;
                UpdatePickedFolderDisplay();
            }
        }

        public string SuggestedFileName
        {
            get => FileNameSuggestBox.Text;
            set => FileNameSuggestBox.Text = value;
        }

        public void SetFileTypeChoices(IDictionary<string, IList<string>> fileTypeChoices)
        {
            var items = new List<FileTypeChoiceListItem>();
            foreach (var choice in fileTypeChoices)
            {
                var extensions = string.Join(",", choice.Value.Select(extension => $"*{extension}"));
                var label = $"{choice.Key} ({extensions})";
                items.Add(new FileTypeChoiceListItem(label, choice.Value));
            }
            FileTypeComboBox.ItemsSource = items;
            FileTypeComboBox.SelectedItem = items.FirstOrDefault();
        }

        private void SaveButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (PickedFolder == null)
            {
                return;
            }

            IList<string> allowedExtensions = Array.Empty<string>();
            if (FileTypeComboBox.SelectedItem is FileTypeChoiceListItem selectedFileTypeChoice)
            {
                allowedExtensions = selectedFileTypeChoice.Extensions;
            }

            bool needsExtension = false;
            var fileName = FileNameSuggestBox.Text;
            if (Path.HasExtension(fileName))
            {
                var extension = Path.GetExtension(fileName);
                if (!allowedExtensions.Any(e => e.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase)))
                {
                    needsExtension = true;
                }
            }
            else
            {
                needsExtension = true;
            }

            if (needsExtension)
            {
                // Entered extension is not allowed, append from the list
                var firstExtension = allowedExtensions.FirstOrDefault();
                if (firstExtension != null)
                {
                    if (fileName.EndsWith('.'))
                    {
                        fileName = fileName.Substring(0, fileName.Length - 1);
                    }

                    fileName += firstExtension;
                }
            }

            PickedFileName = fileName;
        }

        private void CancelButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            PickedFileName = null;
        }

        private async void PickButtonClick(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            var pickedFolder = await folderPicker.PickSingleFolderAsync();
            if (pickedFolder != null)
            {
                var filesInFolder = await pickedFolder.GetFilesAsync();
                _existingFileNames = filesInFolder.Select(f => f.Name).ToArray();
            }
            PickedFolder = pickedFolder;
        }

        private void FileNameSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var currentFileTypeChoice = (FileTypeChoiceListItem)FileTypeComboBox.SelectedItem;
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            {
                return;
            }

            var currentText = sender.Text;
            var matchingFileNames = _existingFileNames
                .Where(n => n != null && n.Contains(currentText, StringComparison.CurrentCultureIgnoreCase) &&
                    (!Path.HasExtension(n) ||
                     currentFileTypeChoice.Extensions.Contains(Path.GetExtension(n))))
                .Take(5)
                .ToArray();

            sender.ItemsSource = matchingFileNames;
        }

        private void UpdatePickedFolderDisplay()
        {
            IsPrimaryButtonEnabled = _pickedFolder != null;
            PickFolderLabel.Visibility = _pickedFolder != null ? Visibility.Collapsed : Visibility.Visible;
            PickedFolderNameLabel.Text = _pickedFolder?.DisplayName ?? string.Empty;
        }
    }

    internal class FileTypeChoiceListItem
    {
        public FileTypeChoiceListItem(string label, IList<string> extensions)
        {
            Label = label;
            Extensions = extensions;
        }

        public string Label { get; }

        public IList<string> Extensions { get; }
    }
}
