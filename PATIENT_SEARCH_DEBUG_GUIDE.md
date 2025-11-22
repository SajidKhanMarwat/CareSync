# Patient Search - Complete Debug Guide

## Issue: 400 Bad Request Error

## Complete Data Flow

### 1. **UI Layer** (`BookAppointment.cshtml`)
```javascript
// FormData with anti-forgery token
POST /Admin/BookAppointment?handler=SearchPatient
Body: FormData {
    searchTerm: "testpatient1@gmail.com",
    __RequestVerificationToken: "[token]"
}
```

### 2. **Page Model** (`BookAppointment.cshtml.cs`)
```csharp
public async Task<IActionResult> OnPostSearchPatientAsync([FromForm] string searchTerm)
{
    // 1. Validate admin role
    // 2. Validate searchTerm
    // 3. Call AdminApiService.SearchPatientsAsync(searchTerm)
    // 4. Return JSON result
}
```

### 3. **API Service** (`AdminApiService.cs`)
```csharp
public async Task<T?> SearchPatientsAsync<T>(string searchTerm)
{
    // GET http://localhost:5000/api/Admin/patients/search?searchTerm={searchTerm}
    var response = await client.GetAsync($"Admin/patients/search?searchTerm={Uri.EscapeDataString(searchTerm)}");
}
```

### 4. **API Controller** (`AdminController.cs`)
```csharp
[HttpGet("patients/search")]
[AllowAnonymous]
public async Task<Result<List<PatientSearch_DTO>>> SearchPatients([FromQuery] string searchTerm)
{
    return await adminService.SearchPatientsAsync(searchTerm);
}
```

### 5. **Service Layer** (`AdminService.cs`)
```csharp
public async Task<Result<List<PatientSearch_DTO>>> SearchPatientsAsync(string searchTerm)
{
    // Query T_Users table
    // Join with T_PatientDetails
    // Search by: FirstName, LastName, Email, PhoneNumber
    // Return PatientSearch_DTO list
}
```

### 6. **Database**
```sql
SELECT u.*, p.* 
FROM T_Users u
INNER JOIN T_PatientDetails p ON u.Id = p.UserID
WHERE u.RoleID = (SELECT Id FROM AspNetRoles WHERE Name = 'Patient')
AND (
    LOWER(u.FirstName) LIKE '%searchterm%' OR
    LOWER(u.LastName) LIKE '%searchterm%' OR
    LOWER(u.Email) LIKE '%searchterm%' OR
    u.PhoneNumber LIKE '%searchterm%'
)
```

## Testing Steps

### Step 1: Check Application is Running
- ✅ API project running on configured port
- ✅ Web project running and accessible
- ✅ Logged in as Admin role

### Step 2: Open Browser Console (F12)
Look for these console messages:

**Expected Success Flow:**
```
=== Patient Search Started ===
Search term: testpatient1@gmail.com
Anti-forgery token found: Yes
FormData created with searchTerm and token
Calling API: /Admin/BookAppointment?handler=SearchPatient
Response received - Status: 200 OK: true
Search result: { success: true, patient: {...} }
Patient found: John Doe
=== Patient Search Completed ===
```

**If 400 Error:**
```
=== Patient Search Started ===
...
Response received - Status: 400 OK: false
Server error response: [error details]
```

### Step 3: Check Application Logs

**PageModel Handler Logs:**
```
[INFO] Received search term: testpatient1@gmail.com
[INFO] Searching for patient with term: testpatient1@gmail.com
```

**API Service Logs:**
```
[INFO] Calling Admin/patients/search?searchTerm=testpatient1%40gmail.com
```

**API Controller Logs:**
```
[INFO] SearchPatients called with: testpatient1@gmail.com
```

## Troubleshooting 400 Error

### Common Causes:

#### 1. **Anti-Forgery Token Missing**
**Symptom:** 400 error immediately

**Check:**
```javascript
// Browser console
document.querySelector('input[name="__RequestVerificationToken"]')
// Should return: <input type="hidden" name="__RequestVerificationToken" value="...">
```

**Fix:** Ensure `@Html.AntiForgeryToken()` exists in the form

#### 2. **Handler Name Mismatch**
**Symptom:** 404 or 400 error

**Check:**
- URL: `/Admin/BookAppointment?handler=SearchPatient`
- Method name: `OnPostSearchPatientAsync`
- Match pattern: `OnPost{HandlerName}Async`

#### 3. **Parameter Binding Issue**
**Symptom:** searchTerm is null in handler

**Check PageModel:**
```csharp
public async Task<IActionResult> OnPostSearchPatientAsync([FromForm] string searchTerm)
//                                                          ^^^^^^^^^^
// Must use [FromForm] for FormData
```

**Check JavaScript:**
```javascript
formData.append('searchTerm', searchInput);
//               ^^^^^^^^^^^
// Parameter name must match exactly
```

#### 4. **Authorization Issue**
**Symptom:** 401/403 or redirect

**Check:**
- User is logged in
- User has "Admin" role
- `RequireRole("Admin")` is not returning redirect result

#### 5. **API Endpoint Not Accessible**
**Symptom:** AdminApiService call fails

**Test API directly:**
```bash
curl -X GET "http://localhost:5000/api/Admin/patients/search?searchTerm=test" \
     -H "Authorization: Bearer YOUR_TOKEN"
```

### Verification Checklist

- [ ] ✅ Anti-forgery token exists in form
- [ ] ✅ Token is included in FormData
- [ ] ✅ Handler method name matches URL handler parameter
- [ ] ✅ Parameter name in method matches FormData key
- [ ] ✅ User is logged in as Admin
- [ ] ✅ API project is running
- [ ] ✅ Database has patient data
- [ ] ✅ Connection string is correct
- [ ] ✅ No firewall blocking requests

## Manual Testing

### Test 1: Verify Anti-Forgery Token
```javascript
// In browser console
const token = document.querySelector('input[name="__RequestVerificationToken"]');
console.log('Token found:', token !== null);
console.log('Token value:', token?.value);
```

### Test 2: Test FormData Creation
```javascript
const formData = new FormData();
formData.append('searchTerm', 'test');
formData.append('__RequestVerificationToken', 'dummy');
for (let pair of formData.entries()) {
    console.log(pair[0]+ ': ' + pair[1]); 
}
```

### Test 3: Test Handler Directly
```javascript
const formData = new FormData();
formData.append('searchTerm', 'test@example.com');
formData.append('__RequestVerificationToken', document.querySelector('input[name="__RequestVerificationToken"]').value);

fetch('/Admin/BookAppointment?handler=SearchPatient', {
    method: 'POST',
    body: formData
})
.then(r => r.json())
.then(data => console.log('Result:', data))
.catch(e => console.error('Error:', e));
```

### Test 4: Check Database Has Patients
```sql
-- Run in SQL Server Management Studio
SELECT TOP 10 
    u.Id, 
    u.FirstName, 
    u.LastName, 
    u.Email,
    u.PhoneNumber,
    r.Name as Role
FROM T_Users u
INNER JOIN AspNetRoles r ON u.RoleID = r.Id
WHERE r.Name = 'Patient'
AND u.IsDeleted = 0
```

## Expected Response Format

### Success Response:
```json
{
    "success": true,
    "patient": {
        "patientId": 1,
        "userId": "guid-here",
        "fullName": "John Doe",
        "email": "john@example.com",
        "phoneNumber": "555-1234",
        "age": 35,
        "bloodGroup": "A+",
        "lastVisit": "2025-11-20"
    }
}
```

### Not Found Response:
```json
{
    "success": false,
    "message": "No patient found with the provided information"
}
```

### Error Response:
```json
{
    "success": false,
    "message": "Error: [error details]"
}
```

## Quick Fix Steps

1. **Restart Application**
   - Stop both API and Web projects
   - Rebuild solution
   - Start API project first
   - Then start Web project

2. **Clear Browser Cache**
   - Hard refresh: Ctrl+F5
   - Clear cookies for localhost
   - Close and reopen browser

3. **Check Application Logs**
   - Look in CareSync.API/Logs/ folder
   - Check for exceptions or errors
   - Look for "SearchPatient" in logs

4. **Verify Database Connection**
   - Check connection string in appsettings.json
   - Test connection in SQL Server Management Studio
   - Verify database has patient records

## Still Getting 400 Error?

If none of the above works, provide:

1. **Complete console output** from browser (F12)
2. **Application logs** from last search attempt
3. **Database query result** showing patients exist
4. **Network tab details** showing request/response headers

This will help identify the exact issue!
