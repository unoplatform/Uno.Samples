using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using TheCatApiClient.Shared.Models.DataModels;

namespace TheCatApiClient.Shared.WebServices
{
    public class BreedSearchApi : WebApiBase
    {
        public async Task<IEnumerable<Breed>> Search(string search)
        {
            var result = await this.GetAsync(
                $"https://api.thecatapi.com/v1/breeds/search?q={WebUtility.HtmlEncode(search)}",
                new Dictionary<string, string> {
                    {"accept", "application/json" },
                    {"x-api-key", "{YOUR-API-KEY}"}
                });

            if (result != null)
            {
                return JsonSerializer.Deserialize<IEnumerable<Breed>>(result);
            }

            return new List<Breed>();
        }
    }
}
