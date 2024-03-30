/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,js}"],
  theme: {
    extend: {
      colors: {
        MyOrange: "#F2721E",
        MyPaleOrange: "#fef1e8",
        MyBlue: "#024FA0",
        MyVeryPaleGrey: "#f7f7f7",
        MyPaleGrey: "#f0ecec",
        MyGrey: "#d6d4d4",
        MyDarkGrey: "#8F8F8F",
        MyGreen: "#50B846"
      },
      boxShadow: {
        'normal': '0px 0px 20px #f0ecec',
        'orange': '8px -8px 5px #F2721E',
      },
    },
  },
  plugins: [],
}