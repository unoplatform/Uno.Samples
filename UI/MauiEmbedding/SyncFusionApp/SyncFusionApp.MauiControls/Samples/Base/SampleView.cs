namespace SyncFusionApp.MauiControls.Samples.Base;

public class SampleView : ContentView
{
    public static readonly BindableProperty OptionViewProperty = BindableProperty.Create("OptionView", typeof(View), typeof(SampleView));

    private View? busyIndicatorView;

    public View? OptionView
    {
        get
        {
            return (View)GetValue(OptionViewProperty);
        }
        set
        {
            SetValue(OptionViewProperty, value);
        }
    }

    public bool IsCardView { get; set; }

    public SampleView()
    {
    }

    //
    // Parameters:
    //   view:
    public void SetBusyIndicator(View view)
    {
        busyIndicatorView = view;
    }

    //
    // Summary:
    //     Hooked when sample view disappears
    public virtual void OnDisappearing()
    {
    }

    //
    // Summary:
    //     Hooked when sample view appears
    public virtual void OnAppearing()
    {
    }

    //
    // Parameters:
    //   bounds:
    protected override Size ArrangeOverride(Rect bounds)
    {
        HideBusyIndicator();
        return base.ArrangeOverride(bounds);
    }

    private async void HideBusyIndicator()
    {
        await Task.Delay(100);
        if (busyIndicatorView != null)
        {
            busyIndicatorView!.IsVisible = false;
        }
    }
}