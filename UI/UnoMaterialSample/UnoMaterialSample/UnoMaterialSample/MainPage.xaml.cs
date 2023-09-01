using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;

namespace UnoMaterialSample
{
	public sealed partial class MainPage : Page
	{
		public List<int> Items => Enumerable.Range(0, 10).ToList();

		public MainPage()
		{
			this.InitializeComponent();
		}
	}
}
