using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("UnoSamples.Effects")]
[assembly: ExportEffect(typeof(UnoSamples.Effects.Droid.NoUnderlineEffect), nameof(UnoSamples.Effects.Droid.NoUnderlineEffect))]

namespace UnoSamples.Effects.Droid
{
    public class NoUnderlineEffect : PlatformEffect
    {
        Drawable? originalBackground;

        protected override void OnAttached()
        {
            originalBackground = Control.Background;

            var shape = new ShapeDrawable(new RectShape());
            if (shape.Paint != null)
            {
                shape.Paint.Color = global::Android.Graphics.Color.Transparent;
                shape.Paint.StrokeWidth = 0;
                shape.Paint.SetStyle(Paint.Style.Stroke);
            }

            Control.Background = shape;
        }

        protected override void OnDetached() => Control.Background = originalBackground;
    }
}