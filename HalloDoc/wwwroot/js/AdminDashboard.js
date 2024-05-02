
$(document).ready(function () {

    // Local Storage
    var currentpage = 1;
    var pagesize = 3;
    var path;
    var StatusButton;
    var span = localStorage.getItem("statusspan");
    var statuslink = localStorage.getItem("statuslink");
    var triangleid = localStorage.getItem("triangle");
    var trianglecolor = localStorage.getItem("color");
    var storedpartial = localStorage.getItem("partialviewpath");
    path = storedpartial == null ? "AdminDashboardNew" : storedpartial;

    $(".Status-btn").removeClass('activee');
    $(".triangle").css('display', 'none');
    span != null ? $("#statusspan").html(span) : $("#statusspan").html("(New)");
    statuslink != null ? $(statuslink).addClass("activee") : $("#statuslink1").addClass("activee");
    triangleid != null ? $(triangleid).css('display', 'block').css('border-top-color', trianglecolor) : $("#triangle1").css('display', 'block').css('border-top-color', '#203f9a');

    if (localStorage.getItem("statusbutton") != undefined) {
        StatusButton = localStorage.getItem("statusbutton");
    }
    else {
        StatusButton = "1";
    }
    ChangeTable(path, StatusButton, currentpage, pagesize);

    // Local Storage end //

    $(document).on("click", "#pagination a.page-link", function () {
        console.log("Pagination link clicked!");
        var id = $(this).attr("id");
        currentpage = $("#" + id).data("page");
        console.log("Current Page: " + currentpage);
        var storedpartial = localStorage.getItem("partialviewpath");
        storedpartial = storedpartial != null ?  storedpartial : "AdminDashboardNew";
        ChangeTable(storedpartial, StatusButton, currentpage, pagesize);
    });

    $('.buttonOfFilter').click(function () {
        currentpage = 1;
        $('.buttonOfFilter').removeClass('active');
        $(this).addClass('active');
        var storedpartial = localStorage.getItem("partialviewpath");
        storedpartial = storedpartial != null ? storedpartial : "AdminDashboardNew";
        ChangeTable(storedpartial, StatusButton, currentpage, pagesize);
    });

    //Search filter
    $("#SearchString").on("keyup", function () {
        currentpage = 1;
        var storedpartial = localStorage.getItem("partialviewpath");
        storedpartial = storedpartial != null ? storedpartial : "AdminDashboardNew";
        ChangeTable(storedpartial, StatusButton, currentpage, pagesize);
    });

    // Status button 
    $("#SelectedStateId").on("change", function () {
        currentpage = 1;
        var storedpartial = localStorage.getItem("partialviewpath");
        storedpartial = storedpartial != null ? storedpartial : "AdminDashboardNew";
        ChangeTable(storedpartial, StatusButton, currentpage, pagesize);
    });

    // physician dropdown in assign case
    $("#Regionid").on("change", function () {
        var RegionId = $("#Regionid").val();
        filterPhysicianByRegion(RegionId);
    });


    // same as above
    $("#Regionnid").on("change", function () {
        var RegionId = $("#Regionnid").val();
        filterPhysicianByRegion(RegionId);
    });



    // enable inputs in close case
    $('#closecaseeditbtn').on("click", function () {
        $('.inputclass').removeAttr("disabled");
        $('.hiddenbuttons').css('display', 'block');
        $('.buttonstohide').css('display', 'none');
    });

    // Status buttons , set in local storage and some designs 
    $(".Status-btn").click(function () {
        currentpage = 1;
        $(".Status-btn").removeClass('active');
        $(".Status-btn").removeClass('activee');
        $(this).addClass('active');


        StatusButton = $(".Status-btn.active").data("id");


        if (StatusButton == "1") {
            $("#statuslink1").addClass("activee");
            $("#statusspan").html("(New)");
            $(".triangle").css('display', 'none');
            $("#triangle1").css('display', 'block').css('border-top-color', '#203f9a');
            partialviewpath = "AdminDashboardNew";
            localStorage.setItem("partialviewpath", partialviewpath);
            localStorage.setItem("statusbutton", $("#statuslink1").data("id"));
            localStorage.setItem("triangle", "#triangle1");
            localStorage.setItem("statuslink", "#statuslink1");
            localStorage.setItem("color", "#203f9a");
            localStorage.setItem("statusspan", "(New)");

            ChangeTable("AdminDashboardNew", $("#statuslink1").data("id"), currentpage, pagesize);
        }
        else if (StatusButton == "2") {
            $("#statuslink2").addClass("activee")
            $("#statusspan").html("(Pending)");
            $(".triangle").css('display', 'none');
            $("#triangle2").css('display', 'block').css('border-top-color', '#00adef');
            partialviewpath = "AdminDashboardPending";
            localStorage.setItem("partialviewpath", partialviewpath);
            localStorage.setItem("statusbutton", $("#statuslink2").data("id"));
            localStorage.setItem("triangle", "#triangle2");
            localStorage.setItem("statuslink", "#statuslink2");
            localStorage.setItem("color", "#00adef");
            localStorage.setItem("statusspan", "(Pending)");
            ChangeTable("AdminDashboardPending", $("#statuslink2").data("id"), currentpage, pagesize);
        }
        else if (StatusButton == "3") {
            $("#statuslink3").addClass("activee")
            $("#statusspan").html("(Active)");
            $(".triangle").css('display', 'none');
            $("#triangle3").css('display', 'block').css('border-top-color', '#228c20');
            partialviewpath = "AdminDashboardActive";
            localStorage.setItem("partialviewpath", partialviewpath);
            localStorage.setItem("statusbutton", $("#statuslink3").data("id"), currentpage, pagesize);
            localStorage.setItem("triangle", "#triangle3");
            localStorage.setItem("statuslink", "#statuslink3");
            localStorage.setItem("color", "#228c20");
            localStorage.setItem("statusspan", "(Active)");
            ChangeTable("AdminDashboardActive", $("#statuslink3").data("id"), currentpage, pagesize);
        }
        else if (StatusButton == "4") {
            $("#statuslink4").addClass("activee")
            $("#statusspan").html("(Conclude)");
            $(".triangle").css('display', 'none');
            $("#triangle4").css('display', 'block').css('border-top-color', '#da0f82');
            partialviewpath = "AdminDashboardConclude";
            localStorage.setItem("partialviewpath", partialviewpath);
            localStorage.setItem("statusbutton", $("#statuslink4").data("id"), currentpage, pagesize);
            localStorage.setItem("triangle", "#triangle4");
            localStorage.setItem("statusspan", "(Conclude)");
            localStorage.setItem("statuslink", "#statuslink4");
            localStorage.setItem("color", "#da0f82");
            ChangeTable("AdminDashboardConclude", $("#statuslink4").data("id"), currentpage, pagesize);
        }
        else if (StatusButton == "5") {
            $("#statuslink5").addClass("activee")
            $("#statusspan").html("(ToClose)");
            $(".triangle").css('display', 'none');
            $("#triangle5").css('display', 'block').css('border-top-color', '#0370d7');
            partialviewpath = "AdminDashboardToClose";
            localStorage.setItem("partialviewpath", partialviewpath);
            localStorage.setItem("statusbutton", $("#statuslink5").data("id"));
            localStorage.setItem("triangle", "#triangle5");
            localStorage.setItem("statusspan", "(Conclude)");
            localStorage.setItem("statuslink", "#statuslink5");
            localStorage.setItem("color", "#0370d7");
            ChangeTable("AdminDashboardToClose", $("#statuslink5").data("id"), currentpage, pagesize);
        }
        else {
            $("#statuslink6").addClass("activee");
            $("#statusspan").html("(Unpaid)");
            $(".triangle").css('display', 'none');
            $("#triangle6").css('display', 'block').css('border-top-color', '#9966cd');
            partialviewpath = "AdminDashboardUnpaid";
            localStorage.setItem("partialviewpath", partialviewpath);
            localStorage.setItem("statusbutton", $("#statuslink6").data("id"));
            localStorage.setItem("triangle", "#triangle6");
            localStorage.setItem("statusspan", "(Conclude)");
            localStorage.setItem("statuslink", "#statuslink6");
            localStorage.setItem("color", "#9966cd");
            ChangeTable("AdminDashboardUnpaid", $("#statuslink6").data("id"), currentpage, pagesize);
        }
    });


    // Export

    $("#exportFilteredExcelBtn").click(function () {

        // Set the URL for file download
        var downloadUrl = "/AdminDashboard/exportfile?StatusButton=" + StatusButton + "&pagesize=" + pagesize + "&currentpage=" + currentpage;

        // Navigate to the download URL
        window.location.href = downloadUrl;
    });

    $("#exportAllExcelBtn").click(function () {
        var downloadUrl = "/AdminDashboard/exportAll?StatusButton=" + StatusButton;
        window.location.href = downloadUrl;
    });

    // Main function that filters and load partial view in admin dashboard
    function ChangeTable(partialviewpath, StatusButton, currentpage, pagesize) {

        $.get('/Home/CheckSession', function (sessioncheck) {
            if (sessioncheck.sessionExists) {

                var Searchstring = $("#SearchString").val();
                var selectButton = $(".buttonOfFilter.active").data("value");
                var SelectedStateId = $("#SelectedStateId").val();
                //var token = getCookie("jwt");

                if (Searchstring == "" && selectButton == " " && SelectedStateId == "0" && token == null) {
                    console.log('hii')
                    location.reload();
                }

                else {
                    $.ajax({
                        type: "GET",
                        url: "/AdminDashboard/SearchByName",
                        data: { Searchstring: Searchstring, selectButton: selectButton, StatusButton: StatusButton, SelectedStateId: SelectedStateId, partialviewpath: partialviewpath, currentpage: currentpage, pagesize: pagesize },

                        success: function (data) {
                            $(".SearchPartial").html(data);
                        },
                        failure: function (data) {
                            $(".SearchPartial").html(data);
                        },
                        error: function (data) {
                            $(".SearchPartial").html(data);
                        }
                    });

                }
            }
            else {
                window.location.href = "/PatientLoginn";
            }
        });
    }
    // downloadpdf in encouter action which is finalized

    $("#submitBtnEncounterForm").click(function () {
        console.log("......")
    });
    function filterPhysicianByRegion(RegionId) {
        $.get('/Home/CheckSession', function (sessioncheck) {
            if (sessioncheck.sessionExists) {
                if (RegionId != "0") {
                    $.ajax({
                        type: "GET",
                        url: "/AdminDashboard/filterPhyByRegion",
                        data: { RegionId: RegionId },

                        success: function (data) {
                            $('#physicianDrop').empty();
                            $('#physicianDrop').append($('<option>').text("Select Physician").attr('value', 0));
                            $.each(data, function (index, item) {
                                $('#physicianDrop').append($('<option>').text(item.firstName).attr('value', item.physicianId));
                            });
                            $('#physicianDrop option:first').prop('selected', true);
                        }
                    });
                }

            }
            else {
                window.location.href = "/PatientLoginn";
            }
        });
    }

});

