using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WCTDataTreeTabSample.Entities
{
	public class ExplorerItem : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public enum ExplorerItemType { Folder, File };
		public string Name { get; set; }
		public ExplorerItemType Type { get; set; }
		private ObservableCollection<ExplorerItem> m_children;
		public ObservableCollection<ExplorerItem> Children
		{
			get
			{
				if (m_children == null)
				{
					m_children = new ObservableCollection<ExplorerItem>();
				}
				return m_children;
			}
			set
			{
				m_children = value;
			}
		}

		private bool m_isExpanded;
		public bool IsExpanded
		{
			get { return m_isExpanded; }
			set
			{
				if (m_isExpanded != value)
				{
					m_isExpanded = value;
					NotifyPropertyChanged("IsExpanded");
				}
			}
		}

		private bool m_isSelected;
		public bool IsSelected
		{
			get { return m_isSelected; }

			set
			{
				if (m_isSelected != value)
				{
					m_isSelected = value;
					NotifyPropertyChanged("IsSelected");
				}
			}

		}

		private void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
