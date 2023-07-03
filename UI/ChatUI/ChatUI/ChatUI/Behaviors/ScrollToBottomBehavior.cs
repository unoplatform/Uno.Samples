using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ChatUI.Behaviors
{
	public class ScrollToBottomBehavior
	{
		public static bool GetIsAttached(DependencyObject dependencyObject)
		{
			return (bool)dependencyObject.GetValue(IsAttachedProperty);
		}

		public static void SetIsAttached(DependencyObject dependencyObject, bool value)
		{
			dependencyObject.SetValue(IsAttachedProperty, value);
		}

		public static DependencyProperty IsAttachedProperty =
			DependencyProperty.RegisterAttached("IsAttached", typeof(bool), typeof(ScrollToBottomBehavior), new PropertyMetadata(false, OnIsAttachedChanged));

		private static void OnIsAttachedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if(d is ScrollViewer scrollViewer && (bool)d.GetValue(IsAttachedProperty))
			{
				scrollViewer.ChangeView(0, scrollViewer.ScrollableHeight, 1);
			}
		}
	}
}
