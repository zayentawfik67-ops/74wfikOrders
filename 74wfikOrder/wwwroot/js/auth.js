// ═══════════════════════════════════════════════════════════
// 74wfik orders — Auth Page Interactivity
// Tab switching, password toggle, strength meter, validation
// ═══════════════════════════════════════════════════════════

document.addEventListener('DOMContentLoaded', function () {
    initTabs();
    initPasswordToggles();
    initPasswordStrength();
    initFormValidation();
});

// ── Tab Switching ──────────────────────────────────────────
function initTabs() {
    const tabs = document.querySelectorAll('.auth-tab');
    const indicator = document.querySelector('.tab-indicator');
    const loginPanel = document.getElementById('login-panel');
    const registerPanel = document.getElementById('register-panel');

    if (!tabs.length || !loginPanel || !registerPanel) return;

    tabs.forEach(tab => {
        tab.addEventListener('click', function () {
            const target = this.dataset.tab;

            tabs.forEach(t => t.classList.remove('active'));
            this.classList.add('active');

            if (target === 'register') {
                indicator.classList.add('register');
                loginPanel.classList.add('hidden');
                loginPanel.classList.remove('slide-in-left', 'slide-in-right');
                registerPanel.classList.remove('hidden');
                registerPanel.classList.add('slide-in-right');
            } else {
                indicator.classList.remove('register');
                registerPanel.classList.add('hidden');
                registerPanel.classList.remove('slide-in-left', 'slide-in-right');
                loginPanel.classList.remove('hidden');
                loginPanel.classList.add('slide-in-left');
            }
        });
    });
}

// ── Password Visibility Toggle ─────────────────────────────
function initPasswordToggles() {
    document.querySelectorAll('.password-toggle').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            const input = this.closest('.input-wrapper').querySelector('input');
            const isPassword = input.type === 'password';
            input.type = isPassword ? 'text' : 'password';
            this.innerHTML = isPassword
                ? '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17.94 17.94A10.07 10.07 0 0112 20c-7 0-11-8-11-8a18.45 18.45 0 015.06-5.94M9.9 4.24A9.12 9.12 0 0112 4c7 0 11 8 11 8a18.5 18.5 0 01-2.16 3.19m-6.72-1.07a3 3 0 11-4.24-4.24"/><line x1="1" y1="1" x2="23" y2="23"/></svg>'
                : '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>';
        });
    });
}

// ── Password Strength Meter ────────────────────────────────
function initPasswordStrength() {
    const passwordInput = document.getElementById('reg-password');
    const barFill = document.querySelector('.strength-bar-fill');
    const label = document.querySelector('.strength-label');

    if (!passwordInput || !barFill || !label) return;

    passwordInput.addEventListener('input', function () {
        const val = this.value;
        const result = calculateStrength(val);

        barFill.className = 'strength-bar-fill';
        label.className = 'strength-label';

        if (val.length === 0) {
            label.textContent = '';
            return;
        }

        barFill.classList.add(result.cls);
        label.classList.add(result.cls);
        label.textContent = result.text;
    });
}

function calculateStrength(password) {
    let score = 0;
    if (password.length >= 6) score++;
    if (password.length >= 10) score++;
    if (/[A-Z]/.test(password)) score++;
    if (/[0-9]/.test(password)) score++;
    if (/[^A-Za-z0-9]/.test(password)) score++;

    if (score <= 1) return { cls: 's-weak', text: 'Faible' };
    if (score <= 2) return { cls: 's-fair', text: 'Moyen' };
    if (score <= 3) return { cls: 's-good', text: 'Bon' };
    return { cls: 's-strong', text: 'Excellent' };
}

// ── Client-side Validation ─────────────────────────────────
function initFormValidation() {
    // Login form
    const loginForm = document.getElementById('login-form');
    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            let valid = true;
            const email = loginForm.querySelector('[name="Email"]');
            const password = loginForm.querySelector('[name="Password"]');

            clearFieldError(email);
            clearFieldError(password);

            if (!email.value.trim()) {
                showFieldError(email, "L'email est obligatoire");
                valid = false;
            } else if (!isValidEmail(email.value)) {
                showFieldError(email, "Format email invalide");
                valid = false;
            }

            if (!password.value) {
                showFieldError(password, "Le mot de passe est obligatoire");
                valid = false;
            }

            if (!valid) {
                e.preventDefault();
                return;
            }

            // Show loading state
            const btn = loginForm.querySelector('.auth-submit');
            btn.classList.add('loading');
        });
    }

    // Register form
    const regForm = document.getElementById('register-form');
    if (regForm) {
        regForm.addEventListener('submit', function (e) {
            let valid = true;
            const fields = {
                nom: regForm.querySelector('[name="Nom"]'),
                prenom: regForm.querySelector('[name="Prenom"]'),
                email: regForm.querySelector('[name="Email"]'),
                password: regForm.querySelector('[name="Password"]'),
                confirm: regForm.querySelector('[name="ConfirmPassword"]')
            };

            Object.values(fields).forEach(f => { if (f) clearFieldError(f); });

            if (!fields.nom.value.trim()) { showFieldError(fields.nom, "Le nom est obligatoire"); valid = false; }
            if (!fields.prenom.value.trim()) { showFieldError(fields.prenom, "Le prénom est obligatoire"); valid = false; }
            if (!fields.email.value.trim()) { showFieldError(fields.email, "L'email est obligatoire"); valid = false; }
            else if (!isValidEmail(fields.email.value)) { showFieldError(fields.email, "Format email invalide"); valid = false; }
            if (!fields.password.value) { showFieldError(fields.password, "Le mot de passe est obligatoire"); valid = false; }
            else if (fields.password.value.length < 6) { showFieldError(fields.password, "Minimum 6 caractères"); valid = false; }
            if (fields.password.value !== fields.confirm.value) { showFieldError(fields.confirm, "Les mots de passe ne correspondent pas"); valid = false; }

            if (!valid) {
                e.preventDefault();
                return;
            }

            const btn = regForm.querySelector('.auth-submit');
            btn.classList.add('loading');
        });
    }

    // Real-time validation on blur
    document.querySelectorAll('.input-wrapper input').forEach(input => {
        input.addEventListener('blur', function () {
            if (this.value.trim()) {
                this.classList.remove('input-error');
                this.classList.add('input-success');
            }
        });
        input.addEventListener('input', function () {
            this.classList.remove('input-error', 'input-success');
            const errorEl = this.closest('.form-group')?.querySelector('.field-error');
            if (errorEl) errorEl.remove();
        });
    });
}

function showFieldError(input, message) {
    input.classList.add('input-error');
    const existing = input.closest('.form-group')?.querySelector('.field-error');
    if (existing) existing.remove();
    const el = document.createElement('div');
    el.className = 'field-error';
    el.textContent = message;
    input.closest('.input-wrapper')?.after(el);
}

function clearFieldError(input) {
    input.classList.remove('input-error', 'input-success');
    const errorEl = input.closest('.form-group')?.querySelector('.field-error');
    if (errorEl) errorEl.remove();
}

function isValidEmail(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

// ── Switch to tab from footer links ─────────────────────────
function switchTab(tabName) {
    const tab = document.querySelector(`.auth-tab[data-tab="${tabName}"]`);
    if (tab) tab.click();
}
