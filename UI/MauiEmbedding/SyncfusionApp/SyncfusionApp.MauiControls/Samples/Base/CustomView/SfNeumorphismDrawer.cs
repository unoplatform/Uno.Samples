using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncfusionApp.MauiControls.Samples.Base.CustomView;
public class SfNeumorphismDrawer : SfShadowDrawer
{
    private SizeF lightOffSet = new SizeF(-6f, -6f);

    private float lightOpacity = 0.5f;

    private Color lightShadowColor;

    private bool isPressedState;

    public SizeF LightOffSet
    {
        get
        {
            return lightOffSet;
        }
        set
        {
            lightOffSet = value;
        }
    }

    public float LightOpacity
    {
        get
        {
            return lightOpacity;
        }
        set
        {
            lightOpacity = value;
        }
    }

    public Color LightShadowColor
    {
        get
        {
            return lightShadowColor;
        }
        set
        {
            lightShadowColor = value;
        }
    }

    public bool IsPressedState
    {
        get
        {
            return isPressedState;
        }
        set
        {
            isPressedState = value;
        }
    }

    public SfNeumorphismDrawer()
    {
        lightShadowColor = Colors.White;
        base.Blur = 10f;
        base.Opacity = 0.2f;
        base.Offset = new SizeF(6f, 6f);
        base.ShadowColor = Colors.Black;
    }

    //
    // Parameters:
    //   canvas:
    //
    //   dirtyRect:
    protected override void DrawShadow(ICanvas canvas, RectF dirtyRect)
    {
        if (IsPressedState)
        {
            double num = ((base.CornerRadius > (double)(dirtyRect.Width / 2f)) ? ((double)(dirtyRect.Width / 2f)) : base.CornerRadius);
            if ((double)(dirtyRect.Width / 3f) < num)
            {
                ApplyShadow(canvas, new RectF(dirtyRect.Left, dirtyRect.Top, 0f - dirtyRect.Width, dirtyRect.Height), base.Offset, base.ShadowColor, base.Opacity, num);
                ApplyShadow(canvas, new RectF(dirtyRect.Right, dirtyRect.Top, dirtyRect.Width, dirtyRect.Height), LightOffSet, lightShadowColor, LightOpacity, num);
                return;
            }

            ApplyShadow(canvas, new RectF(dirtyRect.Left, dirtyRect.Top, 0f - dirtyRect.Width, dirtyRect.Height), base.Offset, base.ShadowColor, base.Opacity, num);
            ApplyShadow(canvas, new RectF(dirtyRect.Left - 10f, dirtyRect.Top, dirtyRect.Width + 10f, 0f - dirtyRect.Height), base.Offset, base.ShadowColor, base.Opacity, num);
            ApplyShadow(canvas, new RectF(dirtyRect.Right, dirtyRect.Top, dirtyRect.Width, dirtyRect.Height), LightOffSet, lightShadowColor, LightOpacity, num);
            ApplyShadow(canvas, new RectF(dirtyRect.Left, dirtyRect.Bottom, dirtyRect.Width, dirtyRect.Height), LightOffSet, lightShadowColor, LightOpacity, num);
        }
        else
        {
            RectF rectF = default(RectF);
            rectF.Left = dirtyRect.Left + base.Padding;
            rectF.Top = dirtyRect.Top + base.Padding;
            rectF.Right = dirtyRect.Right - base.Padding;
            rectF.Bottom = dirtyRect.Bottom - base.Padding;
            RectF dirtyRect2 = rectF;
            double num2 = ((base.CornerRadius > (double)(dirtyRect2.Width / 2f)) ? ((double)(dirtyRect2.Width / 2f)) : base.CornerRadius);
            ApplyShadow(canvas, dirtyRect2, base.Offset, base.ShadowColor, base.Opacity, num2);
            ApplyShadow(canvas, dirtyRect2, LightOffSet, lightShadowColor, LightOpacity, num2);
        }
    }
}