using Microsoft.AspNetCore.Mvc;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Doctor;

public class ProfileModel : BasePageModel
{
    private readonly ILogger<ProfileModel> _logger;

    public ProfileModel(ILogger<ProfileModel> logger)
    {
        _logger = logger;
    }

    public List<QualificationDto> Qualifications { get; set; } = new();

    public IActionResult OnGet()
    {
        // Check if user is authenticated and has Doctor role
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        // Load qualifications (TODO: Replace with actual database query)
        LoadQualifications();

        return Page();
    }

    public async Task<IActionResult> OnPostAddQualificationAsync(
        string qualificationType,
        string degree,
        string institution,
        int yearOfCompletion,
        double? duration,
        string? grade,
        string? country,
        string? fieldOfStudy,
        string? description,
        IFormFile? certificateFile,
        bool isVerified)
    {
        try
        {
            // Get current doctor ID (mock - replace with actual user context)
            var doctorId = 1; // TODO: Get from authentication context

            // Handle file upload
            string? certificatePath = null;
            if (certificateFile != null && certificateFile.Length > 0)
            {
                // TODO: Implement file upload logic
                // Save file to storage and get path
                certificatePath = $"uploads/certificates/{certificateFile.FileName}";
                _logger.LogInformation("Certificate uploaded: {FileName}", certificateFile.FileName);
            }

            // TODO: Save to database
            /* Example database insert:
            var qualification = new T_DoctorQualifications
            {
                DoctorID = doctorId,
                QualificationType = qualificationType,
                Degree = degree,
                Institution = institution,
                YearOfCompletion = yearOfCompletion,
                Duration = duration,
                Grade = grade,
                Country = country,
                FieldOfStudy = fieldOfStudy,
                Description = description,
                CertificatePath = certificatePath,
                IsVerified = isVerified,
                CreatedOn = DateTime.Now
            };
            
            await _context.T_DoctorQualifications.AddAsync(qualification);
            await _context.SaveChangesAsync();
            */

            _logger.LogInformation("Qualification added successfully for doctor {DoctorId}: {Degree} from {Institution}",
                doctorId, degree, institution);

            TempData["Success"] = "Qualification added successfully!";
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding qualification");
            TempData["Error"] = "Failed to add qualification. Please try again.";
            return RedirectToPage();
        }
    }

    public async Task<IActionResult> OnPostUpdateQualificationAsync(
        int qualificationId,
        string qualificationType,
        string degree,
        string institution,
        int yearOfCompletion,
        double? duration,
        string? grade,
        string? country,
        string? fieldOfStudy,
        string? description,
        IFormFile? certificateFile,
        bool isVerified)
    {
        try
        {
            // Handle file upload if new file provided
            string? certificatePath = null;
            if (certificateFile != null && certificateFile.Length > 0)
            {
                // TODO: Implement file upload logic
                certificatePath = $"uploads/certificates/{certificateFile.FileName}";
                _logger.LogInformation("New certificate uploaded: {FileName}", certificateFile.FileName);
            }

            // TODO: Update database
            /* Example database update:
            var qualification = await _context.T_DoctorQualifications.FindAsync(qualificationId);
            if (qualification != null)
            {
                qualification.QualificationType = qualificationType;
                qualification.Degree = degree;
                qualification.Institution = institution;
                qualification.YearOfCompletion = yearOfCompletion;
                qualification.Duration = duration;
                qualification.Grade = grade;
                qualification.Country = country;
                qualification.FieldOfStudy = fieldOfStudy;
                qualification.Description = description;
                qualification.IsVerified = isVerified;
                
                if (certificatePath != null)
                {
                    qualification.CertificatePath = certificatePath;
                }
                
                qualification.UpdatedOn = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            */

            _logger.LogInformation("Qualification updated successfully: {QualificationId}", qualificationId);

            TempData["Success"] = "Qualification updated successfully!";
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating qualification {QualificationId}", qualificationId);
            TempData["Error"] = "Failed to update qualification. Please try again.";
            return RedirectToPage();
        }
    }

    public async Task<IActionResult> OnPostDeleteQualificationAsync(int qualificationId)
    {
        try
        {
            // TODO: Soft delete from database
            /* Example database delete:
            var qualification = await _context.T_DoctorQualifications.FindAsync(qualificationId);
            if (qualification != null)
            {
                qualification.IsDeleted = true;
                qualification.UpdatedOn = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            */

            _logger.LogInformation("Qualification deleted successfully: {QualificationId}", qualificationId);

            TempData["Success"] = "Qualification deleted successfully!";
            return new JsonResult(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting qualification {QualificationId}", qualificationId);
            return new JsonResult(new { success = false, message = "Failed to delete qualification" });
        }
    }

    private void LoadQualifications()
    {
        // TODO: Replace with actual database query
        // SELECT * FROM T_DoctorQualifications WHERE DoctorID = @doctorId AND IsDeleted = false
        
        Qualifications = new List<QualificationDto>
        {
            new QualificationDto
            {
                QualificationID = 1,
                QualificationType = "Degree",
                Degree = "MBBS",
                Institution = "Harvard Medical School",
                YearOfCompletion = 2010,
                Duration = 5,
                Grade = "First Class Honors",
                Country = "United States",
                FieldOfStudy = "Medicine",
                CertificatePath = "cert1.pdf",
                IsVerified = true
            },
            new QualificationDto
            {
                QualificationID = 2,
                QualificationType = "Specialization",
                Degree = "MD Cardiology",
                Institution = "Johns Hopkins University",
                YearOfCompletion = 2013,
                Duration = 3,
                Grade = "Distinction",
                Country = "United States",
                FieldOfStudy = "Cardiology",
                CertificatePath = "cert2.pdf",
                IsVerified = true
            },
            new QualificationDto
            {
                QualificationID = 3,
                QualificationType = "Board Certification",
                Degree = "Board Certification",
                Institution = "American Board of Cardiology",
                YearOfCompletion = 2014,
                Country = "United States",
                FieldOfStudy = "Cardiology",
                CertificatePath = "cert3.pdf",
                IsVerified = true
            }
        };
    }
}

// DTO for Qualification
public class QualificationDto
{
    public int QualificationID { get; set; }
    public string QualificationType { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public int YearOfCompletion { get; set; }
    public double? Duration { get; set; }
    public string? Grade { get; set; }
    public string? Country { get; set; }
    public string? FieldOfStudy { get; set; }
    public string? Description { get; set; }
    public string? CertificatePath { get; set; }
    public bool IsVerified { get; set; }
}
