using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace SplashScreenSample
{
    public sealed partial class Shell : UserControl
    {
#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
        public bool IsSplashCapable => true;
#else
        public bool IsSplashCapable => false;
#endif
        public Shell()
        {
            this.InitializeComponent();
        }
    }
}