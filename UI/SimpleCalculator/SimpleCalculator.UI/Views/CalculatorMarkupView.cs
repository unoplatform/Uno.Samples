using System;
using System.Collections.Generic;
using System.Text;
using global::System;
using global::System.Windows.Input;
using global::Uno.Toolkit.UI;
using global::Microsoft.UI.Xaml;
using global::Microsoft.UI.Xaml.Controls;
using global::Microsoft.UI.Xaml.Controls.Primitives;
using global::Microsoft.UI.Xaml.Media;

namespace SimpleCalculator.Views
{
    public sealed partial class CalculatorMarkupView : Page
    {
        public CalculatorMarkupView()
        {
            InitMarkupView();
        }

        private void InitMarkupView()
        {
            this
                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("BackgroundBrush"))
                .Content
                (
                    new global::Uno.Toolkit.UI.AutoLayout()
                        .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("SurfaceBrush"))
                        .Children
                        (
                            new global::Microsoft.UI.Xaml.Controls.ToggleSwitch()
                                .Margin(8)
                                .AutoLayout(counterAlignment: AutoLayoutAlignment.Center)
                            ,
                            new global::Uno.Toolkit.UI.AutoLayout()
                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                .Spacing(8)
                                .Padding(8)
                                .PrimaryAxisAlignment(global::Uno.Toolkit.UI.AutoLayoutAlignment.End)
                                .Children
                                (
                                    new global::Microsoft.UI.Xaml.Controls.TextBlock()
                                        .Text(b => b.Bind("Calculator.Equation"))
                                        .AutoLayout(counterAlignment: AutoLayoutAlignment.End)
                                        .Foreground(Microsoft.UI.Xaml.Resource.Theme<Brush>("OnSecondaryContainerBrush"))
                                    ,
                                    new global::Microsoft.UI.Xaml.Controls.TextBlock()
                                        .Text(b => b.Bind("Calculator.OutPut"))
                                        .AutoLayout(counterAlignment: AutoLayoutAlignment.End)
                                        .Foreground(Microsoft.UI.Xaml.Resource.Theme<Brush>("OnSecondaryContainerBrush"))
                                        .Style(Microsoft.UI.Xaml.Resource.Static<global::Microsoft.UI.Xaml.Style>("DisplayLarge"))
                                )
                            ,
                            new global::Uno.Toolkit.UI.AutoLayout()
                                .Spacing(5)
                                .Padding(8)
                                .Height(500)
                                .Children
                                (
                                    new global::Uno.Toolkit.UI.AutoLayout()
                                        .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                        .Spacing(8)
                                        .Padding(10)
                                        .Orientation(global::Microsoft.UI.Xaml.Controls.Orientation.Horizontal)
                                        .Children
                                        (
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("C")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("SecondaryBrush"))
                                                .Content("C")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                                .Foreground(Microsoft.UI.Xaml.Resource.Theme<Brush>("OnSecondaryBrush"))
                                            ,
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("+-")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("SecondaryBrush"))
                                                .Content("+/-")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                                .Foreground(Microsoft.UI.Xaml.Resource.Theme<Brush>("OnSecondaryBrush"))
                                            ,
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("%")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("SecondaryBrush"))
                                                .Content("%")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                                .Foreground(Microsoft.UI.Xaml.Resource.Theme<Brush>("OnSecondaryBrush"))
                                            ,
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("÷")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("TertiaryBrush"))
                                                .Content("÷")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                                .Foreground(Microsoft.UI.Xaml.Resource.Theme<Brush>("OnTertiaryBrush"))
                                        )
                                    ,
                                    new global::Uno.Toolkit.UI.AutoLayout()
                                        .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                        .Spacing(8)
                                        .Padding(10)
                                        .Orientation(global::Microsoft.UI.Xaml.Controls.Orientation.Horizontal)
                                        .Children
                                        (
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("7")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("PrimaryBrush"))
                                                .Content("7")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                            ,
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("8")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("PrimaryBrush"))
                                                .Content("8")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                            ,
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("9")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("PrimaryBrush"))
                                                .Content("9")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                            ,
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("×")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("TertiaryBrush"))
                                                .Content("×")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                                .Foreground(Microsoft.UI.Xaml.Resource.Theme<Brush>("OnTertiaryBrush"))
                                        )
                                    ,
                                    new global::Uno.Toolkit.UI.AutoLayout()
                                        .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                        .Spacing(8)
                                        .Padding(10)
                                        .Orientation(global::Microsoft.UI.Xaml.Controls.Orientation.Horizontal)
                                        .Children
                                        (
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("4")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("PrimaryBrush"))
                                                .Content("4")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                            ,
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("5")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("PrimaryBrush"))
                                                .Content("5")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                            ,
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("6")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("PrimaryBrush"))
                                                .Content("6")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                            ,
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("-")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("TertiaryBrush"))
                                                .Content("-")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                                .Foreground(Microsoft.UI.Xaml.Resource.Theme<Brush>("OnTertiaryBrush"))
                                        )
                                    ,
                                    new global::Uno.Toolkit.UI.AutoLayout()
                                        .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                        .Spacing(8)
                                        .Padding(10)
                                        .Orientation(global::Microsoft.UI.Xaml.Controls.Orientation.Horizontal)
                                        .Children
                                        (
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("1")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("PrimaryBrush"))
                                                .Content("1")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                            ,
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("2")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("PrimaryBrush"))
                                                .Content("2")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                            ,
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("3")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("PrimaryBrush"))
                                                .Content("3")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                            ,
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("+")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("TertiaryBrush"))
                                                .Content("+")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                                .Foreground(Microsoft.UI.Xaml.Resource.Theme<Brush>("OnTertiaryBrush"))
                                        )
                                    ,
                                    new global::Uno.Toolkit.UI.AutoLayout()
                                        .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                        .Spacing(8)
                                        .Padding(10)
                                        .Orientation(global::Microsoft.UI.Xaml.Controls.Orientation.Horizontal)
                                        .Children
                                        (
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter(".")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("PrimaryBrush"))
                                                .Content(".")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                            ,
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("0")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("PrimaryBrush"))
                                                .Content("0")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                            ,
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("back")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("PrimaryBrush"))
                                                .Content("<")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                            ,
                                            new global::Microsoft.UI.Xaml.Controls.Button()
                                                .Command(b => b.Bind("Input"))
                                                .CommandParameter("=")
                                                .Background(Microsoft.UI.Xaml.Resource.Theme<Brush>("TertiaryBrush"))
                                                .Content("=")
                                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                                .Foreground(Microsoft.UI.Xaml.Resource.Theme<Brush>("OnTertiaryBrush"))
                                        )
                                )
                        )
                )
            ;
        }
    }
}
