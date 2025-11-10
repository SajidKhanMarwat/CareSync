using AutoMapper;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.DataLayer.Entities;
using CareSync.DataLayer.DataEnums;

namespace CareSync.ApplicationLayer.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserRegisteration_DTO, T_Users>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString())) // Generate new GUID for ID
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.ArabicUserName, opt => opt.MapFrom(src => src.UserName)) // Default to UserName
            .ForMember(dest => dest.LoginID, opt => opt.MapFrom(src => 0)) // Default value
            .ForMember(dest => dest.RoleID, opt => opt.Ignore()) // Will be set by service
            .ForMember(dest => dest.RoleType, opt => opt.MapFrom(src => RoleType.Patient)) // Default to Patient
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => Gender.Other)) // Default value
            .ForMember(dest => dest.DateOfBirth, opt => opt.Ignore()) // Not provided in registration
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Address, opt => opt.Ignore())
            .ForMember(dest => dest.Age, opt => opt.Ignore())
            .ForMember(dest => dest.ProfileImage, opt => opt.Ignore())
            .ForMember(dest => dest.LastLogin, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "System")) // Default for registration
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled))
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.UserRole, opt => opt.Ignore());
    }
}
