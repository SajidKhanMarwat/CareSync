using Microsoft.AspNetCore.Mvc;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Doctor
{
    public class DashboardModel : BasePageModel
    {
        private readonly ILogger<DashboardModel> _logger;

        public DashboardModel(ILogger<DashboardModel> logger)
        {
            _logger = logger;
        }

        // Doctor Information
        public string DoctorName { get; set; } = "Dr. Yolanda Skinner";
        public string Specialization { get; set; } = "BDS, MDS - General & Gynecologist";
        public int TotalRatings { get; set; } = 3860;

        // Statistics
        public int TotalPatients { get; set; }
        public int TotalSurgeries { get; set; }
        public decimal MonthlyEarnings { get; set; }
        
        // Detailed Insights
        public int TodayAppointmentsCount { get; set; }
        public int TotalPrescriptions { get; set; }
        public int PendingAppointments { get; set; }
        public int LabReports { get; set; }
        
        // Patient Statistics
        public int NewPatients { get; set; }
        public int RegularPatients { get; set; }
        public int FollowUpPatients { get; set; }

        // Data Collections
        public List<TodayAppointment> TodayAppointments { get; set; } = new();
        public List<PatientReview> PatientReviews { get; set; } = new();
        public List<RecentPatient> RecentPatients { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is authenticated and has Doctor role
            var authResult = RequireRole("Doctor");
            if (authResult != null) return authResult;

            try
            {
                await LoadDashboardData();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading doctor dashboard data");
                TempData["Error"] = "Unable to load dashboard data. Please try again.";
                return Page();
            }
        }

        private async Task LoadDashboardData()
        {
            // Load statistics
            TotalPatients = 680;
            TotalSurgeries = 30;
            MonthlyEarnings = 980;
            
            // Load detailed insights
            TodayAppointmentsCount = 15;
            TotalPrescriptions = 245;
            PendingAppointments = 8;
            LabReports = 12;
            
            // Load patient statistics
            NewPatients = 45;
            RegularPatients = 520;
            FollowUpPatients = 115;

            // Load today's appointments
            TodayAppointments = new List<TodayAppointment>
            {
                new TodayAppointment
                {
                    Id = 1,
                    PatientName = "Willian Mathews",
                    PatientAge = 21,
                    AppointmentTime = DateTime.Today.AddHours(10.5),
                    Diagnosis = "Heart Attack",
                    Type = "General"
                },
                new TodayAppointment
                {
                    Id = 2,
                    PatientName = "Adam Bradley",
                    PatientAge = 36,
                    AppointmentTime = DateTime.Today.AddHours(10.75),
                    Diagnosis = "Diabetes",
                    Type = "Consulting"
                },
                new TodayAppointment
                {
                    Id = 3,
                    PatientName = "Merle Daniel",
                    PatientAge = 82,
                    AppointmentTime = DateTime.Today.AddHours(11),
                    Diagnosis = "Chancroid",
                    Type = "General"
                },
                new TodayAppointment
                {
                    Id = 4,
                    PatientName = "Nicole Sellers",
                    PatientAge = 29,
                    AppointmentTime = DateTime.Today.AddHours(11.25),
                    Diagnosis = "Pediatric",
                    Type = "Consulting"
                },
                new TodayAppointment
                {
                    Id = 5,
                    PatientName = "Kathy Atkinson",
                    PatientAge = 58,
                    AppointmentTime = DateTime.Today.AddHours(11.75),
                    Diagnosis = "Alphaviruses",
                    Type = "General"
                }
            };

            // Load patient reviews
            PatientReviews = new List<PatientReview>
            {
                new PatientReview
                {
                    PatientId = 1,
                    PatientName = "Wendi Combs",
                    ReviewText = "I had a very good experience here. I got a best psychiatrist and a therapist. They analysed my case very deeply and their medicines helped me a lot.",
                    Rating = 5
                },
                new PatientReview
                {
                    PatientId = 2,
                    PatientName = "Nick Morrow",
                    ReviewText = "Dr. Jessika listens to you very patiently & gives you sufficient time to say your problems. She diagnosed in no time & I was able to recover quickly.",
                    Rating = 5
                },
                new PatientReview
                {
                    PatientId = 3,
                    PatientName = "Carole Dodson",
                    ReviewText = "She is very supportive and suggests well. Good surgeon known from past 10 years. My mother was a renal transplant patient but in most risky condition she treated her in day care procedure.",
                    Rating = 4
                },
                new PatientReview
                {
                    PatientId = 4,
                    PatientName = "Ashley Clay",
                    ReviewText = "Jessika is a very good Doctor because I had good experience with her, she treated my father who is a diabetic patient.",
                    Rating = 5
                },
                new PatientReview
                {
                    PatientId = 5,
                    PatientName = "Emmitt Macias",
                    ReviewText = "One of the finest doctor's I ever met. A very good human being more than a doctor.",
                    Rating = 4
                }
            };

            // Load recent patients
            RecentPatients = new List<RecentPatient>
            {
                new RecentPatient
                {
                    Id = 39,
                    Name = "Emmitt Macias",
                    LastAppointment = DateTime.Now.AddDays(-5),
                    Notes = "Need an appointment urgent."
                },
                new RecentPatient
                {
                    Id = 63,
                    Name = "Kathy Atkinson",
                    LastAppointment = DateTime.Now.AddDays(-2),
                    Notes = "Follow-up required for diabetes management."
                },
                new RecentPatient
                {
                    Id = 86,
                    Name = "Merle Daniel",
                    LastAppointment = DateTime.Now.AddDays(-10),
                    Notes = "Routine checkup needed."
                }
            };
        }
    }

    // Supporting Models
    public class TodayAppointment
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class PatientReview
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string ReviewText { get; set; } = string.Empty;
        public int Rating { get; set; }
    }

    public class RecentPatient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime LastAppointment { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
