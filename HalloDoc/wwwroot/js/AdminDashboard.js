
$(document).ready(function () {

    $('.buttonOfFilter').click(function () { 
        $('.buttonOfFilter').removeClass('active')
        $(this).addClass('active')
        ChangeTable();
    });

    $("#SearchString").on("input", function () {
        ChangeTable();
    });

    $("#SelectedStateId").on("change", function () {  
        ChangeTable();  
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
        ChangeTable();

        var StatusButton = $(".Status-btn.active").data("id");

        if (StatusButton == "1") {
            $("#statusspan").html("(New)");
            $(".triangle").css('display', 'none');
            $("#triangle1").css('display', 'block').css('border-top-color', '#203f9a');

        }
        else if (StatusButton == "2") {
            $("#statusspan").html("(Pending)");
            $(".triangle").css('display', 'none');
            $("#triangle2").css('display', 'block').css('border-top-color', '#00adef');

        }
        else if (StatusButton == "3") {
            $("#statusspan").html("(Active)");
            $(".triangle").css('display', 'none');
            $("#triangle3").css('display', 'block').css('border-top-color', '#228c20');

        }
        else if (StatusButton == "4") {
            $("#statusspan").html("(Conclude)");
            $(".triangle").css('display', 'none');
            $("#triangle4").css('display', 'block').css('border-top-color', '#da0f82');

        }
        else if (StatusButton == "5") {
            $("#statusspan").html("(ToClose)");
            $(".triangle").css('display', 'none');
            $("#triangle5").css('display', 'block').css('border-top-color', '#0370d7');

        }
        else {
            $("#statusspan").html("(Unpaid)");
            $(".triangle").css('display', 'none');
            $("#triangle6").css('display', 'block').css('border-top-color', '#9966cd');

        }
    });


    function ChangeTable() {

        var Searchstring = $("#SearchString").val();
        var selectButton = $(".buttonOfFilter.active").data("value");
        var StatusButton = $(".Status-btn.active").data("id");
        var SelectedStateId = $("#SelectedStateId").val();
        

        if (Searchstring == " " && selectButton == "" && SelectedStateId == "0") {
            console.log('hii')
            location.reload();
        }
        else {
            $.ajax({
                type: "GET",
                url: "/AdminDashboard/SearchByName",
                data: { Searchstring: Searchstring, selectButton: selectButton, StatusButton: StatusButton, SelectedStateId: SelectedStateId },

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

