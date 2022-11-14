using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using SimpleCalculator.Presentation;
using System;
using System.Collections.Generic;
using System.Text;
using Uno.Material;
using Uno.Toolkit.UI;

namespace SimpleCalculator
{
    [Bindable]
    public sealed partial class MainPageMarkup : Page
    {
        public MainPageMarkup()
        {
            DataContext = new MainModel.MainViewModel();

            this.DataContext<MainModel.MainViewModel>((page, vm)
                => page
                .Resources(r => r
                    .Add("Icon_Brightness", "F1 M 3 0 C 1.9500000476837158 0 0.949999988079071 0.1600000262260437 0 0.46000003814697266 C 4.059999942779541 1.7300000190734863 7 5.519999980926514 7 10 C 7 14.480000019073486 4.059999942779541 18.27000093460083 0 19.540000915527344 C 0.949999988079071 19.840000927448273 1.9500000476837158 20 3 20 C 8.519999980926514 20 13 15.519999980926514 13 10 C 13 4.480000019073486 8.519999980926514 0 3 0 Z")
                    .Add("Icon_Wb_Sunny", "F1 M 5.760000228881836 4.289999961853027 L 3.9600000381469727 2.5 L 2.549999952316284 3.9100000858306885 L 4.340000152587891 5.699999809265137 L 5.760000228881836 4.289999961853027 Z M 3 9.949999809265137 L 0 9.949999809265137 L 0 11.949999809265137 L 3 11.949999809265137 L 3 9.949999809265137 Z M 12 0 L 10 0 L 10 2.950000047683716 L 12 2.950000047683716 L 12 0 L 12 0 Z M 19.450000762939453 3.9100000858306885 L 18.040000915527344 2.5 L 16.25 4.289999961853027 L 17.65999984741211 5.699999809265137 L 19.450000762939453 3.9100000858306885 Z M 16.239999771118164 17.610000610351562 L 18.030000686645508 19.40999984741211 L 19.440000534057617 18 L 17.639999389648438 16.21000099182129 L 16.239999771118164 17.610000610351562 Z M 19 9.949999809265137 L 19 11.949999809265137 L 22 11.949999809265137 L 22 9.949999809265137 L 19 9.949999809265137 Z M 11 4.949999809265137 C 7.690000057220459 4.949999809265137 5 7.639999866485596 5 10.949999809265137 C 5 14.259999752044678 7.690000057220459 16.950000762939453 11 16.950000762939453 C 14.309999942779541 16.950000762939453 17 14.259999752044678 17 10.949999809265137 C 17 7.639999866485596 14.309999942779541 4.949999809265137 11 4.949999809265137 Z M 10 21.900001525878906 L 12 21.900001525878906 L 12 18.950000762939453 L 10 18.950000762939453 L 10 21.900001525878906 Z M 2.549999952316284 17.990001678466797 L 3.9600000381469727 19.400001525878906 L 5.75 17.600000381469727 L 4.340000152587891 16.190000534057617 L 2.549999952316284 17.990001678466797 Z")
                )
                .Background(ThemeResource.Get<Brush>("BackgroundBrush"))
                .Content
                (
                    new AutoLayout()
                    .MaxWidth(700)
                    .SafeArea(SafeArea.InsetMask.VisibleBounds)
                    .Children
                    (
                        Header(vm),

                        // Output zone
                        new AutoLayout()
                            .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                            .Spacing(16)
                            .Padding(16, 8)
                            .PrimaryAxisAlignment(AutoLayoutAlignment.End)
                            .Children
                            (
                               Equation(vm),
                               Result(vm)
                            ),

                        // Keypad
                        KeyPad(vm)
                    )
                )
            );
        }

        private ToggleButton Header(MainModel.MainViewModel vm)
           => new ToggleButton()
               .Margin(8)
               .AutoLayout(counterAlignment: AutoLayoutAlignment.Center)
               .Style(StaticResource.Get<Style>("IconToggleButtonStyle"))
               //.IsChecked(x => x.Bind(() => vm.IsDark).Mode(BindingMode.TwoWay))
               .Content
               (
                   new PathIcon()
                       .Data(StaticResource.Get<Geometry>("Icon_Wb_Sunny"))
                       .Foreground(ThemeResource.Get<Brush>("TertiaryBrush"))
               )
               .ControlExtensions
               (
                   alternateContent:
                   new PathIcon()
                       .Data(StaticResource.Get<Geometry>("Icon_Brightness"))
                       .Foreground(ThemeResource.Get<Brush>("TertiaryBrush"))
               );

        private TextBlock Equation(MainModel.MainViewModel vm)
            => new TextBlock()
            .Text(() => vm.Calculator.Equation)
            .FontFamily(new FontFamily("Roboto"))
            .FontSize(36)
            .AutoLayout(counterAlignment: AutoLayoutAlignment.End)
            .Foreground(ThemeResource.Get<Brush>("OnSecondaryContainerBrush"))
            .Opacity(0.4)
            .Style(StaticResource.Get<Style>("DisplaySmall"));

        private TextBlock Result(MainModel.MainViewModel vm)
            => new TextBlock()
            .Text(() => vm.Calculator.Output)
            .FontFamily(new FontFamily("Roboto"))
            .FontSize(57)
            .AutoLayout(counterAlignment: AutoLayoutAlignment.End)
            .Foreground(ThemeResource.Get<Brush>("OnSecondaryContainerBrush"))
            .Style(StaticResource.Get<Style>("DisplayLarge"));

        private Grid KeyPad(MainModel.MainViewModel vm)
            => new Grid()
            .RowSpacing(16)
            .ColumnSpacing(16)
            .Padding(16)
            .MaxHeight(500)
            .ColumnDefinitions
            (
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            )
            .RowDefinitions
            (
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
            )
            .Children
            (
                // Row 0
                KeyPadButton(vm, 0, 0, "C", "SecondaryBrush", "OnSecondaryContainerBrush"),
                KeyPadButton(vm, 0, 1, "±", "SecondaryBrush", "OnSecondaryContainerBrush", "+-"),
                KeyPadButton(vm, 0, 2, "%", "SecondaryBrush", "OnSecondaryContainerBrush"),
                KeyPadButton(vm, 0, 3, "÷", "TertiaryBrush", "OnTertiaryBrush"),

                // Row 1
                KeyPadButton(vm, 1, 0, "7", "SurfaceBrush", "OnSurfaceBrush"),
                KeyPadButton(vm, 1, 1, "8", "SurfaceBrush", "OnSurfaceBrush"),
                KeyPadButton(vm, 1, 2, "9", "SurfaceBrush", "OnSurfaceBrush"),
                KeyPadButton(vm, 1, 3, "×", "TertiaryBrush", "OnTertiaryBrush"),

                // Row 2
                KeyPadButton(vm, 2, 0, "4", "SurfaceBrush", "OnSurfaceBrush"),
                KeyPadButton(vm, 2, 1, "5", "SurfaceBrush", "OnSurfaceBrush"),
                KeyPadButton(vm, 2, 2, "6", "SurfaceBrush", "OnSurfaceBrush"),
                KeyPadButton(vm, 2, 3, "–", "TertiaryBrush", "OnTertiaryBrush", parameter: "-"),

                //Row 3
                KeyPadButton(vm, 3, 0, "1", "SurfaceBrush", "OnSurfaceBrush"),
                KeyPadButton(vm, 3, 1, "2", "SurfaceBrush", "OnSurfaceBrush"),
                KeyPadButton(vm, 3, 2, "3", "SurfaceBrush", "OnSurfaceBrush"),
                KeyPadButton(vm, 3, 3, "+", "TertiaryBrush", "OnTertiaryBrush"),

                //Row 4
                KeyPadButton(vm, 4, 0, ".", "SurfaceBrush", "OnSurfaceBrush"),
                KeyPadButton(vm, 4, 1, "0", "SurfaceBrush", "OnSurfaceBrush"),
#if HAS_UNO_SKIA
                KeyPadButton(vm, 4, 2, "<-", "SurfaceBrush", "OnSurfaceBrush", parameter: "back"),
#else
                KeyPadButton(vm, 4, 2, "⌫", "SurfaceBrush", "OnSurfaceBrush", parameter: "back"),
#endif
                KeyPadButton(vm, 4, 3, "=", "TertiaryBrush", "OnTertiaryBrush")
            );

        private Button KeyPadButton(MainModel.MainViewModel vm, int gridRow, int gridColumn, string content, string background, string foreground = "OnPrimaryBrush", string? parameter = null)
            => new Button()
            .Command(() => vm.Input)
            .CommandParameter(parameter ?? content)
            .Background(ThemeResource.Get<Brush>(background))
            .Content(content)
            .FontFamily(new FontFamily("Roboto"))
            .FontSize(32)
            .Foreground(ThemeResource.Get<Brush>(foreground))
            .Grid(row: gridRow, column: gridColumn)
            .HorizontalAlignment(HorizontalAlignment.Stretch)
            .VerticalAlignment(VerticalAlignment.Stretch)
            .Height(72);
    }
}
