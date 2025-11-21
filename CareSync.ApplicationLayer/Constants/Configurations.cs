using Microsoft.Extensions.Configuration;


namespace CareSync.InfrastructureLayer.Common.Constants;

public static class Configurations
{
    private static IConfiguration? _configuration;
    public static void Initialize(IConfiguration configuration)
        => _configuration = configuration ?? throw new NotImplementedException();
    public static string GetConfigurationValue(string name)
        => _configuration?.GetConnectionString(name) ?? string.Empty;

    public static T? GetConfigurationSectionByName<T>(string name) where T : class
        => _configuration?.GetSection(name).Get<T>();
}