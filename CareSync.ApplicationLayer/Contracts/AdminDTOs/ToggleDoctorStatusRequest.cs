namespace CareSync.ApplicationLayer.Contracts.AdminDTOs;

public class ToggleDoctorStatusRequest
{
    public string UserId { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
