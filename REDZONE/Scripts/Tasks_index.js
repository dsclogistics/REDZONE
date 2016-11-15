$(document).ready(function () {
    //$('.btn-toolbar .btn').each(function () {
    //    var $target = $(this).parentsUntil('btn-toolbar').next('.collapse');
    //    $target.attr("aria-expanded") ? $target.collapse('toggle') : $target.collapse();
    //    $(this).children('.glyphicon').toggleClass('glyphicon-chevron-right glyphicon-chevron-down');
    //});

    $(".menuItem").removeClass("menuSelected");    // Reset Menu Selections
    $('#mMyTasks').show();                      // Show a new Menu Item when this page is displayed
    $("#mMyTasks").addClass("menuSelected");


    $('.btn-toolbar').on('click', '.btn', function (e) {
        var $target = $(this).parentsUntil('btn-toolbar').next('.collapse');
        //alert($target.attr("aria-expanded"));
        $target.attr("aria-expanded") ? $target.collapse('toggle') : $target.collapse();
        $(this).children('.glyphicon').toggleClass('glyphicon-chevron-right glyphicon-chevron-down');
    });

    $('.panel-heading').on('click', '.glyph-link', function (e) {
        $(this).children('.glyphicon').toggleClass('glyphicon-chevron-right glyphicon-chevron-down');
    });

    //On Quick Review link click
    //------------------------------------------------------------------------
    $("#divReviewerTasks").on('click', '.lnkQuickReview', function () {

        var tempScrollTop = $(window).scrollTop();

        var productname = "Red Zone";
        var rz_bapm_id = $(this).parent().find("#hdnBapmId").val();

        // Make the Ajax Call to Return _ReasonDetail Partial View
        $.ajax({
            type: 'POST', // define the type of HTTP verb we want to use (POST for our form)
            //method    : 'POST',
            //url     : '@Url.Action("_metricPeriodDetails", "MetricPeriod")',
            url: '/Tasks/_quickReview', // the url where we want to POST
            //data    : { id: idToDisplay },     //<---- Data Parameters (if not already passed in the Url)
            data: { productname: productname, rz_bapm_id: rz_bapm_id }, // our data object created earlier
            //dataType  : 'json', // what type of data do we expect back from the server (Remove line if expecting html [partial or full]view result)
            //encode    : true,
            cache: false,
            //--- On error, execute this function ------
            error: function (xhr, status, error) {
                //var err = eval("(" + xhr.responseText + ")");
                $("#quickReviewBody").html(xhr.responseText);
                //$("#msgLoading").hide();
                //alert("An Error has Occurred.");  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            //This code Executes After the Ajax call completes successfully
            //Insert the partial view retrieved into the output 'ReasonDetailsView' panel
            $("#quickReviewBody").html(d);

            $(window).scrollTop(tempScrollTop);

            $('#quickReviewForm').modal('show');
        });
    });

    $('#quickReviewForm').on('click', '#btnRejectActionPlan', function () {
        var status = 'Rejected';

        if ($('#quickReviewComment').val().trim().length > 0) {
            submitAPReview(status);
        }
        else {
            showAlert("Reviewer comments are required when rejecting an action plan.", "");
        }
    });


    $('#quickReviewForm').on('click', '#btnApproveActionPlan', function () {
        var status = 'Approved';

        //buildSubmitAPReviewJSON(status);
        submitAPReview(status);
    });


});


//------------------------------------------------------------------------------------------------
//-------------------------------------------FUNCTIONS--------------------------------------------
//------------------------------------------------------------------------------------------------
function buildSubmitAPReviewJSON(status) {
    var productname = "Red Zone";
    var rz_apd_subm_app_user_id = $('#userId').val();
    var rz_apd_id = $('#apDetailId').val();
    var rz_apd_ap_review_text = $('#quickReviewComment').val();
    var rz_apd_ap_status = status;

    //If status is rejected, review text element is required (optional for Approved status)

    var jsonPayload = '{"productname":"' + productname + '","rz_apd_revw_app_user_id":"' + rz_apd_subm_app_user_id + '","rz_apd_id":"' + rz_apd_id + '","rz_apd_ap_review_text":"' + rz_apd_ap_review_text + '","rz_apd_ap_status":"' + rz_apd_ap_status + '"}';

    //alert("Json submitted:\n" + jsonPayloadDetail);
    return jsonPayload
}

function submitAPReview(status) {
    var payload = buildSubmitAPReviewJSON(status);
    var bapmId = $('#hdnWipBapmId').val();
    //alert(payload);

    $.ajax({
        url: '/ActionPlan/submitActionPlanReview',
        //url: 'http://dscapidev/dscmtrc/api/v1/metric/submitactionplanreview',
        method: "POST",
        cache: false,
        //type: "POST",
        //data: payload,
        data: { raw_json: payload },
        //contentType: "application/json; charset=utf-8",
        //dataType: "json",
        error: function (jqXHR, textStatus, errorThrown) {
            showAlert("Failed to Save Data. Ajax Failed!!<br/>Error:" + textStatus + "," + errorThrown, "danger"); //<-- Trap and alert of any errors if they occurred
        }
    }).done(function (d) {
        if (d == "Success") {
            $('.hdnBapmId').each(function (index) {
                if ($(this).val() == bapmId) {
                    $(this).parent().find('.lnkQuickReview').text('Review Complete');
                    $(this).parent().find('.lnkQuickReview').contents().unwrap();
                    //$(this).parent().find('.lnkQuickReview').removeClass('lnkQuickReview');
                    $(this).parent().siblings('.cellAPStatus').text(status);
                    
                    if (status == 'Rejected') {
                        $(this).parent().siblings('.cellAPStatus').css('color', 'red');
                    }
                    else if (status == 'Approved') {
                        $(this).parent().siblings('.cellAPStatus').css('color', 'green');
                    }

                    $('#quickReviewForm').modal('hide');
                    showAlert("Action Plan Review Submitted!", "");
                    return false;
                }
            });
            //location.reload();
        } else {
            alert("Error Saving the data!\n" + JSON.stringify(d));
        }
    });
}