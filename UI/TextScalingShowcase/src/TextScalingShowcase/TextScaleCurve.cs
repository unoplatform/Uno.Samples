using System;

namespace TextScalingShowcase;

/// <summary>
/// The WinUI text-scaling curve, mirrored so the sample can print the sizes the framework
/// computes. Uno applies the very same formula in <c>TextScaleHelper.GetScaledFontSize</c>.
/// </summary>
internal static class TextScaleCurve
{
	/// <summary>
	/// Scales <paramref name="fontSize"/> by <paramref name="factor"/>: the added amount shrinks
	/// logarithmically with the font size, and reaches zero around 750 pt.
	/// </summary>
	public static double Scale(double fontSize, double factor)
	{
		if (factor <= 1 || fontSize <= 0)
		{
			return fontSize;
		}

		var capped = Math.Max(fontSize, 1);

		// s_o = s_i + max(-e * ln(s_i) + 18, 0) * (f - 1)
		return capped + Math.Max((-Math.E * Math.Log(capped)) + 18, 0) * (factor - 1);
	}
}
