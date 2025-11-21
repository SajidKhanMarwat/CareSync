using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Patient;

public class LabResultsModel : BasePageModel
{
    // Statistics
    public int CompletedTests { get; set; } = 12;
    public int PendingResults { get; set; } = 2;
    public int AbnormalResults { get; set; } = 1;
    public int ThisMonth { get; set; } = 3;

    // Lab Results Lists
    public List<LabResultItem> RecentResults { get; set; } = new();
    public List<PendingTestItem> PendingTests { get; set; } = new();
    public List<TestCategoryItem> TestCategories { get; set; } = new();

    public void OnGet()
    {
        // Check authorization
        var authResult = RequireRole("Patient");
        if (authResult != null) return;

        LoadRecentResults();
        LoadPendingTests();
        LoadTestCategories();
    }

    private void LoadRecentResults()
    {
        RecentResults = new List<LabResultItem>
        {
            new LabResultItem
            {
                Id = 1,
                TestName = "Complete Blood Count (CBC)",
                Description = "Comprehensive blood analysis including RBC, WBC, platelets",
                Date = "November 10, 2024",
                LabName = "City Lab Center",
                Status = "Normal",
                StatusColor = "success",
                Icon = "ri-heart-pulse-line",
                IconColor = "success",
                AppointmentId = 101
            },
            new LabResultItem
            {
                Id = 2,
                TestName = "Lipid Profile",
                Description = "Cholesterol, HDL, LDL, and triglycerides analysis",
                Date = "November 8, 2024",
                LabName = "Metro Diagnostics",
                Status = "Borderline",
                StatusColor = "warning",
                Icon = "ri-drop-line",
                IconColor = "warning",
                AlertMessage = "LDL Cholesterol slightly elevated (145 mg/dL). Normal: <130 mg/dL",
                AppointmentId = 102
            },
            new LabResultItem
            {
                Id = 3,
                TestName = "HbA1c (Diabetes Monitoring)",
                Description = "3-month average blood sugar levels",
                Date = "November 5, 2024",
                LabName = "City Lab Center",
                Status = "Good Control",
                StatusColor = "success",
                Icon = "ri-medicine-bottle-line",
                IconColor = "info",
                AlertMessage = "HbA1c: 6.8% - Good diabetic control. Target: <7%",
                AppointmentId = 103
            },
            new LabResultItem
            {
                Id = 4,
                TestName = "Urine Analysis",
                Description = "Complete urine examination for infections and abnormalities",
                Date = "October 28, 2024",
                LabName = "Metro Diagnostics",
                Status = "Normal",
                StatusColor = "success",
                Icon = "ri-microscope-line",
                IconColor = "primary",
                AppointmentId = 104
            }
        };
    }

    private void LoadPendingTests()
    {
        PendingTests = new List<PendingTestItem>
        {
            new PendingTestItem
            {
                Id = 1,
                TestName = "Thyroid Function Test",
                DoctorName = "Dr. Sarah Johnson",
                ExpectedDate = "November 15, 2024",
                Status = "Processing",
                StatusColor = "warning",
                Progress = 75,
                StatusMessage = "Sample collected - Results pending"
            },
            new PendingTestItem
            {
                Id = 2,
                TestName = "Vitamin D Level",
                DoctorName = "Dr. Michael Brown",
                ExpectedDate = "November 18, 2024",
                Status = "Scheduled",
                StatusColor = "info",
                Progress = 25,
                StatusMessage = "Sample collection pending"
            }
        };
    }

    private void LoadTestCategories()
    {
        TestCategories = new List<TestCategoryItem>
        {
            new TestCategoryItem { Name = "Blood Tests", Icon = "ri-heart-pulse-line", Color = "danger", Count = 8 },
            new TestCategoryItem { Name = "Urine Tests", Icon = "ri-drop-line", Color = "warning", Count = 3 },
            new TestCategoryItem { Name = "Imaging", Icon = "ri-scan-line", Color = "info", Count = 2 },
            new TestCategoryItem { Name = "Microbiology", Icon = "ri-microscope-line", Color = "success", Count = 1 }
        };
    }
}

public class LabResultItem
{
    public int Id { get; set; }
    public string TestName { get; set; } = "";
    public string Description { get; set; } = "";
    public string Date { get; set; } = "";
    public string LabName { get; set; } = "";
    public string Status { get; set; } = "";
    public string StatusColor { get; set; } = "";
    public string Icon { get; set; } = "";
    public string IconColor { get; set; } = "";
    public string AlertMessage { get; set; } = "";
    public int AppointmentId { get; set; }
}

public class PendingTestItem
{
    public int Id { get; set; }
    public string TestName { get; set; } = "";
    public string DoctorName { get; set; } = "";
    public string ExpectedDate { get; set; } = "";
    public string Status { get; set; } = "";
    public string StatusColor { get; set; } = "";
    public int Progress { get; set; }
    public string StatusMessage { get; set; } = "";
}

public class TestCategoryItem
{
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Color { get; set; } = "";
    public int Count { get; set; }
}
