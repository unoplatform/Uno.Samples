using Caffe.Models;

namespace Caffe.Controls;

public sealed partial class GrindSelector : UserControl
{
    private GrindLevel _lastParticleLevel = (GrindLevel)(-1);

    public static readonly DependencyProperty GrindLevelProperty =
        DependencyProperty.Register(
            nameof(GrindLevel),
            typeof(GrindLevel),
            typeof(GrindSelector),
            new PropertyMetadata(GrindLevel.Fine, OnGrindLevelChanged));

    public GrindLevel GrindLevel
    {
        get => (GrindLevel)GetValue(GrindLevelProperty);
        set => SetValue(GrindLevelProperty, value);
    }

    public GrindSelector()
    {
        this.InitializeComponent();
        UpdateParticles(GrindLevel.Fine);
    }

    private static void OnGrindLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GrindSelector selector)
            selector.UpdateParticles((GrindLevel)e.NewValue);
    }

    private void OnFineClick(object sender, RoutedEventArgs e) => GrindLevel = GrindLevel.Fine;
    private void OnMediumClick(object sender, RoutedEventArgs e) => GrindLevel = GrindLevel.Medium;
    private void OnCoarseClick(object sender, RoutedEventArgs e) => GrindLevel = GrindLevel.Coarse;

    // Particle layout is generated, so it stays in code; labels and button
    // highlights are driven by the x:Bind function bindings below.
    private void UpdateParticles(GrindLevel level)
    {
        if (_lastParticleLevel == level) return;
        _lastParticleLevel = level;

        var particleBrush = (Brush)Application.Current.Resources["CaffeParticleBrush"];

        ParticleGrid.Children.Clear();
        ParticleGrid.RowDefinitions.Clear();
        ParticleGrid.ColumnDefinitions.Clear();

        var (count, size) = level switch
        {
            GrindLevel.Fine => (12, 6.0),
            GrindLevel.Medium => (9, 9.0),
            GrindLevel.Coarse => (6, 13.0),
            _ => (9, 9.0)
        };

        var cols = level switch
        {
            GrindLevel.Fine => 4,
            GrindLevel.Medium => 3,
            GrindLevel.Coarse => 3,
            _ => 3
        };

        var rows = (int)Math.Ceiling((double)count / cols);

        for (int c = 0; c < cols; c++)
            ParticleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        for (int r = 0; r < rows; r++)
            ParticleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        for (int i = 0; i < count; i++)
        {
            var row = i / cols;
            var col = i % cols;

            var ellipse = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = particleBrush,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetRow(ellipse, row);
            Grid.SetColumn(ellipse, col);
            ParticleGrid.Children.Add(ellipse);
        }
    }

    // x:Bind function bindings (see GrindSelector.xaml).
    private string GrindLabel(GrindLevel level) => level.ToLabel();
    private string GrindHint(GrindLevel level) => level.ToHint();
    private Brush FineBrush(GrindLevel level) => GrindButtonBrush(level == GrindLevel.Fine);
    private Brush MediumBrush(GrindLevel level) => GrindButtonBrush(level == GrindLevel.Medium);
    private Brush CoarseBrush(GrindLevel level) => GrindButtonBrush(level == GrindLevel.Coarse);

    private static Brush GrindButtonBrush(bool isSelected) =>
        (Brush)Application.Current.Resources[isSelected ? "CaffePrimaryBrush" : "CaffeBorderBrush"];
}
