# Doctor Role - Complete Workflow Documentation

## Overview
This document outlines the comprehensive workflow for doctors in the CareSync medical management system. The workflow is designed to facilitate efficient patient care, from appointment management to prescription writing and patient monitoring.

---

## Pages Created/Enhanced

### 1. **Doctor Dashboard** (`/Doctor/Dashboard`)
Enhanced dashboard with comprehensive insights and data visualization.

#### Features:
- **Doctor Profile Header**: Displays doctor name, specialization, ratings, and availability status
- **Key Statistics Cards**: 
  - Total Patients (with percentage increase)
  - Total Surgeries
  - Monthly Earnings
- **Detailed Insights Row**:
  - Today's Appointments Count
  - Total Prescriptions Written This Month
  - Pending Appointments (Awaiting Approval)
  - Lab Reports Pending Review
- **Patient Status Chart**: Donut chart showing distribution of:
  - New Patients
  - Regular Patients
  - Follow-up Patients
- **Appointments Overview Chart**: Area chart showing monthly appointment trends
- **Appointments Calendar**: Full calendar view with appointment scheduling (integrated from theme)
- **Today's Appointments Table**: List of today's appointments with actions:
  - Reject Appointment
  - Accept Appointment
  - Start Checkup (navigates to checkup page)
- **Patient Reviews Section**: Recent patient feedback and ratings
- **Recent Patients Section**: Quick access to recent patient profiles

#### Technologies Used:
- Apex Charts for data visualization
- FullCalendar for appointment calendar
- Bootstrap 5 for responsive design
- RemixIcon for icons

---

### 2. **Patient Checkup** (`/Doctor/Checkup`)
Comprehensive checkup interface for examining patients and creating treatment plans.

#### Features:

##### Patient Information Card:
- Patient profile with photo
- Basic demographics (Age, Gender, Blood Group, Marital Status)
- Appointment details (Date, Type, Reason)
- Emergency contact information

##### Tab 1: Vitals & Health
- **Update Patient Vitals**:
  - Height (cm)
  - Weight (kg)
  - Pulse Rate (bpm)
  - Blood Pressure (e.g., 120/80)
  - Diabetic status and readings
  - High blood pressure status and readings history

- **Chronic Diseases Management**:
  - Add/Remove chronic diseases
  - Disease name
  - Diagnosed date
  - Current status (Controlled, Uncontrolled, In Remission, Progressive)
  - Dynamic form to add multiple diseases

- **Save Functionality**: Saves all vitals and chronic disease updates to database

##### Tab 2: Medical History
- **Previous Prescriptions**: Accordion view of past prescriptions with:
  - Prescription date
  - Prescribing doctor
  - General notes
  - Medications list
  
- **Previous Vitals Records**: Table view of historical vitals data for trend analysis

##### Tab 3: Prescription
- **Write New Prescription**:
  - Add multiple medications dynamically
  - For each medication:
    - Medicine Name
    - Dosage (e.g., 500mg)
    - Frequency (Once Daily, Twice Daily, etc.)
    - Duration (in days)
    - Special Instructions
  - General prescription notes
  
- **Actions**:
  - Create Prescription (saves to database)
  - Complete Checkup (marks appointment as completed and returns to dashboard)

#### Workflow:
1. Doctor accepts appointment from dashboard
2. Clicks "Start Checkup" button
3. Reviews patient information and medical history
4. Updates patient vitals and chronic diseases
5. Writes prescription with medications
6. Completes checkup (appointment marked as completed)

#### Database Entities Used:
- `T_PatientVitals` - Stores vital signs
- `T_ChronicDiseases` - Manages chronic conditions
- `T_Prescriptions` - Prescription header
- `T_PrescriptionItems` - Individual medication items
- `T_Appointments` - Updates appointment status

---

### 3. **Patient Details Management** (`/Doctor/PatientDetails`)
Dedicated page for comprehensive patient medical information management.

#### Features:

##### Left Sidebar - Patient Profile:
- Patient photo and basic information
- Patient ID
- Demographics (Age, Gender, Blood Group, Marital Status, Occupation)
- Emergency contact details
- Quick Statistics Card:
  - Total Appointments
  - Total Prescriptions
  - Number of Chronic Conditions
  - Last Visit Date

##### Right Panel - Tabbed Interface:

###### Tab 1: Vitals History
- **Vitals History Table**: Complete history of vital measurements
  - Date
  - Height, Weight, BMI
  - Blood Pressure (with high BP indicator)
  - Pulse Rate
  - Diabetic status badges
  
- **Vitals Trend Chart**: Line chart showing:
  - Weight trends over time
  - Blood pressure trends
  - Visual representation of health improvements/declines
  
- **Add New Vitals**: Modal form to add new vitals record

###### Tab 2: Chronic Diseases
- Card-based layout showing all chronic conditions
- For each disease:
  - Disease name
  - Diagnosed date
  - Current status with color-coded badges
  - Edit and Delete buttons
  
- **Add Disease**: Modal form to add new chronic condition

###### Tab 3: Prescriptions History
- Accordion view of all prescriptions
- Expandable prescription details showing:
  - Prescription date and doctor
  - General notes
  - Table of all medications with dosage, frequency, duration, and instructions

###### Tab 4: Appointments History
- Table view of all appointments (past and future)
- Appointment details:
  - Date and time
  - Type and reason
  - Status badges (Completed, Scheduled, Cancelled)
  - View button to access appointment details

#### Actions Available:
- Add new vitals record
- Add chronic disease
- View prescription history
- Start new checkup from this page
- View appointment history

---

## Doctor Workflow Process

### Complete Patient Care Cycle:

```
┌─────────────────────────────────────────────────────────────┐
│                     DOCTOR DASHBOARD                         │
│  - View appointments calendar                                │
│  - See today's appointments                                  │
│  - Monitor patient statistics                                │
└────────────────┬────────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────────┐
│               APPOINTMENT MANAGEMENT                         │
│  1. Doctor receives appointment request                      │
│  2. Review appointment details                               │
│  3. ACCEPT or REJECT appointment                             │
└────────────────┬────────────────────────────────────────────┘
                 │
                 │ (If Accepted)
                 ▼
┌─────────────────────────────────────────────────────────────┐
│                  PATIENT CHECKUP                             │
│  1. Click "Start Checkup" button                             │
│  2. View patient information & medical history               │
│  3. Update patient vitals (BP, weight, pulse, etc.)          │
│  4. Update chronic diseases                                  │
│  5. Write prescription with medications                      │
│  6. Complete checkup                                         │
└────────────────┬────────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────────┐
│              PRESCRIPTION CREATED                            │
│  - Prescription linked to patient                            │
│  - Prescription linked to appointment                        │
│  - Patient receives prescription                             │
│  - Appointment marked as "Completed"                         │
└────────────────┬────────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────────┐
│            PATIENT DETAILS MANAGEMENT                        │
│  (Accessed anytime for patient monitoring)                   │
│  - View complete vitals history with charts                  │
│  - Manage chronic diseases                                   │
│  - Review prescription history                               │
│  - Track appointment history                                 │
│  - Add new vitals records                                    │
└─────────────────────────────────────────────────────────────┘
```

---

## Key Relationships

### Patient ↔ Prescription Flow:
1. **Prescriptions are linked to PATIENTS**, not directly to doctors
2. However, prescriptions also reference the doctor who wrote them
3. Each prescription is associated with a specific appointment
4. This creates the relationship: `Patient ← Appointment → Prescription → Doctor`

### Entity Relationships:
```
T_Appointments
├── PatientID (FK to T_PatientDetails)
├── DoctorID (FK to T_DoctorDetails)
└── Prescriptions Collection
    └── T_Prescriptions
        ├── PatientID (FK to T_PatientDetails)
        ├── DoctorID (FK to T_DoctorDetails)
        ├── AppointmentID (FK to T_Appointments)
        └── PrescriptionItems Collection
            └── T_PrescriptionItems
                ├── MedicineName
                ├── Dosage
                ├── Frequency
                └── Instructions

T_PatientDetails
├── UserID (FK to T_Users)
├── PatientVitals Collection
│   └── T_PatientVitals
│       ├── Height, Weight, BMI
│       ├── BloodPressure
│       ├── IsDiabetic
│       └── HasHighBloodPressure
├── ChronicDiseases Collection
│   └── T_ChronicDiseases
│       ├── DiseaseName
│       ├── DiagnosedDate
│       └── CurrentStatus
└── Prescriptions Collection (from appointments)
```

---

## Features Implemented

### 1. **Dashboard Enhancements**
- ✅ Detailed statistics cards with insights
- ✅ Appointments overview chart (Apex Charts - Area chart)
- ✅ Patient status distribution chart (Apex Charts - Donut chart)
- ✅ Appointments calendar (FullCalendar integration)
- ✅ Today's appointments with accept/reject/checkup actions
- ✅ Patient reviews section
- ✅ Recent patients quick access

### 2. **Appointment Management**
- ✅ Accept appointment functionality
- ✅ Reject appointment functionality
- ✅ Start checkup from appointment
- ✅ Appointment status tracking

### 3. **Patient Checkup Flow**
- ✅ Comprehensive patient information display
- ✅ Vitals update form (Height, Weight, BP, Pulse, Diabetic status)
- ✅ Chronic diseases management (Add/Remove)
- ✅ Medical history view (Previous prescriptions and vitals)
- ✅ Prescription writing interface
- ✅ Multiple medications support
- ✅ Complete checkup functionality

### 4. **Patient Details Management**
- ✅ Patient profile with complete demographics
- ✅ Vitals history with trend charts
- ✅ Chronic diseases management
- ✅ Prescriptions history
- ✅ Appointments history
- ✅ Add new vitals modal
- ✅ Add chronic disease modal

### 5. **Data Visualization**
- ✅ Apex Charts integration
- ✅ FullCalendar integration
- ✅ Responsive charts and tables
- ✅ Color-coded status badges
- ✅ Interactive UI elements

---

## API Endpoints Required

The following API endpoints need to be implemented in the backend:

### Appointments
- `POST /api/appointments/{id}/approve` - Accept appointment
- `POST /api/appointments/{id}/reject` - Reject appointment
- `POST /api/appointments/{id}/complete` - Mark appointment as completed
- `GET /api/appointments/doctor/{doctorId}` - Get doctor's appointments

### Vitals
- `POST /api/vitals` - Create new vitals record
- `GET /api/vitals/patient/{patientId}` - Get patient vitals history
- `PUT /api/vitals/{id}` - Update vitals record

### Chronic Diseases
- `POST /api/chronic-diseases` - Add chronic disease
- `GET /api/chronic-diseases/patient/{patientId}` - Get patient chronic diseases
- `PUT /api/chronic-diseases/{id}` - Update chronic disease
- `DELETE /api/chronic-diseases/{id}` - Delete chronic disease

### Prescriptions
- `POST /api/prescriptions` - Create prescription
- `GET /api/prescriptions/patient/{patientId}` - Get patient prescriptions
- `GET /api/prescriptions/{id}` - Get prescription details

---

## Technical Stack

### Frontend
- **ASP.NET Core Razor Pages** - Server-side rendering
- **Bootstrap 5** - Responsive UI framework
- **Apex Charts** - Modern charting library
- **FullCalendar** - Calendar and scheduling
- **RemixIcon** - Icon library
- **jQuery** - DOM manipulation and AJAX

### Backend (To be implemented)
- **Entity Framework Core** - ORM for database operations
- **ASP.NET Core API** - RESTful API endpoints
- **SQL Server** - Database

### Database Entities
- `T_PatientDetails`
- `T_PatientVitals`
- `T_ChronicDiseases`
- `T_Appointments`
- `T_Prescriptions`
- `T_PrescriptionItems`
- `T_DoctorDetails`
- `T_Users`

---

## Next Steps / TODO

### Backend Implementation Required:
1. ✅ Create API controllers for appointments, vitals, chronic diseases, and prescriptions
2. ✅ Implement service layer for business logic
3. ✅ Connect pages to actual database using Entity Framework
4. ✅ Add authentication and authorization
5. ✅ Implement real-time updates for appointments
6. ✅ Add validation and error handling
7. ✅ Implement file upload for patient documents/reports
8. ✅ Add email notifications for appointments
9. ✅ Implement search and filtering for patient lists
10. ✅ Add export functionality for prescriptions and reports

### Additional Features to Consider:
- Video consultation integration
- Lab test ordering from checkup page
- Patient messaging system
- Appointment reminders (SMS/Email)
- Doctor's notes and observations
- Medical certificates generation
- Analytics and reporting dashboard
- Mobile responsive optimization

---

## File Structure

```
CareSync/Pages/Doctor/
├── Dashboard.cshtml                    # Enhanced dashboard with charts
├── Dashboard.cshtml.cs                 # Dashboard logic
├── Checkup.cshtml                      # Patient checkup page
├── Checkup.cshtml.cs                   # Checkup logic
├── PatientDetails.cshtml               # Patient management page
├── PatientDetails.cshtml.cs            # Patient details logic
├── Appointments.cshtml                 # Existing appointments page
├── Patients.cshtml                     # Existing patients list
├── Prescriptions.cshtml                # Existing prescriptions page
├── MedicalRecords.cshtml              # Existing medical records
├── Profile.cshtml                      # Doctor profile
└── DOCTOR_WORKFLOW_DOCUMENTATION.md   # This file
```

---

## Usage Examples

### Example 1: Accepting and Completing an Appointment
1. Doctor logs into dashboard
2. Views "Today's Appointments" section
3. Sees pending appointment from "Willian Mathews"
4. Clicks green checkmark to accept appointment
5. Clicks stethoscope icon to start checkup
6. Updates patient vitals: BP 120/80, Weight 75kg, Pulse 72bpm
7. Adds chronic disease if discovered during examination
8. Writes prescription:
   - Paracetamol 500mg, Three times daily, 5 days
   - Vitamin C 1000mg, Once daily, 7 days
9. Clicks "Complete Checkup"
10. Returns to dashboard

### Example 2: Monitoring Patient Health
1. Doctor navigates to Patients page
2. Clicks on patient "Willian Mathews"
3. Views patient details page
4. Reviews vitals history chart showing weight trend
5. Sees blood pressure is stable
6. Checks chronic diseases tab - Asthma is controlled
7. Reviews prescription history to avoid drug interactions
8. Adds new vitals record via modal
9. Returns to dashboard

---

## Summary

The doctor workflow in CareSync provides a complete, integrated solution for patient care management. From appointment scheduling to prescription writing and long-term patient monitoring, doctors have all the tools needed to deliver quality healthcare efficiently.

**Key Achievements:**
- ✅ Enhanced dashboard with detailed insights and visualizations
- ✅ Integrated appointments calendar from theme
- ✅ Complete patient checkup workflow
- ✅ Prescription management linked to patients
- ✅ Comprehensive patient details management with vitals tracking
- ✅ Chronic disease management
- ✅ Medical history tracking
- ✅ Modern, responsive UI with charts and interactive elements

**Developer Note:** Backend API implementation is required to connect these pages to the actual database. All frontend functionality is ready and waiting for backend integration.
