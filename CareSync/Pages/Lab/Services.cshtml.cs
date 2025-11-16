using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Lab
{
    public class ServicesModel : PageModel
    {
        public List<LabServiceItem> Services { get; set; } = new();
        public int TotalServices { get; set; }
        public int ActiveServices { get; set; }
        public int PendingReview { get; set; }
        public decimal MonthlyRevenue { get; set; }

        public void OnGet()
        {
            // Statistics
            TotalServices = 59;
            ActiveServices = 52;
            PendingReview = 7;
            MonthlyRevenue = 45000;

            // Mock service data
            Services = new List<LabServiceItem>
            {
                new LabServiceItem { Id = 1, Name = "Complete Blood Count (CBC)", Description = "Full blood analysis with differential", Category = "Hematology", SampleType = "Blood", Price = 45.00m, EstimatedTime = "2-4 hours", Status = "Active", StatusColor = "success" },
                new LabServiceItem { Id = 2, Name = "Hemoglobin (Hb)", Description = "Measure of oxygen-carrying capacity", Category = "Hematology", SampleType = "Blood", Price = 20.00m, EstimatedTime = "1-2 hours", Status = "Active", StatusColor = "success" },
                new LabServiceItem { Id = 3, Name = "Blood Typing (ABO & Rh)", Description = "Determine blood group and Rh factor", Category = "Hematology", SampleType = "Blood", Price = 35.00m, EstimatedTime = "1-2 hours", Status = "Active", StatusColor = "success" },
                new LabServiceItem { Id = 4, Name = "Basic Metabolic Panel (BMP)", Description = "Glucose, electrolytes, kidney function", Category = "Clinical Chemistry", SampleType = "Serum", Price = 55.00m, EstimatedTime = "2-4 hours", Status = "Active", StatusColor = "success" },
                new LabServiceItem { Id = 5, Name = "Lipid Profile", Description = "Cholesterol, triglycerides, HDL, LDL", Category = "Clinical Chemistry", SampleType = "Serum", Price = 65.00m, EstimatedTime = "3-5 hours", Status = "Active", StatusColor = "success" },
                new LabServiceItem { Id = 6, Name = "Liver Function Tests (LFT)", Description = "ALT, AST, bilirubin, alkaline phosphatase", Category = "Clinical Chemistry", SampleType = "Serum", Price = 75.00m, EstimatedTime = "3-5 hours", Status = "Active", StatusColor = "success" },
                new LabServiceItem { Id = 7, Name = "Blood Culture", Description = "Bacterial and fungal detection", Category = "Microbiology", SampleType = "Blood", Price = 85.00m, EstimatedTime = "24-48 hours", Status = "Active", StatusColor = "success" },
                new LabServiceItem { Id = 8, Name = "Urine Culture", Description = "UTI detection and sensitivity", Category = "Microbiology", SampleType = "Urine", Price = 65.00m, EstimatedTime = "24-48 hours", Status = "Active", StatusColor = "success" },
                new LabServiceItem { Id = 9, Name = "Thyroid Function Tests (TFT)", Description = "TSH, T3, T4 levels", Category = "Immunology", SampleType = "Serum", Price = 80.00m, EstimatedTime = "4-6 hours", Status = "Active", StatusColor = "success" },
                new LabServiceItem { Id = 10, Name = "HbA1c (Glycated Hemoglobin)", Description = "Long-term glucose control", Category = "Clinical Chemistry", SampleType = "Blood", Price = 50.00m, EstimatedTime = "2-3 hours", Status = "Active", StatusColor = "success" },
                new LabServiceItem { Id = 11, Name = "Vitamin D", Description = "25-hydroxyvitamin D levels", Category = "Immunology", SampleType = "Serum", Price = 70.00m, EstimatedTime = "3-5 hours", Status = "Active", StatusColor = "success" },
                new LabServiceItem { Id = 12, Name = "COVID-19 PCR", Description = "SARS-CoV-2 detection", Category = "Molecular Biology", SampleType = "Nasopharyngeal Swab", Price = 120.00m, EstimatedTime = "6-12 hours", Status = "Active", StatusColor = "success" },
                new LabServiceItem { Id = 13, Name = "Urinalysis", Description = "Complete urine examination", Category = "Clinical Chemistry", SampleType = "Urine", Price = 30.00m, EstimatedTime = "1-2 hours", Status = "Active", StatusColor = "success" },
                new LabServiceItem { Id = 14, Name = "Electrolyte Panel", Description = "Sodium, potassium, chloride", Category = "Clinical Chemistry", SampleType = "Serum", Price = 40.00m, EstimatedTime = "2-3 hours", Status = "Active", StatusColor = "success" },
                new LabServiceItem { Id = 15, Name = "Pregnancy Test (hCG)", Description = "Beta-hCG quantitative", Category = "Immunology", SampleType = "Serum", Price = 35.00m, EstimatedTime = "1-2 hours", Status = "Pending", StatusColor = "warning" }
            };
        }
    }

    public class LabServiceItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string SampleType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string EstimatedTime { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusColor { get; set; } = "info";
    }
}
