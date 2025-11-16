using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Lab
{
    public class ProfileModel : PageModel
    {
        // Lab Basic Information
        public string LabName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string AccreditationNumber { get; set; } = string.Empty;
        public string LabType { get; set; } = string.Empty;
        public string EstablishedYear { get; set; } = string.Empty;
        
        // Contact Information
        public string ContactNumber { get; set; } = string.Empty;
        public string AlternateNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        
        // Operating Hours
        public string OpeningTime { get; set; } = string.Empty;
        public string ClosingTime { get; set; } = string.Empty;
        public string WorkingDays { get; set; } = string.Empty;
        public bool EmergencyServices { get; set; }
        
        // Statistics
        public int TotalServices { get; set; }
        public int CompletedTestsToday { get; set; }
        public int PendingReports { get; set; }
        public int ActiveRequests { get; set; }
        public int TotalStaff { get; set; }
        public int TestsThisMonth { get; set; }
        public decimal MonthlyRevenue { get; set; }
        
        // Staff Information
        public List<StaffMember> StaffMembers { get; set; } = new();
        
        // Services
        public List<string> SpecializedServices { get; set; } = new();
        
        // Certifications
        public List<Certification> Certifications { get; set; } = new();

        public void OnGet()
        {
            // Mock data for Lab Profile
            LabName = "CareSync Diagnostic Laboratory";
            LicenseNumber = "LAB-2024-00567";
            AccreditationNumber = "NABL-ACC-12345";
            LabType = "Multi-Specialty Diagnostic Lab";
            EstablishedYear = "2018";
            
            ContactNumber = "+1 (555) 123-4567";
            AlternateNumber = "+1 (555) 123-4568";
            Email = "info@caresynclab.com";
            Website = "www.caresynclab.com";
            Address = "123 Medical Plaza, Building A, 3rd Floor";
            City = "San Francisco";
            State = "California";
            ZipCode = "94102";
            
            OpeningTime = "07:00 AM";
            ClosingTime = "10:00 PM";
            WorkingDays = "Monday - Sunday";
            EmergencyServices = true;
            
            TotalServices = 59;
            CompletedTestsToday = 47;
            PendingReports = 12;
            ActiveRequests = 23;
            TotalStaff = 18;
            TestsThisMonth = 1245;
            MonthlyRevenue = 125000;
            
            StaffMembers = new List<StaffMember>
            {
                new StaffMember { Name = "Dr. Sarah Johnson", Role = "Lab Director", Specialization = "Clinical Pathology", Status = "Active" },
                new StaffMember { Name = "Dr. Michael Chen", Role = "Senior Pathologist", Specialization = "Hematology", Status = "Active" },
                new StaffMember { Name = "Emily Rodriguez", Role = "Lab Technician", Specialization = "Clinical Chemistry", Status = "Active" },
                new StaffMember { Name = "David Kumar", Role = "Lab Technician", Specialization = "Microbiology", Status = "Active" },
                new StaffMember { Name = "Lisa Anderson", Role = "Phlebotomist", Specialization = "Sample Collection", Status = "On Leave" }
            };
            
            SpecializedServices = new List<string>
            {
                "Complete Blood Count (CBC)",
                "Molecular Diagnostics (PCR)",
                "Immunology & Serology",
                "Clinical Chemistry",
                "Microbiology & Culture",
                "Histopathology",
                "Cytopathology",
                "Genetic Testing"
            };
            
            Certifications = new List<Certification>
            {
                new Certification { Name = "NABL Accreditation", IssuedBy = "National Accreditation Board", ValidUntil = "Dec 2025", Status = "Active" },
                new Certification { Name = "ISO 15189:2012", IssuedBy = "International Organization for Standardization", ValidUntil = "Mar 2026", Status = "Active" },
                new Certification { Name = "CAP Certification", IssuedBy = "College of American Pathologists", ValidUntil = "Jun 2025", Status = "Active" },
                new Certification { Name = "CLIA License", IssuedBy = "Clinical Laboratory Improvement Amendments", ValidUntil = "Sep 2025", Status = "Active" }
            };
        }
    }
    
    public class StaffMember
    {
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
    
    public class Certification
    {
        public string Name { get; set; } = string.Empty;
        public string IssuedBy { get; set; } = string.Empty;
        public string ValidUntil { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
