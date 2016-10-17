//------------------------------------------------------------------------------------------------
//----------------------------------------LOCAL VARIABLES-----------------------------------------
//------------------------------------------------------------------------------------------------
function getMPid() { return localStorage.getItem("mpId"); }
function getBuildingName() { return localStorage.getItem("mpBuildingName"); }
function getBuildingId() { return localStorage.getItem("buildingId"); }
function getMetricName() { return localStorage.getItem("mpName"); }
function getMPvalueId() { return localStorage.getItem("mpValueId"); }
function getMetricDate() { return localStorage.getItem("mpValueDate"); }

$(document).ready(function () {
    //-----------------------------------------------------------------------------------------------
    //-----------------------------------------Initialize--------------------------------------------
    //-----------------------------------------------------------------------------------------------
    $("#metricName").html(getMetricName());
    $("#buildingName").html(getBuildingName());
    $(".mNameCell").html(getMetricName());
    alert($("#metricDate").text);
    if ($("#metricDate").text = "") { $("#metricDate").html(getMetricDate()); }
    //$("#metricDate").html(getMetricDate());
    //displayPriorActionPlans();

    //-----------------------------------------------------------------------------------------------
    //------------------------------------------BEHAVIOR---------------------------------------------
    //-----------------------------------------------------------------------------------------------

    $('#apText').on('keyup', function () {
        var characters = $(this).val().length;
        //var linebreaks = ($(this).val().match(/\n/g) || []).length;
        $('#apTextChars').text(characters + ' / 2000');
    })

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

    $('#btnsReasons').on('click', '#btnEditReasons', function () {
        localStorage.setItem("backUrl", document.URL);
        //alert(localStorage.getItem("backUrl"));
        //alert("Metric Id is: " + getMPid());
        if (getMPid() == null) {
            alert("Session variables were lost");
        }
        else {
            window.location.href = "/MPVreasons/Assigment/" + getMPvalueId() + "?mpId=" + getMPid() + "&returnUrl=" + document.URL;
        }
    });

    $('#btnsActionPlan').on('click', '#btnSubmitActionPlan', function () {
        var validated = validateActionPlanSubmit();
        (validated) ? submitActionPlan() : showAlert("Action Plan text is required!", "", "N");;
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

    $('#divPriorActionPlans').on('click', '#btnPriorActionPlanDetail', function () {
        var bapm_id = $(this).parent().find('#priorBapmId').val();
        var mpv_id = $(this).parent().find('#priorMpvId').val();
        var mp_id = $(this).parent().find('#priorMpId').val();
        var bldg_id = $(this).parent().find('#priorBldgId').val();
        var metricDate = $(this).parent().find('#priorApMonth').val() + " " + $(this).parent().find('#priorApYear').val();

        window.location.href = "/ActionPlan/viewEdit/?mp_id=" + mp_id +"&bldg_id=" + bldg_id + "&bapm_id=" + bapm_id;
    });
});

//------------------------------------------------------------------------------------------------
//-------------------------------------------FUNCTIONS--------------------------------------------
//------------------------------------------------------------------------------------------------

function displayPriorActionPlans() {

    var res = getMetricDate().split(" ", 2);
    var month = monthToInt(res[0]);
    var year = res[1];
    var begmonth
    var begyear
    var endmonth
    var endyear

    var productname = "Red Zone";
    var mtrc_period_id = getMPid();
    var dsc_mtrc_lc_bldg_id = getBuildingId();

    if (month >= 1 && month <= 12) {
        begmonth = month;
        begyear = year - 1;
    }
    else {
        begmonth = "";
        begyear = "";
    }

    if (month == 1) {
        endmonth = 12;
        endyear = year - 1;
    } else if (month >= 2 && month <= 12) {
        endmonth = month - 1;
        endyear = year;
    } else {
        endmonth = "";
        endyear = "";
    }

    var formData = {
        productname: productname,
        mtrc_period_id: mtrc_period_id,
        dsc_mtrc_lc_bldg_id: dsc_mtrc_lc_bldg_id,
        begmonth: begmonth,
        begyear: begyear,
        endmonth: endmonth,
        endyear: endyear
    };

    $.ajax({
        url: '/ActionPlan/_priorActionPlans',
        //url: 'http://dscapidev/dscmtrc/api/v1/metric/getpriorap',
        method: "POST",
        cache: false,
        //type: "POST",
        //data: payload,
        data: formData,
        //contentType: "application/json; charset=utf-8",
        //dataType: "json",
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Failed to Save Data. Ajax Failed!!\nError:" + textStatus + "," + errorThrown);  //<-- Trap and alert of any errors if they occurred
        }
    }).done(function (d) {
        $("#divPriorActionPlans").html(d);
        $(".mNameCell").text(getMetricName());
    });
}


//------------------------------------------------------------------------------------------------
function validateActionPlanSubmit() {
    var rz_apd_ap_text = $('#apText').val();

    var confirmed = confirm("Are you sure you want to submit this action plan? You will not be able to make changes afterwards.");

    return (rz_apd_ap_text == "" || !confirmed) ? false : true;
}

function buildSubmitActionPlanJSON() {
    var productname = "Red Zone";
    var rz_bapm_id = $('#bapmId').val();
    var rz_apd_ap_ver = $('#apVersion').val();
    var rz_apd_subm_app_user_id = "1";
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
    var rz_apd_subm_app_user_id = "1";
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
            showAlert("Action Plan Saved!", "", "Y");
            //location.reload();
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