namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents lifestyle and behavioral information for patients in the CareSync system.
/// This entity captures important lifestyle factors that significantly impact health outcomes
/// including exercise habits, smoking status, diet, sleep patterns, and occupation.
/// Essential for holistic patient care, risk assessment, and personalized treatment planning.
/// </summary>
public class T_LifestyleInfo : BaseEntity
{
    /// <summary>
    /// Unique identifier for the lifestyle information record.
    /// Primary key that serves as the main reference for this specific set of lifestyle data.
    /// Auto-incremented integer value assigned when lifestyle information is first documented.
    /// </summary>
    public int LifestyleID { get; set; }

    /// <summary>
    /// Reference to the patient whose lifestyle information is being documented.
    /// Links to the PatientID in T_PatientDetails table to associate lifestyle data with the correct patient.
    /// Required field as every lifestyle record must belong to a specific patient.
    /// </summary>
    public int PatientID { get; set; }

    /// <summary>
    /// Indicates whether the patient currently smokes tobacco products.
    /// Critical health risk factor for cardiovascular disease, cancer, and respiratory conditions.
    /// Default value is false; updated based on patient self-reporting or medical assessment.
    /// </summary>
    public bool? IsSmoking { get; set; } = false;

    /// <summary>
    /// Frequency of physical exercise or fitness activities (e.g., "Daily", "3 times per week", "Rarely").
    /// Important for cardiovascular health assessment, diabetes management, and overall wellness planning.
    /// Nullable as exercise patterns may vary or be under assessment.
    /// </summary>
    public string? ExerciseFrequency { get; set; }

    /// <summary>
    /// Type of physical exercise or activities the patient engages in (e.g., "Walking", "Swimming", "Gym workouts").
    /// Helps in understanding fitness level, joint health, and appropriate exercise recommendations.
    /// Nullable as patients may have varied or changing exercise routines.
    /// </summary>
    public string? ExerciseType { get; set; }

    /// <summary>
    /// Description of the patient's typical daily activities and activity level.
    /// Includes work-related activities, household tasks, and general mobility patterns.
    /// Nullable but valuable for assessing overall activity level and mobility status.
    /// </summary>
    public string? DailyActivity { get; set; }

    /// <summary>
    /// Indicates whether the patient is currently following a specific diet plan.
    /// Important for nutritional assessment, diabetes management, and weight control programs.
    /// Nullable as diet status may be under evaluation or changing.
    /// </summary>
    public bool? IsOnDiet { get; set; }

    /// <summary>
    /// Type of diet the patient follows (e.g., "Low sodium", "Diabetic diet", "Mediterranean", "Vegetarian").
    /// Critical for medication interactions, nutritional counseling, and disease management.
    /// Nullable and only relevant when IsOnDiet is true or diet is being assessed.
    /// </summary>
    public string? DietType { get; set; }

    /// <summary>
    /// Average number of hours the patient sleeps per night.
    /// Important for mental health assessment, chronic disease management, and overall wellness evaluation.
    /// Nullable as sleep patterns may vary or be under monitoring.
    /// </summary>
    public int? SleepHours { get; set; }

    /// <summary>
    /// The patient's current occupation or profession.
    /// Relevant for occupational health risks, work-related stress assessment, and injury prevention.
    /// Nullable as occupation may change or patients may be unemployed/retired.
    /// </summary>
    public string? Occupation { get; set; }

    // Navigation properties
    /// <summary>
    /// Navigation property to the patient whose lifestyle information is documented.
    /// Provides access to complete patient profile, medical history, and other health information.
    /// </summary>
    public virtual T_PatientDetails? Patient { get; set; }
}
