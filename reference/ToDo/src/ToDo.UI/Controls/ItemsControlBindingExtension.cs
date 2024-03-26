#nullable enable

using System.Reflection;

namespace ToDo.Controls
{
	/// <summary>
	/// Provides a binding in the data context of the parent ItemsControl.
	/// This markup can be used to access the parent ItemsControl data-context from inside of the ItemTemplate.
	/// </summary>
	public class ItemsControlBindingExtension : MarkupExtension
	{
		/// <summary>
		/// Binding path from the data context of the parent ItemsControl.
		/// </summary>
		public string? Path { get; set; }

		public ItemsControlBindingExtension()
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

					if (GetAncestors(fe).OfType<ItemsControl>().FirstOrDefault() is { } source)
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
