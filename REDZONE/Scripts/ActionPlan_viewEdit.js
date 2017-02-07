//------------------------------------------------------------------------------------------------
//----------------------------------------LOCAL VARIABLES-----------------------------------------
//------------------------------------------------------------------------------------------------
function getMPid() { return localStorage.getItem("mpId"); }
function getBuildingName() { return localStorage.getItem("mpBuildingName"); }
function getBuildingId() { return localStorage.getItem("buildingId"); }
function getMetricName() { return localStorage.getItem("mpName"); }
function getMPvalueId() { return localStorage.getItem("mpValueId"); }
function getMetricDate() { return localStorage.getItem("mpValueDate"); }
function getBapmId() { return localStorage.getItem("bapmId") }

$(document).ready(function () {
    //-----------------------------------------------------------------------------------------------
    //-----------------------------------------Initialize--------------------------------------------
    //-----------------------------------------------------------------------------------------------
    $(".menuItem").removeClass("menuSelected");    // Reset Menu Selections
    $('#mActionPlan').show();                      // Show a new Menu Item when this page is displayed
    $("#mActionPlan").addClass("menuSelected");


    //$("#metricName").html(getMetricName());
    //$("#buildingName").html(getBuildingName());
    //$(".mNameCell").html(getMetricName());
    if ($("#bapmId").val() == getBapmId()) {
        $("#btnBackToCurrentAP").hide();
    }
    //$("#metricDate").html(getMetricDate());
    //displayPriorActionPlans();

    //-----------------------------------------------------------------------------------------------
    //------------------------------------------BEHAVIOR---------------------------------------------
    //-----------------------------------------------------------------------------------------------
    //Inhibit browser Navigate and Prevent page to be closed if there is unsaved data
    $(window).bind('beforeunload', function () {
        if ($("#pageModified").val() == "Y") {
            //if the form has been changed
            return 'There are unsaved changed. Do you want to discard Changes and Exit?';
        }
        else { return undefined; }
    });

    $('.panel-heading').on('click', '.glyph-link', function (e) {
        $(this).find('.glyphicon').toggleClass('glyphicon-chevron-right glyphicon-chevron-down');
    });

    $('#apText').on('keyup', function () {
        var characters = $(this).val().length;
        //var linebreaks = ($(this).val().match(/\n/g) || []).length;
        $('#apTextChars').text(characters + ' / 2000');
    })

    $('#apText').change(function () {
        $('#pageModified').val("Y");
    });

    $('#apReviewComment').on('keyup', function () {
        var characters = $(this).text().length;
        $('#apReviewChars').text(characters + ' / 2000');
    })


    //-----------------------------------------------------------------------------------------------
    //-------------------------------------------ACTIONS---------------------------------------------
    //-----------------------------------------------------------------------------------------------
    $('.btn-toolbar').on('click', '.btn', function (e) {
        var $target = $(this).parentsUntil('btn-toolbar').next('.collapse');
        //alert($target.attr("aria-expanded"));
        $target.attr("aria-expanded") ? $target.collapse('toggle') : $target.collapse();
        $(this).children('.glyphicon').toggleClass('glyphicon-chevron-right glyphicon-chevron-down');
    })

    $('#divBackToCurrentAP').on('click', '#btnBackToCurrentAP', function () {
        if (getMPid() == null) {
            alert("Session variables were lost");
        } else {
            //alert(getBapmId());
            window.location.href = "/ActionPlan/viewEdit/?" + "mp_id=" + getMPid() + "&bldg_id=" + getBuildingId() + "&bapm_id=" + getBapmId();
        }
    });

    $('#btnsReasons').on('click', '#btnEditReasons', function () {
        //alert("Setting Local storage back URL: " + window.location.href);
        localStorage.setItem("backUrl", window.location.href);
        localStorage.setItem("mpvStatus", "Not Started");
        $('#pageModified').val("Y");
        mpvId = $('#mpvId').val();
        mpId = $('#mpId').val();

        //alert(localStorage.getItem("backUrl"));
        //alert("Metric Id is: " + getMPid());
        if (mpId == null || mpvId == null) {
            alert("Session variables were lost");
        }
        else {
            localStorage.setItem("mpId", mpId);
            localStorage.setItem("mpBuildingName", $("#buildingName").text());
            localStorage.setItem("mpName", $("#metricName").text());
            localStorage.setItem("mpGoal", $("#goalText").val());
            localStorage.setItem("mpValueId", mpvId);
            localStorage.setItem("mpValue", $("#mpValue").val());
            localStorage.setItem("mpValueDisplayClass", "Closed-Missed");
            localStorage.setItem("mpValueDate", $("#metricDate").text());
            localStorage.setItem("buildingId", $('#bldgId').val());
            localStorage.setItem("bapmId", $('#bapmId').val());
            //window.location.href = "/MPVreasons/Assigment/" + getMPvalueId() + "?mpId=" + getMPid() + "&returnUrl=" + document.URL;
            window.location.href = "/MPVreasons/Assigment/" + mpvId + "?mpId=" + mpId + "&buildingID=" + $('#bldgId').val();
        }
    });

    $('#btnsActionPlan').on('click', '#btnSubmitActionPlan', function () {
        if ($('#apText').val() == "") {
            showAlert("Action Plan text is required!", "", "N");
        }
        else {
            if (validateActionPlanSubmit()) {
                submitActionPlan()
            }
        }
    });

    $('#btnsReviewAP').on('click', '#btnRejectActionPlan', function () {
        var status = 'Rejected';

        //buildSubmitAPReviewJSON(status);
        //submitAPReview(status);

        if ($('#apReviewComment').val().trim().length > 0) {
            submitAPReview(status);
        }
        else {
            showAlert("Reviewer comments are required when rejecting an action plan.", "");
        }
    });


    $('#btnsReviewAP').on('click', '#btnApproveActionPlan', function () {
        var status = 'Approved';

        //buildSubmitAPReviewJSON(status);
        submitAPReview(status);
    });

    $('#btnsActionPlan').on('click', '#btnSaveActionPlan', function () {

        //alert(buildSaveActionPlanJSON());
        saveActionPlan();
    });

    $('#divPriorActionPlans').on('click', '#btnPriorActionPlanDetail', function () {
        //var bapm_id = $(this).parent().find('#priorBapmId').val();
        //var mpv_id = $(this).parent().find('#priorMpvId').val();
        //var mp_id = $(this).parent().find('#priorMpId').val();
        //var bldg_id = $(this).parent().find('#priorBldgId').val();
        //var metricDate = $(this).parent().find('#priorApMonth').val() + " " + $(this).parent().find('#priorApYear').val();

        //Before Navigating out of the page, prompt the user to save/discard changes if any
        if ($("#pageModified").val() == "Y") {
            //if ($("#bootstrapBtnReset").val() == "N") {
            //    $.fn.bootstrapBtn = $.fn.button.noConflict();  // To restore the "X" close button of the alert window. Must be done only once (at most) to prevent js errors
            //    $("#bootstrapBtnReset").val("Y");   // Set it to "Y" so we don't try to reset it again which would cause an error
            //}
            $("#dialog-confirm-exit #p_mp_id").val($(this).parent().find('#priorMpId').val());
            $("#dialog-confirm-exit #p_bldg_id").val($(this).parent().find('#priorBldgId').val());
            $("#dialog-confirm-exit #p_bapm_id").val($(this).parent().find('#priorBapmId').val());
            $('#dialog-confirm-exit').modal('show');

            //$("#dialog-confirm-exit").dialog({
            //    resizable: false,
            //    height: 250,
            //    width: 400,
            //    modal: true,
            //    buttons: {
            //        "Discard Changes": function () {
            //            $('#pageModified').val("N");  //So we don't get prompted again when leaving the page
            //            window.location.href = "/ActionPlan/viewEdit/?mp_id=" + mp_id + "&bldg_id=" + bldg_id + "&bapm_id=" + bapm_id;
            //        },
            //        "Continue Working": function () {
            //            $(this).dialog("close");
            //        }
            //    }
            //});
        }
        else {
            var mp_id = $(this).parent().find('#priorMpId').val();
            var bldg_id = $(this).parent().find('#priorBldgId').val();
            var bapm_id = $(this).parent().find('#priorBapmId').val();
            //alert("Navigating out using...\nmp_id: " + mp_id + "\nbldg_id: " + bldg_id + "\nbapm_id: " + bapm_id);
            window.location.href = "/ActionPlan/viewEdit/?mp_id=" + mp_id + "&bldg_id=" + bldg_id + "&bapm_id=" + bapm_id;
        }
    });

    $('#btnDiscard').click(function () {
        var p1 = $("#dialog-confirm-exit #p_mp_id").val();
        var p2 = $("#dialog-confirm-exit #p_bldg_id").val();
        var p3 = $("#dialog-confirm-exit #p_bapm_id").val();
        //alert("Changes have been Discarded! Navigating out using...\nmp_id: " + p1 + "\nbldg_id: " + p2 + "\nbapm_id: " + p3);
        $('#pageModified').val("N");  //So we don't get prompted again when leaving the page
        window.location.href = "/ActionPlan/viewEdit/?mp_id=" + p1 + "&bldg_id=" + p2 + "&bapm_id=" + p3;
    });

    $('#lnkBack').click(function () {
        //alert("Local Storage Return URL is: " + localStorage.getItem("backUrl"));
        //window.location.href = decodeURIComponent(localStorage.getItem("backUrl"));
        window.location.href = decodeURIComponent(localStorage.getItem("backUrl"));
    });
    //$('.btnViewDetails').click(function () {
    //});
});

//------------------------------------------------------------------------------------------------
//-------------------------------------------FUNCTIONS--------------------------------------------
//------------------------------------------------------------------------------------------------
function validateActionPlanSubmit() {
    return confirm("Are you sure you want to submit this action plan? \nYou will not be able to make changes afterwards.") ? true : false;
}

function buildSubmitActionPlanJSON() {
    var productname = "Red Zone";
    var rz_bapm_id = $('#bapmId').val();
    var rz_apd_ap_ver = $('#apVersion').val();
    var rz_apd_subm_app_user_id = $('#userId').val();
    var rz_apd_id = $('#apDetailId').val();
    var rz_apd_ap_text = $('#apText').val().replace('\n', '\\n');

    var jsonPayload = '{"productname":"' + productname + '", "rz_bapm_id":"' + rz_bapm_id + '", "rz_apd_ap_ver":"' + rz_apd_ap_ver + '","rz_apd_subm_app_user_id":"' + rz_apd_subm_app_user_id + '","rz_apd_id":"' + rz_apd_id + '","rz_apd_ap_text":"' + rz_apd_ap_text + '"}';
    //alert("Json submitted:\n" + jsonPayloadDetail);
    return jsonPayload
}

function submitActionPlan() {
    var payload = buildSubmitActionPlanJSON();
    //alert(payload);

    $.ajax({
        url: '/ActionPlan/submitActionPlan',
        //url: 'http://dscapidev/dscmtrc/api/v1/metric/submitactionplan',
        method: "POST",
        cache: false,
        //type: "POST",
        //data: payload,
        data: { raw_json: payload },
        //contentType: "application/json; charset=utf-8",
        //dataType: "json",
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Failed to Save Data. Ajax Failed!!\nError:" + textStatus + "," + errorThrown);  //<-- Trap and alert of any errors if they occurred
        }
    }).done(function (d) {
        if (d == "Success") {
            $('#pageModified').val("N");
            showAlert("Action Plan Submitted Successfully!", "", "Y");
            //location.reload();
        } else {
            alert('<div class="alert-danger">Error Saving the Data!<br />' + JSON.stringify(d) + '</div>');
        }
    });
}

function buildSaveActionPlanJSON() {
    var productname = "Red Zone";
    var rz_bapm_id = $('#bapmId').val();
    var rz_apd_ap_ver = $('#apVersion').val();
    var rz_apd_subm_app_user_id = $('#userId').val();
    var rz_apd_id = $('#apDetailId').val();
    var rz_apd_ap_text = $('#apText').val().replace('\n', '\\n');

    var jsonPayload = '{"productname":"' + productname + '", "rz_bapm_id":"' + rz_bapm_id + '", "rz_apd_ap_ver":"' + rz_apd_ap_ver + '","rz_apd_subm_app_user_id":"' + rz_apd_subm_app_user_id + '","rz_apd_id":"' + rz_apd_id + '","rz_apd_ap_text":"' + rz_apd_ap_text + '"}';
    //alert("Json submitted:\n" + jsonPayloadDetail);
    return jsonPayload
}

function saveActionPlan() {
    var payload = buildSaveActionPlanJSON();
    //alert(payload);

    $.ajax({
        url: '/ActionPlan/saveActionPlan',
        //url: 'http://dscapidev/dscmtrc/api/v1/metric/saveactionplan',
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
            $('#pageModified').val("N");
            showAlert("Action Plan Saved!", "", "Y");
            //location.reload();
        } else {
            alert("Error Saving the data!\n" + JSON.stringify(d));
        }
    });
}

function buildSubmitAPReviewJSON(status) {
    var productname = "Red Zone";
    var rz_apd_subm_app_user_id = $('#userId').val();
    var rz_apd_id = $('#apDetailId').val();
    var rz_apd_ap_review_text = $('#apReviewComment').val();
    var rz_apd_ap_status = status;

    //If status is rejected, review text element is required (optional for Approved status)

    var jsonPayload = '{"productname":"' + productname + '","rz_apd_revw_app_user_id":"' + rz_apd_subm_app_user_id + '","rz_apd_id":"' + rz_apd_id + '","rz_apd_ap_review_text":"' + rz_apd_ap_review_text + '","rz_apd_ap_status":"' + rz_apd_ap_status + '"}';

    //alert("Json submitted:\n" + jsonPayloadDetail);
    return jsonPayload
}

function submitAPReview(status) {
    var payload = buildSubmitAPReviewJSON(status);
    var submitter = $('#hdnSubmitter').val();
    var metric = $('#metricName').text();
    var building = $('#buildingName').text();
    //alert(submitter);
    //alert(status);
    //alert(payload);

    $.ajax({
        url: '/ActionPlan/submitActionPlanReview',
        //url: 'http://dscapidev/dscmtrc/api/v1/metric/submitactionplanreview',
        method: "POST",
        cache: false,
        //type: "POST",
        //data: payload,
        data: { raw_json: payload, submitter: submitter, reviewResult: status, metric: metric, building: building },
        //contentType: "application/json; charset=utf-8",
        //dataType: "json",
        error: function (jqXHR, textStatus, errorThrown) {
            showAlert("Failed to Save Data. Ajax Failed!!<br/>Error:" + textStatus + "," + errorThrown, "danger"); //<-- Trap and alert of any errors if they occurred
        }
    }).done(function (d) {
        if (d == "Success") {
            showAlert("Action Plan Review Submitted!", "", "Y");
            //location.reload();
        } else {
            alert("Error Saving the data!\n" + JSON.stringify(d));
        }
    });
}

//------------------------------------------------------------------------------------------------
//---------------------------------------HELPER FUNCTIONS-----------------------------------------
//------------------------------------------------------------------------------------------------
function monthToInt(monthName)
{
    var monthNo = 0;
    switch (monthName)
    {
        case "January": monthNo = 1;
            break;
        case "February": monthNo = 2;
            break;
        case "March": monthNo = 3;
            break;
        case "April": monthNo = 4;
            break;
        case "May": monthNo = 5;
            break;
        case "June": monthNo = 6;
            break;
        case "July": monthNo = 7;
            break;
        case "August": monthNo = 8;
            break;
        case "September": monthNo = 9;
            break;
        case "October": monthNo = 10;
            break;
        case "November": monthNo = 11;
            break;
        case "December": monthNo = 12;
            break;
        default:
            break;
    }
return monthNo;
}