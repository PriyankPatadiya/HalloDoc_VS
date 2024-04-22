
$(document).ready(function () {
    var currentPage = 1;
    var pageSize = 5;
    filterPartial("0", currentPage);
    $("#SelectedStateId").on("change", function () {
        var stateid = $(this).val();
        filterPartial(stateid, "1");
    });

    function filterPartial(stateid, currentPage) {
        $.ajax({
            type: "GET",
            url: "/Admindashboard/filterProviderTable",
            data: { currentPage: currentPage, pageSize: pageSize, stateid: stateid },

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
    $(document).on("click", "#pagination a.page-link", function () {
        console.log("Pagination link clicked!");
        var id = $(this).attr("id");
        currentpage = $("#" + id).data("page");
        console.log("Current Page: " + currentpage);
        console.log($("#SelectedStateId").val());
        filterPartial($("#SelectedStateId").val(), currentpage);
    });
});