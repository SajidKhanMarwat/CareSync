using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Patient
{
    public class DocumentsModel : BasePageModel
    {
        // Statistics
        public int TotalDocuments { get; set; } = 0;
        public int MedicalReports { get; set; } = 0;
        public int ImagesScans { get; set; } = 0;
        public int UploadedFiles { get; set; } = 0;

        // Document Lists
        public List<DocumentItem> Documents { get; set; } = new();
        public List<DocumentCategoryItem> Categories { get; set; } = new();

        public void OnGet()
        {
            // Check authorization
            var authResult = RequireRole("Patient");
            if (authResult != null) return;

            LoadStatistics();
            LoadDocuments();
            LoadCategories();
        }

        private void LoadStatistics()
        {
            TotalDocuments = 15;
            MedicalReports = 8;
            ImagesScans = 5;
            UploadedFiles = 2;
        }

        private void LoadDocuments()
        {
            Documents = new List<DocumentItem>
            {
                new DocumentItem
                {
                    Id = 1,
                    Name = "Blood Test Report - CBC",
                    Category = "Lab Reports",
                    Type = "PDF",
                    Size = "245 KB",
                    Date = "Nov 10, 2024",
                    DoctorName = "Dr. Sarah Johnson",
                    AppointmentId = 101,
                    FilePath = "/documents/blood-test-cbc.pdf",
                    Icon = "ri-file-pdf-line",
                    IconColor = "danger"
                },
                new DocumentItem
                {
                    Id = 2,
                    Name = "Chest X-Ray",
                    Category = "Imaging & Scans",
                    Type = "IMAGE",
                    Size = "1.2 MB",
                    Date = "Nov 8, 2024",
                    DoctorName = "Dr. Michael Brown",
                    AppointmentId = 102,
                    FilePath = "/documents/chest-xray.jpg",
                    Icon = "ri-image-line",
                    IconColor = "info"
                },
                new DocumentItem
                {
                    Id = 3,
                    Name = "Prescription - Metformin",
                    Category = "Prescriptions",
                    Type = "PDF",
                    Size = "128 KB",
                    Date = "Oct 15, 2024",
                    DoctorName = "Dr. Sarah Johnson",
                    AppointmentId = 103,
                    FilePath = "/documents/prescription-metformin.pdf",
                    Icon = "ri-file-text-line",
                    IconColor = "success"
                },
                new DocumentItem
                {
                    Id = 4,
                    Name = "Discharge Summary",
                    Category = "Hospital Records",
                    Type = "PDF",
                    Size = "456 KB",
                    Date = "Sep 20, 2024",
                    DoctorName = "Dr. Lisa Anderson",
                    AppointmentId = 104,
                    FilePath = "/documents/discharge-summary.pdf",
                    Icon = "ri-hospital-line",
                    IconColor = "primary"
                },
                new DocumentItem
                {
                    Id = 5,
                    Name = "Vaccination Certificate - COVID-19",
                    Category = "Vaccination Records",
                    Type = "PDF",
                    Size = "189 KB",
                    Date = "Aug 5, 2024",
                    DoctorName = "City Health Center",
                    AppointmentId = 0,
                    FilePath = "/documents/covid-vaccine.pdf",
                    Icon = "ri-syringe-line",
                    IconColor = "warning"
                },
                new DocumentItem
                {
                    Id = 6,
                    Name = "Insurance Card",
                    Category = "Insurance Documents",
                    Type = "PDF",
                    Size = "98 KB",
                    Date = "Jan 1, 2024",
                    DoctorName = "",
                    AppointmentId = 0,
                    FilePath = "/documents/insurance-card.pdf",
                    Icon = "ri-shield-check-line",
                    IconColor = "success"
                }
            };
        }

        private void LoadCategories()
        {
            Categories = new List<DocumentCategoryItem>
            {
                new DocumentCategoryItem { Name = "Lab Reports", Icon = "ri-test-tube-line", Color = "danger", Count = 8 },
                new DocumentCategoryItem { Name = "Imaging & Scans", Icon = "ri-scan-line", Color = "info", Count = 5 },
                new DocumentCategoryItem { Name = "Prescriptions", Icon = "ri-medicine-bottle-line", Color = "success", Count = 6 },
                new DocumentCategoryItem { Name = "Hospital Records", Icon = "ri-hospital-line", Color = "primary", Count = 3 },
                new DocumentCategoryItem { Name = "Vaccination Records", Icon = "ri-syringe-line", Color = "warning", Count = 4 },
                new DocumentCategoryItem { Name = "Insurance Documents", Icon = "ri-shield-check-line", Color = "success", Count = 2 }
            };
        }
    }

    public class DocumentItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public string Type { get; set; } = "";
        public string Size { get; set; } = "";
        public string Date { get; set; } = "";
        public string DoctorName { get; set; } = "";
        public int AppointmentId { get; set; }
        public string FilePath { get; set; } = "";
        public string Icon { get; set; } = "";
        public string IconColor { get; set; } = "";
    }

    public class DocumentCategoryItem
    {
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Color { get; set; } = "";
        public int Count { get; set; }
    }
}
