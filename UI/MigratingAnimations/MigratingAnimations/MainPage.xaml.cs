using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;

namespace MigratingAnimations
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Storyboard_Click(object sender, RoutedEventArgs e)
        {
            ComplexXamlStoryboard.Begin();
        }

        private void CSharp_Click(object sender, RoutedEventArgs e)
        {
            Storyboard fadeOutStoryboard = new Storyboard();
            var fadeAnimation = new DoubleAnimationUsingKeyFrames();
            fadeAnimation.KeyFrames.Add(new LinearDoubleKeyFrame { Value = 0.0, KeyTime = TimeSpan.FromSeconds(1.0) });
            fadeAnimation.KeyFrames.Add(new LinearDoubleKeyFrame { Value = 1.0, KeyTime = TimeSpan.FromSeconds(2.0) });
            Storyboard.SetTarget(fadeAnimation, MoveableRectangle);
            Storyboard.SetTargetProperty(fadeAnimation, "Opacity");
            fadeOutStoryboard.Children.Add(fadeAnimation);
            fadeOutStoryboard.Begin();
        }

        private void Keyframes_Click(object sender, RoutedEventArgs e)
        {
            Storyboard storyboard = new Storyboard();
            var translateAnimationX = new DoubleAnimation();
            Storyboard.SetTarget(translateAnimationX, RectangleTranslation);
            Storyboard.SetTargetProperty(translateAnimationX, "X");
            translateAnimationX.From = -1 * this.ActualWidth / 4f;
            translateAnimationX.To = this.ActualWidth / 4f;
            translateAnimationX.Duration = new Duration(TimeSpan.FromSeconds(4));
            translateAnimationX.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            storyboard.Children.Add(translateAnimationX);

            var translateAnimationY = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(translateAnimationY, RectangleTranslation);
            Storyboard.SetTargetProperty(translateAnimationY, "Y");
            translateAnimationY.KeyFrames.Add(new DiscreteDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero), Value = this.ActualHeight / 4f });
            translateAnimationY.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2)), Value = -1 * this.ActualHeight / 4f, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
            translateAnimationY.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(4)), Value = this.ActualHeight / 4f, EasingFunction = new BounceEase() { Bounces = 4, EasingMode = EasingMode.EaseOut } });
            storyboard.Children.Add(translateAnimationY);

            storyboard.Begin();
        }
    }
}