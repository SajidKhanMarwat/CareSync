# CareSync Admin - Testing Guide

## Quick Start Testing

### Prerequisites Checklist
- [ ] SQL Server is running
- [ ] Database `CareSync` exists and migrations applied
- [ ] API project running on `http://localhost:5157`
- [ ] UI project running (default: `http://localhost:5000`)
- [ ] At least one admin user exists in database

### Create Admin User (If needed)

If no admin user exists, create one manually in database:

```sql
-- Create Admin Role (if not exists)
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES (NEWID(), 'Admin', 'ADMIN', NEWID())

-- Create Admin User
DECLARE @AdminRoleId NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'Admin')
DECLARE @AdminUserId NVARCHAR(450) = NEWID()

INSERT INTO AspNetUsers (
    Id, UserName, NormalizedUserName, Email, NormalizedEmail,
    EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
    PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled,
    AccessFailedCount, FirstName, Gender, RoleType, RoleID, IsActive
)
VALUES (
    @AdminUserId, 'admin@caresync.com', 'ADMIN@CARESYNC.COM',
    'admin@caresync.com', 'ADMIN@CARESYNC.COM', 1,
    'AQAAAAIAAYagAAAAEKH2LCJ7+O5X4YpYMl8JE7bVJKKxJJE4kKDmf7RKxkL8vKLY8RxY1sKQCzQqQxBD6g==', -- Password: Admin@123456
    NEWID(), NEWID(), 0, 0, 0, 0,
    'Admin', 0, 0, @AdminRoleId, 1
)

-- Assign Role
INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES (@AdminUserId, @AdminRoleId)
```

## Test Scenarios

### 1. Login & Authentication
```
URL: http://localhost:5000/auth/login
Credentials: admin@caresync.com / Admin@123456
Expected: Redirect to /Admin/Dashboard
Verify: Session contains UserToken and UserRole=Admin
```

### 2. Dashboard - View Statistics
```
URL: http://localhost:5000/Admin/Dashboard
Actions:
1. Observe statistics cards (Appointments, Doctors, Patients)
2. Check percentages show correctly
3. Verify urgent items section

Expected Results:
✓ Real numbers from database
✓ Percentage calculations (green for positive, red for negative)
✓ No console errors
✓ Loading is fast (parallel API calls)

Test Data Verification:
- Check browser Network tab for API calls
- Verify responses from /api/Admin/dashboard/stats
- Check browser console for errors
```

### 3. Doctors List
```
URL: http://localhost:5000/Admin/Doctors
Actions:
1. View all doctors list
2. Filter by specialization
3. Toggle doctor status (activate/deactivate)

Expected Results:
✓ Doctors list populated from database
✓ Each doctor shows: name, email, specialization, status
✓ Statistics card shows total/active/inactive counts
✓ Toggle status updates immediately
✓ Success message after toggle

API Calls to Monitor:
- GET /api/Admin/doctors
- GET /api/Admin/doctors/stats
- PATCH /api/Admin/doctors/{userId}/toggle-status?isActive=true/false
```

### 4. Create New Doctor
```
URL: http://localhost:5000/Admin/CreateDoctor
Test Data:
Email: dr.sarah.johnson@caresync.com
Password: Doctor@123456
Confirm Password: Doctor@123456
First Name: Sarah
Last Name: Johnson
Phone: +1234567890
Gender: Female
Date of Birth: 1985-03-15
Specialization: Cardiology
License Number: MED-2024-001

Actions:
1. Fill form with test data
2. Submit

Expected Results:
✓ Form validates
✓ Success message: "Doctor Sarah registered successfully!"
✓ Redirects to /Admin/Doctors
✓ New doctor appears in list
✓ Can login with created credentials

Database Verification:
SELECT * FROM AspNetUsers WHERE Email = 'dr.sarah.johnson@caresync.com'
SELECT * FROM T_DoctorDetails WHERE UserID = (SELECT Id FROM AspNetUsers WHERE Email = '...')
```

### 5. Patients List
```
URL: http://localhost:5000/Admin/Patients
Actions:
1. View all patients
2. Filter by blood group (e.g., O+, A+)
3. Filter by active status
4. Toggle patient status

Expected Results:
✓ Patients list with all details
✓ Filter works correctly
✓ Statistics accurate
✓ Toggle updates immediately
```

### 6. Patient Search
```
URL: http://localhost:5000/Admin/Patients/Search
Test Searches:
1. Search by name: "John"
2. Search by email: "john.doe@email.com"
3. Search by phone: "1234567890"

Actions:
1. Enter search term
2. Click Search or press Enter

Expected Results:
✓ Results appear below search box
✓ Matches are case-insensitive
✓ Shows relevant patient info
✓ "No results" message if not found

API Call:
GET /api/Admin/patients/search?searchTerm=john
```

### 7. Book Appointment - Existing Patient
```
URL: http://localhost:5000/Admin/BookAppointment
Mode: Existing Patient

Test Data:
Patient: Select from dropdown
Doctor: Select from dropdown
Appointment Date: Tomorrow's date
Time: 10:00 AM
Type: In-Person
Reason: Regular checkup

Actions:
1. Switch to "Existing Patient" tab
2. Fill form
3. Submit

Expected Results:
✓ Dropdowns populated
✓ Form submits successfully
✓ Redirects to /Admin/Appointments (or success page)
✓ Appointment created in database

Database Verification:
SELECT TOP 1 * FROM T_Appointments ORDER BY CreatedOn DESC
```

### 8. Book Appointment - Quick Patient Registration
```
URL: http://localhost:5000/Admin/BookAppointment
Mode: Quick Patient Registration

Test Data (New Patient):
First Name: Michael
Last Name: Brown
Email: michael.brown@email.com
Phone: +1987654321
Gender: Male
Date of Birth: 1990-06-20
Blood Group: B+
Marital Status: Single

Doctor: Select from dropdown
Appointment Date: Tomorrow
Type: In-Person
Reason: First consultation

Actions:
1. Switch to "Quick Registration" tab
2. Fill patient details
3. Select doctor and appointment details
4. Submit

Expected Results:
✓ Single submission creates patient AND appointment
✓ Success message appears
✓ Redirects appropriately
✓ Patient can now login (password auto-generated or specified)

Database Verification:
-- All 3 records created in single transaction
SELECT * FROM AspNetUsers WHERE Email = 'michael.brown@email.com'
SELECT * FROM T_PatientDetails WHERE UserID = (SELECT Id FROM AspNetUsers WHERE Email = '...')
SELECT * FROM T_Appointments WHERE PatientID = (SELECT PatientID FROM T_PatientDetails WHERE UserID = '...')
```

### 9. Create Laboratory Staff
```
URL: http://localhost:5000/Admin/CreateLaboratory
Test Data:
Email: lab.tech@caresync.com
Password: Lab@123456
First Name: Emily
Last Name: Wilson
Phone: +1122334455
Gender: Female

Actions:
1. Fill form
2. Submit

Expected Results:
✓ Lab staff created
✓ Success message
✓ Redirects to /Admin/MedicalStaff
✓ New user has Lab role
```

## API Testing with Postman/Insomnia

### Get JWT Token
```http
POST http://localhost:5157/api/account/login
Content-Type: application/json

{
  "email": "admin@caresync.com",
  "password": "Admin@123456"
}

Response includes: token, refreshToken, role
Copy the token for subsequent requests
```

### Test Dashboard Stats
```http
GET http://localhost:5157/api/Admin/dashboard/stats
Authorization: Bearer {your-token-here}

Expected Response:
{
  "isSuccess": true,
  "data": {
    "totalAppointments": 260,
    "thisVsLastMonthPercentageAppointment": 15.5,
    "totalDoctors": 24,
    "thisVsLastMonthPercentageDoctors": 8.3,
    "totalPatients": 980,
    "thisVsLastMonthPercentagePatients": 12.7
  }
}
```

### Test Get All Doctors
```http
GET http://localhost:5157/api/Admin/doctors
Authorization: Bearer {your-token-here}

Optional Query Parameters:
?specialization=Cardiology
?isActive=true
```

### Test Search Patients
```http
GET http://localhost:5157/api/Admin/patients/search?searchTerm=john
Authorization: Bearer {your-token-here}
```

### Test Create Doctor
```http
POST http://localhost:5157/api/Admin/doctor-registration
Authorization: Bearer {your-token-here}
Content-Type: application/json

{
  "email": "new.doctor@caresync.com",
  "password": "Doctor@123456",
  "confirmPassword": "Doctor@123456",
  "firstName": "New",
  "lastName": "Doctor",
  "phoneNumber": "+1234567890",
  "gender": 0,
  "roleType": 2
}
```

## Browser Developer Tools Testing

### Network Tab Monitoring
1. Open browser DevTools (F12)
2. Go to Network tab
3. Filter: XHR/Fetch
4. Perform actions and observe:
   - Request URLs
   - Request headers (Authorization Bearer token)
   - Response status codes (200, 400, 401, 500)
   - Response data
   - Response times

### Console Tab Monitoring
Watch for:
- JavaScript errors (should be none)
- API call logs
- Authentication warnings

### Application Tab
Check Session Storage:
- UserToken should be present
- UserRole should be "Admin"
- RoleRights (if applicable)

## Performance Testing

### Dashboard Load Time
```
Expected: < 2 seconds for initial load
Measure: DevTools → Network → Load Time
Parallel Calls: Should see multiple API requests firing simultaneously
```

### List Pages (Doctors, Patients)
```
Expected: < 1 second for lists up to 100 items
With Filters: Should maintain fast performance
```

## Security Testing

### 1. Test Unauthorized Access
```
1. Logout
2. Try to access: http://localhost:5000/Admin/Dashboard
Expected: Redirect to /auth/login
```

### 2. Test Wrong Role Access
```
1. Login as Patient or Doctor
2. Try to access: http://localhost:5000/Admin/Dashboard
Expected: Redirect to /auth/AccessDenied or appropriate error
```

### 3. Test API Without Token
```http
GET http://localhost:5157/api/Admin/dashboard/stats
Authorization: (none)

Expected: 401 Unauthorized
```

### 4. Test API With Expired Token
```http
GET http://localhost:5157/api/Admin/dashboard/stats
Authorization: Bearer expired-token-here

Expected: 401 Unauthorized
```

## Error Handling Testing

### 1. Network Failure
```
Actions:
1. Stop API project
2. Try to load Dashboard
Expected: User-friendly error message displayed
```

### 2. Invalid Data Submission
```
Actions:
1. Submit CreateDoctor form with invalid email
Expected: Validation error shown
```

### 3. Duplicate Email
```
Actions:
1. Try to create doctor with existing email
Expected: Error message: "Email already in use"
```

## Database State Verification

### Check User Creation
```sql
-- Verify doctor created
SELECT u.Email, u.FirstName, u.LastName, u.RoleType, r.Name as RoleName
FROM AspNetUsers u
LEFT JOIN AspNetRoles r ON u.RoleID = r.Id
WHERE u.Email = 'dr.sarah.johnson@caresync.com'

-- Verify doctor details
SELECT dd.*, u.FirstName, u.LastName
FROM T_DoctorDetails dd
JOIN AspNetUsers u ON dd.UserID = u.Id
WHERE u.Email = 'dr.sarah.johnson@caresync.com'
```

### Check Appointment Creation
```sql
-- Latest appointments
SELECT TOP 10
    a.AppointmentID,
    a.AppointmentDate,
    a.Status,
    p_user.FirstName + ' ' + p_user.LastName as PatientName,
    d_user.FirstName + ' ' + d_user.LastName as DoctorName
FROM T_Appointments a
JOIN T_PatientDetails pd ON a.PatientID = pd.PatientID
JOIN AspNetUsers p_user ON pd.UserID = p_user.Id
JOIN T_DoctorDetails dd ON a.DoctorID = dd.DoctorID
JOIN AspNetUsers d_user ON dd.UserID = d_user.Id
ORDER BY a.CreatedOn DESC
```

## Common Issues & Solutions

### Issue: "Unauthorized" on all API calls
**Solution**: 
- Check if token is in session
- Verify AuthorizationMessageHandler is configured
- Check if token has expired
- Re-login

### Issue: Data not loading
**Solution**:
- Check API is running
- Verify API base URL in Program.cs
- Check database connection string
- Look at browser console for errors

### Issue: "CORS" error
**Solution**:
Add CORS policy in API Program.cs:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUI", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});
app.UseCors("AllowUI");
```

### Issue: Page redirects to login immediately
**Solution**:
- Session might be expired
- Check session timeout configuration
- Verify cookie settings

## Continuous Testing Checklist

For each new feature, test:
- [ ] Authentication required
- [ ] Authorization (Admin role) checked
- [ ] API call with token
- [ ] Success response handled
- [ ] Error response handled
- [ ] Loading state shown
- [ ] Success message displayed
- [ ] Error message displayed
- [ ] Data persisted to database
- [ ] UI updates after action
- [ ] No console errors
- [ ] No network errors
- [ ] Proper validation
- [ ] XSS prevention (Razor auto-encode)
- [ ] SQL injection prevention (EF parameterized)

## Test Coverage Summary

✅ **Implemented & Tested**:
- Login/Logout flow
- Session management
- JWT token attachment
- Dashboard with real data
- Doctor CRUD operations
- Patient CRUD operations
- Patient search
- Appointment booking (both modes)
- User registration (doctors, lab staff)
- Error handling at all layers
- Authorization checks

🚧 **Needs Implementation**:
- Appointments list page integration
- User management page
- Role management page
- Reports generation
- Audit logs
- File uploads
- Real-time notifications

## Performance Benchmarks

**Target Response Times**:
- Login: < 500ms
- Dashboard Load: < 2s
- List Pages: < 1s
- Create Operations: < 500ms
- Search: < 300ms

**Concurrency**:
- Should handle 10 simultaneous admins
- No degradation with 100 doctors in system
- No degradation with 1000 patients in system

## Security Audit Checklist

- [x] All admin pages require authentication
- [x] All admin pages check for Admin role
- [x] API endpoints secured with [Authorize(Roles = "Admin")]
- [x] JWT tokens used for authentication
- [x] Passwords hashed (ASP.NET Identity)
- [x] XSS protection (Razor auto-encode)
- [x] SQL injection protection (EF Core)
- [ ] HTTPS enforced (configure in production)
- [ ] CSRF protection (configure anti-forgery tokens)
- [ ] Rate limiting (add middleware)
- [ ] Input sanitization (add validators)
- [ ] Audit logging (implement)

---

## Quick Test Commands

```bash
# Start API
cd d:\Projects\CareSync.API
dotnet run

# Start UI (separate terminal)
cd d:\Projects\CareSync
dotnet run

# Run database migrations
cd d:\Projects\CareSync.API
dotnet ef database update

# Check database
sqlcmd -S localhost -d CareSync -Q "SELECT COUNT(*) FROM AspNetUsers"
```

Happy Testing! 🎉
