using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

namespace ChatUI.Behaviors;

public class CommandOnKeyPressBehavior
{
	public static ICommand GetTextBoxCommand(DependencyObject dependencyObject)
	{
		return (ICommand)dependencyObject.GetValue(TextBoxCommandProperty);
	}

	public static void SetTextBoxCommand(DependencyObject dependencyObject, ICommand value) 
	{ 
		dependencyObject.SetValue(TextBoxCommandProperty, value);
	}

	public static readonly DependencyProperty TextBoxCommandProperty =
		DependencyProperty.RegisterAttached("TextBoxCommand", typeof(ICommand), typeof(CommandOnKeyPressBehavior), new PropertyMetadata(default(ICommand), OnTextBoxCommandChanged));
	
	private static void OnTextBoxCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if(d is TextBox textBox)
		{
			textBox.KeyDown -= TextBox_KeyDown;

			textBox.KeyDown += TextBox_KeyDown;
		}
	}

	private static void TextBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
	{
		if (e.Key == VirtualKey.Enter)
		{
			var textBox = sender as TextBox;
			var command = GetTextBoxCommand(textBox);
			command.Execute(textBox.Text);
		}
	}
}
