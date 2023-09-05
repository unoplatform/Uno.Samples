#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using SyncfusionApp.MauiControls.Samples.Base;
using System.ComponentModel;
using System.Windows.Input;

namespace SyncfusionApp.MauiControls.Samples.PdfViewer.SfPdfViewer
{
    public class PasswordProtectedViewModel : INotifyPropertyChanged
    {
        private Stream? _documentStream;
        private ICommand? _loadEncryptedDocument;
        private ContentView? _content;
        public event PropertyChangedEventHandler? PropertyChanged;
        private PdfView pdfView;
        private PasswordView passwordView;

        /// <summary>
        /// Gets or sets the PDF document as a stream. 
        /// </summary>
        public Stream? DocumentStream
        {
            get => _documentStream;
            set
            {
                _documentStream = value;
                OnPropertyChanged("DocumentStream");
            }
        }
        public ICommand LoadEncryptedDocument
        {
            get
            {
                if (_loadEncryptedDocument == null)
                    _loadEncryptedDocument = new Command<object>(OpenEncryptedDocument);
                return _loadEncryptedDocument;
            }
        }

        public ContentView Content
        {
            get => _content ??= passwordView;
            set
            {
                _content = value;
                OnPropertyChanged("Content");
            }
        }

        public PasswordProtectedViewModel()
        {
            pdfView = new PdfView(this);
            passwordView = new PasswordView(this);
            Content = passwordView;
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void ToggleContent()
        {
            Content = Content == pdfView ? passwordView : pdfView;
        }

        public void Unload()
        {
            pdfView?.Unload();
        }
        
        void OpenEncryptedDocument(object commandParameter) 
        {
            Content = pdfView;
            string fileName = "password_protected_document.pdf";
            string basePath = "SyncfusionApp.MauiControls.Samples.Resources.Pdf.";
            if (BaseConfig.IsIndividualSB)
                basePath = "SyncfusionApp.MauiControls.Samples.PdfViewer.Samples.Pdf.";
            DocumentStream = this.GetType().Assembly.GetManifestResourceStream(basePath + fileName);
        }
    }
}