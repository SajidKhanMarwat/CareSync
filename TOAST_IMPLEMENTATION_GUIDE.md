# Toast Notification System - Implementation Guide

## Overview
A modern, lightweight toast notification system has been implemented to replace all `alert()` and `confirm()` calls across the CareSync application.

## Features
- ✅ **4 Notification Types**: Success, Error, Warning, Info
- ✅ **Modern UI**: Beautiful, animated toasts with icons
- ✅ **Customizable**: Duration, progress bar, close button
- ✅ **Confirm Dialogs**: Replace `confirm()` with promise-based confirmations
- ✅ **Prompt Dialogs**: Replace `prompt()` with styled input dialogs
- ✅ **Responsive**: Mobile-friendly design
- ✅ **Accessible**: ARIA labels and keyboard support
- ✅ **No Dependencies**: Pure JavaScript, integrates with existing theme

## Files Added
1. `/wwwroot/theme/css/toast.css` - Toast notification styles
2. `/wwwroot/theme/js/toast.js` - Toast notification JavaScript library
3. Updated `_MainLayout.cshtml` to include both files

## Basic Usage

### Simple Toast Notifications

```javascript
// Success notification
toast.success('Operation completed successfully!', 'Success');

// Error notification
toast.error('Something went wrong!', 'Error');

// Warning notification
toast.warning('Please check your input.', 'Warning');

// Info notification
toast.info('Processing your request...', 'Info');
```

### Custom Duration

```javascript
// Show toast for 8 seconds (default is 5 seconds)
toast.success('Patient created successfully!', 'Success', 8000);

// Show toast indefinitely (duration = 0)
toast.info('Click to dismiss', 'Info', 0);
```

### Advanced Usage

```javascript
// Custom options
window.Toast.show({
    type: 'success',              // success, error, warning, info
    title: 'Custom Title',        // Toast title
    message: 'Custom message',    // Toast message
    duration: 5000,               // Duration in milliseconds
    closable: true,               // Show close button
    progress: true                // Show progress bar
});
```

## Replacing alert() Calls

### Before (Old)
```javascript
alert('User created successfully!');
```

### After (New)
```javascript
toast.success('User created successfully!', 'User Created');
```

### More Examples

| Old Code | New Code |
|----------|----------|
| `alert('Error occurred')` | `toast.error('Error occurred', 'Error')` |
| `alert('Please fill all fields')` | `toast.warning('Please fill all fields', 'Validation Error')` |
| `alert('Processing...')` | `toast.info('Processing...', 'Info')` |

## Replacing confirm() Calls

### Before (Old)
```javascript
if (confirm('Delete this user?')) {
    // Delete user
    alert('User deleted successfully!');
}
```

### After (New)
```javascript
toast.confirm({
    title: 'Delete User',
    message: 'Delete this user? This action cannot be undone.',
    type: 'error',
    confirmText: 'Delete',
    cancelText: 'Cancel'
}).then(confirmed => {
    if (confirmed) {
        // Delete user
        toast.success('User deleted successfully!', 'Success');
    }
});
```

### Confirm Dialog Options

```javascript
toast.confirm({
    title: 'Confirm Action',           // Dialog title
    message: 'Are you sure?',          // Dialog message
    type: 'warning',                   // success, error, warning, info
    confirmText: 'Confirm',            // Confirm button text
    cancelText: 'Cancel'               // Cancel button text
}).then(result => {
    if (result) {
        // User clicked Confirm
    } else {
        // User clicked Cancel or closed dialog
    }
});
```

## Replacing prompt() Calls

### Before (Old)
```javascript
const name = prompt('Enter your name:');
if (name) {
    alert(`Hello ${name}!`);
}
```

### After (New)
```javascript
toast.prompt({
    title: 'Enter Name',
    message: 'Please enter your name:',
    placeholder: 'Your name',
    defaultValue: '',
    type: 'info'
}).then(name => {
    if (name !== null) {
        toast.success(`Hello ${name}!`, 'Success');
    }
});
```

## Complete Examples

### User Management Actions

```javascript
// View Profile
toast.info(`Viewing profile for ${userName}...`, 'Profile');

// Edit User
toast.info(`Editing user ${userName}...`, 'Edit User');

// Reset Password
toast.confirm({
    title: 'Reset Password',
    message: `Reset password for ${userName}?`,
    type: 'warning'
}).then(confirmed => {
    if (confirmed) {
        toast.success('Password reset email sent successfully!', 'Success');
    }
});

// Delete User
toast.confirm({
    title: 'Delete User',
    message: `Delete user ${userName}? This action cannot be undone.`,
    type: 'error',
    confirmText: 'Delete',
    cancelText: 'Cancel'
}).then(confirmed => {
    if (confirmed) {
        toast.success('User deleted successfully.', 'Success');
    }
});
```

### Form Validation

```javascript
// Validate form
if (!firstName || !email || !role) {
    toast.warning('Please fill in all required fields.', 'Validation Error');
    return;
}

// Validate email
const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
if (!emailRegex.test(email)) {
    toast.warning('Please enter a valid email address.', 'Invalid Email');
    return;
}

// Success
toast.success('User created successfully!', 'User Added');
```

### API Calls

```javascript
try {
    const response = await fetch('/api/endpoint', { method: 'POST', body: data });
    
    if (!response.ok) {
        toast.error(`Server error (${response.status})`, 'Server Error');
        return;
    }
    
    const result = await response.json();
    
    if (result.success) {
        toast.success('Operation completed successfully!', 'Success');
    } else {
        toast.error(result.message, 'Error');
    }
} catch (error) {
    toast.error(`An error occurred: ${error.message}`, 'Error');
}
```

### Bulk Operations

```javascript
const selectedCount = document.querySelectorAll('input[type="checkbox"]:checked').length;

if (selectedCount === 0) {
    toast.warning('Please select items first.', 'No Selection');
    return;
}

toast.confirm({
    title: 'Bulk Action',
    message: `Apply action to ${selectedCount} selected items?`,
    type: 'warning'
}).then(confirmed => {
    if (confirmed) {
        toast.success(`Action completed for ${selectedCount} items.`, 'Success');
    }
});
```

## Additional Functions

### Clear All Toasts
```javascript
toast.clear(); // Closes all active toasts
```

### Direct Access to Toast Instance
```javascript
// For advanced customization
window.Toast.show({
    type: 'success',
    title: 'Custom',
    message: 'Advanced usage',
    duration: 10000,
    closable: true,
    progress: false
});
```

## Migration Checklist

- [x] ✅ Toast CSS and JS files created
- [x] ✅ Added to _MainLayout.cshtml
- [x] ✅ UserManagement.cshtml updated (17 alerts replaced)
- [x] ✅ BookAppointment.cshtml updated (16 alerts replaced)
- [ ] 🔄 Remaining 180+ alerts across 43 files

## Pages Still Requiring Updates

### High Priority (Most alerts):
1. `ReportsManagement.cshtml` (18 alerts)
2. `LabResults.cshtml` (12 alerts)
3. `Doctor/Dashboard.cshtml` (11 alerts)
4. `Prescriptions.cshtml` (11 alerts)
5. `Lab/Dashboard.cshtml` (10 alerts)
6. `TechnicianProfile.cshtml` (10 alerts)
7. `Users.cshtml` (9 alerts)
8. `TestRequests.cshtml` (9 alerts)

### Medium Priority:
9. `CreateDoctor.cshtml` (7 alerts)
10. `MedicalHistory.cshtml` (7 alerts)
11. `CreateAppointment.cshtml` (6 alerts)
12. `CreatePatient.cshtml` (5 alerts)
13. `PatientAppointments.cshtml` (5 alerts)
14. `Patient/BookAppointment.cshtml` (5 alerts)

### Quick Reference for Replacements

```javascript
// FIND & REPLACE PATTERNS:

// Simple alerts:
alert('message');
→ toast.info('message', 'Title');

// Success messages:
alert('✓ Success message');
→ toast.success('Success message', 'Success');

// Error messages:
alert('Error: ' + message);
→ toast.error(message, 'Error');

// Validation messages:
alert('Please enter...');
→ toast.warning('Please enter...', 'Validation Error');

// Confirm dialogs:
if (confirm('Are you sure?')) { action(); }
→ toast.confirm({ title: 'Confirm', message: 'Are you sure?', type: 'warning' }).then(r => { if (r) action(); });

// Multi-line alerts (use longer duration):
alert('Line 1\nLine 2\nLine 3');
→ toast.success('Line 1. Line 2. Line 3.', 'Title', 8000);
```

## Styling Customization

The toast system uses CSS variables for easy theme customization:

```css
/* In your custom CSS */
:root {
    --toast-success-color: #28a745;
    --toast-error-color: #dc3545;
    --toast-warning-color: #ffc107;
    --toast-info-color: #17a2b8;
}
```

## Browser Support
- ✅ Chrome 90+
- ✅ Firefox 88+
- ✅ Safari 14+
- ✅ Edge 90+
- ✅ Mobile browsers (iOS Safari, Chrome Mobile)

## Performance
- Lightweight: ~8KB total (CSS + JS)
- No external dependencies
- Hardware-accelerated animations
- Auto-cleanup of dismissed toasts

## Accessibility
- ARIA labels on all interactive elements
- Keyboard navigation support
- Screen reader friendly
- Focus management for modals

## Next Steps

1. **Continue migration**: Update remaining pages one by one
2. **Test thoroughly**: Ensure all toasts display correctly
3. **Gather feedback**: Get user feedback on the new notification system
4. **Fine-tune**: Adjust durations, colors, and positions as needed

## Support

For questions or issues:
1. Check this guide
2. Review implemented examples in UserManagement.cshtml or BookAppointment.cshtml
3. Consult toast.js for advanced features
