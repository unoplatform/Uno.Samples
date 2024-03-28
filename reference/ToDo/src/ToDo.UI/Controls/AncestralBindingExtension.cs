using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ToDo.Controls
{
	/// <summary>
	/// Provides a mean to bind to the data context of the ancestor of a specific type.
	/// </summary>
	public class AncestralBindingExtension : MarkupExtension
	{
		/// <summary>
		/// Binding path from the data context of the ancestor.
		/// </summary>
		public string? Path { get; set; }

		// note: Type literal are not recognized by Uno's XamlSourceGenerator for Nullable<Type>.
		/// <summary>
		/// Type of ancestor to bind from.
		/// </summary>
		public Type AncestorType { get; set; } = typeof(object);

		public AncestralBindingExtension()
		{
		}

		protected override object? ProvideValue(IXamlServiceProvider serviceProvider)
		{
			if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget pvt) return null;
			if (pvt.TargetObject is not FrameworkElement owner) return null;
			if (pvt.TargetProperty is not ProvideValueTargetProperty { DeclaringType: { } ownerType, Name: { } propertyName }) return null;
			if (ownerType.GetProperty(propertyName + "Property", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
				?.GetValue(pvt.TargetObject) is not DependencyProperty property) return null;

			owner.Loaded += OnTargetLoaded;
			void OnTargetLoaded(object s, RoutedEventArgs e)
			{
				if (s is FrameworkElement fe)
				{
					fe.Loaded -= OnTargetLoaded;

					if (GetAncestors(fe).FirstOrDefault(x => AncestorType?.IsAssignableFrom(x.GetType()) == true) is { } source)
					{
						var binding = new Binding
						{
							Path = new PropertyPath("DataContext." + Path),
							Source = source,
							Mode = BindingMode.OneWay,
						};
						fe.SetBinding(property, binding);
					}
				}
			}

			return null;
		}

		private static IEnumerable<DependencyObject> GetAncestors(DependencyObject x)
		{
			if (x is null) yield break;
			while (VisualTreeHelper.GetParent(x) is { } parent)
			{
				yield return x = parent;
			}
		}
	}
}
