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
        public static bool IsSplashCapable = false;

        public Shell()
        {
            this.InitializeComponent();
#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
            IsSplashCapable = true;
#endif
        }
    }
}