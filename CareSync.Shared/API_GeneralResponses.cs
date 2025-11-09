namespace CareSync.ApplicationLayer.Common;

public class GeneralResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
public class GeneralResponse_With_Content
{
    public bool SuccessStatus { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Content { get; set; }
}