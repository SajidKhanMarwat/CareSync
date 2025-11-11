using CareSync.Models;
using System.Text.Json;

namespace CareSync.Result;

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

}
