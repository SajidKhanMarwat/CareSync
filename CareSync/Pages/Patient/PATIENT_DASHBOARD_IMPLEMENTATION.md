# Patient Dashboard - Complete Implementation Guide

## Overview
This document outlines the comprehensive Patient Dashboard implementation with integrated workflows for appointments, prescriptions, and lab tests.

## Completed Work

### 1. Backend Model Updates (`Dashboard.cshtml.cs`)
✅ Added Patient Information Properties:
- PatientName, Gender, Age, BloodType
- PrimaryDoctor, LastVisitDate, NextAppointmentDate

✅ Added Dashboard Statistics:
- UpcomingAppointments, ActivePrescriptions
- PendingLabTests, NewReports

✅ Added Health Vitals:
- CurrentBP, CurrentSugar, CurrentHeartRate, CurrentCholesterol

### 2. Dashboard Features to Implement

#### **A. Patient Profile Header**
- Display patient basic info (Name, Gender, Age, Blood Type)
- Show primary doctor, last visit, next appointment
- Patient avatar/photo

#### **B. Quick Stats Cards** (4 Cards)
1. Upcoming Appointments (with count)
2. Active Prescriptions (with count)
3. Pending Lab Tests (with count)
4. New Lab Reports (with count)

#### **C. Main Content Area (2 Columns)**

**Left Column (60-70% width):**

1. **Upcoming Appointments Section**
   - Calendar integration with date picker (using D:/theme/ components)
   - List of next 3-5 appointments with:
     - Date/Time
     - Doctor name & specialty
     - Status badge (Confirmed/Pending/Scheduled)
     - Quick action buttons (View Details)
   
2. **Available Doctors - Booking Cards**
   - Grid of 4-6 doctor cards
   - Each card shows:
     - Doctor photo
     - Name & Specialization
     - Rating & reviews
     - Years of experience
     - Available time slots (today/tomorrow)
     - "Book Appointment" button
   - "Find More Doctors" link at bottom

**Right Column (30-40% width):**

1. **Active Medications Widget**
   - List of current prescriptions
   - Medication name & dosage
   - Schedule times (badges)
   - Status (Active/Expiring)
   - "View All Prescriptions" link

2. **Lab Tests Status Widget**
   - Pending tests with progress bar
   - Completed tests ready for download
   - "Request New Test" button

3. **Quick Actions Card**
   - Book Appointment
   - View Prescriptions
   - Lab Results
   - Medical History
   - Track Vitals

4. **Health Alerts Card**
   - Expiring prescriptions warning
   - Upcoming appointment reminders
   - Critical lab values (if any)

#### **D. Health Vitals Charts Section** (Bottom)
4 Chart Cards displaying trends:
1. Blood Pressure Levels (with chart)
2. Sugar Levels (with chart)
3. Heart Rate (with chart)
4. Cholesterol Levels (with chart)

Each chart shows:
- Icon
- Current value
- Trend chart (line/area chart)
- Last 3-5 readings with dates

### 3. Theme Integration (D:/theme/)

#### Date Range Picker
```html
<input type="text" id="dateRangePicker" class="form-control custom-daterange" 
       placeholder="Select Date Range">
```

#### Calendar Component
- Use theme's calendar plugin for appointment visualization
- Interactive date selection
- Highlight dates with appointments

#### Charts
- Use theme's chart library (ApexCharts/Chart.js)
- Line charts for vitals tracking
- Color coding for normal/abnormal ranges

### 4. Workflow Integration

#### **Appointment Booking Flow:**
1. Dashboard → Click doctor card → BookAppointment page
2. Select date/time from available slots
3. Enter reason for visit
4. Confirm booking
5. Redirect back to Appointments page

#### **Prescription Management Flow:**
1. Dashboard → View active prescriptions
2. Click "View All" → Prescriptions page
3. See prescription details, refills remaining
4. Request refill or renewal
5. Link to pharmacy finder

#### **Lab Test Flow:**
1. Dashboard → Request Lab Test button
2. Modal opens with test selection
3. Choose from available tests or enter custom request
4. Submit request (links to appointment if needed)
5. Track status in Dashboard
6. Download report when ready

### 5. Navigation Links

All pages should be interconnected:
- `/Patient/Dashboard` - Main overview
- `/Patient/Appointments` - All appointments (list & calendar view)
- `/Patient/BookAppointment` - Booking form with doctor selection
- `/Patient/Prescriptions` - All prescriptions & medications
- `/Patient/LabResults` - Lab tests & reports
- `/Patient/MedicalHistory` - Complete medical history
- `/Patient/Vitals` - Vital signs tracking
- `/Patient/Profile` - Patient profile management

### 6. Modal Components

#### Request Lab Test Modal
```html
<div class="modal" id="requestLabTestModal">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5>Request Lab Test</h5>
            </div>
            <div class="modal-body">
                <!-- Test selection form -->
                <select>
                    <option>Complete Blood Count (CBC)</option>
                    <option>Lipid Profile</option>
                    <option>HbA1c (Diabetes)</option>
                    <option>Thyroid Function</option>
                    <option>Vitamin D Level</option>
                    <option>Other (Specify)</option>
                </select>
                <textarea placeholder="Additional notes"></textarea>
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary">Cancel</button>
                <button class="btn btn-primary">Submit Request</button>
            </div>
        </div>
    </div>
</div>
```

### 7. Page Structure

```
Patient Dashboard
├── Page Actions (Top Right)
│   ├── Date Range Picker
│   ├── Book Appointment Button
│   └── Request Lab Test Button
│
├── Patient Profile Header Card
│   ├── Basic Info (Name, Gender, Age, Blood)
│   └── Footer (Primary Doctor, Last Visit, Next Appointment)
│
├── Quick Stats Row (4 Cards)
│   ├── Upcoming Appointments
│   ├── Active Prescriptions
│   ├── Pending Lab Tests
│   └── New Reports
│
├── Main Content Row (2 Columns)
│   ├── Left Column (60-70%)
│   │   ├── Upcoming Appointments (with Calendar)
│   │   └── Available Doctors for Booking (Grid)
│   │
│   └── Right Column (30-40%)
│       ├── Active Medications
│       ├── Lab Tests Status
│       ├── Quick Actions
│       └── Health Alerts
│
└── Health Vitals Charts Row (4 Charts)
    ├── BP Levels Chart
    ├── Sugar Levels Chart
    ├── Heart Rate Chart
    └── Cholesterol Chart
```

### 8. Responsive Design

- **Desktop (>1200px)**: Full 2-column layout with all widgets
- **Tablet (768-1199px)**: Stacked layout, doctor cards 2 per row
- **Mobile (<768px)**: Single column, simplified cards

### 9. Color Coding & Status

**Appointment Status:**
- ✅ Confirmed: Green badge
- ⏰ Pending: Yellow/Warning badge
- 📅 Scheduled: Blue/Info badge
- ❌ Cancelled: Red/Danger badge

**Prescription Status:**
- ✅ Active: Green
- ⚠️ Expiring Soon: Yellow
- 🔄 Refill Needed: Orange
- ❌ Expired: Red

**Lab Test Status:**
- ⏳ Pending: Yellow with progress bar
- ✅ Ready: Green with download button
- 📝 Scheduled: Blue
- ❌ Cancelled: Red

### 10. JavaScript Functionality

#### Date Range Picker Init
```javascript
$('#dateRangePicker').daterangepicker({
    opens: 'left',
    locale: {
        format: 'MMM DD, YYYY'
    }
});
```

#### Chart Initialization
```javascript
// BP Levels Chart
var bpChart = new ApexCharts(document.querySelector("#bpLevels"), {
    series: [{
        name: 'BP',
        data: [120, 125, 118, 122, 120]
    }],
    chart: {
        height: 120,
        type: 'line',
        sparkline: { enabled: true }
    },
    colors: ['#0d6efd'],
    stroke: { curve: 'smooth', width: 2 }
});
bpChart.render();
```

### 11. API Endpoints Needed (Future Integration)

```
GET  /api/patient/dashboard - Get dashboard summary
GET  /api/patient/appointments/upcoming - Get upcoming appointments
GET  /api/patient/doctors/available - Get available doctors
GET  /api/patient/prescriptions/active - Get active prescriptions
GET  /api/patient/labtests/pending - Get pending lab tests
GET  /api/patient/vitals/latest - Get latest vitals
POST /api/patient/appointments/book - Book new appointment
POST /api/patient/labtests/request - Request new lab test
```

### 12. Entity Relationships

**Patient Dashboard Data Sources:**
- `T_PatientDetails` - Patient information
- `T_Appointments` - Appointments (filter by PatientID, Status, Date)
- `T_Prescriptions` - Prescriptions (filter by PatientID, active)
- `T_LabRequests` - Lab test requests
- `T_LabReports` - Lab reports
- `T_PatientVitals` - Vital signs history
- `T_DoctorDetails` - Doctor information for booking
- `T_Users` - User authentication

### 13. Next Steps

1. ✅ Update Dashboard.cshtml.cs with properties
2. ⏳ Replace Dashboard.cshtml content with new layout
3. ⏳ Add chart initialization scripts
4. ⏳ Implement date range picker
5. ⏳ Create lab test request modal
6. ⏳ Update BookAppointment page to accept doctor parameter
7. ⏳ Create FindDoctors page for doctor search
8. ⏳ Integrate with actual database queries
9. ⏳ Add real-time notifications
10. ⏳ Implement print/download functionality

### 14. Files Modified/Created

**Modified:**
- ✅ `Dashboard.cshtml.cs` - Added model properties
- ⏳ `Dashboard.cshtml` - Complete redesign
- ⏳ `Appointments.cshtml` - Add calendar view
- ⏳ `BookAppointment.cshtml` - Add doctor parameter
- ⏳ `Prescriptions.cshtml` - Already updated
- ⏳ `LabResults.cshtml` - Already updated

**New Files Needed:**
- `FindDoctors.cshtml` - Doctor search page
- `RequestLabTest.cshtml` or Modal component

### 15. Sample Data for Testing

Use mock data in the model until database integration:
- 3 upcoming appointments with different statuses
- 5 active prescriptions (2 expiring soon)
- 2 pending lab tests (1 ready for download)
- 4 available doctors with time slots
- Vital signs history (last 5 readings for each)

## Summary

The Patient Dashboard is the central hub for patients to:
- ✅ View their health overview at a glance
- ✅ Book appointments with available doctors
- ✅ Track prescriptions and medications
- ✅ Monitor lab test requests and download reports
- ✅ View vital signs trends over time
- ✅ Get health alerts and reminders
- ✅ Quick access to all patient features

The design follows the D:/theme/ components and styling for consistency with the rest of the CareSync application.
