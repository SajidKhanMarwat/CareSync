# Role-Based UI Implementation Guide

## Overview
This guide explains the role-based UI access system implemented in CareSync using Razor Pages. The system provides UI-only role visibility control without backend authorization.

## Architecture

### 1. Session Management
- User role is stored in `HttpContext.Session` after successful login
- Session timeout: 30 minutes
- Additional data stored: Token, RefreshToken, RoleRights

### 2. Base Page Model
**File:** `Pages/Shared/BasePageModel.cs`

Provides common functionality for all pages:
- `UserRole`: Gets current user's role
- `IsAuthenticated`: Checks if user has valid session
- `HasRole(string role)`: Checks specific role
- `HasAnyRole(params string[] roles)`: Checks multiple roles
- `RequireAuthentication()`: Redirects to login if not authenticated
- `RequireRole(string role)`: Enforces role requirement

### 3. Role-Based Sidebar
**File:** `Pages/Shared/Components/_Sidebar.cshtml`

Features:
- Dynamic menu based on user role
- Role-specific profile display
- Active page highlighting
- Logout functionality

## Roles and Access

### Admin Role
**Pages:**
- `/Admin/Dashboard` - System overview and statistics
- `/Admin/Users` - User management
- `/Admin/Roles` - Role management  
- `/Admin/RoleRights` - Role rights mapping
- `/Admin/SystemOverview` - System analytics

**Sidebar Menu:**
- Admin Dashboard
- User Management
- Role Management
- Role Rights
- System Overview

### Doctor Role
**Pages:**
- `/Doctor/Profile` - Professional profile management
- `/Doctor/Appointments` - Appointment scheduling and management
- `/Doctor/Prescriptions` - Prescription management

**Sidebar Menu:**
- My Profile
- Appointments
- Prescriptions

### Patient Role
**Pages:**
- `/Patient/Dashboard` - Personal health dashboard
- `/Patient/Profile` - Personal profile
- `/Patient/MedicalHistory` - Medical history view
- `/Patient/Vitals` - Health vitals tracking
- `/Patient/Appointments` - Appointment booking and history

**Sidebar Menu:**
- My Dashboard
- My Profile
- Medical History
- Health Vitals
- My Appointments

## Implementation Examples

### 1. Login Process
```csharp
// Store user data in session after successful login
HttpContext.Session.SetString("UserRole", result.Data.Role ?? "Patient");
HttpContext.Session.SetString("UserToken", result.Data.Token ?? "");
HttpContext.Session.SetString("RefreshToken", result.Data.RefreshToken ?? "");

// Redirect based on role
return result.Data.Role?.ToLower() switch
{
    "admin" => RedirectToPage("/Admin/Dashboard"),
    "doctor" => RedirectToPage("/Doctor/Profile"),
    "patient" => RedirectToPage("/Patient/Dashboard"),
    _ => RedirectToPage("/Dashboard/Index")
};
```

### 2. Page Model with Role Check
```csharp
public class AdminDashboardModel : BasePageModel
{
    public IActionResult OnGet()
    {
        // Enforce Admin role requirement
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        // Page logic here
        return Page();
    }
}
```

### 3. Conditional UI Content
```html
@if (Model.HasRole("Admin"))
{
    <button class="btn btn-primary">
        <i class="ri-user-add-line me-2"></i>Add User
    </button>
}
else if (Model.HasRole("Doctor"))
{
    <button class="btn btn-primary">
        <i class="ri-calendar-line me-2"></i>New Appointment
    </button>
}
```

### 4. Sidebar Role Check
```html
@{
    var userRole = Context.Session.GetString("UserRole") ?? "Patient";
}

@if (userRole == "Admin")
{
    <!-- Admin menu items -->
}
else if (userRole == "Doctor")
{
    <!-- Doctor menu items -->
}
else if (userRole == "Patient")
{
    <!-- Patient menu items -->
}
```

## Security Considerations

### UI-Only Protection
- This implementation provides **UI visibility control only**
- Backend API endpoints should have proper authorization
- Session data can be manipulated by users
- Not suitable for sensitive operations without backend validation

### Session Security
- Sessions expire after 30 minutes of inactivity
- HttpOnly cookies prevent JavaScript access
- Secure flag should be enabled in production

## File Structure
```
/Pages
├── /Auth
│   ├── Login.cshtml (.cs)
│   ├── Logout.cshtml (.cs)
│   └── AccessDenied.cshtml (.cs)
├── /Admin
│   ├── Dashboard.cshtml (.cs)
│   ├── Users.cshtml (.cs)
│   ├── Roles.cshtml (.cs)
│   ├── RoleRights.cshtml (.cs)
│   └── SystemOverview.cshtml (.cs)
├── /Doctor
│   ├── Profile.cshtml (.cs)
│   ├── Appointments.cshtml (.cs)
│   └── Prescriptions.cshtml (.cs)
├── /Patient
│   ├── Dashboard.cshtml (.cs)
│   ├── Profile.cshtml (.cs)
│   ├── MedicalHistory.cshtml (.cs)
│   ├── Vitals.cshtml (.cs)
│   └── Appointments.cshtml (.cs)
└── /Shared
    ├── BasePageModel.cs
    ├── Components/_Sidebar.cshtml
    └── _RoleBasedContent.cshtml
```

## Usage Guidelines

### 1. Creating New Pages
1. Inherit from `BasePageModel`
2. Use `RequireRole()` or `RequireAuthentication()` in OnGet()
3. Use role checking methods in Razor views

### 2. Adding Menu Items
1. Update `_Sidebar.cshtml`
2. Add role-specific menu items
3. Include active page highlighting

### 3. Role-Based Content
1. Use `Model.HasRole()` in Razor views
2. Implement fallback content for unauthorized access
3. Consider using partial views for complex role logic

## Testing

### Test Scenarios
1. **Login with different roles** - Verify correct redirects
2. **Direct URL access** - Ensure role enforcement works
3. **Session expiry** - Test redirect to login
4. **Role switching** - Verify UI updates correctly
5. **Logout process** - Confirm session cleanup

### Test Users
Create test users for each role:
- Admin: admin@caresync.com
- Doctor: doctor@caresync.com  
- Patient: patient@caresync.com

## Troubleshooting

### Common Issues
1. **Session not persisting** - Check session configuration in Program.cs
2. **Role not updating** - Verify session storage in login process
3. **Sidebar not showing** - Check role string comparison (case sensitivity)
4. **Page access denied** - Verify BasePageModel inheritance

### Debug Tips
1. Check session values in browser dev tools
2. Add logging to role checking methods
3. Verify session middleware order in Program.cs
4. Test with different browsers/incognito mode

## Future Enhancements

### Possible Improvements
1. **Role hierarchy** - Implement role inheritance
2. **Permission-based access** - Use RoleRights for granular control
3. **Dynamic menus** - Load menu items from database
4. **Audit logging** - Track role-based access attempts
5. **Multi-tenant support** - Role scoping by organization
