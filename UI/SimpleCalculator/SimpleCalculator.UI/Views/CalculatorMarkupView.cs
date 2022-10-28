using Uno.Toolkit.UI;
using Button = Microsoft.UI.Xaml.Controls.Button;
using Resource = Microsoft.UI.Xaml.Resource;
using Orientation = Microsoft.UI.Xaml.Controls.Orientation;
using Microsoft.UI.Xaml.MarkupHelpers;

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
                .Background(ResourceTheme("BackgroundBrush"))
                .Content
                (
                    new AutoLayout()
                        .Background(ResourceTheme("SurfaceBrush"))
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
                                        .Text(b => b.Bind("Calculator.Equation"))
                                        .AutoLayout(counterAlignment: AutoLayoutAlignment.End)
                                        .Foreground(ResourceTheme("OnSecondaryContainerBrush"))
                                    ,
                                    new TextBlock()
                                        .Text(b => b.Bind("Calculator.OutPut"))
                                        .AutoLayout(counterAlignment: AutoLayoutAlignment.End)
                                        .Foreground(ResourceTheme("OnSecondaryContainerBrush"))
                                        .Style(Resource.Static<Style>("DisplayLarge"))
                                ),
                            
                            // Keypad
                            new Grid()
                                .RowSpacing(8)
                                .ColumnSpacing(8)
                                .Padding(8)
                                .Height(500)
                                .RowDefinitions("*", "*", "*", "*", "*")
                                .ColumnDefinitions("*", "*", "*", "*")
                                .Children
                                (
                                    // Row 0
                                    RenderButton("C", "SecondaryBrush", "OnSecondaryBrush").Grid(row: 0, column: 0),
                                    RenderButton("+/-", "SecondaryBrush", "OnSecondaryBrush", "+-").Grid(row: 0, column: 1),
                                    RenderButton("%", "SecondaryBrush", "OnSecondaryBrush").Grid(row: 0, column: 2),
                                    RenderButton("÷", "TertiaryBrush", "OnTertiaryBrush").Grid(row: 0, column: 3),

                                    // Row 1
                                    RenderButton("7", "PrimaryBrush").Grid(row: 1, column: 0),
                                    RenderButton("8", "PrimaryBrush").Grid(row: 1, column: 1),
                                    RenderButton("9", "PrimaryBrush").Grid(row: 1, column: 2),
                                    RenderButton("×", "TertiaryBrush", "OnTertiaryBrush").Grid(row: 1, column: 3),

                                    // Row 2
                                    RenderButton("4", "PrimaryBrush"),
                                    RenderButton("5", "PrimaryBrush"),
                                    RenderButton("6", "PrimaryBrush"),
                                    RenderButton("-", "TertiaryBrush", "OnTertiaryBrush")
                                )

                            // Keypad
                            new AutoLayout()
                                .Spacing(5)
                                .Padding(8)
                                .Height(500)
                                .Children
                                (
                                    RenderAutoLayout()
                                        .Children
                                        (
                                            RenderButton("C", "SecondaryBrush", "OnSecondaryBrush"),
                                            RenderButton("+/-", "SecondaryBrush", "OnSecondaryBrush", "+-"),
                                            RenderButton("%", "SecondaryBrush", "OnSecondaryBrush"),
                                            RenderButton("÷", "TertiaryBrush", "OnTertiaryBrush")
                                        )
                                    ,
                                    RenderAutoLayout()
                                        .Children
                                        (
                                            RenderButton("7", "PrimaryBrush"),
                                            RenderButton("8", "PrimaryBrush"),
                                            RenderButton("9", "PrimaryBrush"),
                                            RenderButton("×", "TertiaryBrush", "OnTertiaryBrush")
                                        )
                                    ,
                                    RenderAutoLayout()
                                        .Children
                                        (
                                            RenderButton("4", "PrimaryBrush"),
                                            RenderButton("5", "PrimaryBrush"),
                                            RenderButton("6", "PrimaryBrush"),
                                            RenderButton("-", "TertiaryBrush", "OnTertiaryBrush")
                                        )
                                    ,
                                    RenderAutoLayout()
                                        .Children
                                        (
                                            RenderButton("1", "PrimaryBrush"),
                                            RenderButton("2", "PrimaryBrush"),
                                            RenderButton("3", "PrimaryBrush"),
                                            RenderButton("+", "TertiaryBrush", "OnTertiaryBrush")
                                        )
                                    ,
                                    RenderAutoLayout()
                                        .Children
                                        (
                                            RenderButton(".", "PrimaryBrush"),
                                            RenderButton("0", "PrimaryBrush"),
                                            RenderButton("<", "PrimaryBrush", parameter: "back"),
                                            RenderButton("=", "TertiaryBrush", "OnTertiaryBrush")
                                        )
                                )
                        )
                )
            ;
        }

        private Button RenderButton(string content, string background, string foreground = "OnPrimaryBrush", string? parameter = null)
            => new Button()
            .Command(b => b.Bind("Input"))
            .CommandParameter(parameter ?? content)
            .Background(ResourceTheme(background))
            .Content(content)
            //.AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
            .Foreground(ResourceTheme(foreground));

        // private AutoLayout RenderAutoLayout()
        //     => new AutoLayout()
        //     .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
        //     .Spacing(8)
        //     .Padding(10)
        //     .Orientation(Orientation.Horizontal);

        private Action<IDependencyPropertyBuilder<Brush>> ResourceTheme(string name) 
            => Resource.Theme<Brush>(name);

    }
}
