using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace CareSync.Pages.Dashboard
{
     
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        // Dashboard statistics
        public int TotalPatients { get; set; }
        public int TodayAppointments { get; set; }
        public int PendingLabResults { get; set; }
        public int ActivePrescriptions { get; set; }

        // Recent data
        public List<RecentAppointment> RecentAppointments { get; set; } = new();
        public List<RecentLabResult> RecentLabResults { get; set; } = new();
        public List<SystemBulletin> SystemBulletins { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await LoadDashboardData();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                TempData["Error"] = "Unable to load dashboard data. Please try again.";
                return Page();
            }
        }

        private async Task LoadDashboardData()
        {
            // Load statistics
            TotalPatients = 1247; // TODO: Get from database
            TodayAppointments = 23;
            PendingLabResults = 8;
            ActivePrescriptions = 156;

            // Load recent appointments
            RecentAppointments = new List<RecentAppointment>
            {
                new RecentAppointment
                {
                    Id = 1,
                    PatientName = "John Doe",
                    DoctorName = "Dr. Smith",
                    AppointmentTime = DateTime.Today.AddHours(9),
                    Duration = 30,
                    Type = "Regular Checkup",
                    Status = "Confirmed"
                },
                new RecentAppointment
                {
                    Id = 2,
                    PatientName = "Sarah Johnson",
                    DoctorName = "Dr. Wilson",
                    AppointmentTime = DateTime.Today.AddHours(10.5),
                    Duration = 45,
                    Type = "Follow-up Consultation",
                    Status = "Pending"
                },
                new RecentAppointment
                {
                    Id = 3,
                    PatientName = "Michael Brown",
                    DoctorName = "Dr. Davis",
                    AppointmentTime = DateTime.Today.AddHours(14),
                    Duration = 60,
                    Type = "Specialist Consultation",
                    Status = "In Progress"
                }
            };

            // Load recent lab results
            RecentLabResults = new List<RecentLabResult>
            {
                new RecentLabResult
                {
                    Id = 1,
                    TestName = "Blood Test - Complete Panel",
                    PatientName = "Emma Wilson",
                    CompletedAt = DateTime.Now.AddHours(-2),
                    Status = "Normal",
                    IconClass = "ri-heart-pulse-line text-danger"
                },
                new RecentLabResult
                {
                    Id = 2,
                    TestName = "Urine Analysis",
                    PatientName = "Robert Taylor",
                    CompletedAt = DateTime.Now.AddHours(-4),
                    Status = "Review Required",
                    IconClass = "ri-microscope-line text-info"
                }
            };

            // Load system bulletins
            SystemBulletins = new List<SystemBulletin>
            {
                new SystemBulletin
                {
                    Id = 1,
                    Title = "System Maintenance Scheduled",
                    Content = "Scheduled maintenance will occur tonight from 11:00 PM to 2:00 AM. Some services may be temporarily unavailable.",
                    Type = "warning",
                    Priority = "high",
                    Author = "System Administrator",
                    CreatedAt = DateTime.Now.AddHours(-1)
                },
                new SystemBulletin
                {
                    Id = 2,
                    Title = "New Lab Equipment Available",
                    Content = "The new MRI machine is now operational and available for patient appointments. Contact the lab department for scheduling.",
                    Type = "success",
                    Priority = "normal",
                    Author = "Lab Department",
                    CreatedAt = DateTime.Now.AddHours(-3)
                }
            };
        }
    }

    // Supporting models
    public class RecentAppointment
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentTime { get; set; }
        public int Duration { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class RecentLabResult
    {
        public int Id { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public DateTime CompletedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
    }

    public class SystemBulletin
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
