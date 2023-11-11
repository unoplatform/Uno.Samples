using System;
using System.Runtime.CompilerServices;

public static class NavigationHelper
{
	/// <summary>
	/// Provides an equivalent method to Xamarin.Forms.Navigation.PopToRootAsync()
	/// </summary>
	/// <param name="frame"></param>
	public static void PopToRoot(this Frame frame)
	{
		// remove all intermediate pages
		while(frame.BackStackDepth > 1)
		{
			frame.BackStack.RemoveAt(frame.BackStackDepth - 1);
		}
		
		// go back to the first page in a single navigation
		frame.GoBack();
	}
}
