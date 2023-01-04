namespace ListViewSample.Wasm
{
    public class Program
    {
        private static App _app;

        private static int Main(string[] args)
        {
            Microsoft.UI.Xaml.Application.Start(_ => _app = new App());

            return 0;
        }
    }
}