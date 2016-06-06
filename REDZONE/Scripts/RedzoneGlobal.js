// --------------------- REDZONE JAVASCRIPT GLOBAL FUNCTIONS ------------------------\\
// Created by Feliciano Delgado          June 03, 2016                               ||
//\==================================================================================//

//================ USER AUTHENTICATION RELATED FUNCTIONS =============================
var SESSION_EXP = 60;   // Define The Maximum Session Inactivity Timeout (In Minutes)
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
function getUsrRole() {
    //if (!isUsrTokenValid()) {
    //    return "";
    //}else
    return localStorage.getItem("usrRole");
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

