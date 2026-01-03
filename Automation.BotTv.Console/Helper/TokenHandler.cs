using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using Automation.BotTv.Repository;

namespace Automation.BotTv.Console.Helper;
public class TokenHttpHandler : DelegatingHandler
{
    #region Properties
    private readonly TokenService _serviceToken;
    private readonly IMemoryCache _cache;
    #endregion

    #region Constructor
    public TokenHttpHandler(TokenService serviceToken, IMemoryCache cache)
    {
        _serviceToken = serviceToken;
        _cache = cache;
    }
    #endregion

    #region Methods
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!_cache.TryGetValue("AuthToken", out string token))
        {
            var Jwt = await _serviceToken.GerarToken();
            var expiration = (Jwt.Expiration.AddSeconds(-5) - DateTime.Now).TotalSeconds;
            _cache.Set("AuthToken", Jwt.Token, absoluteExpirationRelativeToNow: TimeSpan.FromSeconds(expiration));
        }
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
    #endregion
}
