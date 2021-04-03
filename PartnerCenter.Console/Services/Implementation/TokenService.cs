using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using PartnerCenter.Console.Configuration;
using PartnerCenter.Console.Models;

namespace PartnerCenter.Console.Services.Implementation
{
    public class TokenService : ITokenService
    {
        private readonly PartnerCenterConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public TokenService(PartnerCenterConfiguration configuration,
                            HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }
        
        public async Task<PartnerCenterToken> GetPartnerCenterTokenAsync(CancellationToken cancellationToken = default)
        {
            var body = new FormUrlEncodedContent(new[]
                                                 {
                                                     new KeyValuePair<string?, string?>("grant_type", "client_credentials"),
                                                     new KeyValuePair<string?, string?>("client_id", _configuration.ClientId),
                                                     new KeyValuePair<string?, string?>("client_secret", _configuration.ClientSecret),
                                                     new KeyValuePair<string?, string?>("resource", "https://manage.devcenter.microsoft.com")
                                                 });
            var response = await _httpClient.PostAsync($"/{_configuration.Tenant}/oauth2/token", body, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PartnerCenterToken>(cancellationToken: cancellationToken);
            return result!;
        }
    }
}