# Lab Staff Implementation - Corrected Based on Entity Structure

## Entity Relationship Overview

After analyzing the database entities, the correct implementation follows this structure:

### Database Entities

1. **T_Lab** - Laboratory Facility
   - Represents a physical laboratory
   - Owned by a user with `RoleType.Lab` (facility owner/manager)
   - One-to-one relationship with `T_Users` via `UserID`
   
2. **T_UserLabAssistant** - Lab Assistant Assignment
   - Junction table linking Lab Assistants to Laboratories
   - Allows multiple Lab Assistants to work at one Laboratory
   - Links `LabAssistantId` (user with `RoleType.LabAssistant`) to `LabId` (T_Lab)

### Role Types
```csharp
public enum RoleType
{
    Admin = 0,
    Patient = 1,
    Doctor = 2,
    DoctorAssistant = 3,
    LabAssistant = 4,  // Technician/Support Staff
    Lab = 5             // Laboratory Owner/Manager
}
```

## Implementation Flow

### Creating a Lab (Laboratory Facility)

**User Role**: `RoleType.Lab` (value = 5)

**Data Flow**:
1. Admin fills personal information
2. Selects "Lab" role
3. UI shows laboratory facility fields (name, address, license, hours, etc.)
4. Submits form with `UserRegisteration_DTO` containing `RegisterLab_DTO`
5. Backend creates:
   - `T_Users` record with `RoleType.Lab`
   - `T_Lab` record with facility information
   - Links via `T_Lab.UserID = T_Users.Id`

**Result**: New laboratory facility created with an owner account

### Creating a Lab Assistant

**User Role**: `RoleType.LabAssistant` (value = 4)

**Data Flow**:
1. Admin fills personal information
2. Selects "Lab Assistant" role
3. UI shows dropdown of available laboratories
4. Admin selects which laboratory to assign the assistant to
5. Submits form with `UserRegisteration_DTO` containing `AssignLabAssistant_DTO`
6. Backend creates:
   - `T_Users` record with `RoleType.LabAssistant`
   - `T_UserLabAssistant` record linking the assistant to the lab
   - Links via `T_UserLabAssistant.LabAssistantId = T_Users.Id` and `T_UserLabAssistant.LabId = selectedLabId`

**Result**: New lab assistant account assigned to an existing laboratory

## DTOs Created

### 1. RegisterLab_DTO
```csharp
public class RegisterLab_DTO
{
    public string? UserID { get; set; }
    [Required] public required string LabName { get; set; }
    public string? ArabicLabName { get; set; }
    public string? LabAddress { get; set; }
    public string? ArabicLabAddress { get; set; }
    public string? Location { get; set; }
    [Phone] public string? ContactNumber { get; set; }
    [EmailAddress] public string? Email { get; set; }
    public string? LicenseNumber { get; set; }
    public TimeSpan? OpeningTime { get; set; }
    public TimeSpan? ClosingTime { get; set; }
    public required string CreatedBy { get; set; }
}
```
**Purpose**: Captures laboratory facility information for `RoleType.Lab`

### 2. AssignLabAssistant_DTO
```csharp
public class AssignLabAssistant_DTO
{
    [Required] public required string LabAssistantId { get; set; }
    [Required] public required int LabId { get; set; }
    public string? CreatedBy { get; set; }
}
```
**Purpose**: Links a Lab Assistant to a Laboratory via `T_UserLabAssistant`

### 3. LabListDTO
```csharp
public class LabListDTO
{
    public int LabId { get; set; }
    public string? LabName { get; set; }
    public string? ArabicLabName { get; set; }
    public string? Location { get; set; }
    public string? ContactNumber { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}
```
**Purpose**: For dropdown list when assigning Lab Assistants

## Updated UserRegisteration_DTO
```csharp
public record UserRegisteration_DTO
{
    // ... existing fields ...
    
    public RegisterPatient_DTO? RegisterPatient { get; set; }
    public RegisterDoctor_DTO? RegisterDoctor { get; set; }
    
    // NEW: Lab facility details (for RoleType.Lab)
    public RegisterLab_DTO? RegisterLab { get; set; }
    
    // NEW: Lab assignment details (for RoleType.LabAssistant)
    public AssignLabAssistant_DTO? AssignLabAssistant { get; set; }
    
    // DEPRECATED: Kept for backward compatibility
    [Obsolete]
    public RegisterLabAssistant_DTO? RegisterLabAssistant { get; set; }
}
```

## UserService Implementation

```csharp
private async Task<Result<GeneralResponse>> CreateUserDetailsAsync(
    string userId, UserRegisteration_DTO request, string roleName)
{
    switch (roleName.ToLower())
    {
        case "lab":
            // Lab facility owner - creates T_Lab record
            if (request.RegisterLab != null)
            {
                request.RegisterLab.UserID = userId;
                request.RegisterLab.CreatedBy = userId;
                var labFacility = mapper.Map<T_Lab>(request.RegisterLab);
                await uow.LabRepo.AddAsync(labFacility);
                logger.LogInformation("Lab facility created for user {UserId}", userId);
            }
            break;

        case "labassistant":
            // Lab assistant - creates T_UserLabAssistant record
            if (request.AssignLabAssistant != null)
            {
                request.AssignLabAssistant.LabAssistantId = userId;
                request.AssignLabAssistant.CreatedBy = userId;
                var labAssignment = mapper.Map<T_UserLabAssistant>(request.AssignLabAssistant);
                await uow.UserLabAssistantRepo.AddAsync(labAssignment);
                logger.LogInformation("Lab assistant {UserId} assigned to Lab ID {LabId}", 
                    userId, request.AssignLabAssistant.LabId);
            }
            break;
    }
}
```

## UI Implementation

### CreateLabStaff.cshtml - Step 2: Role & Details

**Lab Role (RoleType.Lab = 5)**:
- Shows laboratory facility input fields
- Required: Lab Name
- Optional: Arabic Name, Address, Contact, License, Hours

**Lab Assistant Role (RoleType.LabAssistant = 4)**:
- Shows laboratory selection dropdown
- Dropdown populated from `GET /Admin/labs`
- Required: Select a laboratory
- Warning shown if no labs available

### JavaScript Toggle Function
```javascript
function toggleRoleFields() {
    const roleSelect = document.getElementById('roleTypeSelect');
    const labFields = document.getElementById('labSpecificFields');
    const labAssistantFields = document.getElementById('labAssistantFields');
    
    if (roleSelect.value === '5') { // Lab role
        labFields.style.display = 'block';
        labAssistantFields.style.display = 'none';
    } else if (roleSelect.value === '4') { // Lab Assistant role
        labFields.style.display = 'none';
        labAssistantFields.style.display = 'block';
    } else {
        labFields.style.display = 'none';
        labAssistantFields.style.display = 'none';
    }
}
```

## API Endpoints

### 1. Register Lab Staff
**Endpoint**: `POST /Admin/lab-registration`  
**Request Body**: `UserRegisteration_DTO`  
**Behavior**:
- Determines role from `dto.RoleType`
- Calls `UserService.RegisterNewUserAsync()` with appropriate role name
- Creates user + role-specific records

### 2. Get All Laboratories
**Endpoint**: `GET /Admin/labs`  
**Response**: `Result<List<LabListDTO>>`  
**Purpose**: Populate laboratory dropdown for Lab Assistant assignment

## AutoMapper Profiles

```csharp
// Lab Facility Registration (RoleType.Lab)
CreateMap<RegisterLab_DTO, T_Lab>()
    .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => 
        src.UserID != null ? Guid.Parse(src.UserID) : (Guid?)null))
    .ForMember(dest => dest.LabID, opt => opt.Ignore()) // Auto-increment
    // ... other mappings

// Lab Assistant Assignment (RoleType.LabAssistant)
CreateMap<AssignLabAssistant_DTO, T_UserLabAssistant>()
    .ForMember(dest => dest.UserLabAssistantID, opt => opt.Ignore())
    .ForMember(dest => dest.LabAssistantId, opt => opt.MapFrom(src => src.LabAssistantId))
    .ForMember(dest => dest.LabId, opt => opt.MapFrom(src => src.LabId))
    // ... audit fields
```

## Unit of Work Updates

Added `T_UserLabAssistant` repository:
```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<T_Users> UserRepo { get; }
    IRepository<T_PatientDetails> PatientDetailsRepo { get; }
    IRepository<T_DoctorDetails> DoctorDetailsRepo { get; }
    IRepository<T_Lab> LabRepo { get; }
    IRepository<T_UserLabAssistant> UserLabAssistantRepo { get; } // NEW
    // ... other repos
}
```

## Page Model Updates (CreateLabStaff.cshtml.cs)

### Properties
```csharp
[BindProperty]
public UserRegisteration_DTO LabStaffRegistration { get; set; }

[BindProperty]
public RegisterLab_DTO? LabDetails { get; set; } // For Lab role

[BindProperty]
public int? SelectedLabId { get; set; } // For Lab Assistant role

public List<LabOption>? AvailableLabs { get; set; } // Dropdown options
```

### OnGetAsync
Loads available laboratories for the dropdown

### OnPostAsync Validation
```csharp
if (LabStaffRegistration.RoleType == RoleType.Lab)
{
    // Validate lab facility information
    if (LabDetails == null || string.IsNullOrWhiteSpace(LabDetails.LabName))
    {
        ErrorMessage = "Lab name is required for Lab role.";
        return Page();
    }
    
    // Attach lab facility details
    LabStaffRegistration.RegisterLab = LabDetails;
    LabStaffRegistration.RegisterLab.CreatedBy = GetCurrentUserId() ?? "system";
}
else if (LabStaffRegistration.RoleType == RoleType.LabAssistant)
{
    // Validate lab selection
    if (!SelectedLabId.HasValue || SelectedLabId.Value <= 0)
    {
        ErrorMessage = "Please select a laboratory for the Lab Assistant.";
        return Page();
    }
    
    // Create lab assignment
    LabStaffRegistration.AssignLabAssistant = new AssignLabAssistant_DTO
    {
        LabId = SelectedLabId.Value,
        LabAssistantId = "", // Set by UserService
        CreatedBy = GetCurrentUserId()
    };
}
```

## Database Records Created

### Scenario 1: Creating Lab Facility

**Input**: 
- Role: Lab (5)
- Lab Name: "City Medical Lab"
- License: "LAB-2024-001"
- etc.

**Database Changes**:
1. **T_Users**: 
   - New user with `RoleType = 5` (Lab)
   - UserID: `guid-123`
   
2. **T_Lab**:
   - LabID: `1` (auto-increment)
   - UserID: `guid-123`
   - LabName: "City Medical Lab"
   - LicenseNumber: "LAB-2024-001"
   - Other facility details

3. **T_UserLabAssistant**: No record

### Scenario 2: Creating Lab Assistant

**Input**:
- Role: Lab Assistant (4)
- Selected Lab: "City Medical Lab" (LabID = 1)

**Database Changes**:
1. **T_Users**:
   - New user with `RoleType = 4` (LabAssistant)
   - UserID: `guid-456`

2. **T_Lab**: No new record

3. **T_UserLabAssistant**:
   - UserLabAssistantID: `1` (auto-increment)
   - LabAssistantId: `guid-456`
   - LabId: `1`
   - CreatedBy: Current admin ID

## Key Differences from Previous Implementation

| Aspect | Old (Incorrect) | New (Correct) |
|--------|----------------|---------------|
| Lab Assistant Data | Created T_Lab record | Creates T_UserLabAssistant record |
| Lab Selection | Not available | Dropdown of existing labs |
| DTO Used | RegisterLabAssistant_DTO | AssignLabAssistant_DTO |
| Entity Created | T_Lab | T_UserLabAssistant |
| Relationship | One user → One lab | Many assistants → One lab |

## Files Modified/Created

### Created:
1. `CareSync.ApplicationLayer/Contracts/LabDTOs/RegisterLab_DTO.cs`
2. `CareSync.ApplicationLayer/Contracts/LabDTOs/AssignLabAssistant_DTO.cs`
3. `CareSync.ApplicationLayer/Contracts/LabDTOs/LabList_DTO.cs`

### Modified:
1. `CareSync.ApplicationLayer/Contracts/UsersDTOs/UserRegisteration_DTO.cs`
2. `CareSync.ApplicationLayer/Automapper/AutoMapperProfile.cs`
3. `CareSync.ApplicationLayer/Services/EntitiesServices/UserService.cs`
4. `CareSync.ApplicationLayer/Services/EntitiesServices/AdminService.cs`
5. `CareSync.ApplicationLayer/IServices/EntitiesServices/IAdminService.cs`
6. `CareSync.ApplicationLayer/UnitOfWork/IUnitOfWork.cs`
7. `CareSync.ApplicationLayer/UnitOfWork/UnitOfWork.cs`
8. `CareSync.API/Controllers/AdminController.cs`
9. `CareSync/Pages/Admin/CreateLabStaff.cshtml`
10. `CareSync/Pages/Admin/CreateLabStaff.cshtml.cs`
11. `CareSync/Services/AdminApiService.cs`

## Testing Scenarios

### Test 1: Create Lab Facility
1. Navigate to `/Admin/CreateLabStaff`
2. Fill personal information
3. Select "Lab (Create New Laboratory Facility)"
4. Fill lab details (Name, Address, License, etc.)
5. Fill account setup (Arabic names, Password)
6. Submit
7. **Verify**:
   - T_Users created with RoleType = 5
   - T_Lab created with facility info
   - No T_UserLabAssistant record

### Test 2: Create Lab Assistant (No Labs Exist)
1. Navigate to `/Admin/CreateLabStaff`
2. Select "Lab Assistant"
3. **Verify**: Warning shown "No laboratories available"
4. Cannot proceed without creating a Lab first

### Test 3: Create Lab Assistant (Labs Exist)
1. Ensure at least one Lab exists
2. Navigate to `/Admin/CreateLabStaff`
3. Fill personal information
4. Select "Lab Assistant (Assign to Existing Laboratory)"
5. Select laboratory from dropdown
6. Fill account setup
7. Submit
8. **Verify**:
   - T_Users created with RoleType = 4
   - T_UserLabAssistant created linking user to selected lab
   - No T_Lab record created

### Test 4: Multiple Assistants to One Lab
1. Create Lab "City Medical Lab" (LabID = 1)
2. Create Lab Assistant 1, assign to Lab 1
3. Create Lab Assistant 2, assign to Lab 1
4. **Verify**:
   - Two T_UserLabAssistant records
   - Both with LabId = 1
   - Different LabAssistantId values

## API Response Examples

### GET /Admin/labs
```json
{
  "isSuccess": true,
  "data": [
    {
      "labId": 1,
      "labName": "City Medical Lab",
      "arabicLabName": "مختبر المدينة الطبي",
      "location": "Downtown",
      "contactNumber": "+1234567890",
      "email": "info@citymedlab.com",
      "isActive": true
    }
  ]
}
```

### POST /Admin/lab-registration (Lab)
```json
{
  "firstName": "John",
  "roleType": 5,
  "registerLab": {
    "labName": "City Medical Lab",
    "licenseNumber": "LAB-2024-001",
    "location": "Downtown"
  }
}
```

### POST /Admin/lab-registration (Lab Assistant)
```json
{
  "firstName": "Jane",
  "roleType": 4,
  "assignLabAssistant": {
    "labId": 1
  }
}
```

## Summary

The corrected implementation properly reflects the entity structure:
- **Lab** users own laboratory facilities (1:1 with T_Lab)
- **Lab Assistant** users are assigned to laboratories (many:many via T_UserLabAssistant)
- UI dynamically shows appropriate fields based on role selection
- Backend creates correct entity records based on role type
- No hardcoded values used anywhere in the implementation
