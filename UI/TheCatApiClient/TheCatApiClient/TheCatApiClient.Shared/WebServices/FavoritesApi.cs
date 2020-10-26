using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using TheCatApiClient.Shared.Models.DataModels;

namespace TheCatApiClient.Shared.WebServices
{
    public class FavoritesApi : WebApiBase
    {
        private Dictionary<string, string> _defaultHeaders = new Dictionary<string, string> {
                {"accept", "application/json" },
                {"x-api-key", "{YOUR-API-KEY}"}
            };

        // Insert GetAll and Get below here
        public async Task<IEnumerable<Favorite>> GetAll()
        {
            var result = await this.GetAsync(
                $"https://api.thecatapi.com/v1/favourites",
                _defaultHeaders);

            if (result != null)
            {
                return JsonSerializer.Deserialize<IEnumerable<Favorite>>(result);
            }

            return new List<Favorite>();
        }

        public async Task<Favorite> Get(int id)
        {
            var result = await this.GetAsync(
                $"https://api.thecatapi.com/v1/favourites/{id}",
                _defaultHeaders);

            if (result != null)
            {
                return JsonSerializer.Deserialize<Favorite>(result);
            }

            return null;
        }

        // Insert Add below here
        public async Task<Response> Add(CatImage image)
        {
            var result = await this.PostAsync(
                $"https://api.thecatapi.com/v1/favourites",
                JsonSerializer.Serialize(
                    new Dictionary<string, string> 
                    { 
                        { "image_id", image.Id }, 
                        { "sub_id", "uno-client" } 
                    }),
                _defaultHeaders);

            if (result != null)
            {
                return JsonSerializer.Deserialize<Response>(result);
            }

            return null;
        }

        // Insert Delete below here
        public async Task<Response> Delete(Favorite favorite)
        {
            var result = await this.DeleteAsync(
                $"https://api.thecatapi.com/v1/favourites/{favorite.Id}",
                _defaultHeaders);

            if (result != null)
            {
                return JsonSerializer.Deserialize<Response>(result);
            }

            return null;
        }
    }
}