using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using PartnerCenter.Console.Models;
using PartnerCenter.Console.Services;

namespace PartnerCenter.Console.Utilities
{
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private const string AccessTokenKey = "access_token";
        
        private readonly ITokenService _tokenService;
        private readonly MemoryCache _memoryCache;
        
        public AuthenticationDelegatingHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
            _memoryCache = new MemoryCache("Authentication");
        }
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = GetToken() ?? await RefreshTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                token = await RefreshTokenAsync(cancellationToken);
                request.Headers.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
                response = await base.SendAsync(request, cancellationToken);
            }

            return response;
        }

        private PartnerCenterToken? GetToken()
        {
            return _memoryCache.Get(AccessTokenKey) as PartnerCenterToken;
        }
        
        private async Task<PartnerCenterToken> RefreshTokenAsync(CancellationToken cancellationToken)
        {
            var accessToken = await _tokenService.GetPartnerCenterTokenAsync(cancellationToken);
            _memoryCache.Set(AccessTokenKey, accessToken, accessToken.ExpiresAt);
            return accessToken;
        }
    }
}