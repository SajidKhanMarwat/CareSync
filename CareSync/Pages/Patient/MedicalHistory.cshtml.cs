using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Patient
{
    public class MedicalHistoryModel : BasePageModel
    {
        // Patient Basic Info
        public string PatientName { get; set; } = "John Doe";
        public string Gender { get; set; } = "Male";
        public int Age { get; set; } = 35;
        public string BloodGroup { get; set; } = "B+";
        public string MRN { get; set; } = "MRN-2024-001";

        // Medical History Summary
        public string MainDiagnosis { get; set; } = "";
        public List<string> ChronicDiseases { get; set; } = new();
        public List<string> Allergies { get; set; } = new();
        public List<string> PastDiseases { get; set; } = new();
        public List<string> Surgeries { get; set; } = new();
        public string FamilyHistory { get; set; } = "";

        // Recent Medical Records
        public List<MedicalRecordItem> RecentRecords { get; set; } = new();
        
        // Statistics
        public int TotalAppointments { get; set; } = 0;
        public int TotalPrescriptions { get; set; } = 0;
        public int TotalLabReports { get; set; } = 0;
        public int TotalDocuments { get; set; } = 0;

        public void OnGet()
        {
            // Check authorization
            var authResult = RequireRole("Patient");
            if (authResult != null) return;

            // Load mock data (replace with actual database queries)
            LoadPatientInfo();
            LoadMedicalHistory();
            LoadRecentRecords();
            LoadStatistics();
        }

        private void LoadPatientInfo()
        {
            PatientName = "John Michael Doe";
            Gender = "Male";
            Age = 35;
            BloodGroup = "B+";
            MRN = "MRN-2024-001234";
        }

        private void LoadMedicalHistory()
        {
            MainDiagnosis = "Type 2 Diabetes Mellitus";
            
            ChronicDiseases = new List<string>
            {
                "Type 2 Diabetes",
                "Hypertension",
                "Mild Asthma"
            };

            Allergies = new List<string>
            {
                "Penicillin",
                "Peanuts",
                "Dust Mites"
            };

            PastDiseases = new List<string>
            {
                "Chickenpox (Childhood)",
                "Pneumonia (2019)",
                "COVID-19 (2021)"
            };

            Surgeries = new List<string>
            {
                "Appendectomy - March 2018",
                "Wisdom Teeth Extraction - June 2020"
            };

            FamilyHistory = "Father: Diabetes Type 2, Hypertension | Mother: Breast Cancer (Survivor) | Siblings: No significant medical history";
        }

        private void LoadRecentRecords()
        {
            RecentRecords = new List<MedicalRecordItem>
            {
                new MedicalRecordItem
                {
                    Id = 1,
                    Date = "Nov 10, 2024",
                    Type = "Consultation",
                    DoctorName = "Dr. Sarah Johnson",
                    DoctorImage = "~/theme/images/doctor1.png",
                    Department = "Cardiology",
                    Diagnosis = "Hypertension Follow-up",
                    HasPrescription = true,
                    HasLabReports = true,
                    StatusColor = "success",
                    Status = "Completed"
                },
                new MedicalRecordItem
                {
                    Id = 2,
                    Date = "Oct 25, 2024",
                    Type = "Lab Test",
                    DoctorName = "Dr. Michael Brown",
                    DoctorImage = "~/theme/images/doctor2.png",
                    Department = "General Medicine",
                    Diagnosis = "Routine Blood Test",
                    HasPrescription = false,
                    HasLabReports = true,
                    StatusColor = "success",
                    Status = "Completed"
                },
                new MedicalRecordItem
                {
                    Id = 3,
                    Date = "Oct 15, 2024",
                    Type = "Follow-up",
                    DoctorName = "Dr. Lisa Anderson",
                    DoctorImage = "~/theme/images/doctor3.png",
                    Department = "Endocrinology",
                    Diagnosis = "Diabetes Management Review",
                    HasPrescription = true,
                    HasLabReports = true,
                    StatusColor = "success",
                    Status = "Completed"
                }
            };
        }

        private void LoadStatistics()
        {
            TotalAppointments = 28;
            TotalPrescriptions = 45;
            TotalLabReports = 32;
            TotalDocuments = 15;
        }
    }

    public class MedicalRecordItem
    {
        public int Id { get; set; }
        public string Date { get; set; } = "";
        public string Type { get; set; } = "";
        public string DoctorName { get; set; } = "";
        public string DoctorImage { get; set; } = "";
        public string Department { get; set; } = "";
        public string Diagnosis { get; set; } = "";
        public bool HasPrescription { get; set; }
        public bool HasLabReports { get; set; }
        public string StatusColor { get; set; } = "";
        public string Status { get; set; } = "";
    }
}
