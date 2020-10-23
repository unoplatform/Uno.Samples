using System;
using System.Text.Json.Serialization;

namespace TheCatApiClient.Shared.Models.DataModels
{
    public partial class Favorite
    {
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("image")]
        public FavoriteImage Image { get; set; }

        [JsonPropertyName("image_id")]
        public string ImageId { get; set; }

        [JsonPropertyName("sub_id")]
        public string SubId { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
    }
}
