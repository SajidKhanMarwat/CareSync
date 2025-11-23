# Database Migration for Password Reset Feature

## Overview
This migration adds a new column to the T_Users (AspNetUsers) table to support password reset flow for admin-created users.

## New Column
**IsPasswordResetRequired** (bit/boolean) - Indicates if user must reset password on next login

## Migration Commands

### Step 1: Create Migration
Open a terminal in the CareSync.Data project directory and run:

```bash
cd D:\Projects\CareSync.Data
dotnet ef migrations add AddPasswordResetField --startup-project ../CareSync.API
```

### Step 2: Review Migration
The migration should add the following column to AspNetUsers table:
- IsPasswordResetRequired (bit, NOT NULL, DEFAULT 0)

### Step 3: Update Database
```bash
dotnet ef database update --startup-project ../CareSync.API
```

## Manual SQL Script (Alternative)
If you prefer to run the migration manually, use this SQL script:

```sql
-- Add new column to AspNetUsers table
ALTER TABLE AspNetUsers
ADD IsPasswordResetRequired bit NOT NULL DEFAULT 0;

-- Optional: Set existing admin-created users to require password reset
-- UPDATE AspNetUsers 
-- SET IsPasswordResetRequired = 1
-- WHERE CreatedBy = 'Admin';
```

## Verification Query
After running the migration, verify the column exists:

```sql
SELECT TOP 5 
    Id,
    UserName,
    Email,
    IsPasswordResetRequired,
    CreatedBy,
    CreatedOn
FROM AspNetUsers
ORDER BY CreatedOn DESC;
```

## Testing the Feature

### 1. Create a User as Admin
1. Login as admin
2. Go to Admin > Create Doctor/Patient/Lab
3. Create a new user with a temporary password

### 2. Test Password Reset Flow
1. Logout
2. Login with the newly created user credentials
3. System should redirect to /Auth/ResetPassword
4. Enter new password
5. After successful reset, login with new password
6. User should be redirected to their dashboard

## Rollback (if needed)
```bash
dotnet ef migrations remove --startup-project ../CareSync.API
```

Or SQL:
```sql
ALTER TABLE AspNetUsers DROP COLUMN IsPasswordResetRequired;
```

## Implementation Files Modified

### Backend
- `CareSync.Data/Entities/User Entities/T_Users.cs` - Added IsPasswordResetRequired property
- `CareSync.ApplicationLayer/Services/EntitiesServices/UserService.cs` - Updated login and password reset logic
- `CareSync.ApplicationLayer/Services/EntitiesServices/AdminService.cs` - Set flag for admin-created users
- `CareSync.ApplicationLayer/Contracts/UsersDTOs/UserRegisteration_DTO.cs` - Added RequiresPasswordReset flag
- `CareSync.Shared/Models/LoginResponse.cs` - Added RequiresPasswordReset property
- `CareSync.API/Controllers/AdminController.cs` - Set RequiresPasswordReset flag

### Frontend
- `CareSync/Pages/Auth/ResetPassword.cshtml` - New password reset page
- `CareSync/Pages/Auth/ResetPassword.cshtml.cs` - Password reset logic
- `CareSync/Pages/Auth/Login.cshtml.cs` - Check for password reset requirement

## Notes
- Users created by admin will have `IsPasswordResetRequired = true`
- After successful password reset, `IsPasswordResetRequired` is set to false
- Self-registered users have the flag set to false by default
- The `CreatedBy` field can be used to identify admin-created users for audit purposes
