// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// JavaScript code to toggle between light and dark theme
const toggleButton = document.getElementById("theme-toggle");
let currentTheme = localStorage.getItem("theme") || "light";

console.log(toggleButton)
// Apply the stored theme
document.documentElement.setAttribute("data-bs-theme", currentTheme);

toggleButton.addEventListener("click", function () {

    console.log("nhcsdcjkd")
    if (currentTheme === "light") {
        document.documentElement.setAttribute("data-bs-theme", "dark");
        currentTheme = "dark";
    } else {
        document.documentElement.setAttribute("data-bs-theme", "light");
        currentTheme = "light";
    }
    // Store the theme in localStorage
    localStorage.setItem("theme", currentTheme);
});