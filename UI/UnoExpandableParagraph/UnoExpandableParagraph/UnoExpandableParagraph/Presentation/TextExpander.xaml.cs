using Microsoft.UI;
using Microsoft.UI.Text;

using Windows.UI;
using Windows.UI.Text;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UnoExpandableParagraph.Presentation
{
    public sealed partial class TextExpander : UserControl
    {
        static TextBlock _bodyText;

        public TextExpander()
        {
            this.InitializeComponent();

            //_bodyText = this.FindName("bodyText") as TextBlock;
        }

        #region Custom Dependency Properties' Register
       
        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(string), typeof(TextExpander), new PropertyMetadata(null));
        public static readonly DependencyProperty BodyTextProperty = DependencyProperty.Register("BodyText", typeof(string), typeof(TextExpander), new PropertyMetadata(null));

        public static readonly DependencyProperty FooterButtonTextProperty = DependencyProperty.Register("FooterButtonText", typeof(string), typeof(TextExpander), new PropertyMetadata("More"));
        public static readonly DependencyProperty ExpandedButtonTextProperty = DependencyProperty.Register("ExpandedButtonText", typeof(string), typeof(TextExpander), new PropertyMetadata("Hide"));

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(TextExpander), new PropertyMetadata(false));

        public static readonly DependencyProperty HeaderTextFontSizeProperty = DependencyProperty.Register(nameof(HeaderTextFontSize), typeof(double), typeof(TextExpander), new PropertyMetadata(14.0));
        public static readonly DependencyProperty HeaderTextColorProperty = DependencyProperty.Register(nameof(HeaderTextColor), typeof(Brush), typeof(TextExpander), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        public static readonly DependencyProperty HeaderTextWeightProperty = DependencyProperty.Register(nameof(HeaderTextWeight), typeof(FontWeight), typeof(TextExpander), new PropertyMetadata(FontWeights.Normal));

        public static readonly DependencyProperty BodyTextFontSizeProperty = DependencyProperty.Register(nameof(BodyTextFontSize), typeof(double), typeof(TextExpander), new PropertyMetadata(14.0));
        public static readonly DependencyProperty BodyTextColorProperty = DependencyProperty.Register(nameof(BodyTextColor), typeof(Brush), typeof(TextExpander), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        public static readonly DependencyProperty BodyTextWeightProperty = DependencyProperty.Register(nameof(BodyTextWeight), typeof(FontWeight), typeof(TextExpander), new PropertyMetadata(FontWeights.Normal));

        public static readonly DependencyProperty FooterButtonTextColorProperty = DependencyProperty.Register(nameof(FooterButtonTextColor), typeof(Brush), typeof(TextExpander), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty FooterMoreCommmandProperty = DependencyProperty.Register(nameof(FooterMoreCommmand), typeof(ICommand), typeof(TextExpander), new PropertyMetadata(defaultValue: GetDefaultMoreCommand));

        public static readonly DependencyProperty BodyTextDefaultLinesProperty = DependencyProperty.Register(nameof(BodyTextDefaultLines), typeof(int), typeof(TextExpander), new PropertyMetadata(5));

        #endregion

        public static RelayCommand<TextExpander> GetDefaultMoreCommand => new RelayCommand<TextExpander>(GetDefaultMore);

        private static void GetDefaultMore(TextExpander bindable)
        {
            bindable.IsExpanded = !bindable.IsExpanded;
            _bodyText = bindable.FindName("bodyText") as TextBlock;

            if (bindable.IsExpanded)
            {                
                 _bodyText.TextTrimming = TextTrimming.None;
                _bodyText.MaxLines = 0;
            }
            else
            {
                _bodyText.TextTrimming = TextTrimming.WordEllipsis;
                _bodyText.MaxLines = 5;
            }
        }

        #region Custom Dependency Properties Getters & Setters

        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public string BodyText
        {
            get { return (string)GetValue(BodyTextProperty); }
            set { SetValue(BodyTextProperty, value); }
        }

        public string FooterButtonText
        {
            get { return (string)GetValue(FooterButtonTextProperty); }
            set { SetValue(FooterButtonTextProperty, value); }
        }

        public string ExpandedButtonText
        {
            get { return (string)GetValue(ExpandedButtonTextProperty); }
            set { SetValue(ExpandedButtonTextProperty, value); }
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set
            {
                SetValue(IsExpandedProperty, value);
                if (value)
                {
                    FooterButtonText = ExpandedButtonText;
                }
                else
                {
                    FooterButtonText = "More";
                }
            }
        }

        public double HeaderTextFontSize
        {
            get { return (double)GetValue(HeaderTextFontSizeProperty); }
            set { SetValue(HeaderTextFontSizeProperty, value); }
        }

        public SolidColorBrush HeaderTextColor
        {
            get { return (SolidColorBrush)GetValue(HeaderTextColorProperty); }
            set { SetValue(HeaderTextColorProperty, value); }
        }

        public FontWeight HeaderTextWeight
        {
            get { return (FontWeight)GetValue(HeaderTextWeightProperty); }
            set { SetValue(HeaderTextWeightProperty, value); }
        }

        public double BodyTextFontSize
        {
            get { return (double)GetValue(BodyTextFontSizeProperty); }
            set { SetValue(BodyTextFontSizeProperty, value); }
        }

        public SolidColorBrush BodyTextColor
        {
            get { return (SolidColorBrush)GetValue(BodyTextColorProperty); }
            set { SetValue(BodyTextColorProperty, value); }
        }

        public FontWeight BodyTextWeight
        {
            get { return (FontWeight)GetValue(BodyTextWeightProperty); }
            set { SetValue(BodyTextWeightProperty, value); }
        }

        public SolidColorBrush FooterButtonTextColor
        {
            get { return (SolidColorBrush)GetValue(FooterButtonTextColorProperty); }
            set { SetValue(FooterButtonTextColorProperty, value); }
        }

        public ICommand FooterMoreCommmand
        {
            get { return (ICommand)GetValue(FooterMoreCommmandProperty); }
            set { SetValue(FooterMoreCommmandProperty, value); }
        }

        public int BodyTextDefaultLines
        {
            get { return (int)GetValue(BodyTextDefaultLinesProperty); }
            set { SetValue(BodyTextDefaultLinesProperty, value); }
        }

        #endregion
    }
}
