using Microsoft.AspNetCore.Http;

namespace CareSync.InfrastructureLayer.Services.CookieServices;

public static class CookieService
{
    private static IHttpContextAccessor? _httpContextAccessor;

    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    #region Start | Set Cookies
    public static Task Set_RefreshToken_Cookies(string refreshToken, int expireDays = 7)
    {
        if (_httpContextAccessor == null)
            throw new InvalidOperationException("CookieService is not configured.");

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(expireDays)
        };

        _httpContextAccessor.HttpContext?.Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);

        return Task.CompletedTask;
    }
    #endregion End | Set Cookies

    #region Start | Get Cookies
    public static Task<string?> Get_RefreshToken_Cookies()
    {
        if (_httpContextAccessor == null)
            throw new InvalidOperationException("CookieService is not configured.");

        string refreshToken = "";
        _httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue("RefreshToken", out refreshToken);

        return Task.FromResult(refreshToken);
    }
    #endregion End | Get Cookies
}