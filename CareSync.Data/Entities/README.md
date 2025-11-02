# CareSync Entity Framework Models

This document provides an overview of all the entities created for the CareSync medical management system.

## Base Entity
- **BaseEntity.cs**: Contains audit fields (IsDeleted, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn)

## Identity Entities (User Entities folder)
- **T_Users.cs**: Main user entity extending IdentityUser<Guid>
- **T_Roles.cs**: Role entity extending IdentityRole<Guid>
- **T_UserClaim.cs**: User claims extending IdentityUserClaim<Guid>
- **T_RoleClaim.cs**: Role claims extending IdentityRoleClaim<Guid>
- **T_UserRole.cs**: User-role mapping extending IdentityUserRole<Guid>
- **T_UserLogin.cs**: External login extending IdentityUserLogin<Guid>
- **T_UserToken.cs**: User tokens extending IdentityUserToken<Guid>
- **T_Rights.cs**: System rights/permissions
- **T_RoleRights.cs**: Role-rights mapping

## Patient Entities (Patient Entities folder)
- **T_PatientDetails.cs**: Main patient information
- **T_AdditionalNotes.cs**: Additional patient notes and recommendations
- **T_ChronicDiseases.cs**: Patient chronic diseases
- **T_LifestyleInfo.cs**: Patient lifestyle information
- **T_MedicalFollowUp.cs**: Medical follow-up records
- **T_MedicalHistory.cs**: Patient medical history
- **T_MedicationPlan.cs**: Patient medication plans
- **T_PatientVitals.cs**: Patient vital signs
- **T_PatientReports.cs**: Patient reports and documents

## Doctor Entities (Doctor Entities folder)
- **T_DoctorDetails.cs**: Doctor profile information
- **T_Qualifications.cs**: Doctor qualifications and certifications

## Appointment Entities (Appointment Entities folder)
- **T_Appointments.cs**: Appointment scheduling and management

## Prescription Entities (Prescription Entities folder)
- **T_Prescriptions.cs**: Prescription headers
- **T_PrescriptionItems.cs**: Individual prescription items/medicines

## Lab Entities (Lab Entities folder)
- **T_Lab.cs**: Laboratory information
- **T_LabServices.cs**: Laboratory services offered
- **T_LabRequests.cs**: Lab test requests
- **T_LabReports.cs**: Lab test reports and results

## Key Features
1. **Audit Trail**: All entities (except T_PatientReports) inherit from BaseEntity with audit fields
2. **Identity Integration**: Full ASP.NET Core Identity integration with Guid primary keys
3. **Navigation Properties**: Proper EF Core navigation properties for relationships
4. **Database Schema Compliance**: Entities match the provided SQL Server database schema
5. **Nullable Fields**: Appropriate nullable fields matching database constraints

## Notes
- All entities use the namespace `CareSync.DataLayer.Entities`
- Primary keys follow the database schema (int for most entities, Guid for Identity entities)
- Foreign key relationships are properly defined with navigation properties
- Default values are set where specified in the database schema
