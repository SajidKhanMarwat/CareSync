using Microsoft.AspNetCore.Http;

namespace CareSync.Handlers;

/// <summary>
/// Message handler to attach JWT token to outgoing HTTP requests
/// </summary>
public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationMessageHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Get token from session
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            var token = httpContext.Session.GetString("UserToken");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
