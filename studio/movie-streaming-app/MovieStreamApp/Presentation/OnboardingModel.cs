namespace MovieStreamApp.Presentation;

/// <summary>
/// Backs <see cref="OnboardingPage"/> — a static feature carousel. Each slide carries an icon KEY
/// (a stable semantic name the view resolves to vector geometry via the Icon converter), never a
/// Segoe glyph codepoint (lessons 11, 38).
/// </summary>
[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record OnboardingModel
{
    public string AppName => "CineStream";
    public string Tagline => "Your world. Unlimited movies.";
    public string HeroImageUrl => MovieData.OnboardingHero;

    // Get-only (not expression-bodied) so the FlipView and its PipsPager bind ONE shared instance
    // rather than a fresh array per access.
    public IReadOnlyList<OnboardingSlide> Slides { get; } = new[]
    {
        new OnboardingSlide(
            "Discover Cinema",
            "Thousands of movies and series from every genre, curated just for you.",
            MovieData.OnboardingHero,
            "browse"),
        new OnboardingSlide(
            "Watch Anywhere",
            "Stream in 4K on your phone, tablet, or TV. Download for offline viewing.",
            MovieData.TheaterScreen,
            "download_quality"),
        new OnboardingSlide(
            "Personalized For You",
            "Smart recommendations that learn your taste and surface hidden gems.",
            MovieData.FestivalBanner,
            "star"),
    };
}

public partial record OnboardingSlide(
    string Title,
    string Subtitle,
    string ImageUrl,
    string IconKey);
