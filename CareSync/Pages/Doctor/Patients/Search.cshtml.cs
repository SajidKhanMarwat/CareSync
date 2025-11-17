using Microsoft.AspNetCore.Mvc;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Doctor.Patients;

public class SearchModel : BasePageModel
{
    private readonly ILogger<SearchModel> _logger;

    public SearchModel(ILogger<SearchModel> logger)
    {
        _logger = logger;
    }

    // Statistics
    public int TotalPatients { get; set; }
    public int ActivePatients { get; set; }
    public int NewThisMonth { get; set; }
    public int AppointmentsToday { get; set; }

    // Blood Group Counts
    public BloodGroupCounts BloodGroupCounts { get; set; } = new();

    // Patient List
    public List<PatientSearchDto> Patients { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if user is authenticated and has Doctor role
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        try
        {
            await LoadPatientsData();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patients search data");
            TempData["Error"] = "Unable to load patients data. Please try again.";
            return Page();
        }
    }

    private async Task LoadPatientsData()
    {
        // Load statistics (mock data - replace with actual database queries)
        TotalPatients = 245;
        ActivePatients = 230;
        NewThisMonth = 18;
        AppointmentsToday = 12;

        // Blood group distribution
        BloodGroupCounts = new BloodGroupCounts
        {
            OPositive = 85,
            APositive = 62,
            BPositive = 48,
            ABPositive = 25,
            ONegative = 15,
            Others = 10
        };

        // Load patient list (mock data - replace with actual database queries)
        Patients = new List<PatientSearchDto>
        {
            new PatientSearchDto
            {
                PatientID = 1,
                Name = "Emily Johnson",
                Age = 28,
                Gender = "Female",
                BloodGroup = "O+",
                MaritalStatus = "Married",
                Email = "emily.johnson@email.com",
                Phone = "+1 234 567 8903",
                Address = "123 Main St, City",
                LastVisit = DateTime.Now.AddDays(-7),
                LastVisitReason = "Routine Checkup",
                NextAppointment = DateTime.Now.AddDays(14),
                HasChronicDiseases = false,
                IsActive = true
            },
            new PatientSearchDto
            {
                PatientID = 2,
                Name = "Michael Brown",
                Age = 45,
                Gender = "Male",
                BloodGroup = "A+",
                MaritalStatus = "Single",
                Email = "michael.brown@email.com",
                Phone = "+1 234 567 8904",
                Address = "456 Oak Ave, City",
                LastVisit = DateTime.Now.AddDays(-14),
                LastVisitReason = "Hypertension Follow-up",
                NextAppointment = DateTime.Now.AddDays(7),
                HasChronicDiseases = true,
                IsActive = true
            },
            new PatientSearchDto
            {
                PatientID = 3,
                Name = "Sarah Williams",
                Age = 32,
                Gender = "Female",
                BloodGroup = "B+",
                MaritalStatus = "Married",
                Email = "sarah.williams@email.com",
                Phone = "+1 234 567 8905",
                Address = "789 Pine Rd, City",
                LastVisit = DateTime.Now.AddDays(-21),
                LastVisitReason = "Diabetes Management",
                NextAppointment = null,
                HasChronicDiseases = true,
                IsActive = true
            },
            new PatientSearchDto
            {
                PatientID = 4,
                Name = "David Martinez",
                Age = 58,
                Gender = "Male",
                BloodGroup = "AB+",
                MaritalStatus = "Married",
                Email = "david.martinez@email.com",
                Phone = "+1 234 567 8906",
                Address = "321 Elm St, City",
                LastVisit = DateTime.Now.AddDays(-3),
                LastVisitReason = "Cardiology Consultation",
                NextAppointment = DateTime.Now.AddDays(30),
                HasChronicDiseases = true,
                IsActive = true
            },
            new PatientSearchDto
            {
                PatientID = 5,
                Name = "Jennifer Taylor",
                Age = 24,
                Gender = "Female",
                BloodGroup = "O-",
                MaritalStatus = "Single",
                Email = "jennifer.taylor@email.com",
                Phone = "+1 234 567 8907",
                Address = "654 Maple Ave, City",
                LastVisit = DateTime.Now.AddDays(-10),
                LastVisitReason = "General Checkup",
                NextAppointment = null,
                HasChronicDiseases = false,
                IsActive = true
            }
        };
    }
}

// DTOs
public class PatientSearchDto
{
    public int PatientID { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime LastVisit { get; set; }
    public string LastVisitReason { get; set; } = string.Empty;
    public DateTime? NextAppointment { get; set; }
    public bool HasChronicDiseases { get; set; }
    public bool IsActive { get; set; }
}

public class BloodGroupCounts
{
    public int OPositive { get; set; }
    public int APositive { get; set; }
    public int BPositive { get; set; }
    public int ABPositive { get; set; }
    public int ONegative { get; set; }
    public int Others { get; set; }
}
