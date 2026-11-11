// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.querySelectorAll('input[type="email"]').forEach(input => {
    input.value = input.value.toLowerCase();
});

document.addEventListener('input', event => {
    const input = event.target;
    if (input instanceof HTMLInputElement && input.type === 'email') {
        const lowerCaseValue = input.value.toLowerCase();
        if (input.value !== lowerCaseValue) {
            const selectionStart = input.selectionStart;
            const selectionEnd = input.selectionEnd;
            input.value = lowerCaseValue;
            if (selectionStart !== null && selectionEnd !== null) {
                input.setSelectionRange(selectionStart, selectionEnd);
            }
        }
    }
});

document.addEventListener('submit', event => {
    const form = event.target;
    if (form instanceof HTMLFormElement) {
        form.querySelectorAll('input[type="email"]').forEach(input => {
            input.value = input.value.trim().toLowerCase();
        });
    }
});

const themeOptions = document.querySelectorAll('[data-theme-option]');
const themeToggle = document.querySelector('[data-theme-toggle]');
const themeStatus = document.getElementById('theme-status');

function updateThemeControls(theme) {
    themeOptions.forEach(option => {
        const isActive = option.dataset.themeOption === theme;
        option.classList.toggle('active', isActive);
        option.setAttribute('aria-pressed', String(isActive));
    });

    if (themeToggle) {
        const icon = themeToggle.querySelector('i');
        const label = theme === 'dark' ? 'Cambiar al tema claro' : 'Cambiar al tema oscuro';
        icon.className = theme === 'dark' ? 'bi bi-moon-stars-fill' : 'bi bi-sun-fill';
        themeToggle.setAttribute('aria-label', label);
        themeToggle.title = label;
    }
}

function setTheme(theme) {
    const selectedTheme = theme === 'dark' ? 'dark' : 'light';
    document.documentElement.dataset.theme = selectedTheme;
    updateThemeControls(selectedTheme);

    try {
        localStorage.setItem('sistema-asistencia-theme', selectedTheme);
        if (themeStatus) themeStatus.textContent = 'La apariencia se guardó en este dispositivo.';
    } catch {
        if (themeStatus) themeStatus.textContent = 'La apariencia se aplicará mientras esta página esté abierta.';
    }
}

updateThemeControls(document.documentElement.dataset.theme || 'light');

themeOptions.forEach(option => {
    option.addEventListener('click', () => setTheme(option.dataset.themeOption));
});

themeToggle?.addEventListener('click', () => {
    const currentTheme = document.documentElement.dataset.theme;
    setTheme(currentTheme === 'dark' ? 'light' : 'dark');
});

const navigationOptions = document.querySelectorAll('[data-nav-layout-option]');
const navigationStatus = document.getElementById('navigation-status');

function setNavigationLayout(layout) {
    const selectedLayout = layout === 'vertical' ? 'vertical' : 'horizontal';
    document.documentElement.dataset.navLayout = selectedLayout;
    navigationOptions.forEach(option => {
        const isActive = option.dataset.navLayoutOption === selectedLayout;
        option.classList.toggle('active', isActive);
        option.setAttribute('aria-pressed', String(isActive));
    });

    try {
        localStorage.setItem('sistema-asistencia-navigation', selectedLayout);
        if (navigationStatus) navigationStatus.textContent = 'La preferencia se guardó en este dispositivo. En pantallas pequeñas se mantiene el menú adaptable.';
    } catch {
        if (navigationStatus) navigationStatus.textContent = 'La preferencia se aplicará mientras esta página esté abierta.';
    }
}

setNavigationLayout(document.documentElement.dataset.navLayout || 'horizontal');
navigationOptions.forEach(option => {
    option.addEventListener('click', () => setNavigationLayout(option.dataset.navLayoutOption));
});

document.querySelectorAll('[data-section-toggle]').forEach(button => {
    button.addEventListener('click', () => {
        const sections = document.getElementById(button.dataset.sectionToggle);
        if (!sections) return;

        const isExpanded = button.getAttribute('aria-expanded') === 'true';
        const expanded = !isExpanded;
        sections.classList.toggle('show-all', expanded);
        button.setAttribute('aria-expanded', String(expanded));
        button.querySelector('span').textContent = expanded
            ? button.dataset.expandedText
            : button.dataset.collapsedText;
    });
});
