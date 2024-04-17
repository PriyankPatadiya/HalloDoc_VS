
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
        var elements = document.querySelectorAll('.leaflet-layer, .leaflet-control-zoom-in, .leaflet-control-zoom-out, .leaflet-control-attribution');
        elements.forEach(function (element) {
            element.style.filter = "invert(100%) hue-rotate(180deg) brightness(95%) contrast(90%)";
        });
    } else {
        document.documentElement.setAttribute("data-bs-theme", "light");
        currentTheme = "light";
        var elements = document.querySelectorAll('.leaflet-layer, .leaflet-control-zoom-in, .leaflet-control-zoom-out, .leaflet-control-attribution');
        elements.forEach(function (element) {
            element.style.filter = "";
        });
    }
    // Store the theme in localStorage
    localStorage.setItem("theme", currentTheme);
});