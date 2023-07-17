using Android.Content;
using Android.Text.Util;
using CustomRenderers;
using CustomRenderers.Droid;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(RichLabel), typeof(RichLabelRenderer))]

namespace CustomRenderers.Droid
{
    public class RichLabelRenderer : LabelRenderer
    {
        public RichLabelRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if(e.PropertyName == "Text" || e.PropertyName == "Renderer")
            {
                Linkify.AddLinks(Control, MatchOptions.All);
            }
        }
    }
}