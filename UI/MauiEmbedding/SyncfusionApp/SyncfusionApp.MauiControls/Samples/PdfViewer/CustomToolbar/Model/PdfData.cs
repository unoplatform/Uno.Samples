#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using SyncfusionApp.MauiControls.Samples.Base;
using System.ComponentModel;

namespace SyncfusionApp.MauiControls.Samples.PdfViewer.SfPdfViewer
{
    public class PdfData : INotifyPropertyChanged
    {
        private Stream? _documentStream;
        private string? _fileName;

        public event PropertyChangedEventHandler? PropertyChanged;

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

        /// <summary>
        /// Gets or sets the selected PDF file name.
        /// </summary>
        public string? FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                string basePath = "SyncfusionApp.MauiControls.Samples.Resources.Pdf.";
                if (BaseConfig.IsIndividualSB)
                    basePath = "SyncfusionApp.MauiControls.Samples.PdfViewer.Samples.Pdf.";
                if (string.IsNullOrEmpty(value) == false)
                    DocumentStream = this.GetType().Assembly.GetManifestResourceStream(basePath + value);
            }
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}