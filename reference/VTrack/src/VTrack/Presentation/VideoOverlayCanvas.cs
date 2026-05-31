using SkiaSharp;
using Uno.WinUI.Graphics2DSK;
using VTrack.DataContracts;

namespace VTrack.Presentation;

public sealed class VideoOverlayCanvas : SKCanvasElement
{
    private SKBitmap? _frame;
    private IReadOnlyList<BoundingBox> _boxes = Array.Empty<BoundingBox>();
    private IReadOnlyList<TrackedSubject> _subjects = Array.Empty<TrackedSubject>();

    public void SetFrame(SKBitmap? frame)
    {
        _frame = frame;
        Invalidate();
    }

    public void SetOverlay(IReadOnlyList<BoundingBox> boxes, IReadOnlyList<TrackedSubject> subjects)
    {
        _boxes = boxes;
        _subjects = subjects;
        Invalidate();
    }

    protected override void RenderOverride(SKCanvas canvas, Windows.Foundation.Size area)
    {
        canvas.Clear(SKColors.Black);

        SKRect videoRect;
        if (_frame is { Width: > 0, Height: > 0 } frame)
        {
            videoRect = ComputeUniformRect(frame.Width, frame.Height, (float)area.Width, (float)area.Height);
            canvas.DrawBitmap(frame, videoRect);
        }
        else
        {
            videoRect = new SKRect(0, 0, (float)area.Width, (float)area.Height);
        }

        if (_boxes.Count == 0 || _subjects.Count == 0) return;

        using var stroke = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = 3, IsAntialias = true };
        using var fill = new SKPaint { Style = SKPaintStyle.Fill };
        using var font = new SKFont(SKTypeface.FromFamilyName("Segoe UI", SKFontStyle.Bold), 14);
        using var text = new SKPaint { Color = SKColors.White, IsAntialias = true };

        foreach (var box in _boxes)
        {
            var subject = _subjects.FirstOrDefault(s => s.Id == box.SubjectId);
            if (subject == null) continue;

            var color = SKColor.Parse(subject.Color);
            stroke.Color = color;
            fill.Color = color.WithAlpha(200);

            var x = videoRect.Left + (float)(box.X * videoRect.Width);
            var y = videoRect.Top + (float)(box.Y * videoRect.Height);
            var w = (float)(box.Width * videoRect.Width);
            var h = (float)(box.Height * videoRect.Height);

            canvas.DrawRect(x, y, w, h, stroke);

            var label = $"{subject.Label} ({box.Confidence:P0})";
            font.MeasureText(label, out var bounds, text);

            var labelRect = new SKRect(x, y - bounds.Height - 8, x + bounds.Width + 12, y);
            canvas.DrawRoundRect(labelRect, 4, 4, fill);
            canvas.DrawText(label, x + 6, y - 6, SKTextAlign.Left, font, text);
        }
    }

    private static SKRect ComputeUniformRect(int srcW, int srcH, float dstW, float dstH)
    {
        var scale = MathF.Min(dstW / srcW, dstH / srcH);
        var w = srcW * scale;
        var h = srcH * scale;
        var left = (dstW - w) / 2f;
        var top = (dstH - h) / 2f;
        return new SKRect(left, top, left + w, top + h);
    }
}
