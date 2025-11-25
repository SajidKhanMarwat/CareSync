namespace CareSync.Shared.Models;

public class LoginResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<string> RoleRights { get; set; } = new();
    public bool RequiresPasswordReset { get; set; } = false;
}