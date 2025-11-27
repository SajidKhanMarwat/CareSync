/**
 * Modern Toast Notification System
 * Lightweight toast notifications for CareSync
 */

class ToastNotification {
    constructor() {
        this.container = null;
        this.init();
    }

    init() {
        // Create toast container if it doesn't exist
        if (!document.querySelector('.toast-container')) {
            this.container = document.createElement('div');
            this.container.className = 'toast-container';
            document.body.appendChild(this.container);
        } else {
            this.container = document.querySelector('.toast-container');
        }
    }

    /**
     * Show a toast notification
     * @param {Object} options - Toast options
     * @param {string} options.type - Type of toast (success, error, warning, info)
     * @param {string} options.title - Toast title
     * @param {string} options.message - Toast message
     * @param {number} options.duration - Duration in milliseconds (default: 5000)
     * @param {boolean} options.closable - Show close button (default: true)
     * @param {boolean} options.progress - Show progress bar (default: true)
     */
    show(options) {
        const {
            type = 'info',
            title = '',
            message = '',
            duration = 5000,
            closable = true,
            progress = true
        } = options;

        // Create toast element
        const toast = document.createElement('div');
        toast.className = `toast-notification ${type} ${!progress ? 'no-progress' : ''}`;
        
        // Icon mapping
        const icons = {
            success: 'ri-check-line',
            error: 'ri-close-line',
            warning: 'ri-error-warning-line',
            info: 'ri-information-line'
        };

        // Build toast HTML
        toast.innerHTML = `
            <div class="toast-icon">
                <i class="${icons[type]}"></i>
            </div>
            <div class="toast-content">
                ${title ? `<div class="toast-title">${this.escapeHtml(title)}</div>` : ''}
                ${message ? `<div class="toast-message">${this.escapeHtml(message)}</div>` : ''}
            </div>
            ${closable ? '<button class="toast-close" aria-label="Close"><i class="ri-close-line"></i></button>' : ''}
        `;

        // Add to container
        this.container.appendChild(toast);

        // Trigger animation
        setTimeout(() => toast.classList.add('show'), 10);

        // Close button handler
        if (closable) {
            const closeBtn = toast.querySelector('.toast-close');
            closeBtn.addEventListener('click', () => this.close(toast));
        }

        // Auto close
        if (duration > 0) {
            setTimeout(() => this.close(toast), duration);
        }

        return toast;
    }

    /**
     * Close a toast
     */
    close(toast) {
        toast.classList.remove('show');
        toast.classList.add('hide');
        setTimeout(() => {
            if (toast.parentElement) {
                toast.parentElement.removeChild(toast);
            }
        }, 300);
    }

    /**
     * Success toast
     */
    success(message, title = 'Success', duration = 5000) {
        return this.show({
            type: 'success',
            title: title,
            message: message,
            duration: duration
        });
    }

    /**
     * Error toast
     */
    error(message, title = 'Error', duration = 5000) {
        return this.show({
            type: 'error',
            title: title,
            message: message,
            duration: duration
        });
    }

    /**
     * Warning toast
     */
    warning(message, title = 'Warning', duration = 5000) {
        return this.show({
            type: 'warning',
            title: title,
            message: message,
            duration: duration
        });
    }

    /**
     * Info toast
     */
    info(message, title = 'Info', duration = 5000) {
        return this.show({
            type: 'info',
            title: title,
            message: message,
            duration: duration
        });
    }

    /**
     * Clear all toasts
     */
    clearAll() {
        const toasts = this.container.querySelectorAll('.toast-notification');
        toasts.forEach(toast => this.close(toast));
    }

    /**
     * Escape HTML to prevent XSS
     */
    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    /**
     * Confirm dialog with toast-style UI
     */
    confirm(options) {
        const {
            title = 'Confirm',
            message = 'Are you sure?',
            confirmText = 'Confirm',
            cancelText = 'Cancel',
            type = 'warning'
        } = options;

        return new Promise((resolve) => {
            // Create overlay
            const overlay = document.createElement('div');
            overlay.style.cssText = `
                position: fixed;
                top: 0;
                left: 0;
                right: 0;
                bottom: 0;
                background: rgba(0, 0, 0, 0.5);
                z-index: 10000;
                display: flex;
                align-items: center;
                justify-content: center;
                animation: fadeIn 0.2s;
            `;

            // Create modal
            const modal = document.createElement('div');
            modal.style.cssText = `
                background: white;
                border-radius: 8px;
                padding: 24px;
                max-width: 400px;
                width: 90%;
                box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
                animation: slideUp 0.3s;
            `;

            const icons = {
                success: 'ri-check-line',
                error: 'ri-close-line',
                warning: 'ri-error-warning-line',
                info: 'ri-information-line'
            };

            const colors = {
                success: '#28a745',
                error: '#dc3545',
                warning: '#ffc107',
                info: '#17a2b8'
            };

            modal.innerHTML = `
                <div style="display: flex; align-items: flex-start; gap: 16px; margin-bottom: 20px;">
                    <div style="font-size: 32px; color: ${colors[type]};">
                        <i class="${icons[type]}"></i>
                    </div>
                    <div style="flex: 1;">
                        <h5 style="margin: 0 0 8px 0; font-size: 18px; color: #2c3e50;">${this.escapeHtml(title)}</h5>
                        <p style="margin: 0; font-size: 14px; color: #6c757d; line-height: 1.5;">${this.escapeHtml(message)}</p>
                    </div>
                </div>
                <div style="display: flex; gap: 12px; justify-content: flex-end;">
                    <button class="cancel-btn" style="
                        padding: 8px 20px;
                        border: 1px solid #dee2e6;
                        background: white;
                        color: #6c757d;
                        border-radius: 4px;
                        cursor: pointer;
                        font-size: 14px;
                        transition: all 0.2s;
                    ">${this.escapeHtml(cancelText)}</button>
                    <button class="confirm-btn" style="
                        padding: 8px 20px;
                        border: none;
                        background: ${colors[type]};
                        color: white;
                        border-radius: 4px;
                        cursor: pointer;
                        font-size: 14px;
                        transition: all 0.2s;
                    ">${this.escapeHtml(confirmText)}</button>
                </div>
            `;

            overlay.appendChild(modal);
            document.body.appendChild(overlay);

            // Add animations
            const style = document.createElement('style');
            style.textContent = `
                @keyframes fadeIn {
                    from { opacity: 0; }
                    to { opacity: 1; }
                }
                @keyframes slideUp {
                    from { transform: translateY(20px); opacity: 0; }
                    to { transform: translateY(0); opacity: 1; }
                }
            `;
            document.head.appendChild(style);

            // Event handlers
            const close = (result) => {
                overlay.style.animation = 'fadeIn 0.2s reverse';
                setTimeout(() => {
                    document.body.removeChild(overlay);
                    document.head.removeChild(style);
                }, 200);
                resolve(result);
            };

            modal.querySelector('.confirm-btn').addEventListener('click', () => close(true));
            modal.querySelector('.cancel-btn').addEventListener('click', () => close(false));
            overlay.addEventListener('click', (e) => {
                if (e.target === overlay) close(false);
            });

            // Hover effects
            modal.querySelectorAll('button').forEach(btn => {
                btn.addEventListener('mouseenter', function() {
                    this.style.transform = 'translateY(-1px)';
                    this.style.boxShadow = '0 2px 8px rgba(0,0,0,0.15)';
                });
                btn.addEventListener('mouseleave', function() {
                    this.style.transform = 'translateY(0)';
                    this.style.boxShadow = 'none';
                });
            });
        });
    }

    /**
     * Prompt dialog with toast-style UI
     */
    async prompt(options) {
        const {
            title = 'Input Required',
            message = '',
            placeholder = '',
            defaultValue = '',
            confirmText = 'Submit',
            cancelText = 'Cancel',
            type = 'info',
            inputType = 'text'
        } = options;

        return new Promise((resolve) => {
            // Create overlay
            const overlay = document.createElement('div');
            overlay.style.cssText = `
                position: fixed;
                top: 0;
                left: 0;
                right: 0;
                bottom: 0;
                background: rgba(0, 0, 0, 0.5);
                z-index: 10000;
                display: flex;
                align-items: center;
                justify-content: center;
                animation: fadeIn 0.2s;
            `;

            const modal = document.createElement('div');
            modal.style.cssText = `
                background: white;
                border-radius: 8px;
                padding: 24px;
                max-width: 400px;
                width: 90%;
                box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
                animation: slideUp 0.3s;
            `;

            const icons = {
                success: 'ri-check-line',
                error: 'ri-close-line',
                warning: 'ri-error-warning-line',
                info: 'ri-information-line'
            };

            const colors = {
                success: '#28a745',
                error: '#dc3545',
                warning: '#ffc107',
                info: '#17a2b8'
            };

            modal.innerHTML = `
                <div style="display: flex; align-items: flex-start; gap: 16px; margin-bottom: 20px;">
                    <div style="font-size: 32px; color: ${colors[type]};">
                        <i class="${icons[type]}"></i>
                    </div>
                    <div style="flex: 1;">
                        <h5 style="margin: 0 0 8px 0; font-size: 18px; color: #2c3e50;">${this.escapeHtml(title)}</h5>
                        ${message ? `<p style="margin: 0 0 12px 0; font-size: 14px; color: #6c757d; line-height: 1.5;">${this.escapeHtml(message)}</p>` : ''}
                        <input type="${inputType}" class="prompt-input" value="${this.escapeHtml(defaultValue)}" placeholder="${this.escapeHtml(placeholder)}" style="
                            width: 100%;
                            padding: 8px 12px;
                            border: 1px solid #dee2e6;
                            border-radius: 4px;
                            font-size: 14px;
                            margin-top: 8px;
                        ">
                    </div>
                </div>
                <div style="display: flex; gap: 12px; justify-content: flex-end;">
                    <button class="cancel-btn" style="
                        padding: 8px 20px;
                        border: 1px solid #dee2e6;
                        background: white;
                        color: #6c757d;
                        border-radius: 4px;
                        cursor: pointer;
                        font-size: 14px;
                    ">${this.escapeHtml(cancelText)}</button>
                    <button class="confirm-btn" style="
                        padding: 8px 20px;
                        border: none;
                        background: ${colors[type]};
                        color: white;
                        border-radius: 4px;
                        cursor: pointer;
                        font-size: 14px;
                    ">${this.escapeHtml(confirmText)}</button>
                </div>
            `;

            overlay.appendChild(modal);
            document.body.appendChild(overlay);

            const input = modal.querySelector('.prompt-input');
            input.focus();

            const close = (value) => {
                overlay.style.animation = 'fadeIn 0.2s reverse';
                setTimeout(() => {
                    document.body.removeChild(overlay);
                }, 200);
                resolve(value);
            };

            modal.querySelector('.confirm-btn').addEventListener('click', () => close(input.value));
            modal.querySelector('.cancel-btn').addEventListener('click', () => close(null));
            input.addEventListener('keypress', (e) => {
                if (e.key === 'Enter') close(input.value);
            });
        });
    }
}

// Create global instance
window.Toast = new ToastNotification();

// Add convenient global functions
window.toast = {
    success: (message, title, duration) => window.Toast.success(message, title, duration),
    error: (message, title, duration) => window.Toast.error(message, title, duration),
    warning: (message, title, duration) => window.Toast.warning(message, title, duration),
    info: (message, title, duration) => window.Toast.info(message, title, duration),
    confirm: (options) => window.Toast.confirm(options),
    prompt: (options) => window.Toast.prompt(options),
    show: (options) => window.Toast.show(options),
    clear: () => window.Toast.clearAll()
};
