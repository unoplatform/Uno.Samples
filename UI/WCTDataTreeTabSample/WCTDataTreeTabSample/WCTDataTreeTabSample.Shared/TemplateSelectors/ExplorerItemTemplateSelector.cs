using WCTDataTreeTabSample.Entities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WCTDataTreeTabSample.TemplateSelectors
{
	public class ExplorerItemTemplateSelector : DataTemplateSelector
	{
		public DataTemplate FolderTemplate { get; set; }
		public DataTemplate FileTemplate { get; set; }

		protected override DataTemplate SelectTemplateCore(object item)
		{
			var explorerItem = (ExplorerItem)item;

			if (explorerItem == null)
			{
				return FolderTemplate;
			}

			return explorerItem.Type == ExplorerItem.ExplorerItemType.Folder
				? FolderTemplate
				: FileTemplate;
		}
	}
}