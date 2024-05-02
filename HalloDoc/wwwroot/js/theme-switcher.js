const currentTheme = localStorage.getItem("theme");
if (currentTheme) {
    document.documentElement.setAttribute("data-bs-theme", currentTheme);
}
$(window).ready(function () {
    const toggleButton = document.getElementById("theme-toggle");
    let currentTheme = localStorage.getItem("theme");
    currentTheme = currentTheme != null ? currentTheme : "light";

    document.documentElement.setAttribute("data-bs-theme", currentTheme);

    toggleButton.addEventListener("click", function () {
        if (currentTheme === "light") {
            document.documentElement.setAttribute("data-bs-theme", "dark");
            currentTheme = "dark";
        } else {
            document.documentElement.setAttribute("data-bs-theme", "light");
            currentTheme = "light";
        }
        localStorage.setItem("theme", currentTheme);
    });
}); 