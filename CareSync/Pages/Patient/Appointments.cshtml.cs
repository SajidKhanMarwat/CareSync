using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Patient
{
    public class AppointmentsModel : BasePageModel
    {
        // Statistics
        public int TotalUpcoming { get; set; } = 0;
        public int TotalCompleted { get; set; } = 0;
        public int TotalPending { get; set; } = 0;
        public int TotalCancelled { get; set; } = 0;

        // Appointment Lists
        public List<AppointmentItem> CurrentAppointments { get; set; } = new();
        public List<AppointmentItem> NextAppointments { get; set; } = new();
        public List<AppointmentItem> RecentAppointments { get; set; } = new();

        public IActionResult OnGet()
        {
            // Check if user is authenticated and has Patient role
            var authResult = RequireRole("Patient");
            if (authResult != null) return authResult;

            // Mock data for statistics
            TotalUpcoming = 5;
            TotalCompleted = 28;
            TotalPending = 2;
            TotalCancelled = 1;

            // Mock data for Current Appointments (Today)
            CurrentAppointments = new List<AppointmentItem>
            {
                new AppointmentItem
                {
                    Id = 1,
                    DoctorName = "Dr. Sarah Johnson",
                    DoctorImage = "~/theme/images/user.png",
                    Specialization = "Cardiologist",
                    Department = "Cardiology",
                    AppointmentDate = DateTime.Today.ToString("MMMM dd, yyyy"),
                    AppointmentTime = "10:00 AM",
                    AppointmentType = "Consultation",
                    Status = "Confirmed",
                    StatusColor = "success",
                    RoomNumber = "Room 205",
                    HasPrescription = true,
                    HasLabReports = true
                },
                new AppointmentItem
                {
                    Id = 2,
                    DoctorName = "Dr. Michael Chen",
                    DoctorImage = "~/theme/images/user.png",
                    Specialization = "General Physician",
                    Department = "General Medicine",
                    AppointmentDate = DateTime.Today.ToString("MMMM dd, yyyy"),
                    AppointmentTime = "2:30 PM",
                    AppointmentType = "Follow-up",
                    Status = "Confirmed",
                    StatusColor = "success",
                    RoomNumber = "Room 108",
                    HasPrescription = false,
                    HasLabReports = true
                }
            };

            // Mock data for Next/Upcoming Appointments
            NextAppointments = new List<AppointmentItem>
            {
                new AppointmentItem
                {
                    Id = 3,
                    DoctorName = "Dr. Emily Roberts",
                    DoctorImage = "~/theme/images/user.png",
                    Specialization = "Neurologist",
                    Department = "Neurology",
                    AppointmentDate = DateTime.Today.AddDays(3).ToString("MMMM dd, yyyy"),
                    AppointmentTime = "11:00 AM",
                    AppointmentType = "Consultation",
                    Status = "Scheduled",
                    StatusColor = "info",
                    RoomNumber = "Room 301",
                    HasPrescription = false,
                    HasLabReports = false
                },
                new AppointmentItem
                {
                    Id = 4,
                    DoctorName = "Dr. James Wilson",
                    DoctorImage = "~/theme/images/user.png",
                    Specialization = "Orthopedic Surgeon",
                    Department = "Orthopedics",
                    AppointmentDate = DateTime.Today.AddDays(5).ToString("MMMM dd, yyyy"),
                    AppointmentTime = "3:00 PM",
                    AppointmentType = "Surgery Consultation",
                    Status = "Pending Confirmation",
                    StatusColor = "warning",
                    RoomNumber = "TBD",
                    HasPrescription = false,
                    HasLabReports = true
                },
                new AppointmentItem
                {
                    Id = 5,
                    DoctorName = "Dr. Lisa Martinez",
                    DoctorImage = "~/theme/images/user.png",
                    Specialization = "Dermatologist",
                    Department = "Dermatology",
                    AppointmentDate = DateTime.Today.AddDays(7).ToString("MMMM dd, yyyy"),
                    AppointmentTime = "9:30 AM",
                    AppointmentType = "Check-up",
                    Status = "Scheduled",
                    StatusColor = "info",
                    RoomNumber = "Room 115",
                    HasPrescription = true,
                    HasLabReports = false
                }
            };

            // Mock data for Recent Appointments
            RecentAppointments = new List<AppointmentItem>
            {
                new AppointmentItem
                {
                    Id = 6,
                    DoctorName = "Dr. Sarah Johnson",
                    DoctorImage = "~/theme/images/user.png",
                    Specialization = "Cardiologist",
                    Department = "Cardiology",
                    AppointmentDate = DateTime.Today.AddDays(-5).ToString("MMMM dd, yyyy"),
                    AppointmentTime = "10:00 AM",
                    AppointmentType = "Follow-up",
                    Status = "Completed",
                    StatusColor = "success",
                    RoomNumber = "Room 205",
                    HasPrescription = true,
                    HasLabReports = true
                },
                new AppointmentItem
                {
                    Id = 7,
                    DoctorName = "Dr. Robert Davis",
                    DoctorImage = "~/theme/images/user.png",
                    Specialization = "Endocrinologist",
                    Department = "Endocrinology",
                    AppointmentDate = DateTime.Today.AddDays(-10).ToString("MMMM dd, yyyy"),
                    AppointmentTime = "2:00 PM",
                    AppointmentType = "Consultation",
                    Status = "Completed",
                    StatusColor = "success",
                    RoomNumber = "Room 412",
                    HasPrescription = true,
                    HasLabReports = true
                }
            };

            return Page();
        }
    }

    // Supporting Model
    public class AppointmentItem
    {
        public int Id { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorImage { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string AppointmentDate { get; set; } = string.Empty;
        public string AppointmentTime { get; set; } = string.Empty;
        public string AppointmentType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusColor { get; set; } = "info";
        public string RoomNumber { get; set; } = string.Empty;
        public bool HasPrescription { get; set; }
        public bool HasLabReports { get; set; }
    }
}
