using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Uno.UI.Runtime.Skia.Wpf;

namespace SkiaSharpTest.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var host = new WpfHost(Dispatcher, () => new SkiaSharpTest.App());
            host.Run();
        }
    }
}
