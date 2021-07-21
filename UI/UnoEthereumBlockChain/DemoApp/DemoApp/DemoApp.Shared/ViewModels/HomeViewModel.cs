namespace DemoApp.ViewModels
{
    public class HomeViewModel
    {
        public ManufacturerVM ManufacturerVM { get; set; }
        public WholesalerVM WholesalerVM { get; set; }
        public PharmacyVM PharmacyVM { get; set; }
        public PatientVM PatientVM { get; set; }

        public HomeViewModel()
        {
            ManufacturerVM = new ManufacturerVM();
            WholesalerVM = new WholesalerVM();
            PharmacyVM = new PharmacyVM();
            PatientVM = new PatientVM();
        }
    }
}
