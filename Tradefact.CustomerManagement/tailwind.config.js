const defaultTheme = require('tailwindcss/defaultTheme')

const fontFamily = defaultTheme.fontFamily;
fontFamily['sans'] = [
    '"Inter"', 
    'system-ui', // <-- Inter is a default sans font now
];

module.exports = {
    purge: [],
    darkMode: false, // or 'media' or 'class'
    theme: {
        fontFamily: fontFamily,
        extend: {
            colors: {
                'tradefact-blue': '#243c5a',
                'tradefact-red': '#243c5a',
                'tradefact-txt': '#243c5a'
            },
        },
    },
    variants: {
        extend: {},
    },
    plugins: [],
}