using Meridian.Models;
using SkiaSharp;
using SkiaSharp.Views.Windows;

namespace Meridian.Controls;

public sealed class RevenueBarChartControl : SKXamlCanvas
{
	private static readonly SKColor ColorGain = new(0x2D, 0x6A, 0x4F);

	public static readonly DependencyProperty FinancialsProperty =
		DependencyProperty.Register(nameof(Financials), typeof(IList<Financial>),
			typeof(RevenueBarChartControl), new PropertyMetadata(null, OnDataChanged));

	public IList<Financial>? Financials
	{
		get => (IList<Financial>?)GetValue(FinancialsProperty);
		set => SetValue(FinancialsProperty, value);
	}

	private float _animationProgress = 1f;

	private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var ctrl = (RevenueBarChartControl)d;
		ctrl._animationProgress = 1f;
		ctrl.Invalidate();
	}

	public RevenueBarChartControl()
	{
		PaintSurface += OnPaintSurface;
	}

	private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
	{
		var canvas = e.Surface.Canvas;
		canvas.Clear(SKColors.Transparent);

		var items = Financials;
		if (items is null || items.Count == 0)
			return;

		var info = e.Info;
		var maxRev = items.Max(f => f.RevenueNum);
		if (maxRev <= 0) return;

		var count = items.Count;
		var gap = 8f;
		var maxBarWidth = 48f;
		var totalGap = (count - 1) * gap;
		var availableWidth = info.Width - totalGap;
		var barWidth = Math.Min(maxBarWidth, availableWidth / count);
		var totalBarsWidth = count * barWidth + totalGap;
		var startX = (info.Width - totalBarsWidth) / 2f;
		var maxBarHeight = info.Height - 20f; // leave room for labels

		using var barPaint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill };
		using var textPaint = new SKPaint
		{
			IsAntialias = true,
			Color = new SKColor(0xC4, 0xC0, 0xB8), // subtle
		};
		using var labelFont = new SKFont(SKTypeface.Default, size: 9f);

		for (var i = 0; i < count; i++)
		{
			var item = items[i];
			var pct = (float)(item.RevenueNum / maxRev);
			var barHeight = pct * maxBarHeight * _animationProgress;
			if (barHeight < 2f) barHeight = 2f;

			var x = startX + i * (barWidth + gap);
			var y = info.Height - 16f - barHeight;

			barPaint.Color = ColorGain;
			// Latest bar (last) = 0.85 opacity, older = 0.3
			barPaint.Color = barPaint.Color.WithAlpha((byte)(i == count - 1 ? 217 : 77));

			var rect = new SKRoundRect(new SKRect(x, y, x + barWidth, info.Height - 16f), 6f, 6f);
			canvas.DrawRoundRect(rect, barPaint);

			// Year label below
			var label = item.Period.Length > 4 ? item.Period[..4] : item.Period;
			canvas.DrawText(label, x + barWidth / 2f, info.Height - 2f, SKTextAlign.Center, labelFont, textPaint);
		}
	}
}
