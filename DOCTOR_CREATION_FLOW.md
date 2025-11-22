# Doctor Creation Flow - Complete Implementation

## Overview
Complete end-to-end flow for creating a doctor in the CareSync system via `/Admin/CreateDoctor`.

## Implementation Components

### 1. **Frontend UI** (`CreateDoctor.cshtml`)
- **Multi-step form** with 4 steps:
  - Step 1: Personal Information (Name, Email, Phone, DOB, Gender, Address)
  - Step 2: Professional Details (License, Specialization, Department, Experience, Qualifications)
  - Step 3: Schedule & Fees (Working Days, Time Range, Appointment Duration, Max Patients)
  - Step 4: Documents (Profile Picture, License Document, Certificates) - *UI only, not yet processed*

- **Features**:
  - Form validation for required fields
  - Time range presets (Morning, Evening, Standard, Extended shifts)
  - Real-time time range validation
  - Working days checkbox selection
  - Progress indicator showing current step
  - AJAX submission with loading states
  - Anti-forgery token for security

### 2. **Page Model** (`CreateDoctor.cshtml.cs`)
- **Request Model**: `DoctorRegistrationRequest`
  - Maps frontend form fields to a flat structure
  - Accepts all doctor-related data

- **Handler**: `OnPostAsync([FromBody] DoctorRegistrationRequest request)`
  - Validates required fields (FirstName, Email, LicenseNumber, Specialization)
  - Generates username from email
  - Sets default password: `CareSync@123`
  - Creates nested structure with `UserRegisteration_DTO` containing `RegisterDoctor_DTO`
  - Calls AdminApiService to register doctor
  - Returns JSON response for AJAX handling

### 3. **API Service** (`AdminApiService.cs`)
- **Method**: `RegisterDoctorAsync<T>(object doctorData)`
  - Sends POST request to `Admin/doctor-registration`
  - Returns typed result

### 4. **API Controller** (`AdminController.cs`)
- **Endpoint**: `POST /api/Admin/doctor-registration`
  - Accepts `UserRegisteration_DTO` with nested `RegisterDoctor`
  - Delegates to `UserService.RegisterNewUserAsync(dto, "doctor")`

### 5. **Business Logic** (`UserService.cs`)
- **Method**: `RegisterNewUserAsync(UserRegisteration_DTO request, string roleName)`
  - Creates user account with ASP.NET Identity
  - Assigns Doctor role
  - Creates doctor-specific details via `CreateUserDetailsAsync`
  - Saves to database with transaction support
  - Returns success/failure result

## Data Flow

```
Frontend Form
    ↓ (AJAX POST with JSON)
CreateDoctorModel.OnPostAsync()
    ↓ (Maps to UserRegisteration_DTO with nested RegisterDoctor_DTO)
AdminApiService.RegisterDoctorAsync()
    ↓ (HTTP POST to API)
AdminController.RegisterDoctor()
    ↓
UserService.RegisterNewUserAsync()
    ↓
Database (T_Users + T_DoctorDetails)
```

## Request Structure

```csharp
// Frontend sends flat structure
{
  "firstName": "John",
  "lastName": "Smith",
  "email": "john.smith@example.com",
  "phoneNumber": "1234567890",
  "dateOfBirth": "1985-05-15",
  "gender": "Male",
  "address": "123 Medical Plaza",
  "licenseNumber": "MD12345",
  "experienceYears": 10,
  "specialization": "Cardiology",
  "department": "Cardiology",
  "qualifications": "MBBS, MD",
  "certifications": "Board Certified Cardiologist",
  "availableDays": "Monday, Wednesday, Friday",
  "startTime": "09:00",
  "endTime": "17:00",
  "appointmentDuration": 30,
  "maxPatients": 20
}

// Backend transforms to nested structure
UserRegisteration_DTO {
  // User fields...
  RegisterDoctor: RegisterDoctor_DTO {
    // Doctor-specific fields...
  }
}
```

## Database Tables

### T_Users
- Stores basic user account information
- Managed by ASP.NET Identity
- Links to T_DoctorDetails via UserId

### T_DoctorDetails
- Stores doctor-specific professional information
- Fields: Specialization, LicenseNumber, ExperienceYears, Department, etc.
- Schedule information: AvailableDays, StartTime, EndTime, AppointmentDuration
- Patient capacity: MaxPatientsPerDay

## Default Values

- **Username**: Generated from email (part before @)
- **Password**: `CareSync@123` (must be changed on first login)
- **Appointment Duration**: 30 minutes (default)
- **Max Patients Per Day**: 20 (default)
- **Available Days**: Monday-Friday (if not specified)
- **Working Hours**: 09:00-17:00 (if not specified)

## Security

- ✅ **Authorization**: Only Admins can access `/Admin/CreateDoctor`
- ✅ **CSRF Protection**: Anti-forgery token validation
- ✅ **Role Assignment**: Automatically assigned Doctor role
- ✅ **Transaction Support**: Database changes rolled back on error
- ✅ **Input Validation**: Required fields validated on both client and server

## Success Response

```json
{
  "success": true,
  "message": "Dr. John Smith registered successfully! Default password: CareSync@123"
}
```

After successful creation, the page redirects to `/Admin/Doctors`.

## Error Handling

- **Validation Errors**: Returns specific field validation messages
- **Duplicate Email**: Catches SQL unique constraint violations
- **Database Errors**: Rolls back transaction and returns error
- **Network Errors**: Displays user-friendly error message

## Next Steps for Enhancement

1. **Password Policy**: Implement first-login password change requirement
2. **File Upload**: Process profile picture and document uploads (Step 4)
3. **Email Notification**: Send welcome email with credentials to new doctor
4. **Qualifications List**: Support adding multiple qualifications with degrees and institutions
5. **Specialization Dropdown**: Load from database instead of hardcoded options
6. **Department Management**: Link to department master data
7. **Schedule Validation**: Check for overlapping schedules or time conflicts

## Testing Checklist

- [ ] Form validation for all required fields
- [ ] Time range validation (end > start)
- [ ] Working days selection (at least one required)
- [ ] Email format validation
- [ ] Duplicate email handling
- [ ] Successful doctor creation
- [ ] Redirection to doctors list after success
- [ ] Error messages display correctly
- [ ] Loading states work properly
- [ ] Anti-forgery token validation

## Usage Example

1. Navigate to `/Admin/CreateDoctor` (requires Admin role)
2. Fill in Step 1: Personal Information
3. Click "Next" to proceed to Step 2: Professional Details
4. Fill in license, specialization, experience, etc.
5. Click "Next" to proceed to Step 3: Schedule & Fees
6. Select working days and set time range
7. Click "Next" to proceed to Step 4: Documents (optional for now)
8. Click "Create Doctor" to submit
9. System creates user account + doctor profile
10. Redirects to `/Admin/Doctors` with success message

## Notes

- The form is designed for future expansion (document upload functionality is UI-ready)
- All doctor data is stored in a single transaction (atomicity guaranteed)
- The system automatically handles user account creation and role assignment
- Default password must be communicated to the doctor securely
