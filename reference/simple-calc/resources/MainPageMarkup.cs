﻿using Uno.Extensions.Markup;
using Uno.Material;
using Uno.Themes.Markup;
using Uno.Toolkit.UI;
using BrushBuilder = System.Action<Uno.Extensions.Markup.IDependencyPropertyBuilder<Microsoft.UI.Xaml.Media.Brush>>;
#if MVU
using DataContextClass = SimpleCalculator.Presentation.BindableMainModel;
#else
using DataContextClass = SimpleCalculator.Presentation.MainViewModel;
#endif
namespace SimpleCalculator;

public sealed partial class MainPageMarkup : Page
{
    public MainPageMarkup()
    {
        this.DataContext(new DataContextClass(), (page, vm)
            => page
            .Resources(r => r
                .Add(AppIcons.Dark)
                .Add(AppIcons.Light)
            )
            .Background(Theme.Brushes.Background.Default)
            .Content
            (
                new Grid()
                .RowDefinitions<Grid>("Auto,*")
                .MaxWidth(700)
                .Background(Theme.Brushes.Background.Default)
                .VerticalAlignment(VerticalAlignment.Stretch)
                .SafeArea(SafeArea.InsetMask.VisibleBounds)
                .Children
                (
                    Header(vm),
                    new StackPanel()
                    .Grid(row: 1)
                    .VerticalAlignment(VerticalAlignment.Bottom)
                    .Children
                    (
                        Output(vm),
                        KeyPad(vm)
                    )
                )
            )
        );
    }

    private ToggleButton Header(DataContextClass vm)
        => new ToggleButton()
            .Grid(row: 0)
            .Margin(8)
            .CornerRadius(20)
            .HorizontalAlignment(HorizontalAlignment.Center)
            .Background(Theme.Brushes.Secondary.Container.Default)
            .Style(Theme.Styles.ToggleButton.Icon)
            .IsChecked(x => x.Bind(() => vm.IsDark).Mode(BindingMode.TwoWay))
            .Content
            (
                new PathIcon()
                    .Data(AppIcons.Light)
                    .Foreground(Theme.Brushes.Primary.VariantDark.Default)
            )
            .ControlExtensions
            (
                alternateContent:
                new PathIcon()
                    .Data(AppIcons.Dark)
                    .Foreground(Theme.Brushes.Primary.VariantDark.Default)
            );

    private StackPanel Output(DataContextClass vm)
        => new StackPanel()
        .Spacing(16)
        .Padding(16, 8)
        .HorizontalAlignment(HorizontalAlignment.Stretch)
        .VerticalAlignment(VerticalAlignment.Bottom)
        .Children
        (
            Equation(vm),
            Result(vm)
        );

    private TextBlock Equation(DataContextClass vm)
        => new TextBlock()
        .Text(() => vm.Calculator.Equation)
        .HorizontalAlignment(HorizontalAlignment.Right)
        .Foreground(Theme.Brushes.OnSecondary.Container.Default)
        .Style(Theme.Styles.TextBlock.DisplaySmall);

    private TextBlock Result(DataContextClass vm)
        => new TextBlock()
        .Text(() => vm.Calculator.Output)
        .HorizontalAlignment(HorizontalAlignment.Right)
        .Foreground(Theme.Brushes.OnBackground.Default)
        .Style(Theme.Styles.TextBlock.DisplayLarge);

    private Grid KeyPad(DataContextClass vm)
        => new Grid()
        .RowSpacing(16)
        .ColumnSpacing(16)
        .Padding(16)
        .MaxHeight(500)
        .ColumnDefinitions<Grid>("*,*,*,*")
        .RowDefinitions<Grid>("*,*,*,*,*")
        .Children
        (
            // Row 0
            KeyPadButton(vm, 0, 0, "C", Theme.Brushes.Primary.Container.Default, Theme.Brushes.OnSecondary.Container.Default),
            KeyPadButton(vm, 0, 1, "±", Theme.Brushes.Primary.Container.Default, Theme.Brushes.OnSecondary.Container.Default),
            KeyPadButton(vm, 0, 2, "%", Theme.Brushes.Primary.Container.Default, Theme.Brushes.OnSecondary.Container.Default),
            KeyPadButton(vm, 0, 3, "÷", Theme.Brushes.Primary.VariantDark.Default, Theme.Brushes.OnTertiary.Default),

            // Row 1
            KeyPadButton(vm, 1, 0, "7"),
            KeyPadButton(vm, 1, 1, "8"),
            KeyPadButton(vm, 1, 2, "9"),
            KeyPadButton(vm, 1, 3, "×", Theme.Brushes.Primary.VariantDark.Default, Theme.Brushes.OnTertiary.Default),

            // Row 2
            KeyPadButton(vm, 2, 0, "4"),
            KeyPadButton(vm, 2, 1, "5"),
            KeyPadButton(vm, 2, 2, "6"),
            KeyPadButton(vm, 2, 3, "−", Theme.Brushes.Primary.VariantDark.Default, Theme.Brushes.OnTertiary.Default),

            //Row 3
            KeyPadButton(vm, 3, 0, "1"),
            KeyPadButton(vm, 3, 1, "2"),
            KeyPadButton(vm, 3, 2, "3"),
            KeyPadButton(vm, 3, 3, "+", Theme.Brushes.Primary.VariantDark.Default, Theme.Brushes.OnTertiary.Default),

            //Row 4
            KeyPadButton(vm, 4, 0, "."),
            KeyPadButton(vm, 4, 1, "0"),
            KeyPadButton(vm, 4, 2, "<-"),
            KeyPadButton(vm, 4, 3, "=", Theme.Brushes.Primary.VariantDark.Default, Theme.Brushes.OnTertiary.Default)
        );

    private Button KeyPadButton(DataContextClass vm, int gridRow, int gridColumn, string content, BrushBuilder? background = null, BrushBuilder? foreground = null)
        => new Button()
        .Command(() => vm.InputCommand)
        .CommandParameter(content)
        .Background(background ?? Theme.Brushes.Secondary.Container.Default)
        .Content(content)
        .FontSize(32)
        .Foreground(foreground ?? Theme.Brushes.OnSurface.Default)
        .Grid(row: gridRow, column: gridColumn)
        .HorizontalAlignment(HorizontalAlignment.Stretch)
        .VerticalAlignment(VerticalAlignment.Stretch)
        .Height(72);
}

public static class AppIcons
{
    public static readonly Resource<Geometry> Dark =
      StaticResource.Create<Geometry>("MoonIcon", "F1 M 3 0 C 1.9500000476837158 0 0.949999988079071 0.1600000262260437 0 0.46000003814697266 C 4.059999942779541 1.7300000190734863 7 5.519999980926514 7 10 C 7 14.480000019073486 4.059999942779541 18.27000093460083 0 19.540000915527344 C 0.949999988079071 19.840000927448273 1.9500000476837158 20 3 20 C 8.519999980926514 20 13 15.519999980926514 13 10 C 13 4.480000019073486 8.519999980926514 0 3 0 Z");

    public static readonly Resource<Geometry> Light =
      StaticResource.Create<Geometry>("SunIcon", "F1 M 5.760000228881836 4.289999961853027 L 3.9600000381469727 2.5 L 2.549999952316284 3.9100000858306885 L 4.340000152587891 5.699999809265137 L 5.760000228881836 4.289999961853027 Z M 3 9.949999809265137 L 0 9.949999809265137 L 0 11.949999809265137 L 3 11.949999809265137 L 3 9.949999809265137 Z M 12 0 L 10 0 L 10 2.950000047683716 L 12 2.950000047683716 L 12 0 L 12 0 Z M 19.450000762939453 3.9100000858306885 L 18.040000915527344 2.5 L 16.25 4.289999961853027 L 17.65999984741211 5.699999809265137 L 19.450000762939453 3.9100000858306885 Z M 16.239999771118164 17.610000610351562 L 18.030000686645508 19.40999984741211 L 19.440000534057617 18 L 17.639999389648438 16.21000099182129 L 16.239999771118164 17.610000610351562 Z M 19 9.949999809265137 L 19 11.949999809265137 L 22 11.949999809265137 L 22 9.949999809265137 L 19 9.949999809265137 Z M 11 4.949999809265137 C 7.690000057220459 4.949999809265137 5 7.639999866485596 5 10.949999809265137 C 5 14.259999752044678 7.690000057220459 16.950000762939453 11 16.950000762939453 C 14.309999942779541 16.950000762939453 17 14.259999752044678 17 10.949999809265137 C 17 7.639999866485596 14.309999942779541 4.949999809265137 11 4.949999809265137 Z M 10 21.900001525878906 L 12 21.900001525878906 L 12 18.950000762939453 L 10 18.950000762939453 L 10 21.900001525878906 Z M 2.549999952316284 17.990001678466797 L 3.9600000381469727 19.400001525878906 L 5.75 17.600000381469727 L 4.340000152587891 16.190000534057617 L 2.549999952316284 17.990001678466797 Z");
}