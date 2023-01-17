using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EmbeddedResourcesSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string currentFile;

        public MainPage()
        {
            this.InitializeComponent();
        }

        public string[] EmbeddedResources { get; } = typeof(MainPage).Assembly.GetManifestResourceNames();

        public string CurrentFile
        {
            get => currentFile;
            set
            {
                currentFile = value;

                using (var s = typeof(MainPage).Assembly.GetManifestResourceStream(value))
                {
                    var r = new StreamReader(s);
                    content.Text = r.ReadToEnd();
                }
            }
        }
    }
}
