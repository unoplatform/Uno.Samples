using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace XamlControlLibrary;

public partial class MyTemplatedControl : Control
{
	public MyTemplatedControl()
	{
		DefaultStyleKey = typeof(MyTemplatedControl);
	}

	public int MyProperty
	{
		get { return (int)GetValue(MyPropertyProperty); }
		set { SetValue(MyPropertyProperty, value); }
	}

	// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty MyPropertyProperty =
		DependencyProperty.Register(
			"MyProperty",
			typeof(int), 
			typeof(MyTemplatedControl),
			new PropertyMetadata(
				0, 
				(s, e) => ((MyTemplatedControl)s)?.OnMyPropertyChanged(e))
		);


	private void OnMyPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		// System.Diagnostics.Debug.WriteLine($"MyProperty Changed from [{e.OldValue}] to [{e.NewValue}]");
	}
}
