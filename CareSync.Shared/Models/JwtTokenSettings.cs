
namespace CareSync.InfrastructureLayer.Common.Models;

public class JwtTokenSettings
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public int ExpiryMinutes { get; set; } = 1;
}
