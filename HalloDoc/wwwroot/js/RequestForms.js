
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

function CheckEmail() {
    var email = $('#email').val();
    $.ajax({
        type: "POST",
        url: '@Url.Action("CheckMail","PatientForms")',
        data: { email: email },


        success: function (response) {
            if (!response) {
                // Email not available.
                '@Url.Action("SendMail", "PatientForms")';
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
};

var input = document.getElementById("fileInput");
var info = document.getElementById("infoarea");

input.addEventListener('change', showFilename);

function showFilename() {
    var fileinp = event.srcElement;
    var fileName = fileinp.files[0].name;
    info.textContent = fileName;
}