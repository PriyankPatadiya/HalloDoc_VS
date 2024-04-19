$(document).ready(function () {
    $.get('/Home/CheckSession', function (sessioncheck) {
        if (sessioncheck.sessionExists) {

            // Local Storage
            var currentpage = 1;
            var pagesize = 3;
            var path;
            var StatusButton;
            var span = localStorage.getItem("statusspanProvider");
            var statuslink = localStorage.getItem("statuslinkProvider");
            var triangleid = localStorage.getItem("triangleProvider");
            var trianglecolor = localStorage.getItem("colorProvider");
            var storedpartial = localStorage.getItem("providerpartialviewpath");
            path = storedpartial != null ? storedpartial : "Dashboard/_dashboardNew";


            $(".Status-btn").removeClass('activee');
            $(".triangle").css('display', 'none');
            span != null ? $("#statusspan").html(span) : $("#statusspan").html("(New)");
            statuslink != null ? $(statuslink).addClass("activee") : $("#statuslink1").addClass("activee");
            triangleid != null ? $(triangleid).css('display', 'block').css('border-top-color', trianglecolor) : $("#triangle1").css('display', 'block').css('border-top-color', '#203f9a');

            if (localStorage.getItem("statusbuttonProvider") != undefined) {
                StatusButton = localStorage.getItem("statusbuttonProvider");
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
                var storedpartial = localStorage.getItem("providerpartialviewpath");
                storedpartial = storedpartial == null ? "Dashboard/_dashboardNew" : storedpartial;
                ChangeTable(storedpartial, StatusButton, currentpage, pagesize);
            });

            $('.buttonOfFilter').click(function () {
                currentpage = 1;
                $('.buttonOfFilter').removeClass('active');
                $(this).addClass('active');
                var storedpartial = localStorage.getItem("providerpartialviewpath");
                storedpartial = storedpartial == null ? "Dashboard/_dashboardNew" : storedpartial;
                ChangeTable(storedpartial, StatusButton, currentpage, pagesize);
            });

            //Search filter
            $("#SearchString").on("input", function () {
                currentpage = 1;
                var storedpartial = localStorage.getItem("providerpartialviewpath");
                storedpartial = storedpartial == null ? "Dashboard/_dashboardNew" : storedpartial;
                ChangeTable(storedpartial, StatusButton, currentpage, pagesize);
            });

            // Status button 
            $("#SelectedStateId").on("change", function () {
                currentpage = 1;
                var storedpartial = localStorage.getItem("providerpartialviewpath");
                storedpartial = storedpartial == null ? "Dashboard/_dashboardNew" : storedpartial;
                ChangeTable(storedpartial, StatusButton, currentpage, pagesize);
            });

            // physician dropdown in assign case
            //$("#Regionid").on("change", function () {
            //    var RegionId = $("#Regionid").val();
            //    filterPhysicianByRegion(RegionId);
            //});

            //// same as above
            //$("#Regionnid").on("change", function () {
            //    var RegionId = $("#Regionnid").val();
            //    filterPhysicianByRegion(RegionId);
            //});


            //// enable inputs in close case
            //$('#closecaseeditbtn').on("click", function () {
            //    $('.inputclass').removeAttr("disabled");
            //    $('.hiddenbuttons').css('display', 'block');
            //    $('.buttonstohide').css('display', 'none');
            //});

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
                    partialviewpath = "Dashboard/_dashboardNew";
                    localStorage.setItem("providerpartialviewpath", partialviewpath);
                    localStorage.setItem("statusbuttonProvider", $("#statuslink1").data("id"));
                    localStorage.setItem("triangleProvider", "#triangle1");
                    localStorage.setItem("statuslinkProvider", "#statuslink1");
                    localStorage.setItem("colorProvider", "#203f9a");
                    localStorage.setItem("statusspanProvider", "(New)");

                    ChangeTable(partialviewpath, $("#statuslink1").data("id"), currentpage, pagesize);
                }
                else if (StatusButton == "2") {
                    $("#statuslink2").addClass("activee")
                    $("#statusspan").html("(Pending)");
                    $(".triangle").css('display', 'none');
                    $("#triangle2").css('display', 'block').css('border-top-color', '#00adef');
                    partialviewpath = "Dashboard/_dashboardPending";
                    localStorage.setItem("providerpartialviewpath", partialviewpath);
                    localStorage.setItem("statusbuttonProvider", $("#statuslink2").data("id"));
                    localStorage.setItem("triangleProvider", "#triangle2");
                    localStorage.setItem("statuslinkProvider", "#statuslink2");
                    localStorage.setItem("colorProvider", "#00adef");
                    localStorage.setItem("statusspanProvider", "(Pending)");
                    ChangeTable(partialviewpath, $("#statuslink2").data("id"), currentpage, pagesize);
                }
                else if (StatusButton == "3") {
                    $("#statuslink3").addClass("activee")
                    $("#statusspan").html("(Active)");
                    $(".triangle").css('display', 'none');
                    $("#triangle3").css('display', 'block').css('border-top-color', '#228c20');
                    partialviewpath = "Dashboard/_dashboardActive";
                    localStorage.setItem("providerpartialviewpath", partialviewpath);
                    localStorage.setItem("statusbuttonProvider", $("#statuslink3").data("id"), currentpage, pagesize);
                    localStorage.setItem("triangleProvider", "#triangle3");
                    localStorage.setItem("statuslinkProvider", "#statuslink3");
                    localStorage.setItem("colorProvider", "#228c20");
                    localStorage.setItem("statusspanProvider", "(Active)");
                    ChangeTable(partialviewpath, $("#statuslink3").data("id"), currentpage, pagesize);
                }
                else if (StatusButton == "4") {
                    $("#statuslink4").addClass("activee")
                    $("#statusspan").html("(Conclude)");
                    $(".triangle").css('display', 'none');
                    $("#triangle4").css('display', 'block').css('border-top-color', '#da0f82');
                    partialviewpath = "Dashboard/_dashboardConclude";
                    localStorage.setItem("providerpartialviewpath", partialviewpath);
                    localStorage.setItem("statusbuttonProvider", $("#statuslink4").data("id"), currentpage, pagesize);
                    localStorage.setItem("triangleProvider", "#triangle4");
                    localStorage.setItem("statusspanProvider", "(Conclude)");
                    localStorage.setItem("statuslinkProvider", "#statuslink4");
                    localStorage.setItem("colorProvider", "#da0f82");
                    ChangeTable(partialviewpath, $("#statuslink4").data("id"), currentpage, pagesize);
                }

            });


            // Export

            //$("#exportFilteredExcelBtn").click(function () {

            //    // Set the URL for file download
            //    var downloadUrl = "/AdminDashboard/exportfile?StatusButton=" + StatusButton + "&pagesize=" + pagesize + "&currentpage=" + currentpage;

            //    // Navigate to the download URL
            //    window.location.href = downloadUrl;
            //});

            //$("#exportAllExcelBtn").click(function () {
            //    var downloadUrl = "/AdminDashboard/exportAll?StatusButton=" + StatusButton;
            //    window.location.href = downloadUrl;
            //});

            // Main function that filters and load partial view in admin dashboard
            function ChangeTable(partialviewpath, StatusButton, currentpage, pagesize) {

                $.get('/Home/CheckSession', function (sessioncheck) {
                    if (sessioncheck.sessionExists) {

                        var Searchstring = $("#SearchString").val();
                        var selectButton = $(".buttonOfFilter.active").data("value");
                        //var token = getCookie("jwt");

                        if (Searchstring == "" && selectButton == " " && token == null) {
                            console.log('hii')
                            location.reload();
                        }

                        else {
                            $.ajax({
                                type: "GET",
                                url: "/ProviderDashboard/filterDashboardTable",
                                data: { Searchstring: Searchstring, selectButton: selectButton, StatusButton: StatusButton, partialviewpath: partialviewpath, currentpage: currentpage, pagesize: pagesize },

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

            //$("#submitBtnEncounterForm").click(function () {
            //    debugger;
            //    console.log("......")
            //});
            //function filterPhysicianByRegion(RegionId) {

            //    if (RegionId != "0") {
            //        $.ajax({
            //            type: "GET",
            //            url: "/AdminDashboard/filterPhyByRegion",
            //            data: { RegionId: RegionId },

            //            success: function (data) {
            //                $('#physicianDrop').empty();
            //                $('#physicianDrop').append($('<option>').text("Select Physician").attr('value', 0));
            //                $.each(data, function (index, item) {
            //                    $('#physicianDrop').append($('<option>').text(item.firstName).attr('value', item.physicianId));
            //                });
            //                $('#physicianDrop option:first').prop('selected', true);
            //            }
            //        });
            //    }
            //}


        }
        else {
            window.location.href = "/PatientLoginn";
        }
    });

});