/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        'italian-white': '#FFFFFF',
        'italian-red': '#CE2B37',
        'italian-yellow': '#FFD700',
        'italian-green': '#009246',
        'warm-yellow': '#FFF5B7',
        'light-red': '#FFF5F5',
        'hover-red': '#B91C1C',
        'hover-yellow': '#F59E0B',
      },
      fontFamily: {
        'italiana': ['Georgia', 'serif'],
        'modern': ['Inter', 'sans-serif'],
      },
    },
  },
  plugins: [],
}