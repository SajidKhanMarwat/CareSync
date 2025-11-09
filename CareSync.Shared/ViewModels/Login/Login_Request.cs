namespace CareSync.Shared.ViewModels.Login;

public class Login_Request
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public bool RememberMe { get; set; }
}
