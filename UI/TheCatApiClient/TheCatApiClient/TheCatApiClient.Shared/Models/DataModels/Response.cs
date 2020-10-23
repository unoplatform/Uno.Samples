using System;
using System.Text.Json.Serialization;

namespace TheCatApiClient.Shared.Models.DataModels
{
    public partial class Response
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
