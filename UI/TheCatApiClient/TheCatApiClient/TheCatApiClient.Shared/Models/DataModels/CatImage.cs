using System;
using System.Text.Json.Serialization;

namespace TheCatApiClient.Shared.Models.DataModels
{
    public partial class CatImage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }
    }
}