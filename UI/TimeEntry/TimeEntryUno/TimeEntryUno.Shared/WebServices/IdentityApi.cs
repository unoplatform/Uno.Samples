using System.Collections.Generic;
using System.Security.Principal;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TimeEntryUno.Shared.Models;

namespace TimeEntryUno.Shared.WebServices
{
    public sealed class IdentityApi : WebApiBase
    {
        private Dictionary<string, string> _defaultHeaders = new Dictionary<string, string> {
            {"accept", "application/json" }
        };

        private string _identityServiceBaseAddress;

        public IdentityApi(string identityServiceBaseAddress, string accessToken)
        {
            _identityServiceBaseAddress = identityServiceBaseAddress;
            _defaultHeaders.Add("Authorization", "Bearer " + accessToken);
        }

        public async Task<bool> ValidateUser(string userName, string password)
        {
            var result = await this.PostAsync(
                $"{_identityServiceBaseAddress}/identity/validateuser",
                JsonSerializer.Serialize(
                    new Dictionary<string, string>
                    {
                        { "userName", userName },
                        { "password", password }
                    }),
                _defaultHeaders);

            if (result != null)
            {
                return JsonSerializer.Deserialize<bool>(result);
            }

            return false;
        }

        public async Task<User> GetAuthenticatedUser(string userName)
        {
            var result = await this.PostAsync(
                $"{_identityServiceBaseAddress}/identity/getauthenticateduser",
                JsonSerializer.Serialize(
                    new Dictionary<string, string>
                    {
                        { "userName", userName },
                    }),
                _defaultHeaders);

            if (result != null)
            {
                return JsonSerializer.Deserialize<User>(result);
            }

            return null;
        }
    }
}
