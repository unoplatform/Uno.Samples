using Uno.UI.HotDesign;

namespace UnoCRM.Previews;

// The same ContactsPage as ContactsAllPreview, in its no-results state (ContactsPageMockData.NoResults),
// bound in XAML via {x:Bind}.
[Preview("Contacts — No Results", typeof(ContactsPage))]
public sealed partial class ContactsNoResultsPreview : Preview
{
    public ContactsNoResultsPreview() => this.InitializeComponent();
}
