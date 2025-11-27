# Toast Notification - Quick Reference Card

## 🚀 Quick Start

### Basic Usage
```javascript
toast.success('Message', 'Title');
toast.error('Message', 'Title');
toast.warning('Message', 'Title');
toast.info('Message', 'Title');
```

### With Duration
```javascript
toast.success('Message', 'Title', 5000);  // 5 seconds
toast.error('Message', 'Title', 0);       // Permanent
```

## 🔄 Replace Patterns

### Alert → Toast
```javascript
// OLD
alert('User created successfully!');

// NEW
toast.success('User created successfully!', 'Success');
```

### Confirm → Toast Confirm
```javascript
// OLD
if (confirm('Delete user?')) {
    deleteUser();
}

// NEW
toast.confirm({
    title: 'Delete User',
    message: 'Delete this user?',
    type: 'error'
}).then(confirmed => {
    if (confirmed) deleteUser();
});
```

### Prompt → Toast Prompt
```javascript
// OLD
const name = prompt('Enter name:');
if (name) console.log(name);

// NEW
toast.prompt({
    title: 'Enter Name',
    message: 'Please enter your name:',
    type: 'info'
}).then(name => {
    if (name !== null) console.log(name);
});
```

## 📋 Common Scenarios

### Form Validation
```javascript
if (!email) {
    toast.warning('Email is required', 'Validation Error');
    return;
}

if (!emailRegex.test(email)) {
    toast.warning('Invalid email format', 'Invalid Email');
    return;
}

toast.success('Form submitted!', 'Success');
```

### API Calls
```javascript
try {
    const response = await fetch('/api/endpoint', { method: 'POST' });
    
    if (!response.ok) {
        toast.error('Server error', 'Error');
        return;
    }
    
    const result = await response.json();
    toast.success(result.message, 'Success');
    
} catch (error) {
    toast.error(error.message, 'Error');
}
```

### Delete Confirmation
```javascript
toast.confirm({
    title: 'Delete Item',
    message: 'This action cannot be undone.',
    type: 'error',
    confirmText: 'Delete',
    cancelText: 'Cancel'
}).then(confirmed => {
    if (confirmed) {
        // Perform delete
        toast.success('Item deleted', 'Success');
    }
});
```

### Bulk Actions
```javascript
const count = getSelectedCount();

if (count === 0) {
    toast.warning('Please select items first', 'No Selection');
    return;
}

toast.confirm({
    title: 'Bulk Action',
    message: `Apply to ${count} items?`,
    type: 'warning'
}).then(confirmed => {
    if (confirmed) {
        toast.success(`Updated ${count} items`, 'Success');
    }
});
```

## 🎨 Types & Colors

| Type | Color | Use For |
|------|-------|---------|
| `success` | Green | Successful operations |
| `error` | Red | Errors, failures |
| `warning` | Yellow | Warnings, validations |
| `info` | Blue | Information, progress |

## ⚙️ Advanced Options

### Full Options
```javascript
window.Toast.show({
    type: 'success',        // success, error, warning, info
    title: 'Title',         // Toast title
    message: 'Message',     // Toast message
    duration: 5000,         // Milliseconds (0 = permanent)
    closable: true,         // Show close button
    progress: true          // Show progress bar
});
```

### Confirm Options
```javascript
toast.confirm({
    title: 'Confirm',
    message: 'Are you sure?',
    type: 'warning',        // success, error, warning, info
    confirmText: 'Yes',     // Confirm button text
    cancelText: 'No'        // Cancel button text
}).then(result => { });
```

### Prompt Options
```javascript
toast.prompt({
    title: 'Input',
    message: 'Enter value:',
    placeholder: 'Type...',
    defaultValue: '',
    type: 'info',
    inputType: 'text'       // text, email, password, number
}).then(value => { });
```

## 🛠️ Utility Functions

```javascript
toast.clear();              // Clear all toasts
window.Toast.close(toast);  // Close specific toast
```

## 📝 Testing

Access demo page: `/toast-demo.html`

## 💡 Pro Tips

1. **Long messages**: Increase duration
   ```javascript
   toast.info('Long detailed message...', 'Info', 10000);
   ```

2. **Redirect after toast**: Add delay
   ```javascript
   toast.success('Saved!', 'Success');
   setTimeout(() => location.href = '/url', 1500);
   ```

3. **Clear before new toast**:
   ```javascript
   toast.clear();
   toast.error('New error', 'Error');
   ```

4. **Permanent toasts**: Set duration to 0
   ```javascript
   toast.error('Critical error', 'Error', 0);
   ```

## 📚 Full Documentation

See `TOAST_IMPLEMENTATION_GUIDE.md` for complete guide.

## 🐛 Troubleshooting

**Toast not showing?**
- Check if `toast.css` and `toast.js` are loaded
- Check browser console for errors
- Verify RemixIcon is loaded for icons

**Toast position wrong?**
- Edit `.toast-container` in `toast.css`
- Adjust `top` and `right` values

**Custom styling?**
- Edit colors in `toast.css`
- Modify CSS variables in `:root`
