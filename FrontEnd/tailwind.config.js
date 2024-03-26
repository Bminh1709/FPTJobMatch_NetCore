/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,js}"],
  theme: {
    extend: {
      colors: {
        MyOrange: "#F2721E",
        MyPaleOrange: "#fef1e8",
        MyBlue: "#024FA0",
        MyPaleGrey: "#f0ecec"
      },
    },
  },
  plugins: [],
}