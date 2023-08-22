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
    internal class ViewModel : INotifyPropertyChanged
    {
        private Stream? _documentStream;
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
        /// Constructor of the view model class
        /// </summary>
        public ViewModel()
        {
            string fileName = "PDF_Succinctly.pdf";
            string basePath = "SyncfusionApp.MauiControls.Samples.Resources.Pdf.";
            if (BaseConfig.IsIndividualSB)
                basePath = "SyncfusionApp.MauiControls.Samples.Pdf.";
            DocumentStream = this.GetType().Assembly.GetManifestResourceStream(basePath + fileName);
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}