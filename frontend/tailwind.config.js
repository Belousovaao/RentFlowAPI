/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#f0f5ff',
          100: '#e0edff',
          200: '#c7dafe',
          300: '#a5c6fd',
          400: '#82a9fc',
          500: '#6089f8',
          600: '#3b82f6',
          700: '#2563eb',
          800: '#1e40af',
          900: '#1e3a8a',
        },
      },
    },
  },
  plugins: [],
}
