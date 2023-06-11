using CustomRenderers;
using CustomRenderers.iOS;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(RichLabel), typeof(RichLabelRenderer))]

namespace CustomRenderers.iOS
{
    public class RichLabelRenderer : ViewRenderer<RichLabel, UITextView>
    {
        protected override UITextView CreateNativeControl()
        {
            var view = new UITextView();
            view.Editable = false;
            view.DataDetectorTypes = UIDataDetectorType.All;
            return view;
        }
        protected override void OnElementChanged(ElementChangedEventArgs<RichLabel> e)
        {            
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(CreateNativeControl());
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.PropertyName);
            base.OnElementPropertyChanged(sender, e);
            switch(e.PropertyName)
            {
                case "Renderer":
                    UpdateText();
                    UpdateFont();
                    break;

                case "Text":
                    UpdateText(); 
                    break;

                case "FontFamily":
                case "FontSize":
                    UpdateFont(); 
                    break;
            }
        }

        void UpdateText()
        {
            var text = Element.UpdateFormsText(Element.Text, Element.TextTransform);
            if (Control.Text != text)
            {
                Control.Text = text;
            }
        }

        void UpdateFont()
        {
            var font = FontExtensions.ToUIFont(Font.OfSize(Element.FontFamily, Element.FontSize));
            Control.Font = font;
        }
    }
}