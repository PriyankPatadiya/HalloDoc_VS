
$(document).ready(function () {

    // Local Storage

    var path;
    var StatusButton;
    var span = localStorage.getItem("statusspan");
    var statuslink = localStorage.getItem("statuslink");
    var triangleid = localStorage.getItem("triangle");
    var trianglecolor = localStorage.getItem("color");

    if (localStorage.getItem("partialviewpath") != undefined) {
        path = localStorage.getItem("partialviewpath");
    }
    else {
        path = "AdminDashboardNew";
    }
    $(".Status-btn").removeClass('activee');
    $(".triangle").css('display', 'none');
    $("#statusspan").html(span);
    $(statuslink).addClass("activee");
    $(triangleid).css('display', 'block').css('border-top-color', trianglecolor);

    if (localStorage.getItem("statusbutton") != undefined) {
        StatusButton = localStorage.getItem("statusbutton");
    }
    else {
        StatusButton = "1";
    }
    ChangeTable(path, StatusButton);

    // Local Storage end // 

    $('.buttonOfFilter').click(function () { 
        $('.buttonOfFilter').removeClass('active');
        $(this).addClass('active');
        ChangeTable(path, StatusButton);
    });

    //Search filter
    $("#SearchString").on("input", function () {
        ChangeTable(path, StatusButton);
    });

    // Status button 
    $("#SelectedStateId").on("change", function () {  
        ChangeTable(path, StatusButton);
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

    // Send Order 
    $("#SelectProfession").on("change", function () {
        var ProfessionId = $("#SelectProfession").val();
        filterVendorsByProfession(ProfessionId);
    });

    // Send Order 
    $('#SelectBusiness').on("change", function () {
        var businessId = $('#SelectBusiness').val();
        getVendordata(businessId);
    });

    // enable inputs in close case
    $('#closecaseeditbtn').on("click", function () {
        $('.inputclass').removeAttr("disabled");
        $('.hiddenbuttons').css('display', 'block');
        $('.buttonstohide').css('display', 'none');
    });

    // Status buttons , set in local storage and some designs 
    $(".Status-btn").click(function () {
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

            ChangeTable("AdminDashboardNew", $("#statuslink1").data("id"));
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
            ChangeTable("AdminDashboardPending", $("#statuslink2").data("id"));
        }
        else if (StatusButton == "3") {
            $("#statuslink3").addClass("activee")
            $("#statusspan").html("(Active)");
            $(".triangle").css('display', 'none');
            $("#triangle3").css('display', 'block').css('border-top-color', '#228c20');
            partialviewpath = "AdminDashboardActive";
            localStorage.setItem("partialviewpath", partialviewpath);
            localStorage.setItem("statusbutton", $("#statuslink3").data("id"));
            localStorage.setItem("triangle", "#triangle3");
            localStorage.setItem("statuslink", "#statuslink3");
            localStorage.setItem("color", "#228c20");
            localStorage.setItem("statusspan", "(Active)");
            ChangeTable("AdminDashboardActive", $("#statuslink3").data("id"));
        }
        else if (StatusButton == "4") {
            $("#statuslink4").addClass("activee")
            $("#statusspan").html("(Conclude)");
            $(".triangle").css('display', 'none');
            $("#triangle4").css('display', 'block').css('border-top-color', '#da0f82');
            partialviewpath = "AdminDashboardConclude";
            localStorage.setItem("partialviewpath", partialviewpath);
            localStorage.setItem("statusbutton", $("#statuslink4").data("id"));
            localStorage.setItem("triangle", "#triangle4");
            localStorage.setItem("statusspan", "(Conclude)");
            localStorage.setItem("statuslink", "#statuslink4");
            localStorage.setItem("color", "#da0f82");
            ChangeTable("AdminDashboardConclude", $("#statuslink4").data("id"));
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
            ChangeTable("AdminDashboardToClose", $("#statuslink5").data("id"));
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
            ChangeTable("AdminDashboardUnpaid", $("#statuslink6").data("id"));
        }
    });

    // Main function that filters and load partial view in admin dashboard
    function ChangeTable(partialviewpath, StatusButton) {

         $.get('/AdminDashboard/CheckSession', function (sessioncheck) {
            if (sessioncheck.sessionExists) {

                var Searchstring = $("#SearchString").val();
                var selectButton = $(".buttonOfFilter.active").data("value");
                var SelectedStateId = $("#SelectedStateId").val();
                //var token = getCookie("jwt");

                if (Searchstring == " " && selectButton == " " && SelectedStateId == "0" && token == null) {
                    console.log('hii')
                    location.reload();
                }

                else {
                    $.ajax({
                        type: "GET",
                        url: "/AdminDashboard/SearchByName",
                        data: { Searchstring: Searchstring, selectButton: selectButton, StatusButton: StatusButton, SelectedStateId: SelectedStateId, partialviewpath: partialviewpath },

                        success: function (data) {
                            $(".SearchPartial").html(data);
                        },
                        failure: function (data) {
                            alert(data.d);
                        },
                        error: function (data) {
                            alert(data.d);
                        }
                    });

                }
            }
            else {
                window.location.href = "/PatientLoginn";
            }
        });
    }

    function filterPhysicianByRegion(RegionId) {
       
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

    function filterVendorsByProfession(ProfessionId) {

        if (ProfessionId != "0") {
            $.ajax({
                type: "GET",
                url: "/AdminDashboard/filterVenByPro",
                data: { ProfessionId: ProfessionId },

                success: function (data) {
                    $("#Vencontact").val('');
                    $("#Venemail").val('');
                    $("#VenFax").val('');   
                    $('#SelectBusiness').empty();
                    $('#SelectBusiness').append($('<option>').text("Business").attr('value', 0));
                    $.each(data, function (index, item) {
                        $('#SelectBusiness').append($('<option>').text(item.vendorName).attr('value', item.vendorId));
                    });
                    $('#SelectBusiness option:first').prop('selected', true);
                }
            });
        }
    }

    function getVendordata(businessId) {
        if (businessId != 0) {
            $.ajax({
                type: "GET",
                url: "/AdminDashboard/getvendordata",
                data: { businessId: businessId },
                success: function (data) {
                    console.log(data);
                    $("#Vencontact").val(data[0].businessContact);
                    $("#Venemail").val(data[0].email);
                    $("#VenFax").val(data[0].faxNumber);
                }
            });
        }
        else {
            $("#Vencontact").val('');
            $("#Venemail").val('');
            $("#VenFax").val('');
        }
    }
});

