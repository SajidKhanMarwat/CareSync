namespace CareSync.DataLayer.Entities;

public class T_LifestyleInfo : BaseEntity
{
    public int LifestyleID { get; set; }
    public int PatientID { get; set; }
    public bool? IsSmoking { get; set; } = false;
    public string? ExerciseFrequency { get; set; }
    public string? ExerciseType { get; set; }
    public string? DailyActivity { get; set; }
    public bool? IsOnDiet { get; set; }
    public string? DietType { get; set; }
    public int? SleepHours { get; set; }
    public string? Occupation { get; set; }

    // Navigation properties
    public virtual T_PatientDetails Patient { get; set; }
}
