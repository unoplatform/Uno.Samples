namespace MovieStreamApp.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record OnboardingModel
{
    public string AppName { get; } = "CineStream";
    public string Tagline { get; } = "Your world. Unlimited movies.";
    public string HeroImageUrl { get; } = "https://picsum.photos/seed/streaming%20entertainment%20epic%20cinematic%20landscape/1280/720";
    public string Feature1Title { get; } = "Thousands of Titles";
    public string Feature1Subtitle { get; } = "Action, drama, sci-fi, documentaries — curated for every mood.";
    public string Feature2Title { get; } = "Download &amp; Watch Offline";
    public string Feature2Subtitle { get; } = "Save your favourites and enjoy them anywhere, anytime.";
    public string Feature3Title { get; } = "4K Ultra HD";
    public string Feature3Subtitle { get; } = "Crystal-clear picture with Dolby Atmos surround sound.";
    public IReadOnlyList<OnboardingSlide> Slides { get; } = new[]
    {
        new OnboardingSlide(
            "Discover Cinema",
            "Thousands of movies and series from every genre, curated just for you.",
            "https://picsum.photos/seed/cinematic%20movie%20poster%20action/768/1024",
            "\uE714"),
        new OnboardingSlide(
            "Watch Anywhere",
            "Stream in 4K on your phone, tablet, or TV. Download for offline viewing.",
            "https://picsum.photos/seed/movie%20theater%20cinema%20dark%20screen/1280/720",
            "\uE767"),
        new OnboardingSlide(
            "Personalized For You",
            "Smart recommendations that learn your taste and surface hidden gems.",
            "https://picsum.photos/seed/film%20festival%20banner%20widescreen%20cinema/1280/720",
            "\uEB51"),
    };
}

public partial record OnboardingSlide(
    string Title,
    string Subtitle,
    string ImageUrl,
    string IconGlyph);
