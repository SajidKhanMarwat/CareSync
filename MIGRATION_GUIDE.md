# Database Migration Guide - Enum to String Conversion

## 🎯 Purpose
This migration converts all enum columns from INTEGER to VARCHAR(50) for better database readability.

---

## 📋 Pre-Migration Checklist

- [ ] Backup current database
- [ ] Review all configuration files created
- [ ] Ensure no pending changes in git
- [ ] Verify connection string in appsettings.json
- [ ] Close any active database connections

---

## 🚀 Migration Steps

### Step 1: Create Migration

Open terminal in the solution directory:

```powershell
# Navigate to API project
cd d:\Projects\CareSync.API

# Create migration
dotnet ef migrations add EnumToStringConversion --project ..\CareSync.Data --startup-project .

# Alternative: If you have multiple contexts
dotnet ef migrations add EnumToStringConversion --context CareSyncDbContext --project ..\CareSync.Data --startup-project .
```

**Expected Output:**
```
Build started...
Build succeeded.
Done. To undo this action, use 'ef migrations remove'
```

### Step 2: Review Migration File

Navigate to: `d:\Projects\CareSync.Data\Migrations\`

**Check the generated migration file:** `XXXXXXXX_EnumToStringConversion.cs`

**Expected changes:**

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Convert Gender from INT to VARCHAR
    migrationBuilder.AlterColumn<string>(
        name: "Gender",
        table: "T_Users",
        type: "nvarchar(50)",
        maxLength: 50,
        nullable: false,
        oldClrType: typeof(int),
        oldType: "int");

    // Convert RoleType from INT to VARCHAR
    migrationBuilder.AlterColumn<string>(
        name: "RoleType",
        table: "T_Users",
        type: "nvarchar(50)",
        maxLength: 50,
        nullable: false,
        oldClrType: typeof(int),
        oldType: "int");

    // Convert MaritalStatus from INT to VARCHAR
    migrationBuilder.AlterColumn<string>(
        name: "MaritalStatus",
        table: "T_PatientDetails",
        type: "nvarchar(50)",
        maxLength: 50,
        nullable: false,
        oldClrType: typeof(int),
        oldType: "int");

    // Convert Appointment Status and Type
    migrationBuilder.AlterColumn<string>(
        name: "Status",
        table: "T_Appointments",
        type: "nvarchar(50)",
        maxLength: 50,
        nullable: false,
        oldClrType: typeof(int),
        oldType: "int");

    migrationBuilder.AlterColumn<string>(
        name: "AppointmentType",
        table: "T_Appointments",
        type: "nvarchar(50)",
        maxLength: 50,
        nullable: false,
        oldClrType: typeof(int),
        oldType: "int");
}
```

### Step 3: Add Data Migration (If Database Has Data)

**⚠️ IMPORTANT:** If you have existing data, you need to manually add data conversion!

Edit the migration file to include data conversion:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // 1. Add temporary columns
    migrationBuilder.AddColumn<string>(
        name: "Gender_Temp",
        table: "T_Users",
        type: "nvarchar(50)",
        maxLength: 50,
        nullable: true);

    // 2. Convert existing data
    migrationBuilder.Sql(@"
        UPDATE T_Users 
        SET Gender_Temp = CASE Gender
            WHEN 0 THEN 'Male'
            WHEN 1 THEN 'Female'
            ELSE 'Male'
        END");

    migrationBuilder.Sql(@"
        UPDATE T_Users 
        SET RoleType_Temp = CASE RoleType
            WHEN 0 THEN 'Admin'
            WHEN 1 THEN 'Patient'
            WHEN 2 THEN 'Doctor'
            WHEN 3 THEN 'DoctorAssistant'
            WHEN 4 THEN 'LabAssistant'
            WHEN 5 THEN 'Lab'
            ELSE 'Patient'
        END");

    migrationBuilder.Sql(@"
        UPDATE T_PatientDetails 
        SET MaritalStatus_Temp = CASE MaritalStatus
            WHEN 1 THEN 'Single'
            WHEN 2 THEN 'Married'
            WHEN 3 THEN 'Divorced'
            WHEN 4 THEN 'Widowed'
            WHEN 5 THEN 'Separated'
            ELSE 'Single'
        END");

    migrationBuilder.Sql(@"
        UPDATE T_Appointments 
        SET Status_Temp = CASE Status
            WHEN 0 THEN 'Created'
            WHEN 1 THEN 'Pending'
            WHEN 2 THEN 'Approved'
            WHEN 3 THEN 'Rejected'
            ELSE 'Created'
        END");

    migrationBuilder.Sql(@"
        UPDATE T_Appointments 
        SET AppointmentType_Temp = CASE AppointmentType
            WHEN 0 THEN 'WalkIn'
            WHEN 1 THEN 'ABP'
            ELSE 'WalkIn'
        END");

    // 3. Drop old columns
    migrationBuilder.DropColumn(name: "Gender", table: "T_Users");
    migrationBuilder.DropColumn(name: "RoleType", table: "T_Users");
    // ... etc

    // 4. Rename temp columns
    migrationBuilder.RenameColumn(
        name: "Gender_Temp",
        table: "T_Users",
        newName: "Gender");
    // ... etc
}
```

### Step 4: Apply Migration

```powershell
# Apply to database
dotnet ef database update --project ..\CareSync.Data --startup-project .

# Check migration status
dotnet ef migrations list --project ..\CareSync.Data --startup-project .
```

**Expected Output:**
```
Build started...
Build succeeded.
Applying migration '20241121XXXXXX_EnumToStringConversion'.
Done.
```

---

## 🔍 Verification Steps

### 1. Check Database Schema

```sql
-- Check column types
SELECT 
    TABLE_NAME, 
    COLUMN_NAME, 
    DATA_TYPE, 
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE COLUMN_NAME IN ('Gender', 'RoleType', 'MaritalStatus', 'Status', 'AppointmentType')
ORDER BY TABLE_NAME, COLUMN_NAME;

-- Expected: All should be nvarchar(50)
```

### 2. Verify Data

```sql
-- Check user data
SELECT TOP 10 
    UserName, 
    Email, 
    Gender, 
    RoleType 
FROM T_Users;

-- Expected: Gender='Male'/'Female', RoleType='Patient'/'Doctor'/etc.

-- Check patient data
SELECT TOP 10 
    PatientID, 
    MaritalStatus 
FROM T_PatientDetails;

-- Expected: MaritalStatus='Single'/'Married'/etc.

-- Check appointments
SELECT TOP 10 
    AppointmentID, 
    Status, 
    AppointmentType 
FROM T_Appointments;

-- Expected: Status='Created'/'Pending'/etc., AppointmentType='WalkIn'/'ABP'
```

### 3. Test Application

```powershell
# Run the API
cd d:\Projects\CareSync.API
dotnet run
```

**Test Endpoints:**
- ✅ POST /api/Account/Register (Patient)
- ✅ POST /api/Account/Login
- ✅ GET /api/Admin/dashboard-counts
- ✅ POST /api/Admin/patient-registeration

---

## 🔄 Rollback Plan

### If Something Goes Wrong:

#### Option 1: Remove Last Migration (Before applying)
```powershell
dotnet ef migrations remove --project ..\CareSync.Data --startup-project .
```

#### Option 2: Rollback Database (After applying)
```powershell
# Rollback to previous migration
dotnet ef database update [PreviousMigrationName] --project ..\CareSync.Data --startup-project .

# Example:
dotnet ef database update InitialCreate --project ..\CareSync.Data --startup-project .
```

#### Option 3: Restore from Backup
```sql
-- In SQL Server Management Studio
RESTORE DATABASE CareSync 
FROM DISK = 'C:\Backups\CareSync_Backup.bak'
WITH REPLACE;
```

---

## ⚠️ Common Issues & Solutions

### Issue 1: "Cannot convert INT to VARCHAR implicitly"

**Solution:** Add explicit data migration SQL as shown in Step 3.

### Issue 2: "Constraint violation during migration"

**Solution:** 
```sql
-- Temporarily disable constraints
ALTER TABLE T_Users NOCHECK CONSTRAINT ALL;
-- Run migration
-- Re-enable constraints
ALTER TABLE T_Users CHECK CONSTRAINT ALL;
```

### Issue 3: "NULL values after migration"

**Solution:** Review data conversion SQL, ensure all enum values are mapped.

### Issue 4: "Build failed - Cannot find type"

**Solution:**
```powershell
# Clean and rebuild
dotnet clean
dotnet build
```

---

## 📊 Impact Analysis

### Database Changes:
- **5 columns** converted from INT to VARCHAR(50)
- **5 tables** affected (T_Users, T_PatientDetails, T_Appointments)
- **Data conversion** required if database has existing records

### Application Impact:
- ✅ **No code changes required** - Entity Framework handles conversion
- ✅ **LINQ queries work the same** - EF translates automatically
- ✅ **Better SQL readability** - Direct SQL queries show text values

### Performance Impact:
- VARCHAR(50) vs INT: Minimal difference (< 5% for most queries)
- Indexes remain efficient
- Storage increase: ~40 bytes per enum field per row

---

## 🎯 Post-Migration Tasks

- [ ] Verify all API endpoints work
- [ ] Check user registration flow
- [ ] Test appointment creation
- [ ] Run integration tests
- [ ] Update documentation
- [ ] Notify team of changes
- [ ] Monitor application logs
- [ ] Check database performance

---

## 📞 Support

If you encounter issues:

1. Check migration file for syntax errors
2. Verify connection string
3. Review logs: `d:\Projects\CareSync.API\Logs\`
4. Test with empty database first
5. Rollback and restore from backup if needed

---

## ✅ Success Criteria

Migration is successful when:
- ✅ All enum columns are VARCHAR(50)
- ✅ Data is preserved and converted correctly
- ✅ Application starts without errors
- ✅ API endpoints respond correctly
- ✅ User registration works
- ✅ Database queries return readable enum values

---

## 🎉 Completion

After successful migration:
- Database schema updated ✅
- Enums stored as strings ✅
- Application functioning ✅
- Backup created ✅
- Team notified ✅

**Status: READY FOR MIGRATION** 🚀
