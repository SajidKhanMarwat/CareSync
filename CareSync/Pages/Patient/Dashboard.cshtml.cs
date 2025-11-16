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

        public IActionResult OnGet()
        {
            // Check if user is authenticated and has Patient role
            var authResult = RequireRole("Patient");
            if (authResult != null) return authResult;

            // TODO: Load actual patient data from database
            // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Load patient details, appointments, prescriptions, lab tests etc.

            return Page();
        }
    }
}
