namespace SyncFusionApp.MauiControls.Samples.Base.CustomView;

public class SfShadowDrawer : IDrawable
{
    private float padding = 10f;

    private SizeF offset = new SizeF(10f, 10f);

    private float blur = 4f;

    private Color shadowColor = Colors.Black;

    private float opacity = 0.5f;

    private Color background = Colors.GhostWhite;

    private double cornerRadius = 5.0;

    public float Padding
    {
        get
        {
            return padding;
        }
        set
        {
            padding = value;
        }
    }

    public SizeF Offset
    {
        get
        {
            return offset;
        }
        set
        {
            offset = value;
        }
    }

    public float Blur
    {
        get
        {
            return blur;
        }
        set
        {
            blur = value;
        }
    }

    public Color ShadowColor
    {
        get
        {
            return shadowColor;
        }
        set
        {
            shadowColor = value;
        }
    }

    public float Opacity
    {
        get
        {
            return opacity;
        }
        set
        {
            opacity = value;
        }
    }

    public Color BackgroundColor
    {
        get
        {
            return background;
        }
        set
        {
            background = value;
        }
    }

    public double CornerRadius
    {
        get
        {
            return cornerRadius;
        }
        set
        {
            cornerRadius = value;
        }
    }

    //
    // Parameters:
    //   canvas:
    //
    //   dirtyRect:
    protected virtual void DrawShadow(ICanvas canvas, RectF dirtyRect)
    {
        RectF rectF = default(RectF);
        rectF.Left = dirtyRect.Left + padding;
        rectF.Top = dirtyRect.Top + padding;
        rectF.Right = dirtyRect.Right - padding;
        rectF.Bottom = dirtyRect.Bottom - padding;
        RectF dirtyRect2 = rectF;
        ApplyShadow(canvas, dirtyRect2, Offset, ShadowColor, Opacity, CornerRadius);
    }

    //
    // Parameters:
    //   canvas:
    //
    //   dirtyRect:
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        double num = ((CornerRadius > (double)(dirtyRect.Width / 2f)) ? ((double)(dirtyRect.Width / 2f)) : CornerRadius);
        canvas.SaveState();
        canvas.ClipPath(GetDrawPath(dirtyRect, (float)num));
        DrawShadow(canvas, dirtyRect);
        canvas.RestoreState();
    }

    internal void ApplyShadow(ICanvas canvas, RectF dirtyRect, SizeF offset, Color shadowColor, float opacity, double cornerRadius)
    {
        canvas.SaveState();
        canvas.FillColor = BackgroundColor;
        canvas.SetShadow(offset, Blur, shadowColor.WithAlpha(opacity));
        canvas.FillRoundedRectangle(dirtyRect, cornerRadius);
        canvas.RestoreState();
    }

    private PathF GetDrawPath(RectF roundedRect, float roundCornerRadius)
    {
        PointCollection pointCollection = new PointCollection();
        double num = (double)roundCornerRadius * 0.44777152600000003;
        RectF rectF = roundedRect;
        float x = rectF.X;
        float y = rectF.Y;
        float width = rectF.Width;
        float height = rectF.Height;
        pointCollection.Add(new Point(x + roundCornerRadius, y));
        pointCollection.Add(new Point(x + width - roundCornerRadius, y));
        pointCollection.Add(new Point((double)(x + width) - num, y));
        pointCollection.Add(new Point(x + width, (double)y + num));
        pointCollection.Add(new Point(x + width, y + roundCornerRadius));
        pointCollection.Add(new Point(x + width, y + height - roundCornerRadius));
        pointCollection.Add(new Point(x + width, (double)(y + height) - num));
        pointCollection.Add(new Point((double)(x + width) - num, y + height));
        pointCollection.Add(new Point(x + width - roundCornerRadius, y + height));
        pointCollection.Add(new Point(x + roundCornerRadius, y + height));
        pointCollection.Add(new Point((double)x + num, y + height));
        pointCollection.Add(new Point(x, (double)(y + height) - num));
        pointCollection.Add(new Point(x, y + height - roundCornerRadius));
        pointCollection.Add(new Point(x, y + roundCornerRadius));
        pointCollection.Add(new Point(x, (double)y + num));
        pointCollection.Add(new Point((double)x + num, y));
        pointCollection.Add(new Point(x + roundCornerRadius, y));
        PathF pathF = new PathF();
        if (pointCollection != null)
        {
            pathF.MoveTo(pointCollection[0]);
            pathF.LineTo(pointCollection[1]);
            pathF.CurveTo(pointCollection[2], pointCollection[3], pointCollection[4]);
            pathF.LineTo(pointCollection[5]);
            pathF.CurveTo(pointCollection[6], pointCollection[7], pointCollection[8]);
            pathF.LineTo(pointCollection[9]);
            pathF.CurveTo(pointCollection[10], pointCollection[11], pointCollection[12]);
            pathF.LineTo(pointCollection[13]);
            pathF.CurveTo(pointCollection[14], pointCollection[15], pointCollection[16]);
        }

        pathF.Close();
        return pathF;
    }
}
