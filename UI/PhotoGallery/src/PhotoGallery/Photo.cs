using Microsoft.UI.Xaml.Media.Imaging;

namespace PhotoGallery;

public sealed class Photo
{
    public Photo(string title, string place, string file, double aspectRatio)
    {
        Title = title;
        Place = place;
        AspectRatio = aspectRatio;
        Source = new BitmapImage(new Uri($"ms-appx:///Assets/Photos/{file}"));
    }

    public string Title { get; }

    public string Place { get; }

    public double AspectRatio { get; }

    public ImageSource Source { get; }

    public static IReadOnlyList<Photo> CreateSet() => new Photo[]
    {
        new Photo("Ember Ridge", "Lofoten", "photo01.jpg", 1.5),
        new Photo("Glass Harbor", "Dolomites", "photo02.jpg", 0.75),
        new Photo("Morning Fjord", "Faroe Islands", "photo03.jpg", 1.5),
        new Photo("Quiet Dunes", "Namib", "photo04.jpg", 2.4),
        new Photo("Violet Hour", "Patagonia", "photo05.jpg", 1.0),
        new Photo("Pine Terrace", "Kyoto Hills", "photo06.jpg", 1.5),
        new Photo("Rosewater Bay", "Azores", "photo07.jpg", 0.75),
        new Photo("Cobalt Coast", "Dalmatia", "photo08.jpg", 1.33),
        new Photo("Salt Flats", "Atacama", "photo09.jpg", 1.5),
        new Photo("Lantern Valley", "Tuscany", "photo10.jpg", 0.75),
        new Photo("Northern Mist", "Highlands", "photo11.jpg", 2.4),
        new Photo("Amber Plateau", "Arizona", "photo12.jpg", 1.5),
        new Photo("Low Tide", "Normandy", "photo13.jpg", 1.0),
        new Photo("Fern Hollow", "Black Forest", "photo14.jpg", 0.75),
        new Photo("Highland Glow", "Iceland", "photo15.jpg", 1.5),
        new Photo("Tidepool", "Brittany", "photo16.jpg", 1.33),
        new Photo("Dust and Light", "Sahara", "photo17.jpg", 1.5),
        new Photo("Blue Meridian", "Sicily", "photo18.jpg", 0.75),
        new Photo("Last Ferry", "Bosphorus", "photo19.jpg", 1.5),
        new Photo("Cedar Pass", "Tyrol", "photo20.jpg", 2.4),
        new Photo("Pale Summit", "Svalbard", "photo21.jpg", 0.75),
        new Photo("Copper Sky", "Provence", "photo22.jpg", 1.0),
        new Photo("Still Water", "Lake District", "photo23.jpg", 1.5),
        new Photo("Coral Drift", "Maldives", "photo24.jpg", 1.33),
        new Photo("Silver Basin", "Carpathians", "photo25.jpg", 0.75),
        new Photo("Windward", "Cornwall", "photo26.jpg", 1.5),
        new Photo("Dawn Switchback", "Alps", "photo27.jpg", 2.4),
        new Photo("Evening Orchard", "Umbria", "photo28.jpg", 1.5),
        new Photo("Harbor Lights", "Lisbon", "photo29.jpg", 1.0),
        new Photo("Stone Garden", "Hokkaido", "photo30.jpg", 0.75),
    };
}
