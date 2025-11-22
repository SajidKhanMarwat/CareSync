namespace CareSync.ApplicationLayer.Contracts.PatientsDTOs;

/// <summary>
/// DTO for displaying doctor information in patient booking flow
/// </summary>
public class DoctorBooking_DTO
{
    public int DoctorID { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string ProfileImage { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public string Location { get; set; } = string.Empty;
    public string ConsultationFee { get; set; } = string.Empty;
    public string AvailableDays { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public List<string> AvailableDaysList { get; set; } = new();
    public List<string> AvailableTimeSlots { get; set; } = new();
    public string AvailabilityStatus { get; set; } = string.Empty;
    public string NextAvailableSlot { get; set; } = string.Empty;
}

public class BookAppointmentRequest_DTO
{
    public int DoctorID { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string AppointmentTime { get; set; } = string.Empty;
    public string AppointmentType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? CurrentMedications { get; set; }
    public string? Allergies { get; set; }
    public bool UseInsurance { get; set; }
    public bool SendReminders { get; set; }
}

public class DoctorTimeSlot_DTO
{
    public string Time { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public bool IsBooked { get; set; }
}
