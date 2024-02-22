$(document).ready(function () {
    $('.buttonOfFilter').click(function () { 
        $('.buttonOfFilter').removeClass('active')
        $(this).addClass('active')
        ChangeTable();
    });

    $("#SearchString").on("input", function () {  
        ChangeTable();  
    });


    function ChangeTable() {

        var Searchstring = $("#SearchString").val();
        var selectButton = $(".buttonOfFilter.active").data("value");

        if (Searchstring == " " && selectButton == "") {
            console.log('hii')
            locatioen.reload();
        }
        else {
            $.ajax({
                type: "GET",
                url: "/AdminDashboard/SearchByName",
                data: { Searchstring: Searchstring, selectButton: selectButton },

                success: function (data) {
                    console.log(data)
                    $(".SearchPartial").html(data);
                },

            });
        }
    }
});

