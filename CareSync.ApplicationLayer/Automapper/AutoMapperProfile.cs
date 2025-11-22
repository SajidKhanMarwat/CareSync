using AutoMapper;
using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.Contracts.LabDTOs;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.DataLayer.Entities;
using CareSync.Shared.Enums;

namespace CareSync.ApplicationLayer.Automapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // User Registration to User Entity
        CreateMap<UserRegisteration_DTO, T_Users>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.ArabicUserName, opt => opt.MapFrom(src => src.ArabicUserName))
            .ForMember(dest => dest.ArabicFirstName, opt => opt.MapFrom(src => src.ArabicFirstName))
            .ForMember(dest => dest.ArabicLastName, opt => opt.MapFrom(src => src.ArabicLastName))
            .ForMember(dest => dest.LoginID, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.RoleID, opt => opt.Ignore()) // Set by service
            .ForMember(dest => dest.RoleType, opt => opt.MapFrom(src => src.RoleType))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.ProfileImage))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.LastLogin, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.Email)) // Use email as CreatedBy initially
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled))
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.UserRole, opt => opt.Ignore());

        // User Update Mapping
        CreateMap<UserUpdate_DTO, T_Users>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Appointment Mapping
        CreateMap<Appointment_DTO, T_Appointments>()
            .ReverseMap();

        // Patient Registration Mappings
        CreateMap<RegisterPatient_DTO, T_PatientDetails>()
            .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserID))
            .ForMember(dest => dest.PatientID, opt => opt.Ignore()) // Auto-incremented
            .ForMember(dest => dest.BloodGroup, opt => opt.MapFrom(src => src.BloodGroup))
            .ForMember(dest => dest.MaritalStatus, opt => opt.MapFrom(src => src.MaritalStatus))
            .ForMember(dest => dest.Occupation, opt => opt.MapFrom(src => src.Occupation))
            .ForMember(dest => dest.EmergencyContactName, opt => opt.MapFrom(src => src.EmergencyContactName))
            .ForMember(dest => dest.EmergencyContactNumber, opt => opt.MapFrom(src => src.EmergencyContactNumber))
            .ForMember(dest => dest.RelationshipToEmergency, opt => opt.MapFrom(src => src.RelationshipToEmergency))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Appointments, opt => opt.Ignore())
            .ForMember(dest => dest.AdditionalNotes, opt => opt.Ignore())
            .ForMember(dest => dest.ChronicDiseases, opt => opt.Ignore())
            .ForMember(dest => dest.LifestyleInfo, opt => opt.Ignore())
            .ForMember(dest => dest.MedicalFollowUps, opt => opt.Ignore())
            .ForMember(dest => dest.MedicalHistories, opt => opt.Ignore())
            .ForMember(dest => dest.MedicationPlans, opt => opt.Ignore())
            .ForMember(dest => dest.PatientVitals, opt => opt.Ignore())
            .ForMember(dest => dest.Prescriptions, opt => opt.Ignore());

        // Doctor Registration Mappings
        CreateMap<RegisterDoctor_DTO, T_DoctorDetails>()
            .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserID))
            .ForMember(dest => dest.DoctorID, opt => opt.Ignore()) // Auto-incremented
            .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization))
            .ForMember(dest => dest.ArabicSpecialization, opt => opt.MapFrom(src => src.ArabicSpecialization))
            .ForMember(dest => dest.ClinicAddress, opt => opt.MapFrom(src => src.ClinicAddress))
            .ForMember(dest => dest.ArabicClinicAddress, opt => opt.MapFrom(src => src.ArabicClinicAddress))
            .ForMember(dest => dest.ExperienceYears, opt => opt.MapFrom(src => src.ExperienceYears))
            .ForMember(dest => dest.LicenseNumber, opt => opt.MapFrom(src => src.LicenseNumber))
            .ForMember(dest => dest.QualificationSummary, opt => opt.MapFrom(src => src.QualificationSummary))
            .ForMember(dest => dest.HospitalAffiliation, opt => opt.MapFrom(src => src.HospitalAffiliation))
            .ForMember(dest => dest.AvailableDays, opt => opt.MapFrom(src => src.AvailableDays))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Appointments, opt => opt.Ignore())
            .ForMember(dest => dest.Prescriptions, opt => opt.Ignore())
            .ForMember(dest => dest.Qualifications, opt => opt.Ignore());

        // Lab Registration Mappings
        CreateMap<RegisterLabAssistant_DTO, T_Lab>()
            .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserID != null ? Guid.Parse(src.UserID) : (Guid?)null))
            .ForMember(dest => dest.LabID, opt => opt.Ignore()) // Auto-incremented
            .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.LabName))
            .ForMember(dest => dest.ArabicLabName, opt => opt.MapFrom(src => src.ArabicLabName))
            .ForMember(dest => dest.LabAddress, opt => opt.MapFrom(src => src.LabAddress))
            .ForMember(dest => dest.ArabicLabAddress, opt => opt.MapFrom(src => src.ArabicLabAddress))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.ContactNumber, opt => opt.MapFrom(src => src.ContactNumber))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.LicenseNumber, opt => opt.MapFrom(src => src.LicenseNumber))
            .ForMember(dest => dest.OpeningTime, opt => opt.MapFrom(src => src.OpeningTime))
            .ForMember(dest => dest.ClosingTime, opt => opt.MapFrom(src => src.ClosingTime))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.LabServices, opt => opt.Ignore());

        // User Profile Mappings (Entity to DTO - for safe API exposure)
        CreateMap<T_Users, UserProfile_DTO>()
            .ForMember(dest => dest.RoleName, opt => opt.Ignore()); // Set manually with role lookup

        CreateMap<T_Users, AdminUser_DTO>()
            .ForMember(dest => dest.RoleName, opt => opt.Ignore()); // Set manually with role lookup
    }
}
