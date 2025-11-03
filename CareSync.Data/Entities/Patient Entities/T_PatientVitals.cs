namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents vital signs and health measurements for patients in the CareSync system.
/// This entity stores critical health metrics that are regularly monitored during medical visits.
/// These measurements are essential for tracking patient health trends, medication effectiveness,
/// and identifying potential health risks or improvements over time.
/// </summary>
public class T_PatientVitals : BaseEntity
{
    /// <summary>
    /// Unique identifier for the vital signs record.
    /// Primary key that serves as the main reference for this specific set of vital measurements.
    /// Auto-incremented integer value assigned when new vitals are recorded.
    /// </summary>
    public int VitalID { get; set; }

    /// <summary>
    /// Reference to the patient whose vital signs are being recorded.
    /// Links to the PatientID in T_PatientDetails table to associate vitals with the correct patient.
    /// Required field as every vital signs record must belong to a specific patient.
    /// </summary>
    public int PatientID { get; set; }

    /// <summary>
    /// The patient's height measurement in centimeters or inches.
    /// Important for calculating BMI, medication dosages, and growth tracking in pediatric patients.
    /// Nullable as height may not be measured at every visit, especially for adult patients.
    /// </summary>
    public decimal? Height { get; set; }

    /// <summary>
    /// The patient's weight measurement in kilograms or pounds.
    /// Critical for medication dosing, BMI calculation, and monitoring weight-related health conditions.
    /// Nullable but typically recorded at most medical visits for health monitoring.
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// The patient's pulse rate (heart rate) measured in beats per minute.
    /// Essential vital sign for assessing cardiovascular health and detecting arrhythmias.
    /// Nullable as pulse may not be recorded in all types of medical encounters.
    /// </summary>
    public int? PulseRate { get; set; }

    /// <summary>
    /// The patient's blood pressure reading in standard format (e.g., "120/80").
    /// Critical vital sign for cardiovascular health assessment and hypertension monitoring.
    /// Nullable but typically recorded at most medical visits for adult patients.
    /// </summary>
    public string? BloodPressure { get; set; }

    /// <summary>
    /// Indicates whether the patient has been diagnosed with diabetes.
    /// Important flag for medication management, dietary restrictions, and specialized care protocols.
    /// Default value is false; set to true when diabetes is confirmed through medical testing.
    /// </summary>
    public bool? IsDiabetic { get; set; } = false;

    /// <summary>
    /// Blood glucose readings and related diabetic monitoring data.
    /// Contains specific glucose measurements, HbA1c levels, or other diabetes-related metrics.
    /// Nullable and only populated for patients with diabetes or glucose monitoring requirements.
    /// </summary>
    public string? DiabeticReadings { get; set; }

    /// <summary>
    /// Indicates whether the patient has been diagnosed with high blood pressure (hypertension).
    /// Important flag for cardiovascular risk assessment and treatment planning.
    /// Default value is false; set to true when hypertension is medically confirmed.
    /// </summary>
    public bool? HasHighBloodPressure { get; set; } = false;

    /// <summary>
    /// Historical blood pressure readings and hypertension monitoring data.
    /// Contains series of blood pressure measurements for trend analysis and treatment effectiveness.
    /// Nullable and only populated for patients with hypertension or blood pressure monitoring needs.
    /// </summary>
    public string? BloodPressureReadings { get; set; }

    // Navigation properties
    /// <summary>
    /// Navigation property to the patient whose vital signs are recorded.
    /// Provides access to complete patient information, medical history, and other health records.
    /// </summary>
    public virtual T_PatientDetails? Patient { get; set; }
}
