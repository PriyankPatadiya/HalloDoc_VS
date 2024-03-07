
$(document).ready(function () {
    var partialviewpath = "AdminDashboardNew";
    var StatusButton = "1";
    

    $('.buttonOfFilter').click(function () { 
        $('.buttonOfFilter').removeClass('active')
        $(this).addClass('active')
        ChangeTable(partialviewpath);
    });

    $("#SearchString").on("input", function () {
        ChangeTable(partialviewpath);
    });

    $("#SelectedStateId").on("change", function () {  
        ChangeTable(partialviewpath);  
    });

    $("#Regionid").on("change", function () {
        var RegionId = $("#Regionid").val();
        filterPhysicianByRegion(RegionId);
    });

    $("#Regionnid").on("change", function () {
        var RegionId = $("#Regionnid").val();
        filterPhysicianByRegion(RegionId);
    });
    $("#SelectProfession").on("change", function () {
        var ProfessionId = $("#SelectProfession").val();
        filterVendorsByProfession(ProfessionId);
    });
    $('#SelectBusiness').on("change", function () {
        var businessId = $('#SelectBusiness').val();
        getVendordata(businessId);
    });
    $(".Status-btn").click(function () {
        $(".Status-btn").removeClass('active');
        $(this).addClass('active');
        

         StatusButton = $(".Status-btn.active").data("id");


        if (StatusButton == "1") {
            $("#statusspan").html("(New)");
            $(".triangle").css('display', 'none');
            $("#triangle1").css('display', 'block').css('border-top-color', '#203f9a');
            partialviewpath = "AdminDashboardNew"; 
            ChangeTable("AdminDashboardNew");
        }
        else if (StatusButton == "2") {
            $("#statusspan").html("(Pending)");
            $(".triangle").css('display', 'none');
            $("#triangle2").css('display', 'block').css('border-top-color', '#00adef');
            partialviewpath = "AdminDashboardPending";
            ChangeTable("AdminDashboardPending");
        }
        else if (StatusButton == "3") {
            $("#statusspan").html("(Active)");
            $(".triangle").css('display', 'none');
            $("#triangle3").css('display', 'block').css('border-top-color', '#228c20');
            partialviewpath = "AdminDashboardActive";
            ChangeTable("AdminDashboardActive");
        }
        else if (StatusButton == "4") {
            $("#statusspan").html("(Conclude)");
            $(".triangle").css('display', 'none');
            $("#triangle4").css('display', 'block').css('border-top-color', '#da0f82');
            partialviewpath = "AdminDashboardConclude";
            ChangeTable("AdminDashboardConclude");
        }
        else if (StatusButton == "5") {
            $("#statusspan").html("(ToClose)");
            $(".triangle").css('display', 'none');
            $("#triangle5").css('display', 'block').css('border-top-color', '#0370d7');
            partialviewpath = "AdminDashboardToClose";
            ChangeTable("AdminDashboardToClose");
        }
        else {
            $("#statusspan").html("(Unpaid)");
            $(".triangle").css('display', 'none');
            $("#triangle6").css('display', 'block').css('border-top-color', '#9966cd');
            partialviewpath = "AdminDashboardUnpaid";
            ChangeTable("AdminDashboardUnpaid");
        }
    });


    function ChangeTable(partialviewpath) {

        $.get('/AdminDashboard/CheckSession', function (sessioncheck) {
            if (sessioncheck.sessionExists) {

                var Searchstring = $("#SearchString").val();
                var selectButton = $(".buttonOfFilter.active").data("value");
                var StatusButton = $(".Status-btn.active").data("id");
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
                            console.log(data)
                            $(".SearchPartial").html(data);
                        },

                    });

                }
            }
            else {
                window.location.href = "/PatientLoginn";
            }
        });
    }

    function filterPhysicianByRegion(RegionId) {
       
        console.log("hello")
        console.log("hello")
        console.log("hello")
        if (RegionId != "0") {
            $.ajax({
                type: "GET",
                url: "/AdminDashboard/filterPhyByRegion",
                data: { RegionId: RegionId },

                success: function (data) {
                    $('#physicianDrop').empty();
                    $('#physicianDrop').append($('<option>').text("Physician").attr('value', 0));
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
    }
});

