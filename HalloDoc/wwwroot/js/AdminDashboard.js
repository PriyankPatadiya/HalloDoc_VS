
$(document).ready(function () {

    var partialviewpath = "AdminDashboardNew";
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
    $(".Status-btn").click(function () {
        $(".Status-btn").removeClass('active');
        $(this).addClass('active');
        

        var StatusButton = $(".Status-btn.active").data("id");

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

        var Searchstring = $("#SearchString").val();
        var selectButton = $(".buttonOfFilter.active").data("value");
        var StatusButton = $(".Status-btn.active").data("id");
        var SelectedStateId = $("#SelectedStateId").val();
        

        if (Searchstring == " " && selectButton == " " && SelectedStateId == "0") {
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
                    $.each(data, function (index, item) {
                        $('#physicianDrop').append($('<option>').text(item.firstName).attr('value', item.physicianId));
                    });
                    $('#physicianDrop option:first').prop('selected', true);
                }
            });
        }
    }
});

