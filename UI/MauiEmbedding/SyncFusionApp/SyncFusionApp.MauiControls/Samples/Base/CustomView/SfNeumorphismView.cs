namespace SyncFusionApp.MauiControls.Samples.Base.CustomView;

public class SfNeumorphismView : ContentView
{
    private readonly Grid grid;

    private readonly GraphicsView graphicsView;

    public static readonly BindableProperty DrawableProperty = BindableProperty.Create("Drawable", typeof(SfNeumorphismDrawer), typeof(SfNeumorphismView), null, BindingMode.OneWay, null, OnDrawablePropertyChanged);

    public new static readonly BindableProperty ContentProperty = BindableProperty.Create("Content", typeof(View), typeof(SfNeumorphismView), null, BindingMode.OneWay, null, OnContentPropertyChanged);

    public SfNeumorphismDrawer Drawable
    {
        get
        {
            return (SfNeumorphismDrawer)GetValue(DrawableProperty);
        }
        set
        {
            SetValue(DrawableProperty, value);
        }
    }

    public new View Content
    {
        get
        {
            return (View)GetValue(ContentProperty);
        }
        set
        {
            SetValue(ContentProperty, value);
        }
    }

    public SfNeumorphismView()
    {
        Drawable = new SfNeumorphismDrawer();
        grid = new Grid();
        grid.Margin = new Thickness(0.0);
        graphicsView = new GraphicsView();
        graphicsView.Margin = new Thickness(0.0);
        graphicsView.BackgroundColor = Colors.Transparent;
        graphicsView.SetBinding(GraphicsView.DrawableProperty, new Binding
        {
            Path = "Drawable",
            Source = this
        });
        grid.Children.Add(graphicsView);
        base.Content = grid;
    }

    //
    // Parameters:
    //   bindable:
    //
    //   oldValue:
    //
    //   newValue:
    protected static void OnDrawablePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
    }

    public void Invalidate()
    {
        graphicsView.Invalidate();
    }

    //
    // Parameters:
    //   bindable:
    //
    //   oldValue:
    //
    //   newValue:
    protected static void OnContentPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        SfNeumorphismView sfNeumorphismView = (SfNeumorphismView)bindable;
        View view = newValue as View;
        if (view != null && !sfNeumorphismView.grid.Children.Contains(view))
        {
            sfNeumorphismView.grid.Children.Add(view);
        }

        View view2 = oldValue as View;
        if (view2 != null && sfNeumorphismView.grid.Children.Contains(view2))
        {
            sfNeumorphismView.grid.Children.Remove(view2);
        }
    }
}
