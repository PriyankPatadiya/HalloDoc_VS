
    filterPartial("0");
    $("#SelectedStateId").on("change", function () {
        var stateid = $(this).val();
        filterPartial(stateid);   
    });

    function filterPartial(stateid) {
        $.ajax({
            type: "GET",
            url: "/Admindashboard/filterProviderTable",
            data: { stateid: stateid },

            success: function (data) {
                $("#ProviderPartialDiv").html(data);
            },
            failure: function (data) {
                alert(data.d);
            },
            error: function (data) {
                alert(data.d);
            }
        });
    }