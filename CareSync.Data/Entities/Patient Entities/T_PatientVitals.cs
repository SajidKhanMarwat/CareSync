namespace CareSync.DataLayer.Entities;

public class T_PatientVitals : BaseEntity
{
    public int VitalID { get; set; }
    public int PatientID { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public int? PulseRate { get; set; }
    public string? BloodPressure { get; set; }
    public bool? IsDiabetic { get; set; } = false;
    public string? DiabeticReadings { get; set; }
    public bool? HasHighBloodPressure { get; set; } = false;
    public string? BloodPressureReadings { get; set; }

    // Navigation properties
    public virtual T_PatientDetails Patient { get; set; }
}
