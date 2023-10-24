using CommunityToolkit.Mvvm.ComponentModel;

namespace XamlBrewerUnoApp.Models
{
    public partial class Settings : ObservableObject
    {
        [ObservableProperty]
        private bool isLightTheme;

        public Settings()
        {
            // Required for serialization.
        }
    }
}
