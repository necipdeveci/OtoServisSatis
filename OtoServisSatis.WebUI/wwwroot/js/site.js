/**
 * Theme Manager - Handles dark/light theme switching
 * Features:
 * - Theme persistence via localStorage
 * - System preference detection
 * - Smooth transitions between themes
 * - Accessibility support
 */

class ThemeManager {
    constructor() {
        this.storageKey = 'app-theme-preference';
        this.darkTheme = 'dark';
        this.lightTheme = 'light';
        this.htmlElement = document.documentElement;
        this.init();
    }

    /**
     * Initialize theme manager
     */
    init() {
        // Apply theme before rendering (prevent flash)
        this.applyThemeSync(this.getPreferredTheme());

        // Setup event listeners after DOM is ready
        document.addEventListener('DOMContentLoaded', () => {
            this.setupEventListeners();
        });
    }

    /**
     * Get preferred theme from storage, system, or default to dark
     */
    getPreferredTheme() {
        try {
            const stored = localStorage.getItem(this.storageKey);
            if (stored) {
                return stored;
            }
        } catch (e) {
            console.warn('localStorage is not available', e);
        }

        const prefersLight = window.matchMedia('(prefers-color-scheme: light)').matches;
        return prefersLight ? this.lightTheme : this.darkTheme;
    }

    /**
     * Synchronously apply theme (prevents flash on page load)
     */
    applyThemeSync(theme) {
        const validTheme = theme === this.lightTheme ? this.lightTheme : this.darkTheme;
        this.htmlElement.setAttribute('data-theme', validTheme);
    }

    /**
     * Apply theme to HTML element and save preference
     */
    applyTheme(theme) {
        const validTheme = theme === this.lightTheme ? this.lightTheme : this.darkTheme;
        this.htmlElement.setAttribute('data-theme', validTheme);

        try {
            localStorage.setItem(this.storageKey, validTheme);
        } catch (e) {
            console.warn('Failed to save theme preference', e);
        }

        // Dispatch custom event for other scripts to listen
        window.dispatchEvent(new CustomEvent('themeChange', {
            detail: { theme: validTheme }
        }));
    }

    /**
     * Toggle between dark and light theme
     */
    toggleTheme() {
        const currentTheme = this.htmlElement.getAttribute('data-theme') || this.darkTheme;
        const newTheme = currentTheme === this.darkTheme ? this.lightTheme : this.darkTheme;
        this.applyTheme(newTheme);
    }

    /**
     * Set theme toggle button listener and update aria attributes
     */
    setupEventListeners() {
        const toggleButtons = document.querySelectorAll('.theme-toggle-btn');

        toggleButtons.forEach((btn) => {
            // Set initial aria-pressed state
            this.updateButtonState(btn);

            // Add click listener
            btn.addEventListener('click', () => {
                this.toggleTheme();
                this.updateButtonState(btn);
            });

            // Add keyboard support
            btn.addEventListener('keydown', (e) => {
                if (e.code === 'Enter' || e.code === 'Space') {
                    e.preventDefault();
                    btn.click();
                }
            });
        });

        // Listen for system theme changes
        const mediaQueryList = window.matchMedia('(prefers-color-scheme: light)');

        const handleSystemThemeChange = (e) => {
            try {
                if (!localStorage.getItem(this.storageKey)) {
                    this.applyTheme(e.matches ? this.lightTheme : this.darkTheme);
                }
            } catch (e) {
                console.warn('Failed to respond to system theme change', e);
            }
        };

        // Support both old and new API
        if (mediaQueryList.addEventListener) {
            mediaQueryList.addEventListener('change', handleSystemThemeChange);
        } else if (mediaQueryList.addListener) {
            mediaQueryList.addListener(handleSystemThemeChange);
        }
    }

    /**
     * Update button's aria attributes based on current theme
     */
    updateButtonState(btn) {
        const currentTheme = this.htmlElement.getAttribute('data-theme') || this.darkTheme;
        const isLight = currentTheme === this.lightTheme;

        btn.setAttribute('aria-pressed', isLight ? 'true' : 'false');
        btn.setAttribute('aria-label', isLight ? 'Dark mode' : 'Light mode');
        btn.title = isLight ? 'Switch to dark mode' : 'Switch to light mode';
    }

    /**
     * Get current theme
     */
    getCurrentTheme() {
        return this.htmlElement.getAttribute('data-theme') || this.darkTheme;
    }
}

// Initialize theme manager immediately (before DOMContentLoaded)
if (document.readyState === 'loading') {
    window.themeManager = new ThemeManager();
} else {
    window.themeManager = new ThemeManager();
}