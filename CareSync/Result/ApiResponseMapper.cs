using CareSync.Models;
using System.Text.Json;

namespace CareSync.Result;

public class GeneralResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

internal static class ApiResponseMapper
{
    public static ApiResponse<LoginResponse> MapLoginResponse(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(json, options)!;
    }

    public static ApiResponse<GeneralResponse> MapGeneralResponse(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<ApiResponse<GeneralResponse>>(json, options)!;
    }
}
