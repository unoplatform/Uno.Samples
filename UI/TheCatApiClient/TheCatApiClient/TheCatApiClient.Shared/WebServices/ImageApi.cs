using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using TheCatApiClient.Shared.Models.DataModels;

namespace TheCatApiClient.Shared.WebServices
{
    public class ImageApi : WebApiBase
    {
        private Dictionary<string, string> _defaultHeaders = new Dictionary<string, string> {
            {"accept", "application/json" },
            {"x-api-key", "{YOUR-API-KEY}"}
        };

        public async Task<IEnumerable<CatImage>> GetByBreed(string breedId)
        {
            var result = await this.GetAsync(
                $"https://api.thecatapi.com/v1/images/search?breed_id={WebUtility.HtmlEncode(breedId)}",
                _defaultHeaders);

            if (result != null)
            {
                return JsonSerializer.Deserialize<IEnumerable<CatImage>>(result);
            }

            return new List<CatImage>();
        }

        public async Task<CatImage> GetById(string imageId)
        {
            var result = await this.GetAsync(
                $"https://api.thecatapi.com/v1/images/{WebUtility.HtmlEncode(imageId)}",
                _defaultHeaders);

            if (result != null)
            {
                return JsonSerializer.Deserialize<CatImage>(result);
            }

            return null;
        }
    }
}
