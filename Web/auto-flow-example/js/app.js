// Application State Management
class CourseRegistrationApp {
    constructor() {
        this.currentTab = 'guide';
        this.formData = {
            basicInfo: {},
            courseSelection: {}
        };
        this.isBasicInfoCompleted = false;
        this.autoFillInProgress = false;
        this.autoFillData = this.loadAutoFillData();
        
        this.init();
    }

    init() {
        this.bindEvents();
        this.updateProgressBar();
        this.checkTabAccess();
    }

    bindEvents() {
        // Tab navigation
        document.querySelectorAll('.tab-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const targetTab = e.target.dataset.tab;
                if (!e.target.classList.contains('disabled')) {
                    this.switchTab(targetTab);
                }
            });
        });

        // Form submissions
        const basicInfoForm = document.getElementById('basicInfoForm');
        const courseSelectionForm = document.getElementById('courseSelectionForm');

        if (basicInfoForm) {
            basicInfoForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.handleBasicInfoSubmit();
            });
        }

        if (courseSelectionForm) {
            courseSelectionForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.handleCourseSelectionSubmit();
            });
        }

        // Real-time validation
        this.bindRealTimeValidation();
        
        // Course selection change event
        document.querySelectorAll('input[name="courseType"]').forEach(radio => {
            radio.addEventListener('change', () => {
                this.updateSummary();
            });
        });

        document.getElementById('timeSlot')?.addEventListener('change', () => {
            this.updateSummary();
        });

        document.getElementById('certificate')?.addEventListener('change', () => {
            this.updateSummary();
        });
    }

    bindRealTimeValidation() {
        // Basic Info Form validation
        const basicInfoInputs = document.querySelectorAll('#basicInfoForm input, #basicInfoForm select, #basicInfoForm textarea');
        basicInfoInputs.forEach(input => {
            input.addEventListener('blur', () => {
                this.validateField(input);
            });
            input.addEventListener('input', () => {
                this.clearFieldError(input);
            });
        });

        // Course Selection Form validation
        const courseSelectionInputs = document.querySelectorAll('#courseSelectionForm input, #courseSelectionForm select, #courseSelectionForm textarea');
        courseSelectionInputs.forEach(input => {
            input.addEventListener('blur', () => {
                this.validateField(input);
            });
            input.addEventListener('input', () => {
                this.clearFieldError(input);
            });
        });
    }

    switchTab(tabName) {
        // Update current tab
        this.currentTab = tabName;

        // Update tab buttons
        document.querySelectorAll('.tab-btn').forEach(btn => {
            btn.classList.remove('active');
            if (btn.dataset.tab === tabName) {
                btn.classList.add('active');
            }
        });

        // Update tab content
        document.querySelectorAll('.tab-content').forEach(content => {
            content.classList.remove('active');
        });
        document.getElementById(tabName).classList.add('active');

        // Update progress bar
        this.updateProgressBar();

        // Update summary if on course selection tab
        if (tabName === 'course-selection') {
            this.updateSummary();
        }
    }

    updateProgressBar() {
        const progressFill = document.getElementById('progressFill');
        let progress = 33.33; // Default for guide tab

        switch (this.currentTab) {
            case 'guide':
                progress = 33.33;
                break;
            case 'basic-info':
                progress = 66.66;
                break;
            case 'course-selection':
                progress = 100;
                break;
        }

        progressFill.style.width = `${progress}%`;
    }

    validateField(field) {
        const formGroup = field.closest('.form-group');
        const existingError = formGroup.querySelector('.error-message');
        
        // Remove existing error
        if (existingError) {
            existingError.remove();
        }
        formGroup.classList.remove('error');

        let isValid = true;
        let errorMessage = '';

        // Required field validation
        if (field.hasAttribute('required') && !field.value.trim()) {
            isValid = false;
            errorMessage = '此欄位為必填項目';
        }

        // Email validation
        if (field.type === 'email' && field.value.trim()) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(field.value.trim())) {
                isValid = false;
                errorMessage = '請輸入有效的電子郵件地址';
            }
        }

        // Phone validation
        if (field.type === 'tel' && field.value.trim()) {
            const phoneRegex = /^09\d{8}$/;
            if (!phoneRegex.test(field.value.trim().replace(/[-\s]/g, ''))) {
                isValid = false;
                errorMessage = '請輸入有效的手機號碼 (09xxxxxxxx)';
            }
        }

        // Display error if validation fails
        if (!isValid) {
            formGroup.classList.add('error');
            const errorElement = document.createElement('span');
            errorElement.className = 'error-message';
            errorElement.textContent = errorMessage;
            formGroup.appendChild(errorElement);
        }

        return isValid;
    }

    clearFieldError(field) {
        const formGroup = field.closest('.form-group');
        const existingError = formGroup.querySelector('.error-message');
        
        if (existingError) {
            existingError.remove();
        }
        formGroup.classList.remove('error');
    }

    validateForm(formId) {
        const form = document.getElementById(formId);
        const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');
        let isValid = true;

        inputs.forEach(input => {
            if (!this.validateField(input)) {
                isValid = false;
            }
        });

        // Special validation for radio groups
        if (formId === 'courseSelectionForm') {
            const courseTypeChecked = form.querySelector('input[name="courseType"]:checked');
            if (!courseTypeChecked) {
                isValid = false;
                // Add error styling to radio group
                const radioGroup = form.querySelector('.radio-group');
                if (radioGroup && !radioGroup.querySelector('.error-message')) {
                    const errorElement = document.createElement('span');
                    errorElement.className = 'error-message';
                    errorElement.textContent = '請選擇課程類別';
                    radioGroup.parentNode.appendChild(errorElement);
                }
            }
        }

        return isValid;
    }

    handleBasicInfoSubmit() {
        if (!this.validateForm('basicInfoForm')) {
            this.showNotification('請檢查並修正表單中的錯誤', 'error');
            return;
        }

        // Collect form data
        const formData = new FormData(document.getElementById('basicInfoForm'));
        this.formData.basicInfo = Object.fromEntries(formData);
        
        // Mark basic info as completed
        this.isBasicInfoCompleted = true;
        
        // Enable course selection tab
        this.enableCourseSelectionTab();
        
        // Show success message
        this.showNotification('基本資料已儲存，請繼續選擇課程', 'success');
        
        // Auto switch to course selection tab after a short delay
        setTimeout(() => {
            this.switchTab('course-selection');
        }, 1000);
    }

    handleCourseSelectionSubmit() {
        if (!this.validateForm('courseSelectionForm')) {
            this.showNotification('請檢查並修正表單中的錯誤', 'error');
            return;
        }

        // Show loading state
        const submitBtn = document.querySelector('#courseSelectionForm .final-submit');
        const originalText = submitBtn.textContent;
        submitBtn.innerHTML = '<span class="loading"></span>處理中...';
        submitBtn.disabled = true;

        // Collect form data
        const formData = new FormData(document.getElementById('courseSelectionForm'));
        this.formData.courseSelection = Object.fromEntries(formData);

        // Simulate form submission delay
        setTimeout(() => {
            // Reset button
            submitBtn.textContent = originalText;
            submitBtn.disabled = false;
            
            // Show success modal
            this.showSuccessModal();
        }, 2000);
    }

    enableCourseSelectionTab() {
        const courseSelectionBtn = document.querySelector('[data-tab="course-selection"]');
        courseSelectionBtn.classList.remove('disabled');
    }

    checkTabAccess() {
        if (!this.isBasicInfoCompleted) {
            const courseSelectionBtn = document.querySelector('[data-tab="course-selection"]');
            courseSelectionBtn.classList.add('disabled');
        }
    }

    updateSummary() {
        const summaryContent = document.getElementById('summaryContent');
        if (!summaryContent) return;

        const basicInfo = this.formData.basicInfo;
        const courseType = document.querySelector('input[name="courseType"]:checked')?.value;
        const timeSlot = document.getElementById('timeSlot')?.value;
        const certificate = document.getElementById('certificate')?.checked;

        let summaryHTML = '';

        // Basic info summary
        if (basicInfo.fullName) {
            summaryHTML += `<div class="summary-item">
                <span class="summary-label">姓名</span>
                <span class="summary-value">${basicInfo.fullName}</span>
            </div>`;
        }

        if (basicInfo.email) {
            summaryHTML += `<div class="summary-item">
                <span class="summary-label">電子郵件</span>
                <span class="summary-value">${basicInfo.email}</span>
            </div>`;
        }

        if (basicInfo.phone) {
            summaryHTML += `<div class="summary-item">
                <span class="summary-label">手機號碼</span>
                <span class="summary-value">${basicInfo.phone}</span>
            </div>`;
        }

        // Course selection summary
        if (courseType) {
            const courseNames = {
                'frontend': '前端開發',
                'backend': '後端開發',
                'uiux': 'UI/UX 設計'
            };
            summaryHTML += `<div class="summary-item">
                <span class="summary-label">課程類別</span>
                <span class="summary-value">${courseNames[courseType]}</span>
            </div>`;
        }

        if (timeSlot) {
            const timeSlotOptions = {
                'weekday-morning': '平日上午 (09:00-12:00)',
                'weekday-afternoon': '平日下午 (14:00-17:00)',
                'weekday-evening': '平日晚上 (19:00-22:00)',
                'weekend-morning': '假日上午 (09:00-12:00)',
                'weekend-afternoon': '假日下午 (14:00-17:00)'
            };
            summaryHTML += `<div class="summary-item">
                <span class="summary-label">上課時段</span>
                <span class="summary-value">${timeSlotOptions[timeSlot]}</span>
            </div>`;
        }

        if (certificate !== undefined) {
            summaryHTML += `<div class="summary-item">
                <span class="summary-label">課程證書</span>
                <span class="summary-value">${certificate ? '需要 (+NT$500)' : '不需要'}</span>
            </div>`;
        }

        summaryContent.innerHTML = summaryHTML;
    }

    showNotification(message, type = 'info') {
        // Create notification element
        const notification = document.createElement('div');
        notification.className = `notification notification-${type}`;
        notification.textContent = message;
        
        // Add styles
        Object.assign(notification.style, {
            position: 'fixed',
            top: '20px',
            right: '20px',
            padding: '1rem 1.5rem',
            borderRadius: '0.5rem',
            color: 'white',
            fontWeight: '500',
            zIndex: '1001',
            opacity: '0',
            transform: 'translateY(-10px)',
            transition: 'all 0.3s ease'
        });

        // Set background color based on type
        const colors = {
            success: '#10b981',
            error: '#ef4444',
            info: '#6366f1'
        };
        notification.style.backgroundColor = colors[type] || colors.info;

        // Add to page
        document.body.appendChild(notification);

        // Animate in
        setTimeout(() => {
            notification.style.opacity = '1';
            notification.style.transform = 'translateY(0)';
        }, 100);

        // Remove after delay
        setTimeout(() => {
            notification.style.opacity = '0';
            notification.style.transform = 'translateY(-10px)';
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.parentNode.removeChild(notification);
                }
            }, 300);
        }, 3000);
    }

    showSuccessModal() {
        const modal = document.getElementById('successModal');
        modal.style.display = 'block';
        
        // Add click outside to close
        modal.addEventListener('click', (e) => {
            if (e.target === modal) {
                this.closeModal();
            }
        });
    }

    closeModal() {
        const modal = document.getElementById('successModal');
        modal.style.display = 'none';
        
        // Reset form and redirect to guide
        this.resetApplication();
    }

    resetApplication() {
        // Reset state
        this.formData = {
            basicInfo: {},
            courseSelection: {}
        };
        this.isBasicInfoCompleted = false;
        
        // Reset forms
        document.getElementById('basicInfoForm').reset();
        document.getElementById('courseSelectionForm').reset();
        
        // Clear any error states
        document.querySelectorAll('.form-group.error').forEach(group => {
            group.classList.remove('error');
        });
        document.querySelectorAll('.error-message').forEach(error => {
            error.remove();
        });
        
        // Reset tabs
        this.checkTabAccess();
        this.switchTab('guide');
    }

    // Auto-fill functionality
    loadAutoFillData() {
        return {
            basicInfo: {
                steps: [
                    { type: 'click', selector: '#guide button', description: '點擊開始填寫按鈕' },
                    { type: 'change', selector: '#fullName', value: '王小明', description: '填寫姓名' },
                    { type: 'change', selector: '#email', value: 'xiaoming@example.com', description: '填寫電子郵件' },
                    { type: 'change', selector: '#phone', value: '0912345678', description: '填寫手機號碼' },
                    { type: 'change', selector: '#birthDate', value: '1990-01-01', description: '填寫生日' },
                    { type: 'change', selector: '#education', value: 'bachelor', description: '選擇教育程度' },
                    { type: 'change', selector: '#motivation', value: '希望透過學習提升職業技能，增加就業競爭力。', description: '填寫學習動機' }
                ]
            },
            courseSelection: {
                steps: [
                    { type: 'click', selector: '#frontend', description: '選擇前端開發課程' },
                    { type: 'change', selector: '#timeSlot', value: 'weekday-evening', description: '選擇上課時段' },
                    { type: 'click', selector: '#certificate', description: '選擇需要證書' },
                    { type: 'change', selector: '#specialRequests', value: '希望能夠安排實作專案練習時間。', description: '填寫特殊需求' }
                ]
            }
        };
    }

    async startAutoFill() {
        if (this.autoFillInProgress) return;
        
        this.autoFillInProgress = true;
        this.showAutoFillProgressBar();
        
        try {
            // Step 1: Basic Information
            await this.executeAutoFillStep('basic', this.autoFillData.basicInfo.steps);
            
            // Step 2: Course Selection
            await this.executeAutoFillStep('course', this.autoFillData.courseSelection.steps);
            
            this.showAutoFillToast(
                '自動填寫完成',
                '所有表單已自動填寫完成，請檢查資料是否正確。',
                [
                    { text: '確認資料', action: 'confirm' },
                    { text: '重新填寫', action: 'reset' }
                ]
            );
        } catch (error) {
            console.error('Auto-fill error:', error);
            this.showAutoFillToast(
                '自動填寫失敗',
                '自動填寫過程中發生錯誤，請手動填寫表單。',
                [{ text: '確定', action: 'close' }]
            );
        } finally {
            this.autoFillInProgress = false;
            this.hideAutoFillProgressBar();
        }
    }

    async executeAutoFillStep(stepName, steps) {
        this.updateAutoFillProgress(stepName, 'active');
        this.showAutoFillMask(`執行 ${stepName === 'basic' ? '基本資料' : '課程選擇'} 自動填寫...`);
        
        for (let i = 0; i < steps.length; i++) {
            const step = steps[i];
            await this.executeAutoFillAction(step);
            await this.delay(800); // 每個動作間隔800ms
        }
        
        // 如果是基本資料步驟，提交表單
        if (stepName === 'basic') {
            await this.delay(1000);
            const submitBtn = document.querySelector('#basicInfoForm .submit-btn');
            if (submitBtn) {
                submitBtn.click();
                await this.delay(1500); // 等待表單提交處理
            }
        }
        
        this.hideAutoFillMask();
        this.updateAutoFillProgress(stepName, 'completed');
        
        // 步驟完成後顯示確認toast
        this.showStepCompletionToast(stepName);
    }

    async executeAutoFillAction(step) {
        const element = document.querySelector(step.selector);
        if (!element) {
            console.warn(`Element not found: ${step.selector}`);
            return;
        }

        // 突出顯示當前操作的元素
        this.highlightElement(element);

        switch (step.type) {
            case 'click':
                element.click();
                break;
            case 'change':
                if (element.type === 'checkbox' || element.type === 'radio') {
                    element.checked = true;
                    element.dispatchEvent(new Event('change', { bubbles: true }));
                } else {
                    element.value = step.value;
                    element.dispatchEvent(new Event('input', { bubbles: true }));
                    element.dispatchEvent(new Event('change', { bubbles: true }));
                }
                break;
        }

        await this.delay(300);
        this.removeHighlight(element);
    }

    highlightElement(element) {
        element.style.transition = 'all 0.3s ease';
        element.style.boxShadow = '0 0 15px rgba(99, 102, 241, 0.6)';
        element.style.borderColor = 'var(--primary-color)';
    }

    removeHighlight(element) {
        setTimeout(() => {
            element.style.boxShadow = '';
            element.style.borderColor = '';
        }, 300);
    }

    showAutoFillProgressBar() {
        const progressBar = document.getElementById('autoFillProgressBar');
        progressBar.classList.remove('hidden');
        
        // 重置所有步驟狀態
        document.querySelectorAll('.auto-fill-step').forEach(step => {
            step.classList.remove('active', 'completed');
        });
    }

    hideAutoFillProgressBar() {
        setTimeout(() => {
            const progressBar = document.getElementById('autoFillProgressBar');
            progressBar.classList.add('hidden');
        }, 3000);
    }

    updateAutoFillProgress(stepName, status) {
        const step = document.querySelector(`[data-step="${stepName}"]`);
        if (step) {
            step.classList.remove('active', 'completed');
            step.classList.add(status);
        }
    }

    showAutoFillMask(message = '自動填寫中，請稍候...') {
        const mask = document.getElementById('autoFillMask');
        const messageElement = mask.querySelector('p');
        messageElement.textContent = message;
        mask.classList.remove('hidden');
    }

    hideAutoFillMask() {
        const mask = document.getElementById('autoFillMask');
        mask.classList.add('hidden');
    }

    showStepCompletionToast(stepName) {
        const stepNames = {
            'basic': '基本資料',
            'course': '課程選擇'
        };
        
        const message = stepName === 'basic' 
            ? '基本資料已自動填寫完成，請確認資料無誤後進行下一步。'
            : '課程選擇已自動填寫完成，請檢查所選課程和時段是否符合需求。';
            
        this.showAutoFillToast(
            `${stepNames[stepName]}完成`,
            message,
            stepName === 'basic' 
                ? [{ text: '確認並繼續', action: 'continue' }]
                : [{ text: '確認資料', action: 'confirm' }]
        );
    }

    showAutoFillToast(title, message, actions = []) {
        // 移除現有的toast
        const existingToast = document.querySelector('.auto-fill-toast');
        if (existingToast) {
            existingToast.remove();
        }

        // 創建新的toast
        const toast = document.createElement('div');
        toast.className = 'auto-fill-toast';
        
        const actionsHtml = actions.map(action => 
            `<button class="toast-btn ${action.action === 'confirm' || action.action === 'continue' ? 'primary' : 'secondary'}" 
                     onclick="handleToastAction('${action.action}')">${action.text}</button>`
        ).join('');

        toast.innerHTML = `
            <div class="toast-header">
                <div class="toast-icon success">✓</div>
                <div class="toast-title">${title}</div>
            </div>
            <div class="toast-message">${message}</div>
            <div class="toast-actions">
                ${actionsHtml}
            </div>
        `;

        document.body.appendChild(toast);
        
        // 顯示toast
        setTimeout(() => {
            toast.classList.add('show');
        }, 100);

        // 自動隱藏
        setTimeout(() => {
            this.hideAutoFillToast();
        }, 8000);
    }

    hideAutoFillToast() {
        const toast = document.querySelector('.auto-fill-toast');
        if (toast) {
            toast.classList.remove('show');
            setTimeout(() => {
                if (toast.parentNode) {
                    toast.parentNode.removeChild(toast);
                }
            }, 300);
        }
    }

    delay(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }
}

// Global functions for HTML onclick events
function switchTab(tabName) {
    if (window.app) {
        window.app.switchTab(tabName);
    }
}

function closeModal() {
    if (window.app) {
        window.app.closeModal();
    }
}

function startAutoFill() {
    if (window.app) {
        window.app.startAutoFill();
    }
}

function handleToastAction(action) {
    if (!window.app) return;
    
    switch (action) {
        case 'continue':
        case 'confirm':
            window.app.hideAutoFillToast();
            break;
        case 'reset':
            window.app.hideAutoFillToast();
            window.app.resetApplication();
            break;
        case 'close':
            window.app.hideAutoFillToast();
            break;
    }
}

// Initialize application when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.app = new CourseRegistrationApp();
});

// Handle page refresh - warn user about data loss
window.addEventListener('beforeunload', (e) => {
    if (window.app && (Object.keys(window.app.formData.basicInfo).length > 0 || Object.keys(window.app.formData.courseSelection).length > 0)) {
        e.preventDefault();
        e.returnValue = '您有未完成的表單資料，確定要離開嗎？';
        return e.returnValue;
    }
});