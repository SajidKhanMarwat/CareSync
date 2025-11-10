# CareSync Seed Data

This folder contains services that automatically seed the database with initial data when the application starts.

## Seeded Roles

The following roles are automatically created:

| Role Name | Role Type | Description | Arabic Name |
|-----------|-----------|-------------|-------------|
| Admin | Admin | System Administrator with full access to all features | مدير النظام |
| Patient | Patient | Patients who receive medical care and services | مريض |
| Doctor | Doctor | Medical doctors who provide patient care | طبيب |
| DoctorAssistant | DoctorAssistant | Assistant doctors who support medical care | مساعد طبيب |
| LabAssistant | LabAssistant | Laboratory assistants who help with lab operations | مساعد مختبر |
| Lab | Lab | Laboratory technicians who perform medical tests | فني مختبر |

## Default Admin User

A default admin user is created with the following credentials:

- **Email**: admin@caresync.com
- **Username**: admin
- **Password**: Admin@123456
- **Role**: Admin

> ⚠️ **Important**: Please change the default admin password after first login for security purposes!

## How It Works

1. **RoleSeedService**: Creates all system roles if they don't exist
2. **AdminSeedService**: Creates a default admin user if one doesn't exist
3. **SeedDataExtensions**: Orchestrates the seeding process and provides DI registration

The seeding process runs automatically when the API starts up and only creates data that doesn't already exist, making it safe to run multiple times.

## Usage

The seed data is automatically applied when the application starts. No manual intervention is required.

If you need to add more seed data, create additional seed services following the same pattern and register them in `SeedDataExtensions.cs`.
