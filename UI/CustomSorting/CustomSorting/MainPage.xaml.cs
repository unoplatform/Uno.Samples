using Microsoft.UI.Xaml.Data;

namespace CustomSorting;

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
