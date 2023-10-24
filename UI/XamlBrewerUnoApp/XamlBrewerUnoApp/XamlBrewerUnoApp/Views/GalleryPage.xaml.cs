using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml.Input;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using SkiaSharpSample;

namespace XamlBrewerUnoApp
{
    public sealed partial class GalleryPage : Page
    {
        private CancellationTokenSource? cancellations;
        private readonly IList<SampleBase> samples;
        private readonly IList<GroupedSamples> sampleGroups;
        private SampleBase? sample;

        public GalleryPage()
        {
            InitializeComponent();

            samples = SamplesManager.GetSamples(SamplePlatforms.WinUI).ToList();
            sampleGroups = Enum.GetValues(typeof(SampleCategories))
                .Cast<SampleCategories>()
                .Select(c => new GroupedSamples(c, samples.Where(s => s.Category.HasFlag(c))))
                .Where(g => g.Count > 0)
                .OrderBy(g => g.Category == SampleCategories.Showcases ? string.Empty : g.Name)
                .ToList();

            SamplesInitializer.Init();

            samplesViewSource.Source = sampleGroups;

            SetSample(samples.First(s => s.Category.HasFlag(SampleCategories.Showcases)));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            cancellations?.Cancel();
            cancellations = null;
        }

        private void OnSampleSelected(object sender, SelectionChangedEventArgs e)
        {
            var sample = e.AddedItems?.FirstOrDefault() as SampleBase;
            if (sample != null)
            {
                SetSample(sample);
            }
        }

        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            OnPaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        private void OnPaintGL(object sender, SKPaintGLSurfaceEventArgs e)
        {
            OnPaintSurface(e.Surface.Canvas, e.BackendRenderTarget.Width, e.BackendRenderTarget.Height);
        }

        private void SetSample(SampleBase newSample)
        {
            // clean up the old sample
            if (sample != null)
            {
                sample.RefreshRequested -= OnRefreshRequested;
                sample.Destroy();
            }

            sample = newSample;

            // prepare the sample
            if (sample != null)
            {
                sample.RefreshRequested += OnRefreshRequested;
                sample.Init();
            }

            // refresh the view
            OnRefreshRequested(null, null);
        }

        private void OnRefreshRequested(object? sender, EventArgs? e)
        {
            canvas.Invalidate();
        }

        private void OnPaintSurface(SKCanvas canvas, int width, int height)
        {
            sample?.DrawSample(canvas, width, height);
        }

        private void OnSampleTapped(object? sender, TappedRoutedEventArgs e)
        {
            sample?.Tap();
        }
    }

    public class GroupedSamples : ObservableCollection<SampleBase>
    {
        private static readonly Regex EnumSplitRexeg = new("(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])");

        public GroupedSamples(SampleCategories category, IEnumerable<SampleBase> samples)
        {
            Category = category;
            Name = EnumSplitRexeg.Replace(category.ToString(), " $1");
            foreach (var sample in samples.OrderBy(s => s.Title))
            {
                Add(sample);
            }
        }

        public SampleCategories Category { get; private set; }

        public string Name { get; private set; }
    }
}

