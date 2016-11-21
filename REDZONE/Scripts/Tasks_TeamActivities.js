$(document).ready(function () {

    $(".menuItem").removeClass("menuSelected");    // Reset Menu Selections
    $('#mMyTasks').show();                      // Show a new Menu Item when this page is displayed
    $("#mMyTasks").addClass("menuSelected");

    //On Quick Review link click
    //------------------------------------------------------------------------
    $("#divTeamActivities").on('click', '.lnkAccountableUsers', function () {

        var tempScrollTop = $(window).scrollTop();

        var rz_bapm_id = $(this).parent().find("#hdnBapmId").val();

        // Make the Ajax Call to Return _ReasonDetail Partial View
        $.ajax({
            type: 'POST', // define the type of HTTP verb we want to use (POST for our form)
            //method    : 'POST',
            //url     : '@Url.Action("_metricPeriodDetails", "MetricPeriod")',
            url: '/Tasks/_accountableUsers', // the url where we want to POST
            //data    : { id: idToDisplay },     //<---- Data Parameters (if not already passed in the Url)
            data: { rz_bapm_id: rz_bapm_id }, // our data object created earlier
            //dataType  : 'json', // what type of data do we expect back from the server (Remove line if expecting html [partial or full]view result)
            //encode    : true,
            cache: false,
            //--- On error, execute this function ------
            error: function (xhr, status, error) {
                //var err = eval("(" + xhr.responseText + ")");
                $("#accountableUsersBody").html(xhr.responseText);
                //$("#msgLoading").hide();
                //alert("An Error has Occurred.");  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            //This code Executes After the Ajax call completes successfully
            //Insert the partial view retrieved into the output 'ReasonDetailsView' panel
            $("#accountableUsersBody").html(d);

            $(window).scrollTop(tempScrollTop);

            $('#accountableUsersModal').modal('show');
        });
    });


});


//------------------------------------------------------------------------------------------------
//-------------------------------------------FUNCTIONS--------------------------------------------
//------------------------------------------------------------------------------------------------
