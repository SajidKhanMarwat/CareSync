using CareSync.Shared.Enums;

namespace CareSync.ApplicationLayer.Contracts.DoctorsDTOs;

/// <summary>
/// DTO for comprehensive doctor insights and analytics
/// </summary>
public class DoctorInsights_DTO
{
    // Summary Statistics
    public DoctorStatisticsSummary_DTO Statistics { get; set; } = new();
    
    // Performance Metrics
    public List<DoctorPerformance_DTO> TopPerformers { get; set; } = new();
    
    // Specialization Distribution
    public List<SpecializationDistribution_DTO> Specializations { get; set; } = new();
    
    // Availability Overview
    public DoctorAvailabilityOverview_DTO Availability { get; set; } = new();
    
    // Recent Activities
    public List<RecentDoctorActivity_DTO> RecentActivities { get; set; } = new();
}

/// <summary>
/// Enhanced doctor statistics with detailed metrics
/// </summary>
public class DoctorStatisticsSummary_DTO
{
    public int TotalDoctors { get; set; }
    public int ActiveDoctors { get; set; }
    public int InactiveDoctors { get; set; }
    public int OnLeave { get; set; }
    public int NewThisMonth { get; set; }
    public decimal AverageExperience { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalAppointmentsToday { get; set; }
    public int TotalPatientsToday { get; set; }
    public decimal GrowthPercentage { get; set; } // Month over month
}

/// <summary>
/// Doctor performance metrics
/// </summary>
public class DoctorPerformance_DTO
{
    public string DoctorId { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string ProfileImage { get; set; } = string.Empty;
    public int TotalPatientsTreated { get; set; }
    public int AppointmentsCompleted { get; set; }
    public int AppointmentsCancelled { get; set; }
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public decimal CompletionRate { get; set; } // Percentage of completed appointments
    public decimal PatientSatisfaction { get; set; } // Percentage
    public int ExperienceYears { get; set; }
}

/// <summary>
/// Specialization distribution and statistics
/// </summary>
public class SpecializationDistribution_DTO
{
    public string Specialization { get; set; } = string.Empty;
    public string ArabicSpecialization { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
    public int PatientCount { get; set; }
    public int AppointmentsToday { get; set; }
    public decimal Percentage { get; set; } // Percentage of total doctors
    public string IconClass { get; set; } = string.Empty; // For UI icon
    public string ColorClass { get; set; } = string.Empty; // For UI styling
}

/// <summary>
/// Doctor availability overview
/// </summary>
public class DoctorAvailabilityOverview_DTO
{
    public int AvailableNow { get; set; }
    public int InConsultation { get; set; }
    public int OnBreak { get; set; }
    public int OffDuty { get; set; }
    public List<DoctorSchedule_DTO> TodaySchedules { get; set; } = new();
}

/// <summary>
/// Individual doctor schedule
/// </summary>
public class DoctorSchedule_DTO
{
    public string UserId { get; set; } = string.Empty;
    public string DoctorId { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? AvailableDays { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public int AppointmentsBooked { get; set; }
    public int SlotsAvailable { get; set; }
    public List<DoctorAppointmentSlot> TodaySlots { get; set; } = new();
    public List<DoctorAppointmentSlot> WeekSlots { get; set; } = new();
}

public class DoctorAppointmentSlot
{
    public DateTime SlotTime { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

/// <summary>
/// Recent doctor activities
/// </summary>
public class RecentDoctorActivity_DTO
{
    public string ActivityType { get; set; } = string.Empty; // "Joined", "Consultation", "Leave", etc.
    public string DoctorName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; }
    public string IconClass { get; set; } = string.Empty;
    public string ColorClass { get; set; } = string.Empty;
}

/// <summary>
/// Doctor workload distribution
/// </summary>
public class DoctorWorkload_DTO
{
    public string DoctorId { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public List<DailyWorkload> WeeklyWorkload { get; set; } = new();
    public int TotalHoursThisWeek { get; set; }
    public int TotalPatientsThisWeek { get; set; }
    public decimal AverageConsultationTime { get; set; } // In minutes
}

public class DailyWorkload
{
    public string Day { get; set; } = string.Empty;
    public int PatientCount { get; set; }
    public int AppointmentCount { get; set; }
    public decimal HoursWorked { get; set; }
}

/// <summary>
/// Extended doctor details for grid display
/// </summary>
public class DoctorGridItem_DTO
{
    public int DoctorID { get; set; }
    public string UserID { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public DateTime? JoinedDate { get; set; }
    public string HospitalAffiliation { get; set; } = string.Empty;
    public string AvailableDays { get; set; } = string.Empty;
    public string WorkingHours { get; set; } = string.Empty;
    public int TotalPatients { get; set; }
    public int MonthlyPatients { get; set; }
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty; // "Active", "On Leave", "Inactive"
    public string ProfileImage { get; set; } = string.Empty;
}
