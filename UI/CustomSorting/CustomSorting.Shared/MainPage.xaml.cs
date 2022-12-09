using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System.Linq;

namespace CustomSorting;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage : Page
{
    private People rawPeople = new People();
    public MainPage()
    {
        this.InitializeComponent();
        var sorter = new CustomSorter();

        var sortQuery = rawPeople.OrderBy(p => p, sorter);

        SortedPeopleSource = new CollectionViewSource { Source = sortQuery };
    }

    public CollectionViewSource SortedPeopleSource { get; private set; }
}
