$(document).ready(function () {

    filterPartial("0");

    $("#AccountTypeId").change(function () {
        var value = $(this).val();
        filterPartial(value);
        if (value == "1") {
            $("#providerBtn").css("display", "none");
            $("#adminBtn").css("display", "block");
        }
        else if (value == "3") {
            $("#adminBtn").css("display", "none");
            $("#providerBtn").css("display", "block");
        }
        else {
            $("#providerBtn").css("display", "block");
            $("#adminBtn").css("display", "block");
        }
    });

    function filterPartial(AccTypeId) {
        $.ajax({
            type: "GET",
            url: "/Admindashboard/filterUserAccessTable",
            data: { AccTypeId: AccTypeId },

            success: function (data) {
                $("#UserAccessPartialDiv").html(data);
            },
            failure: function (data) {
                alert(data.d);
            },
            error: function (data) {
                alert(data.d);
            }
        });
    }
});