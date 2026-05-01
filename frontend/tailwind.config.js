/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#1a1a2e',
          light: '#2d2d44',
          dark: '#0f0f1a'
        },
        secondary: {
          DEFAULT: '#16213e',
          light: '#1f305a',
          dark: '#0d1524'
        },
        accent: {
          DEFAULT: '#e94560',
          light: '#ff6b7a',
          dark: '#c73e54'
        }
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', '-apple-system', 'sans-serif'],
      },
      boxShadow: {
        'soft': '0 2px 15px -3px rgba(0, 0, 0, 0.07), 0 10px 20px -2px rgba(0, 0, 0, 0.04)',
      }
    },
  },
  plugins: [],
}
