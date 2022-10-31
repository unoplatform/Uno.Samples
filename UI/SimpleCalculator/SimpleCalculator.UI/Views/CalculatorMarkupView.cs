using Uno.Toolkit.UI;
using Button = Microsoft.UI.Xaml.Controls.Button;
using Microsoft.UI.Xaml.MarkupHelpers;
using static Uno.Toolkit.UI.SafeArea;

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
            this.DataContext<MainViewModel.BindableMainViewModel>((page, vm)
                => page.Background(ThemeResource.Get<Brush>("BackgroundBrush"))
                        .Content
                        (
                            new AutoLayout()
                            .SafeArea(InsetMask.VisibleBounds)
                            .Background(ThemeResource.Get<Brush>("SurfaceBrush"))
                            .Children
                            (
                                // Header
                                new ToggleSwitch()
                                    .Margin(8)
                                    .AutoLayout(counterAlignment: AutoLayoutAlignment.Center),

                                // Output zone
                                new AutoLayout()
                                    .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                    .Spacing(8)
                                    .Padding(8)
                                    .PrimaryAxisAlignment(AutoLayoutAlignment.End)
                                    .Children
                                    (
                                        new TextBlock()
                                            .Text(() => vm.Calculator.Equation)
                                            .AutoLayout(counterAlignment: AutoLayoutAlignment.End)
                                            .Foreground(ThemeResource.Get<Brush>("OnSecondaryContainerBrush"))
                                        ,
                                        new TextBlock()
                                            .Text(() => vm.Calculator.OutPut)
                                            .AutoLayout(counterAlignment: AutoLayoutAlignment.End)
                                            .Foreground(ThemeResource.Get<Brush>("OnSecondaryContainerBrush"))
                                            .Style(StaticResource.Get<Style>("DisplayLarge"))
                                    ),

                                // Keypad
                                new Grid()
                                    .RowSpacing(12)
                                    .ColumnSpacing(12)
                                    .Padding(12)
                                    .Height(500)
                                    .RowDefinitions("*", "*", "*", "*", "*")
                                    .ColumnDefinitions("*", "*", "*", "*")
                                    .Children
                                    (
                                        // Row 0
                                        RenderButton(vm, 0, 0, "C", "SecondaryBrush", "OnSecondaryBrush"),
                                        RenderButton(vm, 0, 1, "+/-", "SecondaryBrush", "OnSecondaryBrush", "+-"),
                                        RenderButton(vm, 0, 2, "%", "SecondaryBrush", "OnSecondaryBrush"),
                                        RenderButton(vm, 0, 3, "÷", "TertiaryBrush", "OnTertiaryBrush"),

                                        // Row 1
                                        RenderButton(vm, 1, 0, "7", "PrimaryBrush"),
                                        RenderButton(vm, 1, 1, "8", "PrimaryBrush"),
                                        RenderButton(vm, 1, 2, "9", "PrimaryBrush"),
                                        RenderButton(vm, 1, 3, "×", "TertiaryBrush", "OnTertiaryBrush"),

                                        // Row 2
                                        RenderButton(vm, 2, 0, "4", "PrimaryBrush"),
                                        RenderButton(vm, 2, 1, "5", "PrimaryBrush"),
                                        RenderButton(vm, 2, 2, "6", "PrimaryBrush"),
                                        RenderButton(vm, 2, 3, "-", "TertiaryBrush", "OnTertiaryBrush"),

                                        //Row 3
                                        RenderButton(vm, 3, 0, "1", "PrimaryBrush"),
                                        RenderButton(vm, 3, 1, "2", "PrimaryBrush"),
                                        RenderButton(vm, 3, 2, "3", "PrimaryBrush"),
                                        RenderButton(vm, 3, 3, "+", "TertiaryBrush", "OnTertiaryBrush"),

                                        //Row 4
                                        RenderButton(vm, 4, 0, ".", "PrimaryBrush"),
                                        RenderButton(vm, 4, 1, "0", "PrimaryBrush"),
                                        RenderButton(vm, 4, 2, "<", "PrimaryBrush", parameter: "back"),
                                        RenderButton(vm, 4, 3, "=", "TertiaryBrush", "OnTertiaryBrush")
                                    )
                            )
                        )
            );
        }

        private Button RenderButton(MainViewModel.BindableMainViewModel vm, int gridRow, int gridColumn, string content, string background, string foreground = "OnPrimaryBrush", string? parameter = null)
            => new Button()
            .Command(() => vm.Input)
            .CommandParameter(parameter ?? content)
            .Background(ThemeResource.Get<Brush>(background))
            .Content(content)
            .Foreground(ThemeResource.Get<Brush>(foreground))
            .Grid(row: gridRow, column: gridColumn)
            .HorizontalAlignment(HorizontalAlignment.Stretch)
            .VerticalAlignment(VerticalAlignment.Stretch);
    }
}
