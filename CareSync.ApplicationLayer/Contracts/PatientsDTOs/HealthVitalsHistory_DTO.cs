namespace CareSync.ApplicationLayer.Contracts.PatientsDTOs;

/// <summary>
/// Health vitals history for dashboard cards
/// </summary>
public class HealthVitalsHistory_DTO
{
    /// <summary>
    /// Blood pressure readings (last 5 records)
    /// </summary>
    public List<VitalReading_DTO> BloodPressureReadings { get; set; } = new();

    /// <summary>
    /// Blood sugar readings (last 5 records)
    /// </summary>
    public List<VitalReading_DTO> BloodSugarReadings { get; set; } = new();

    /// <summary>
    /// Heart rate readings (last 5 records)
    /// </summary>
    public List<VitalReading_DTO> HeartRateReadings { get; set; } = new();

    /// <summary>
    /// Cholesterol readings (last 5 records)
    /// </summary>
    public List<VitalReading_DTO> CholesterolReadings { get; set; } = new();
}

/// <summary>
/// Individual vital reading
/// </summary>
public class VitalReading_DTO
{
    /// <summary>
    /// Date of the reading
    /// </summary>
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// Value of the reading
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Status indicator (Normal, Warning, Critical)
    /// </summary>
    public string Status { get; set; } = "Normal";

    /// <summary>
    /// Badge color class (bg-success, bg-warning, bg-danger)
    /// </summary>
    public string BadgeClass { get; set; } = "bg-success";
}
