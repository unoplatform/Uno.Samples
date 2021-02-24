using System;
using System.Threading.Tasks;
using TimeEntryUno.Shared.WebServices;

namespace TimeEntryUno.Shared.Services
{
    public sealed class TokenService
        : SingletonBase<TokenService>
    {
        private IdentityServerClient _identityServerClient;

        private TokenService()
        {
            _identityServerClient = new IdentityServerClient(
                identityServerBaseAddress: "https://localhost:5001",
                clientId: "TimeEntryUno",
                clientSecret: "A2W7aQVFQWRX",
                scope: "TimeEntryApi");

            // starts the initialization
            Initialization = InitializeAsync();
        }

        public string AccessToken { get; private set; }

        // To ensure initialized, use await TokenService.Instance.Initialization;
        public Task Initialization { get; private set; }

        private async Task InitializeAsync()
        {
            AccessToken = await _identityServerClient.GetAccessTokenAsync();
        }

        public async Task<string> GetAccessTokenAsync()
        {
            await Initialization;
            if (Initialization.IsCompleted && Initialization.Status == TaskStatus.RanToCompletion)
            {
                return AccessToken;
            }

            throw new InvalidOperationException("AccessToken is unavailable");
        }
    }
}

