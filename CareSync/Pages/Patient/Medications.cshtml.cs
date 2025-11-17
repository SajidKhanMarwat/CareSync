using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Patient;

public class MedicationsModel : BasePageModel
{
    // Current Active Medications
    public List<MedicationItem> ActiveMedications { get; set; } = new();
    
    // Medication Schedule for Today
    public List<MedicationScheduleItem> TodaySchedule { get; set; } = new();
    
    // Past Prescriptions
    public List<MedicationPrescriptionItem> PastPrescriptions { get; set; } = new();
    
    // Medication Statistics
    public int TotalActiveMedications { get; set; }
    public int DueTodayCount { get; set; }
    public int RefillsNeeded { get; set; }
    public double AdherenceRate { get; set; }

    public IActionResult OnGet()
    {
        // Check if user is authenticated and has Patient role
        var authResult = RequireRole("Patient");
        if (authResult != null) return authResult;

        // Mock data for statistics
        TotalActiveMedications = 5;
        DueTodayCount = 12;
        RefillsNeeded = 2;
        AdherenceRate = 92.5;

        // Mock data for Active Medications
        ActiveMedications = new List<MedicationItem>
        {
            new MedicationItem
            {
                Id = 1,
                MedicationName = "Metformin",
                Dosage = "500mg",
                Frequency = "Twice daily",
                Duration = "Ongoing",
                Instructions = "Take with meals to reduce stomach upset",
                PrescribedBy = "Dr. Sarah Johnson",
                PrescribedDate = DateTime.Now.AddDays(-90).ToString("MMM dd, yyyy"),
                StartDate = DateTime.Now.AddDays(-90).ToString("MMM dd, yyyy"),
                EndDate = "Ongoing",
                Status = "Active",
                StatusColor = "success",
                PillsRemaining = 45,
                RefillDate = DateTime.Now.AddDays(15).ToString("MMM dd, yyyy"),
                Purpose = "Diabetes Management - Type 2",
                SideEffects = "Nausea, diarrhea (usually temporary)",
                TimesPerDay = new List<string> { "8:00 AM", "8:00 PM" }
            },
            new MedicationItem
            {
                Id = 2,
                MedicationName = "Lisinopril",
                Dosage = "10mg",
                Frequency = "Once daily",
                Duration = "Ongoing",
                Instructions = "Take in the morning, monitor blood pressure regularly",
                PrescribedBy = "Dr. Sarah Johnson",
                PrescribedDate = DateTime.Now.AddDays(-120).ToString("MMM dd, yyyy"),
                StartDate = DateTime.Now.AddDays(-120).ToString("MMM dd, yyyy"),
                EndDate = "Ongoing",
                Status = "Active",
                StatusColor = "success",
                PillsRemaining = 22,
                RefillDate = DateTime.Now.AddDays(7).ToString("MMM dd, yyyy"),
                Purpose = "Blood Pressure Control - Hypertension",
                SideEffects = "Dry cough, dizziness",
                TimesPerDay = new List<string> { "8:00 AM" }
            },
            new MedicationItem
            {
                Id = 3,
                MedicationName = "Atorvastatin",
                Dosage = "20mg",
                Frequency = "Once daily at bedtime",
                Duration = "Ongoing",
                Instructions = "Take at bedtime, avoid grapefruit",
                PrescribedBy = "Dr. Michael Chen",
                PrescribedDate = DateTime.Now.AddDays(-60).ToString("MMM dd, yyyy"),
                StartDate = DateTime.Now.AddDays(-60).ToString("MMM dd, yyyy"),
                EndDate = "Ongoing",
                Status = "Active",
                StatusColor = "success",
                PillsRemaining = 60,
                RefillDate = DateTime.Now.AddDays(30).ToString("MMM dd, yyyy"),
                Purpose = "Cholesterol Management",
                SideEffects = "Muscle pain, headache",
                TimesPerDay = new List<string> { "10:00 PM" }
            },
            new MedicationItem
            {
                Id = 4,
                MedicationName = "Omeprazole",
                Dosage = "40mg",
                Frequency = "Once daily",
                Duration = "8 weeks",
                Instructions = "Take before breakfast, swallow whole",
                PrescribedBy = "Dr. Robert Davis",
                PrescribedDate = DateTime.Now.AddDays(-14).ToString("MMM dd, yyyy"),
                StartDate = DateTime.Now.AddDays(-14).ToString("MMM dd, yyyy"),
                EndDate = DateTime.Now.AddDays(42).ToString("MMM dd, yyyy"),
                Status = "Active",
                StatusColor = "success",
                PillsRemaining = 42,
                RefillDate = "Not Required",
                Purpose = "Acid Reflux - GERD Treatment",
                SideEffects = "Headache, stomach pain",
                TimesPerDay = new List<string> { "7:30 AM" }
            },
            new MedicationItem
            {
                Id = 5,
                MedicationName = "Aspirin",
                Dosage = "81mg",
                Frequency = "Once daily",
                Duration = "Ongoing",
                Instructions = "Take with food, do not crush or chew",
                PrescribedBy = "Dr. Sarah Johnson",
                PrescribedDate = DateTime.Now.AddDays(-180).ToString("MMM dd, yyyy"),
                StartDate = DateTime.Now.AddDays(-180).ToString("MMM dd, yyyy"),
                EndDate = "Ongoing",
                Status = "Active",
                StatusColor = "success",
                PillsRemaining = 90,
                RefillDate = DateTime.Now.AddDays(60).ToString("MMM dd, yyyy"),
                Purpose = "Cardiovascular Protection",
                SideEffects = "Stomach irritation",
                TimesPerDay = new List<string> { "9:00 AM" }
            }
        };

        // Mock data for Today's Schedule
        TodaySchedule = new List<MedicationScheduleItem>
        {
            new MedicationScheduleItem { Time = "7:30 AM", Medication = "Omeprazole 40mg", Status = "Taken", StatusColor = "success", TakenAt = "7:35 AM" },
            new MedicationScheduleItem { Time = "8:00 AM", Medication = "Metformin 500mg", Status = "Taken", StatusColor = "success", TakenAt = "8:05 AM" },
            new MedicationScheduleItem { Time = "8:00 AM", Medication = "Lisinopril 10mg", Status = "Taken", StatusColor = "success", TakenAt = "8:05 AM" },
            new MedicationScheduleItem { Time = "9:00 AM", Medication = "Aspirin 81mg", Status = "Taken", StatusColor = "success", TakenAt = "9:10 AM" },
            new MedicationScheduleItem { Time = "2:00 PM", Medication = "Vitamin D 1000 IU", Status = "Due Soon", StatusColor = "warning", TakenAt = "" },
            new MedicationScheduleItem { Time = "8:00 PM", Medication = "Metformin 500mg", Status = "Upcoming", StatusColor = "info", TakenAt = "" },
            new MedicationScheduleItem { Time = "10:00 PM", Medication = "Atorvastatin 20mg", Status = "Upcoming", StatusColor = "info", TakenAt = "" }
        };

        // Mock data for Past Prescriptions
        PastPrescriptions = new List<MedicationPrescriptionItem>
        {
            new MedicationPrescriptionItem
            {
                Id = 1,
                PrescriptionDate = DateTime.Now.AddDays(-14).ToString("MMM dd, yyyy"),
                DoctorName = "Dr. Robert Davis",
                Specialization = "Gastroenterologist",
                MedicationCount = 2,
                Status = "Active",
                StatusColor = "success",
                Medications = new List<string> { "Omeprazole 40mg - Once daily", "Antacid as needed" }
            },
            new MedicationPrescriptionItem
            {
                Id = 2,
                PrescriptionDate = DateTime.Now.AddDays(-90).ToString("MMM dd, yyyy"),
                DoctorName = "Dr. Sarah Johnson",
                Specialization = "Endocrinologist",
                MedicationCount = 3,
                Status = "Active",
                StatusColor = "success",
                Medications = new List<string> { "Metformin 500mg - Twice daily", "Glimepiride 2mg - Once daily", "Vitamin B12 supplement" }
            },
            new MedicationPrescriptionItem
            {
                Id = 3,
                PrescriptionDate = DateTime.Now.AddDays(-180).ToString("MMM dd, yyyy"),
                DoctorName = "Dr. Michael Chen",
                Specialization = "General Physician",
                MedicationCount = 2,
                Status = "Completed",
                StatusColor = "secondary",
                Medications = new List<string> { "Amoxicillin 500mg - 7 days", "Ibuprofen 400mg - As needed" }
            }
        };

        return Page();
    }
}

// Supporting Models
public class MedicationItem
{
    public int Id { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string PrescribedBy { get; set; } = string.Empty;
    public string PrescribedDate { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StatusColor { get; set; } = "info";
    public int PillsRemaining { get; set; }
    public string RefillDate { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string SideEffects { get; set; } = string.Empty;
    public List<string> TimesPerDay { get; set; } = new();
}

public class MedicationScheduleItem
{
    public string Time { get; set; } = string.Empty;
    public string Medication { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StatusColor { get; set; } = "info";
    public string TakenAt { get; set; } = string.Empty;
}

public class MedicationPrescriptionItem
{
    public int Id { get; set; }
    public string PrescriptionDate { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int MedicationCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusColor { get; set; } = "info";
    public List<string> Medications { get; set; } = new();
}
