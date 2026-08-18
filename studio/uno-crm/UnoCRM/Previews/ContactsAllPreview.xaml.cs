using Uno.UI.HotDesign;

namespace UnoCRM.Previews;

// ContactsPage with the full contact set (ContactsPageMockData.Data), bound in XAML via {x:Bind}.
[Preview("Contacts — All", typeof(ContactsPage))]
public sealed partial class ContactsAllPreview : Preview
{
    public ContactsAllPreview() => this.InitializeComponent();
}
