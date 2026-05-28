using SkiaSharp;
using SkiaSharp.Views.Windows;

namespace Meridian.Controls;

public sealed class VolumeSubChartControl : SKXamlCanvas
{
	private static readonly SKColor ColorAccent = new(0xC9, 0xA9, 0x6E);
	private static readonly SKColor ColorMuted = new(0xE0, 0xDC, 0xD5);
	private static readonly SKColor ColorHighlight = new(0x2D, 0x6A, 0x4F);
	private static readonly SKColor ColorTextPrimary = new(0x3A, 0x36, 0x31);
	private static readonly SKColor ColorCardBg = new(0xFB, 0xF9, 0xF5);
	private static readonly SKColor ColorBorder = new(0xE0, 0xDC, 0xD5);

	private double _pointerX = -1;

	public static readonly DependencyProperty ValuesProperty =
		DependencyProperty.Register(nameof(Values), typeof(IList<decimal>),
			typeof(VolumeSubChartControl), new PropertyMetadata(null, OnDataChanged));

	public IList<decimal>? Values
	{
		get => (IList<decimal>?)GetValue(ValuesProperty);
		set => SetValue(ValuesProperty, value);
	}

	private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((VolumeSubChartControl)d).Invalidate();

	public VolumeSubChartControl()
	{
		PaintSurface += OnPaintSurface;
		PointerMoved += OnPointerMoved;
		PointerExited += OnPointerExited;
	}

	private void OnPointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
	{
		_pointerX = e.GetCurrentPoint(this).Position.X;
		Invalidate();
	}

	private void OnPointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
	{
		_pointerX = -1;
		Invalidate();
	}

	private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
	{
		var canvas = e.Surface.Canvas;
		canvas.Clear(SKColors.Transparent);

		var values = Values;
		if (values is null || values.Count == 0)
			return;

		var info = e.Info;
		var maxVal = values.Max();
		if (maxVal <= 0) return;

		var count = values.Count;
		var barWidth = Math.Max(2f, (info.Width - (count - 1) * 2f) / count);
		var recentThreshold = (int)(count * 0.75);
		var scaleX = ActualWidth > 0 ? info.Width / (float)ActualWidth : 1f;

		// Determine hovered bar index
		var hoveredIndex = -1;
		if (_pointerX >= 0)
		{
			var skPointerX = (float)_pointerX * scaleX;
			hoveredIndex = (int)(skPointerX / (barWidth + 2f));
			if (hoveredIndex < 0 || hoveredIndex >= count) hoveredIndex = -1;
		}

		using var paint = new SKPaint
		{
			IsAntialias = true,
			Style = SKPaintStyle.Fill,
		};

		for (var i = 0; i < count; i++)
		{
			var barHeight = (float)(values[i] / maxVal) * (info.Height - 4f);
			if (barHeight < 1f) barHeight = 1f;

			var x = i * (barWidth + 2f);
			var y = info.Height - barHeight;

			if (i == hoveredIndex)
				paint.Color = ColorHighlight;
			else
				paint.Color = i >= recentThreshold ? ColorAccent : ColorMuted;

			var rect = new SKRoundRect(new SKRect(x, y, x + barWidth, info.Height), 2f, 2f);
			canvas.DrawRoundRect(rect, paint);
		}

		// Draw tooltip for hovered bar
		if (hoveredIndex >= 0 && hoveredIndex < count)
		{
			var val = values[hoveredIndex];
			var text = FormatValue(val);

			using var textPaint = new SKPaint
			{
				IsAntialias = true,
				Color = ColorTextPrimary,
			};
			using var textFont = new SKFont(SKTypeface.Default, size: 10 * scaleX);

			textFont.MeasureText(text, out var textBounds);

			var barX = hoveredIndex * (barWidth + 2f) + barWidth / 2f;
			var barY = info.Height - (float)(values[hoveredIndex] / maxVal) * (info.Height - 4f);

			// Position tooltip above the bar
			var tooltipX = barX - textBounds.Width / 2f;
			var tooltipY = barY - 10;

			// Clamp to canvas bounds
			tooltipX = Math.Clamp(tooltipX, 4, info.Width - textBounds.Width - 4);
			if (tooltipY < textBounds.Height + 8) tooltipY = barY + textBounds.Height + 8;

			// Background pill
			var pillPadH = 6f * scaleX;
			var pillPadV = 3f * scaleX;
			var pillRect = new SKRect(
				tooltipX - pillPadH,
				tooltipY - textBounds.Height - pillPadV,
				tooltipX + textBounds.Width + pillPadH,
				tooltipY + pillPadV
			);

			using var bgPaint = new SKPaint
			{
				IsAntialias = true,
				Color = ColorCardBg.WithAlpha(0xF0),
				Style = SKPaintStyle.Fill,
			};
			var pillRadius = 4f * scaleX;
			canvas.DrawRoundRect(new SKRoundRect(pillRect, pillRadius, pillRadius), bgPaint);

			using var borderPaint = new SKPaint
			{
				IsAntialias = true,
				Color = ColorBorder,
				Style = SKPaintStyle.Stroke,
				StrokeWidth = scaleX,
			};
			canvas.DrawRoundRect(new SKRoundRect(pillRect, pillRadius, pillRadius), borderPaint);

			canvas.DrawText(text, tooltipX, tooltipY, SKTextAlign.Left, textFont, textPaint);
		}
	}

	private static string FormatValue(decimal val)
	{
		if (val >= 1_000_000) return $"{val / 1_000_000:N1}M";
		if (val >= 1_000) return $"{val / 1_000:N1}K";
		return $"{val:N0}";
	}
}
