using System.Text.Json.Serialization;

namespace TheCatApiClient.Shared.Models.DataModels
{
    public partial class FavoriteImage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
