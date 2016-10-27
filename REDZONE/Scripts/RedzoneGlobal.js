// --------------------- REDZONE JAVASCRIPT GLOBAL FUNCTIONS ------------------------\\
// Created by Feliciano Delgado          June 03, 2016                               ||
//\==================================================================================//

//================ USER AUTHENTICATION RELATED FUNCTIONS =============================
var SESSION_EXP = 60;   // Define The Maximum Session Inactivity Timeout (In Minutes)
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
function getUsrAutToken() {
    $.ajax({
        //url: '@Url.Action("getNewCAMid", "cognosUtils")',
        url: '/cognosUtils/getNewCAMid',
        method: "GET",
        cache: false,
        dataType: "text",
        //data: { cft_id: 123, end_eff_date: newEndEffDate },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Failed to Get a New Passport Id from the Cognos Server: " + errorThrown);  //<-- Trap and alert of any errors if they occurred
        }
    }).done(function (d) {
        //alert("ajax CAMid = " + d);
        window.localStorage.setItem("CAMid", d);
        window.localStorage.setItem("CAMidDt", new Date());
    });

    //return localStorage.getItem("CAMid");
}
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
function setUsrIdentity(usrName, usrRole, usrToken) {
    if (usrName && usrRole && usrToken) {
        //If all parameters have valid values, save data to LocalStorage
        var tokenDate = new Date();
        window.localStorage.setItem("usrName", usrName);
        window.localStorage.setItem("usrRole", usrRole);
        window.localStorage.setItem("usrToken", usrToken);
        window.localStorage.setItem("tokenDate", tokenDate);
        alert("User Information Successfully written to local Storage");
    }
    else {
        alert("Invalid User Information. Token Cannot Be written to local Storage");
    }
}
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
function isUsrTokenValid() {
    var tokenId = localStorage.getItem("usrToken");
    var tokenDt = new Date(localStorage.getItem("tokenDate"));
    var tokenAge = Math.ceil(((new Date() - tokenDt) / 1000) / 60);     // In Minutes    
    if (!tokenId || tokenId == 'undefined') {
        alert("User Not Defined");
        return false;  //CAMid does not exist
    }
    //If the Token Date does not exist, is blank, undefined or it is greater than 60 minutes, then it is considered invalid
    if (!tokenAge || tokenAge == 'undefined' || tokenAge > SESSION_EXP) { alert("Token Expired!" + tokenAge); return false; }       //Set the Timeout valid time of the Token to be 60 minutes (After 60 minutes it is considered Invalid)
    alert("User Session Token was " + tokenAge + " minutes Old before being reset.");
    window.localStorage.setItem("tokenDate", new Date());   //Reset the access date every time we verify that the token is valid
    return true;
}
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// Use this function only after the validity of the token has been Stablised.
function getUsrName() {
    //if (!isUsrTokenValid()) {
    //    return "";
    //}else
    return localStorage.getItem("usrName");
}
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// Use this function only after the validity of the token has been Stablised.
function getUserRoles() {
    //if (!isUsrTokenValid()) {
    //    return "";
    //}else
    return localStorage.getItem("userRole");
}
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// Use this function only after the validity of the token has been Stablised.
function getUserBuildings() {
    //if (!isUsrTokenValid()) {
    //    return "";
    //}else
    return localStorage.getItem("userBuildings");
}
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// Use this function only after the validity of the token has been Stablised.
function hasRole(someRole) {
    if (someRole == null || someRole == "" || someRole == "undefined") { return false; }
    var roles = localStorage.getItem("userRole");
    //alert("Current Roles:" + roles);
    if (roles == null) { return false; }
    return (roles.indexOf( '|' + someRole.toUpperCase() + '|' ) !== -1);
}
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// Use this function only after the validity of the token has been Stablised.
function hasBuilding(someBuildingId) {
    if (someBuildingId == null || someBuildingId == "" || someBuildingId == "undefined") { return false; }
    var buildingList = localStorage.getItem("userBuildings");
    if (buildingList == null || buildingList == "") { return false; }
    return (buildingList.indexOf(('|' + someBuildingId.toUpperCase() + '|')) !== -1);
}
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// Use this function only after the validity of the token has been Stablised.
function getUsrToken() {
    //if (!isUsrTokenValid()) {
    //    return "";
    //}else
    return localStorage.getItem("usrToken");
}
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// Use this function only after the validity of the token has been Stablised.
function getUsrTokenAge() {
    //if (!isUsrTokenValid()) {
    //    return "";
    //}else

    var tokenDt = new Date(localStorage.getItem("tokenDate"));
    var tokenAge = Math.ceil(((new Date() - tokenDt) / 1000) / 60);     // In Minutes
    return tokenAge;
}
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
function usrLogOff() {
    window.localStorage.removeItem("usrName");
    window.localStorage.removeItem("usrRole");
    window.localStorage.removeItem("usrToken");
    window.localStorage.removeItem("tokenDate");
}
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -



//================= REDZONE APPLICATION WIDE RELATED JAVASCRIPT and FUNCTIONS =================================\
//----- Javascript to Execute before any HTML Page is even rendered or displayed ------------------------------
function resetUserInfo(pFName, pLName, pLoginName, pEmail, pRole, pBuildings, pId, pTurnOff) {
    $.ajax({
        //url: '@Url.Action("getNewCAMid", "cognosUtils")',
        url: '/Account/resetUserInfo',
        method: "POST",
        cache: false,
        dataType: "text",
        data: { uFName: pFName, uLName: pLName, uLoginName: pLoginName, email: pEmail, uRole: pRole, uBuildings: pBuildings, uId: pId, turnOff: pTurnOff },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Ajax Failed: " + errorThrown);
            localStorage.clear();
        }
    }).done(function (d) {
        //DO Something (Nothing for now)
    });

    //return localStorage.getItem("CAMid");
}
function showPopupForm(formTitle, formText, reload) {
    $(".popupFormTitle").html(formTitle);
    $("#popupFormBodyData").html(formText);
    $('#popupForm').modal('show');
    if (reload == 'Y') { $('#reloadPage').val('Y'); }
}
function showAlert(msg, msgStyle, reload) {
    var msgClass = "alert-" + msgStyle;
    if (reload == 'Y') { $('#reloadPage').val('Y'); }
    if (msgStyle == null || msgStyle == "") { msgClass = ""; }
    $("#msgFormBodyData").removeClass("alert-warning");
    $("#msgFormBodyData").removeClass("alert-danger");
    if (msgStyle) { $("#msgFormBodyData").addClass(msgClass); }
    $("#msgFormBodyData").html(msg);
    $('#msgForm').modal('show');
}


//----- Javascript to Execute only after the HTML Page has been fully rendered/displayed ----------------------
$(document).ready(function () {
    $('#cmdViewCred').click(function () {
        //Retrieve the User Data and display it in a popup Form
        $.ajax({
            url: '/Account/_UserInfo',
            method: "GET",
            cache: false,
            //type: "POST",
            //data: payload,
            //data: formData,
            //contentType: "application/json; charset=utf-8",
            //dataType: "json",
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Failed to Retrieve User credentials from Server!!\n\nError: " + errorThrown + "\n\n");  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (uData) {
            showPopupForm('User Credential Information', uData);
        });
        //var userFirstName = localStorage.getItem("first_name");
        //var userLastName = localStorage.getItem("last_name");
        ////var userFullName = userFirstName.charAt(0).toUpperCase() + userFirstName.slice(1) + ' ' + userLastName.charAt(0).toUpperCase() + userLastName.slice(1);
        //var userFullName = userFirstName + ' ' + userLastName;

        //var userInfo = '<div class="row" style="text-align: center"><h3>Credentials Report for:</h3></div><br /><br /><div class="row"><div class="col-xs-3 col-sm-offset-1">Name:</div><div class="col-xs-8">' +
        //    userFullName + '</div></div><div class="row"><div class="col-xs-3 col-sm-offset-1">Email</div><div class="col-xs-8">' +
        //    localStorage.getItem("email") + '</div></div><div class="row"><div class="col-xs-3 col-sm-offset-1">Role:</div><div class="col-xs-8">' +
        //    userRole + '</div></div>';
        //showPopupForm('User Credential Information', userInfo);
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $('#popupForm').on("hidden.bs.modal", function () {
        if ($('#reloadPage').val() == 'Y') {
            $('#reloadPage').val('N');
            location.reload();
        }
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $('#msgForm').on("hidden.bs.modal", function () {
        if ($('#reloadPage').val() == 'Y') {
            $('#reloadPage').val('N');
            location.reload();
        }
    });

});

//============= END OF THE REDZONE APPLICATION WIDE RELATED JAVASCRIPT and FUNCTIONS ==========================/