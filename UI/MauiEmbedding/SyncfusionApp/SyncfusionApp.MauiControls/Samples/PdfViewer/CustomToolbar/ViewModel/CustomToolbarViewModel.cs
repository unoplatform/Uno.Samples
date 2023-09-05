#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System.Windows.Input;

namespace SyncfusionApp.MauiControls.Samples.PdfViewer.SfPdfViewer
{
    public class CustomToolbarViewModel : INotifyPropertyChanged
    {
        private PdfData _documentData;
        private bool _isFilePickerVisible;
        private ICommand? _openCloseFilePickerCommand;
        private ICommand? _zoomInCommand;
        private ICommand? _zoomOutCommand;
        private ICommand? _closeAllDialogsCommand;
        private object? _selectedFile;
        private double _currentZoom = 1;
        private double? _minZoom = null;
        private double? _maxZoom = null;
        private bool _showPasswordDialog = false;
        private bool _showMessageBoxDialog = false;
        private bool _showHyperlinkDialog = false;
        private bool _showOutlineView = false;
        bool m_isDocumentLoaded;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        ///  Returns the Pdf data. 
        /// </summary>
        public PdfData DocumentData => _documentData;

        /// <summary>
        /// Gets or sets the selected PDF file. 
        /// </summary>
        public object? SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                if (value != null)
                {
                    IsFilePickerVisible = false;
                    UpdateFileName(value.ToString());
                }
            }
        }

        // Initialize the files with some custom values.
        public IList<string> Files
        {
            get
            {
                return new List<string>
                {
                    "PDF Succinctly",
                    "Rotated PDF",
                    "Password protected PDF",
                    "Corrupted PDF",
                    "Single page PDF",
#if !MACCATALYST
                    "Browse files on this device"
#endif
                };
            }
        }

        /// <summary>
        ///  Determines whether the file picker can show in its current state.
        /// </summary>
        public bool IsFilePickerVisible
        {
            get => _isFilePickerVisible;
            set
            {
                _isFilePickerVisible = value;
                OnPropertyChanged("IsFilePickerVisible");
            }
        }

        /// <summary>
        ///  Determines whether the password box dialog can show in its current state.
        /// </summary>
        public bool ShowPasswordDialog
        {
            get => _showPasswordDialog;
            set
            {
                _showPasswordDialog = value;
                OnPropertyChanged("ShowPasswordDialog");
            }
        }

        /// <summary>
        ///  Determines whether the Message box dialog can show in its current state.
        /// </summary>
        public bool ShowMessageBoxDialog
        {
            get => _showMessageBoxDialog;
            set
            {
                _showMessageBoxDialog = value;
                OnPropertyChanged("ShowMessageBoxDialog");
            }
        }

        /// <summary>
        ///  Determines whether the Hyperlink dialog can show in its current state.
        /// </summary>
        public bool ShowHyperlinkDialog
        {
            get => _showHyperlinkDialog;
            set
            {
                _showHyperlinkDialog = value;
                OnPropertyChanged("ShowHyperlinkDialog");
            }
        }

        public bool ShowOutlineView
        {
            get => _showOutlineView;
            set
            {
                _showOutlineView = value;
                OnPropertyChanged("ShowOutlineView");
            }
        }

        /// <summary>
        /// Gets or sets the current zoom.
        /// </summary>
        public double CurrentZoom
        {
            get => _currentZoom;
            set
            {
                _currentZoom = value;
                OnPropertyChanged("CurrentZoom");
                RefreshZoomCommandCanExecutes();
            }
        }

        /// <summary>
        /// Gets or sets the minimum zoom.
        /// </summary>
        public double? MinZoom
        {
            get => _minZoom;
            set
            {
                _minZoom = value;
                RefreshZoomCommandCanExecutes();
            }
        }

        /// <summary>
        /// Gets or sets the value that indicates whether the document is loaded.
        /// </summary>
        public bool IsDocumentLoaded
        {
            get => m_isDocumentLoaded;
            set
            {
                m_isDocumentLoaded = value;
                RefreshZoomCommandCanExecutes();
                OnPropertyChanged(nameof(IsDocumentLoaded));
            }
        }

        /// <summary>
        /// Gets or sets the maximum zoom.
        /// </summary>
        public double? MaxZoom
        {
            get => _maxZoom;
            set
            {
                _maxZoom = value;
                RefreshZoomCommandCanExecutes();
            }
        }

        /// <summary>
        /// Gets the command to open or close the file picker.
        /// </summary>
        public ICommand OpenCloseFilePickerCommand
        {
            get
            {
                if (_openCloseFilePickerCommand == null)
                    _openCloseFilePickerCommand = new Command<object>(OpenCloseFilePicker);
                return _openCloseFilePickerCommand;
            }
        }

        /// <summary>
        /// Gets the command to increase the current zoom.
        /// </summary>
        public ICommand ZoomInCommand
        {
            get
            {
                if (_zoomInCommand == null)
                    _zoomInCommand = new Command(() => CurrentZoom += 0.25,
                        canExecute: () => { return CurrentZoom < MaxZoom && IsDocumentLoaded; });
                return _zoomInCommand;
            }
        }

        /// <summary>
        /// Gets the command to decrease the current zoom.
        /// </summary>
        public ICommand ZoomOutCommand
        {
            get
            {
                if (_zoomOutCommand == null)
                    _zoomOutCommand = new Command(() => CurrentZoom -= 0.25,
                        canExecute: () => { return CurrentZoom > MinZoom && IsDocumentLoaded; });
                return _zoomOutCommand;
            }
        }

        public ICommand CloseAllDialogsCommand
        {
            get
            {
                if (_closeAllDialogsCommand == null)
                    _closeAllDialogsCommand = new Command(CloseAllDialogs);
                return _closeAllDialogsCommand;
            }
        }

        /// <summary>
        /// Constructor of the view model class
        /// </summary>
        public CustomToolbarViewModel()
        {
            _documentData = new PdfData();
            _documentData.FileName = "PDF_Succinctly1.pdf";
        }

        void CloseAllDialogs()
        {
            if (ShowPasswordDialog)
                ShowPasswordDialog = false;

            if (ShowMessageBoxDialog)
                ShowMessageBoxDialog = false;

            if (ShowHyperlinkDialog)
                ShowHyperlinkDialog = false;

            if (ShowOutlineView)
                ShowOutlineView = false;
            
            if (IsFilePickerVisible)
                IsFilePickerVisible = false;
        }

        /// <summary>
        /// Refreshes the can executes of the zoom commands.
        /// </summary>
        void RefreshZoomCommandCanExecutes()
        {
            if (ZoomInCommand is Command zoomInCommand)
                zoomInCommand.ChangeCanExecute();
            if (ZoomOutCommand is Command zoomOutCommand)
                zoomOutCommand.ChangeCanExecute();
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Executes the open or close the file picker command.
        /// </summary>
        void OpenCloseFilePicker(object commandParameter)
        {
            CloseAllDialogs();

            if (IsFilePickerVisible == true)
                IsFilePickerVisible = false;
            else
                IsFilePickerVisible = true;
        }

        /// <summary>
        /// Updates the selected PDF file name.
        /// </summary>
        async internal void UpdateFileName(string? file)
        {
            switch (file)
            {
                case "PDF Succinctly":
                    _documentData.FileName = "PDF_Succinctly1.pdf";
                    break;
                case "Rotated PDF":
                    _documentData.FileName = "rotated_document.pdf";
                    break;
                case "Password protected PDF":
                    _documentData.FileName = "password_protected_document.pdf";
                    break;
                case "Corrupted PDF":
                    _documentData.FileName = "corrupted_document.pdf";
                    break;
                case "Single page PDF":
                    _documentData.FileName = "Invoice.pdf";
                    break;
                case "Browse files on this device":
                    // Create file picker with file type as PDF.
                    FilePickerFileType pdfFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>{
                        { DevicePlatform.iOS, new[] { "public.pdf" } },
                        { DevicePlatform.Android, new[] { "application/pdf" } },
                        { DevicePlatform.WinUI, new[] { "pdf" } },
                        { DevicePlatform.MacCatalyst, new[] { "pdf" } },
                    });
                    // Provide picker title if required.
                    PickOptions options = new()
                    {
                        PickerTitle = "Please select a PDF file",
                        FileTypes = pdfFileType,
                    };
                    await PickAndShow(options);
                    break;
            }
        }

        /// <summary>
        /// Picks the file from local storage and store as stream.
        /// </summary>
        public async Task<FileResult?> PickAndShow(PickOptions options)
        {
            try
            {
                // Pick the file from local storage.
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    // Store the resultant PDF as stream.
                    _documentData.DocumentStream = await result.OpenReadAsync();
                }
                return result;
            }
            catch (Exception ex)
            {
                // Display error when file picker failed to open files.
                string message;
                if (ex != null && string.IsNullOrEmpty(ex.Message) == false)
                    message = ex.Message;
                else
                    message = "File open failed.";
                Application.Current?.MainPage?.DisplayAlert("Error", message, "OK");
            }
            return null;
        }
    }
}