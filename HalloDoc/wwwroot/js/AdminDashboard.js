﻿$(document).ready(function () {
    
    $('.buttonOfFilter').click(function () { 
        $('.buttonOfFilter').removeClass('active')
        $(this).addClass('active')
        ChangeTable();
    });

    $("#SearchString").on("input", function () {  
        ChangeTable();  
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
            $("#statuslink1").css('background-color', '#203f9a');
        }
        else if (StatusButton == "2") {
            $("#statusspan").html("(Pending)");
            $(".triangle").css('display', 'none');
            $("#triangle2").css('display', 'block').css('border-top-color', '#00adef');
            $("#statuslink2").css('background-color', '#00adef');
        }
        else if (StatusButton == "3") {
            $("#statusspan").html("(Active)");
            $(".triangle").css('display', 'none');
            $("#triangle3").css('display', 'block').css('border-top-color', '#228c20');
            $("#statuslink3").css('background-color', '#228c20');
        }
        else if (StatusButton == "4") {
            $("#statusspan").html("(Conclude)");
            $(".triangle").css('display', 'none');
            $("#triangle4").css('display', 'block').css('border-top-color', '#da0f82');
            $("#statuslink4").css('background-color', '#da0f82');
        }
        else if (StatusButton == "5") {
            $("#statusspan").html("(ToClose)");
            $(".triangle").css('display', 'none');
            $("#triangle5").css('display', 'block').css('border-top-color', '#0370d7');
            $("#statuslink5").css('background-color', '#0370d7');
        }
        else {
            $("#statusspan").html("(Unpaid)");
            $(".triangle").css('display', 'none');
            $("#triangle6").css('display', 'block').css('border-top-color', '#9966cd');
            $("#statuslink6").css('background-color', '#9966cd');
        }
    });

    function ChangeTable() {

        var Searchstring = $("#SearchString").val();
        var selectButton = $(".buttonOfFilter.active").data("value");
        var StatusButton = $(".Status-btn.active").data("id");

        if (Searchstring == " " && selectButton == "") {
            console.log('hii')
            locatioen.reload();
        }
        else {
            $.ajax({
                type: "GET",
                url: "/AdminDashboard/SearchByName",
                data: { Searchstring: Searchstring, selectButton: selectButton , StatusButton : StatusButton },

                success: function (data) {
                    console.log(data)
                    $(".SearchPartial").html(data);
                    
                },

            });
        }
    }


});
