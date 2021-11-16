const defaultTheme = require('tailwindcss/defaultTheme')
const colors = require('tailwindcss/colors')

module.exports = {
    prefix: 'cc:',
    purge: [
        './**/*.html',
        './**/*.razor'
    ],
    darkMode: false, // or 'media' or 'class'
    theme: {
        extend: {
            fontFamily: {
                sans: ['Inter var', ...defaultTheme.fontFamily.sans],
            },
            colors: {
                bluegray: colors.blueGray,
                coolgray: colors.coolGray
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
    variants: {
        extend: {
            opacity: ['disabled'],
            backgroundColor: ['disabled'],
            border: ['hover']
        }
    },
    corePlugins: {
        preflight: false,
    },
    plugins: [
        require('@tailwindcss/forms'),
        require('@tailwindcss/typography')
    ],
}
