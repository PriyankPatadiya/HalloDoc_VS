
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