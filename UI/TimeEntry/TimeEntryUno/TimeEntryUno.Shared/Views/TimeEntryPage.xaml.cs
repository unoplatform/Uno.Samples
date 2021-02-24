using Microsoft.Extensions.Logging;
using System.Threading;
using TimeEntryUno.Shared.Models;
using TimeEntryUno.Shared.Services;
using TimeEntryUno.Shared.WebServices;
using Uno.Extensions;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TimeEntryUno.Shared.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimeEntryPage : Page
    {
        public TimeEntryPage()
        {
            this.InitializeComponent();
            this.Log().LogInformation("Constructing TimeEntryPage");
            CallApi();
        }

        async void CallApi()
        {
            await AuthenticationService.Instance.LoginUser("tylerr", "Pa$$W0rd!");
            MessageText.Text = $"User is - {(AuthenticationService.Instance.CurrentPrincipal != null ? ((User)AuthenticationService.Instance.CurrentPrincipal.Identity).FriendlyName : "Login Failed" )}"+
                $" and is in role \"Admin\" - {AuthenticationService.Instance.CurrentPrincipal.IsInRole("Admin")}";
        }
    }
}
