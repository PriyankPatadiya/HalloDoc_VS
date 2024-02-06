function uploadfile() {
    document.getElementById("inputGroupFile04").click();
}
function myFucnction() {
    var element = document.body;
    element.classList.toggle("dark-mode");
    var lightbtn = document.getElementById("lightsvg");
    var darkbtn = document.getElementById("darksvg");
    const isdarkmode = element.classList.contains("dark-mode");

    if (isdarkmode) {
        darkbtn.style.display = 'none';
        lightbtn.style.display = 'block';
    }
    else {
        darkbtn.style.display = 'block';
        lightbtn.style.display = 'none';
    }
}