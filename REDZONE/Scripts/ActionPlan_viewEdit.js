﻿//------------------------------------------------------------------------------------------------
//----------------------------------------LOCAL VARIABLES-----------------------------------------
//------------------------------------------------------------------------------------------------
function getMPid() { return localStorage.getItem("mpId"); }
function getBuildingName() { return localStorage.getItem("mpBuildingName"); }
function getMetricName() { return localStorage.getItem("mpName"); }
function getMPvalueId() { return localStorage.getItem("mpValueId"); }
function getMetricDate() { return localStorage.getItem("mpValueDate"); }

$(document).ready(function () {
    $("#metricName").html(getMetricName());
    $("#buildingName").html(getBuildingName());
    $("#metricDate").html(getMetricDate());

    //-----------------------------------------------------------------------------------------------
    //------------------------------------------BEHAVIOR---------------------------------------------
    //-----------------------------------------------------------------------------------------------
    $('#apText').on('keyup', function () {
        var characters = $(this).text().length;
        $('#apTextChars').text() = 'Characters:' + characters;
    })


    //-----------------------------------------------------------------------------------------------
    //-------------------------------------------ACTIONS---------------------------------------------
    //-----------------------------------------------------------------------------------------------
    $('.btn-toolbar').on('click', '.btn', function (e) {
        var $target = $(this).parent().next();
        //alert($target.attr("aria-expanded"));
        $target.attr("aria-expanded") ? $target.collapse('toggle') : $target.collapse();
        $(this).children('.glyphicon').toggleClass('glyphicon-chevron-right glyphicon-chevron-down');
    })

    $('#btnsReasons').on('click', '#btnEditReasons', function () {
        window.location.href = "/MPVreasons/Assigment/" + getMPvalueId() + "?mpId=" + getMPid();
    });

    $('#btnsActionPlan').on('click', '#btnSubmitActionPlan', function () {
        //var validated = validateReasonRow();
        //if (validated) updateMPReason();
        submitActionPlan();
    });

    $('#btnsReviewAP').on('click', '#btnRejectActionPlan', function () {
        var status = 'Rejected';

        //buildSubmitAPReviewJSON(status);
        submitAPReview(status);
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
});

//------------------------------------------------------------------------------------------------
//-------------------------------------------FUNCTIONS--------------------------------------------
//------------------------------------------------------------------------------------------------



//------------------------------------------------------------------------------------------------
function buildSubmitActionPlanJSON() {
    var productname = "Red Zone";
    var rz_bapm_id = $('#bapmId').val();
    var rz_apd_ap_ver = $('#apVersion').val();
    var rz_apd_subm_app_user_id = "1";
    var rz_apd_id = $('#apDetailId').val();
    var rz_apd_ap_text = $('#apText').text();

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
            alert("Action Plan Submitted!");
            location.reload();
        } else {
            alert("Error Saving the data!\n" + JSON.stringify(d));
        }
    });
}

function buildSaveActionPlanJSON() {
    var productname = "Red Zone";
    var rz_bapm_id = $('#bapmId').val();
    var rz_apd_ap_ver = $('#apVersion').val();
    var rz_apd_subm_app_user_id = "1";
    var rz_apd_id = $('#apDetailId').val();
    var rz_apd_ap_text = $('#apText').text();

    var jsonPayload = '{"productname":"' + productname + '", "rz_bapm_id":"' + rz_bapm_id + '", "rz_apd_ap_ver":"' + rz_apd_ap_ver + '","rz_apd_subm_app_user_id":"' + rz_apd_subm_app_user_id + '","rz_apd_id":"' + rz_apd_id + '","rz_apd_ap_text":"' + rz_apd_ap_text + '"}';
    //alert("Json submitted:\n" + jsonPayloadDetail);
    return jsonPayload
}

function saveActionPlan() {
    var payload = buildSaveActionPlanJSON();
    alert(payload);

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
            alert("Failed to Save Data. Ajax Failed!!\nError:" + textStatus + "," + errorThrown);  //<-- Trap and alert of any errors if they occurred
        }
    }).done(function (d) {
        if (d == "Success") {
            alert("Action Plan Saved!");
            location.reload();
        } else {
            alert("Error Saving the data!\n" + JSON.stringify(d));
        }
    });
}

function buildSubmitAPReviewJSON(status) {
    var productname = "Red Zone";
    var rz_apd_subm_app_user_id = "2";
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
            alert("Failed to Save Data. Ajax Failed!!\nError:" + textStatus + "," + errorThrown);  //<-- Trap and alert of any errors if they occurred
        }
    }).done(function (d) {
        if (d == "Success") {
            alert("Action Plan Review Submitted!");
            location.reload();
        } else {
            alert("Error Saving the data!\n" + JSON.stringify(d));
        }
    });
}