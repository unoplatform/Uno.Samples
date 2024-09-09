namespace MVUX.Presentation.StateSample;
public sealed partial class StatePage : Page
{
	public StatePage()
	{
		this.InitializeComponent();

		DataContext = new BindableStateModel(new StateService());
	}
}
