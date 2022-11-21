const defaultTheme = require('tailwindcss/defaultTheme');

function dynamicHsl(h, s, l) {
    return ({ opacityVariable, opacityValue }) => {
        if (opacityValue !== undefined) {
            return `hsla(${h}, ${s}, ${l}, ${opacityValue})`
        }
        if (opacityVariable !== undefined) {
            return `hsla(${h}, ${s}, ${l}, var(${opacityVariable}, 1))`
        }
        return `hsl(${h}, ${s}, ${l})`
    }
}

function pallete(variableName) {
    const h = `var(${variableName}-h)`
    const s = `var(${variableName}-s)`
    const l = `var(${variableName}-l)`

    return {
        DEFAULT: dynamicHsl(h, s, l),
        100: dynamicHsl(h, s, `calc(${l} + 30%)`),
        200: dynamicHsl(h, s, `calc(${l} + 24%)`),
        300: dynamicHsl(h, s, `calc(${l} + 18%)`),
        400: dynamicHsl(h, s, `calc(${l} + 12%)`),
        500: dynamicHsl(h, s, `calc(${l} + 6%)`),
        600: dynamicHsl(h, s, l),
        700: dynamicHsl(h, s, `calc(${l} - 6%)`),
        800: dynamicHsl(h, s, `calc(${l} - 12%)`),
        900: dynamicHsl(h, s, `calc(${l} - 18%)`),
    }
}

module.exports = {
    prefix: 'cc-',
    important: true,
    content: [
        './**/*.html',
        './**/*.razor',
        './**/*.razor.cs'
    ],
    theme: {
        extend: {
            fontFamily: {
                sans: ['Inter var', ...defaultTheme.fontFamily.sans],
            },
            colors: {
                'brand-accent': "var(--cc-color-accent)",
                'brand-accent-dark': "var(--cc-color-accent-dark)",
                'brand-secondary': "var(--cc-color-secondary)",
                'brand-secondary-dark': "var(--cc-color-secondary-dark)",
                'brand-link': "var(--cc-color-link)",
                'brand-link-highlight': "var(--cc-color-link-highlight)",
                'brand-text': "var(--cc-color-text)",
                'brand-switch-active': "var(--cc-color-switch-active)",
                'brand-modal-background': "var(--cc-color-modal-background)",
            },
            keyframes: {
                'fade-out': {
                    'from': {
                        opacity: '1'
                    },
                    'to': {
                        opacity: '0'
                    },
                },
                'fade-in': {
                    '0%': {
                        opacity: '0'
                    },
                    '100%': {
                        opacity: '1'
                    },
                },
                'fade-move-out': {
                    'from': {
                        opacity: '1',
                        transform: 'translateY(0px)'
                    },
                    'to': {
                        opacity: '0',
                        transform: 'translateY(25px)'
                    },
                },
                'fade-move-in': {
                    '0%': {
                        opacity: '0',
                        transform: 'translateY(25px)'
                    },
                    '100%': {
                        opacity: '1',
                        transform: 'translateY(0)'
                    },
                }
            },
            animation: {
                'fade-out': 'fade-out 0.2s ease-out',
                'fade-in': 'fade-in 0.2s ease-out',
                'fade-move-out': 'fade-move-out 0.2s ease-out',
                'fade-move-in': 'fade-move-in 0.2s ease-out',
            }
        },
    },
    corePlugins: {
        preflight: false,
    }
}
