using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Patient;

public class PrescriptionsModel : BasePageModel
{
    // Statistics
    public int ActivePrescriptions { get; set; } = 5;
    public int ExpiringSoon { get; set; } = 2;
    public int ThisMonth { get; set; } = 12;
    public int TotalPrescriptions { get; set; } = 48;

    // Prescription Lists
    public List<PrescriptionItem> ActiveMedications { get; set; } = new();
    public List<PrescriptionItem> PrescriptionHistory { get; set; } = new();
    public List<PharmacyItem> NearbyPharmacies { get; set; } = new();

    public void OnGet()
    {
        // Check authorization
        var authResult = RequireRole("Patient");
        if (authResult != null) return;

        LoadActivePrescriptions();
        LoadPrescriptionHistory();
        LoadNearbyPharmacies();
    }

    private void LoadActivePrescriptions()
    {
        ActiveMedications = new List<PrescriptionItem>
        {
            new PrescriptionItem
            {
                Id = 1,
                MedicationName = "Metformin 500mg",
                Dosage = "1 tablet twice daily with meals",
                DoctorName = "Dr. Sarah Johnson",
                DoctorId = 1,
                PrescribedDate = "Oct 15, 2024",
                ExpiryDate = "Jan 15, 2025",
                RefillsRemaining = 2,
                TotalRefills = 3,
                Status = "Active",
                StatusColor = "success",
                Schedule = new List<string> { "8:00 AM", "8:00 PM" },
                AppointmentId = 101
            },
            new PrescriptionItem
            {
                Id = 2,
                MedicationName = "Lisinopril 10mg",
                Dosage = "1 tablet once daily in the morning",
                DoctorName = "Dr. Michael Brown",
                DoctorId = 2,
                PrescribedDate = "Sep 20, 2024",
                ExpiryDate = "Nov 20, 2024",
                RefillsRemaining = 0,
                TotalRefills = 2,
                Status = "Expiring Soon",
                StatusColor = "warning",
                Schedule = new List<string> { "8:00 AM" },
                AppointmentId = 102
            },
            new PrescriptionItem
            {
                Id = 3,
                MedicationName = "Aspirin 81mg (Low Dose)",
                Dosage = "1 tablet once daily with food",
                DoctorName = "Dr. Sarah Johnson",
                DoctorId = 1,
                PrescribedDate = "Aug 10, 2024",
                ExpiryDate = "Feb 10, 2025",
                RefillsRemaining = 3,
                TotalRefills = 3,
                Status = "Active",
                StatusColor = "success",
                Schedule = new List<string> { "8:00 AM" },
                AppointmentId = 103
            }
        };
    }

    private void LoadPrescriptionHistory()
    {
        PrescriptionHistory = new List<PrescriptionItem>
        {
            new PrescriptionItem
            {
                Id = 4,
                MedicationName = "Amoxicillin 500mg",
                DoctorName = "Dr. Lisa Anderson",
                DoctorId = 3,
                PrescribedDate = "Jul 22, 2024",
                Status = "Completed",
                StatusColor = "secondary",
                AppointmentId = 104
            },
            new PrescriptionItem
            {
                Id = 5,
                MedicationName = "Ibuprofen 400mg",
                DoctorName = "Dr. James Wilson",
                DoctorId = 4,
                PrescribedDate = "Sep 15, 2024",
                Status = "Discontinued",
                StatusColor = "secondary",
                AppointmentId = 105
            }
        };
    }

    private void LoadNearbyPharmacies()
    {
        NearbyPharmacies = new List<PharmacyItem>
        {
            new PharmacyItem
            {
                Name = "City Pharmacy",
                Distance = "0.5 miles away",
                Rating = 4.2,
                Phone = "(555) 123-4567"
            },
            new PharmacyItem
            {
                Name = "MedPlus Pharmacy",
                Distance = "1.2 miles away",
                Rating = 4.8,
                Phone = "(555) 987-6543"
            }
        };
    }
}

public class PrescriptionItem
{
    public int Id { get; set; }
    public string MedicationName { get; set; } = "";
    public string Dosage { get; set; } = "";
    public string DoctorName { get; set; } = "";
    public int DoctorId { get; set; }
    public string PrescribedDate { get; set; } = "";
    public string ExpiryDate { get; set; } = "";
    public int RefillsRemaining { get; set; }
    public int TotalRefills { get; set; }
    public string Status { get; set; } = "";
    public string StatusColor { get; set; } = "";
    public List<string> Schedule { get; set; } = new();
    public int AppointmentId { get; set; }
}

public class PharmacyItem
{
    public string Name { get; set; } = "";
    public string Distance { get; set; } = "";
    public double Rating { get; set; }
    public string Phone { get; set; } = "";
}
