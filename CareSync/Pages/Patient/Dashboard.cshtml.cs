using Microsoft.AspNetCore.Mvc;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Patient
{
    public class DashboardModel : BasePageModel
    {
        private readonly ILogger<DashboardModel> _logger;

        public DashboardModel(ILogger<DashboardModel> logger)
        {
            _logger = logger;
        }

        // Patient Information Properties
        public string PatientName { get; set; } = "John Smith";
        public string Gender { get; set; } = "Male";
        public int Age { get; set; } = 42;
        public string BloodType { get; set; } = "O+";
        public string PrimaryDoctor { get; set; } = "Dr. Sarah Johnson";
        public string LastVisitDate { get; set; } = "Nov 10, 2024";
        public string NextAppointmentDate { get; set; } = "Nov 18, 2024";

        // Dashboard Statistics
        public int UpcomingAppointments { get; set; } = 3;
        public int ActivePrescriptions { get; set; } = 5;
        public int PendingLabTests { get; set; } = 2;
        public int NewReports { get; set; } = 1;

        // Health Vitals
        public decimal CurrentBP { get; set; } = 120;
        public decimal CurrentSugar { get; set; } = 95;
        public int CurrentHeartRate { get; set; } = 72;
        public decimal CurrentCholesterol { get; set; } = 185;

        // Doctor Visits
        public List<DoctorVisit> DoctorVisits { get; set; } = new();

        // Medical Reports
        public List<MedicalReport> MedicalReports { get; set; } = new();

        public IActionResult OnGet()
        {
            // Check if user is authenticated and has Patient role
            var authResult = RequireRole("Patient");
            if (authResult != null) return authResult;

            // TODO: Load actual patient data from database
            // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Load patient details, appointments, prescriptions, lab tests etc.

            // Mock data for Doctor Visits
            DoctorVisits = new List<DoctorVisit>
            {
                new DoctorVisit
                {
                    Id = 1,
                    DoctorName = "Dr. Sarah Johnson",
                    DoctorImage = "~/theme/images/user.png",
                    VisitDate = "Nov 10, 2024",
                    Department = "Cardiology",
                    Specialization = "Heart Specialist"
                },
                new DoctorVisit
                {
                    Id = 2,
                    DoctorName = "Dr. Michael Chen",
                    DoctorImage = "~/theme/images/user.png",
                    VisitDate = "Oct 28, 2024",
                    Department = "General Medicine",
                    Specialization = "Family Physician"
                },
                new DoctorVisit
                {
                    Id = 3,
                    DoctorName = "Dr. Emily Roberts",
                    DoctorImage = "~/theme/images/user.png",
                    VisitDate = "Oct 15, 2024",
                    Department = "Neurology",
                    Specialization = "Neurologist"
                }
            };

            // Mock data for Medical Reports
            MedicalReports = new List<MedicalReport>
            {
                new MedicalReport
                {
                    Id = 1,
                    ReportTitle = "Blood Test - Complete Blood Count",
                    ReportDate = "Nov 10, 2024",
                    ReportType = "Lab Report"
                },
                new MedicalReport
                {
                    Id = 2,
                    ReportTitle = "ECG Report - Cardiac Evaluation",
                    ReportDate = "Oct 28, 2024",
                    ReportType = "Diagnostic Report"
                },
                new MedicalReport
                {
                    Id = 3,
                    ReportTitle = "X-Ray - Chest Examination",
                    ReportDate = "Oct 15, 2024",
                    ReportType = "Radiology Report"
                }
            };

            return Page();
        }
    }

    // Supporting Models
    public class DoctorVisit
    {
        public int Id { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorImage { get; set; } = string.Empty;
        public string VisitDate { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
    }

    public class MedicalReport
    {
        public int Id { get; set; }
        public string ReportTitle { get; set; } = string.Empty;
        public string ReportDate { get; set; } = string.Empty;
        public string ReportType { get; set; } = string.Empty;
    }
}
