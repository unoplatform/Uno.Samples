using Microsoft.Toolkit.Uwp.SampleApp.Data;
using System.Collections.ObjectModel;
using WCTDataTreeTabSample.Entities;
using Windows.UI.Xaml.Controls;
using mux = Microsoft.UI.Xaml.Controls;

namespace WCTDataTreeTabSample
{
	public sealed partial class TreeViewPage : Page
	{
		//private mux.TreeViewNode personalFolder;
		//private mux.TreeViewNode personalFolder2;
		private ObservableCollection<ExplorerItem> DataSource;

		public TreeViewPage()
		{
			this.InitializeComponent();

			DataSource = GetData();
		}

		private ObservableCollection<ExplorerItem> GetData()
		{
			var list = new ObservableCollection<ExplorerItem>();
			ExplorerItem folder1 = new ExplorerItem()
			{
				Name = "Work Documents",
				Type = ExplorerItem.ExplorerItemType.Folder,
				Children =
				{
					new ExplorerItem()
					{
						Name = "Functional Specifications",
						Type = ExplorerItem.ExplorerItemType.Folder,
						Children =
						{
							new ExplorerItem()
							{
								Name = "TreeView spec",
								Type = ExplorerItem.ExplorerItemType.File,
							  }
						}
					},
					new ExplorerItem()
					{
						Name = "Feature Schedule",
						Type = ExplorerItem.ExplorerItemType.File,
					},
					new ExplorerItem()
					{
						Name = "Overall Project Plan",
						Type = ExplorerItem.ExplorerItemType.File,
					},
					new ExplorerItem()
					{
						Name = "Feature Resources Allocation",
						Type = ExplorerItem.ExplorerItemType.File,
					}
				}
			};
			ExplorerItem folder2 = new ExplorerItem()
			{
				Name = "Personal Folder",
				Type = ExplorerItem.ExplorerItemType.Folder,
				Children =
						{
							new ExplorerItem()
							{
								Name = "Home Remodel Folder",
								Type = ExplorerItem.ExplorerItemType.Folder,
								Children =
								{
									new ExplorerItem()
									{
										Name = "Contractor Contact Info",
										Type = ExplorerItem.ExplorerItemType.File,
									},
									new ExplorerItem()
									{
										Name = "Paint Color Scheme",
										Type = ExplorerItem.ExplorerItemType.File,
									},
									new ExplorerItem()
									{
										Name = "Flooring Woodgrain type",
										Type = ExplorerItem.ExplorerItemType.File,
									},
									new ExplorerItem()
									{
										Name = "Kitchen Cabinet Style",
										Type = ExplorerItem.ExplorerItemType.File,
									}
								}
							}
						}
			};

			list.Add(folder1);
			list.Add(folder2);

			return list;
		}
	}
}