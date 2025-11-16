using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Patient
{
    public class ProfileModel : BasePageModel
    {
        // Personal Information
        public string PatientID { get; set; } = "P-2024-001234";
        public string FirstName { get; set; } = "Emily";
        public string LastName { get; set; } = "Johnson";
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = "emily.johnson@email.com";
        public string PhoneNumber { get; set; } = "+1 (555) 123-4567";
        public string DateOfBirth { get; set; } = "1992-03-15";
        public int Age { get; set; } = 32;
        public string Gender { get; set; } = "Female";
        public string Address { get; set; } = "123 Main Street, Apartment 4B, New York, NY 10001";
        public string City { get; set; } = "New York";
        public string State { get; set; } = "NY";
        public string ZipCode { get; set; } = "10001";
        
        // Medical Information
        public string BloodGroup { get; set; } = "A+";
        public string MaritalStatus { get; set; } = "Married";
        public string Occupation { get; set; } = "Software Engineer";
        public decimal? Height { get; set; } = 165; // cm
        public decimal? Weight { get; set; } = 58; // kg
        public decimal BMI { get; set; } = 21.3m;
        public string BMIStatus { get; set; } = "Normal";
        
        // Emergency Contact
        public string EmergencyContactName { get; set; } = "Michael Johnson";
        public string EmergencyContactNumber { get; set; } = "+1 (555) 987-6543";
        public string RelationshipToEmergency { get; set; } = "Spouse";
        
        // Medical Records Statistics
        public int TotalAppointments { get; set; } = 28;
        public int TotalPrescriptions { get; set; } = 45;
        public int TotalLabReports { get; set; } = 32;
        public int TotalDocuments { get; set; } = 15;
        public int ActivePrescriptions { get; set; } = 5;
        public int UpcomingAppointments { get; set; } = 2;
        
        // Account Information
        public string AccountStatus { get; set; } = "Active";
        public string MemberSince { get; set; } = "January 2024";
        public string LastLogin { get; set; } = "Today, 2:30 PM";
        public string LastVisitDate { get; set; } = "November 10, 2024";
        public string ProfileImageUrl { get; set; } = "~/theme/images/patient5.png";
        
        // Recent Medical Information
        public List<string> Allergies { get; set; } = new();
        public List<string> ChronicDiseases { get; set; } = new();
        public List<RecentActivityItem> RecentActivity { get; set; } = new();

        [TempData]
        public string SuccessMessage { get; set; } = "";
        
        [TempData]
        public string ErrorMessage { get; set; } = "";

        public void OnGet()
        {
            // Check authorization
            var authResult = RequireRole("Patient");
            if (authResult != null) return;

            LoadPatientData();
            LoadMedicalInfo();
            LoadRecentActivity();
            CalculateBMI();
        }

        public IActionResult OnPost(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string dateOfBirth,
            string gender,
            string occupation,
            string maritalStatus,
            string address,
            string bloodGroup,
            decimal? height,
            decimal? weight,
            string emergencyContactName,
            string emergencyContactNumber,
            string relationshipToEmergency)
        {
            // Check authorization
            var authResult = RequireRole("Patient");
            if (authResult != null) return authResult;

            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(firstName) || 
                    string.IsNullOrWhiteSpace(lastName) ||
                    string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(phoneNumber) ||
                    string.IsNullOrWhiteSpace(address) ||
                    string.IsNullOrWhiteSpace(bloodGroup) ||
                    string.IsNullOrWhiteSpace(emergencyContactName) ||
                    string.IsNullOrWhiteSpace(emergencyContactNumber))
                {
                    ErrorMessage = "Please fill in all required fields.";
                    return RedirectToPage();
                }

                // Update patient data
                FirstName = firstName;
                LastName = lastName;
                Email = email;
                PhoneNumber = phoneNumber;
                DateOfBirth = dateOfBirth;
                Gender = gender;
                Occupation = occupation ?? "";
                MaritalStatus = maritalStatus ?? "";
                Address = address;
                BloodGroup = bloodGroup;
                Height = height;
                Weight = weight;
                EmergencyContactName = emergencyContactName;
                EmergencyContactNumber = emergencyContactNumber;
                RelationshipToEmergency = relationshipToEmergency;

                // Recalculate age from date of birth
                if (!string.IsNullOrEmpty(dateOfBirth))
                {
                    var dob = DateTime.Parse(dateOfBirth);
                    Age = DateTime.Now.Year - dob.Year;
                    if (DateTime.Now.DayOfYear < dob.DayOfYear)
                        Age--;
                }

                // Recalculate BMI
                CalculateBMI();

                // TODO: Save to database
                // var patient = await _patientService.UpdatePatientAsync(patientId, updatedData);
                
                // For now, we're just updating the in-memory model
                // In production, you would save to database here:
                /*
                var patientDetails = new T_PatientDetails
                {
                    BloodGroup = bloodGroup,
                    MaritalStatus = maritalStatus,
                    Occupation = occupation,
                    EmergencyContactName = emergencyContactName,
                    EmergencyContactNumber = emergencyContactNumber,
                    RelationshipToEmergency = relationshipToEmergency
                };
                
                var user = new T_Users
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    DateOfBirth = DateTime.Parse(dateOfBirth),
                    Gender = gender,
                    Address = address
                };
                
                var vitals = new T_PatientVitals
                {
                    Height = height,
                    Weight = weight
                };
                
                await _context.SaveChangesAsync();
                */

                SuccessMessage = "Profile updated successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating profile: {ex.Message}";
                return RedirectToPage();
            }
        }

        private void LoadPatientData()
        {
            // Mock data - Replace with actual database queries
            PatientID = "P-2024-001234";
            FirstName = "Emily";
            LastName = "Johnson";
            Email = "emily.johnson@email.com";
            PhoneNumber = "+1 (555) 123-4567";
            DateOfBirth = "1992-03-15";
            Age = 32;
            Gender = "Female";
            Address = "123 Main Street, Apartment 4B, New York, NY 10001";
            
            // Medical records statistics
            TotalAppointments = 28;
            TotalPrescriptions = 45;
            TotalLabReports = 32;
            TotalDocuments = 15;
            ActivePrescriptions = 5;
            UpcomingAppointments = 2;
        }

        private void LoadMedicalInfo()
        {
            BloodGroup = "A+";
            MaritalStatus = "Married";
            Occupation = "Software Engineer";
            Height = 165;
            Weight = 58;
            
            EmergencyContactName = "Michael Johnson";
            EmergencyContactNumber = "+1 (555) 987-6543";
            RelationshipToEmergency = "Spouse";
            
            Allergies = new List<string>
            {
                "Penicillin",
                "Shellfish",
                "Pollen"
            };
            
            ChronicDiseases = new List<string>
            {
                "Type 2 Diabetes",
                "Hypertension"
            };
        }

        private void LoadRecentActivity()
        {
            RecentActivity = new List<RecentActivityItem>
            {
                new RecentActivityItem
                {
                    Icon = "ri-calendar-check-line",
                    IconColor = "success",
                    Title = "Appointment Completed",
                    Description = "Consultation with Dr. Sarah Johnson",
                    Date = "2 days ago"
                },
                new RecentActivityItem
                {
                    Icon = "ri-medicine-bottle-line",
                    IconColor = "info",
                    Title = "New Prescription",
                    Description = "Metformin 500mg prescribed",
                    Date = "5 days ago"
                },
                new RecentActivityItem
                {
                    Icon = "ri-test-tube-line",
                    IconColor = "warning",
                    Title = "Lab Results Available",
                    Description = "Blood test results ready",
                    Date = "1 week ago"
                }
            };
        }

        private void CalculateBMI()
        {
            if (Height.HasValue && Weight.HasValue && Height.Value > 0)
            {
                var heightInMeters = Height.Value / 100;
                BMI = Weight.Value / (heightInMeters * heightInMeters);
                
                if (BMI < 18.5m)
                {
                    BMIStatus = "Underweight";
                }
                else if (BMI >= 18.5m && BMI < 25m)
                {
                    BMIStatus = "Normal";
                }
                else if (BMI >= 25m && BMI < 30m)
                {
                    BMIStatus = "Overweight";
                }
                else
                {
                    BMIStatus = "Obese";
                }
            }
        }
    }

    public class RecentActivityItem
    {
        public string Icon { get; set; } = "";
        public string IconColor { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Date { get; set; } = "";
    }
}
