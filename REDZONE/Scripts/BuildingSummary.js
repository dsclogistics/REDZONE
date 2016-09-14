﻿
$(document).ready(function () {
    $(".menuItem").removeClass("menuSelected");
    $("#mDashboard").addClass("menuSelected");


//================== This section handles the Custom context Menu Capabilities=================
"use strict";
//document.addEventListener("contextmenu", function (e) {
//    console.log(e);
//});

var taskItemClassName = 'valueCell';
//var menu = document.querySelector("context-menu");    // --- The html representation of our menu ---
var menu = document.getElementById("context-menu");    // --- Object with the html representation of our menu ---
var menuState = 0;
//var activeClassName = "context-menu--active";         ///_---------------NEED THIS DEFINED --------------
var menuPosition;
var menuPositionX;
var menuPositionY;
var taskItemInContext;      //Item that was right clicked and it's of the right class to handle the context menu event
var contextMenuClassName = "context-menu";
var contextMenuItemClassName = "context-menu__item";
var contextMenuLinkClassName = "context-menu__link";
var contextMenuActive = "context-menu--active";     ///_---------------NEED THIS DEFINED --------------

init();

/*** Initialise our application's code. */
function init() {
    contextListener();
    clickListener();
    keyupListener();
}

function elementRightClicked(e, className) {

    var el = e.srcElement || e.target;

    //alert("Right Click: (" + className + ")" + el.classList[0].toString());
    if (el.classList.contains(className)) {
        return el;
    } else {
        while (el = el.parentNode) {
            if (el.classList && el.classList.contains(className)) {
                return el;
            }
        }
    }
    return false;
}

function contextListener() {
    document.addEventListener("contextmenu", function (e) {
        taskItemInContext = elementRightClicked(e, taskItemClassName);

        if (taskItemInContext) {
            //A valid cell type was clicked
            //Before displaying menu, verify that the cell is active else do not display menu
            var $cellClicked = $("#" + taskItemInContext.getAttribute("id"));  // Object (CELL) that was right clicked
            var mpId = $cellClicked.find("#mpId").first().val();
            if (!(!mpId || mpId == "")) {
                // If the Cell has a valid Metric Period Id then show the menu
                e.preventDefault();
                positionMenu(e);
                toggleMenuOn();
            }
            else {
                taskItemInContext = null;
                toggleMenuOff();
            }
        }
        else {
            taskItemInContext = null;
            toggleMenuOff();
        }
    });
}

function toggleMenuOn() {
    if (menuState !== 1) {
        //$("#context-menu").show();
        menuState = 1;
        //menu.addClass(contextMenuActive);
        menu.classList.add(contextMenuActive);
    }
}

function toggleMenuOff() {
    if (menuState !== 0) {
        menuState = 0;
        //menu.removeClass(contextMenuActive);
        menu.classList.remove(contextMenuActive);
    }
}

function clickListener() {
    document.addEventListener("click", function (e) {
        var clickeElIsLink = elementRightClicked(e, contextMenuLinkClassName);

        if (clickeElIsLink) {
            //If what was clicked is a link of class "context-menu__link"
            e.preventDefault();
            menuItemListener(clickeElIsLink);
        } else {
            var button = e.which || e.button;
            if (button === 1) {
                toggleMenuOff();
            }
        }
    });


}

function keyupListener() {
    window.onkeyup = function (e) {
        if (e.keyCode === 27) {
            toggleMenuOff();
        }
    }
}
function getPosition(e) {
    var posx = 0;
    var posy = 0;

    if (!e) var e = window.event;

    if (e.pageX || e.pageY) {
        posx = e.pageX;
        posy = e.pageY;
    } else if (e.clientX || e.clientY) {
        posx = e.clientX + document.body.scrollLeft +
                           document.documentElement.scrollLeft;
        posy = e.clientY + document.body.scrollTop +
                           document.documentElement.scrollTop;
    }

    return {
        x: posx,
        y: posy
    }
}
function positionMenu(e) {

    var menuWidth;
    var menuHeight;
    var windowWidth;
    var windowHeight;
    var clickCoords;
    var clickCoordsX;
    var clickCoordsY;

    clickCoords = getPosition(e);
    clickCoordsX = clickCoords.x;
    clickCoordsY = clickCoords.y;
    //alert("Current pos is: x=" + clickCoordsX + " y=" + clickCoordsY);
    menuWidth = menu.offsetWidth + 4; //$("#context-menu").width + 4; //menu.offsetWidth + 4;            // To allow 4 px of padding from the screen edge if needed
    menuHeight = menu.offsetHeight + 4; //$("#context-menu").height + 4;       //  menu.offsetHeight + 4;
    windowWidth = window.innerWidth;
    windowHeight = window.innerHeight;

    // If the menu will display outside the screen recalculate the position so it gets moved to within the screen window
    if ((windowWidth - clickCoordsX) < menuWidth) {
        //menu.css({ left: (windowWidth - menuWidth) });
        menu.style.left = windowWidth - menuWidth + "px";
    } else {
        //menu.css({ left: clickCoordsX });
        menu.style.left = clickCoordsX + "px";
    }

    if ((windowHeight - clickCoordsY) < menuHeight) {
        //menu.css({ top: (windowHeight - menuHeight) });
        menu.style.top = windowHeight - menuHeight + "px";
    } else {
        //menu.css({ top: clickCoordsY });
        menu.style.top = clickCoordsY + "px";
    }
}

//This function can turn the menu off when browser window is being resized
function resizeListener() {
    window.onresize = function (e) {
        toggleMenuOff();
    };
}

function menuItemListener(link) {
    // A valid selection was clicked from the context pop up menu
    //convert jscript object clicked into jQuery Object (For ease of data retrieval)
    var $cellClicked = $("#" + taskItemInContext.getAttribute("id"));  // Object (CELL) that was right clicked
    var contextMenuOption = link.getAttribute("data-action");
    console.log("Task ID - " +
                  taskItemInContext.getAttribute("data-id") +
                  ", Task action - " + link.getAttribute("data-action"));
    toggleMenuOff();
    if (contextMenuOption == "Manage") {
        //Save all context variables into Local Storage for Later use either on the Reason Assigment Page and within the popup Form
        cacheMetricValueVariables($cellClicked);

        //alert("Submitting '" + contextMenuOption + "' Action for:\n\n" + getMetricValueVariablesMessage());

        window.location.href = "/MPVreasons/Assigment/" + getMPvalueId() + "?mpId=" + getMPid();
    }
    else {
        alert("This menu option is not yet Enabled.");
    }
}
    //======================== END OF THE CUSTOM CONTEXT MENU FUNCTIONALITY=================

function cacheMetricValueVariables($cellClicked) {
    var mpId = $cellClicked.children("#mpId").first().val();
    var mpValueId = $cellClicked.prop("id");
    if (!mpValueId) {
        mpId = "0";
        mpValueId = "0";
    }
    // The Metric Date is a compound value using the clicked cell's column month name
    var columnId = "#" + $cellClicked.find("#colIndex").first().val();  // Pointer to the Current Cell in context's Column Month Name
    var metricDate = $(columnId).val() + " " + $("#buildingYear").val(); // Current Column Month concatenated with the Year

    localStorage.setItem("mpId", mpId);
    localStorage.setItem("mpBuildingName", $("#buildingName").val());
    localStorage.setItem("mpName", $cellClicked.parent().find("#mName").first().html());
    localStorage.setItem("mpGoal", $cellClicked.parent().find("#mGoal").first().html().replace("&lt;", "<").replace("&gt;", ">"));
    localStorage.setItem("mpValueId", mpValueId);
    localStorage.setItem("mpValue", $cellClicked.find("#mValue").first().html());
    localStorage.setItem("mpValueDisplayClass", $cellClicked.find("#mDisplayClass").first().val());
    localStorage.setItem("mpValueDate", metricDate);
}

function getMetricValueVariablesMessage() {
    var message = 'Building: ' + getBuildingName() + '\nMetric Name: ' + getMetricName() + '\nMetric Goal: ' + getMetricGoal();
    message = message + '\nMetric Period Id: ' + getMPid() + '\nMP Value Id: ' + getMPvalueId() + '\nMetric Value: ' + getMPvalue();
    message = message + "\nDisplay Class: " + getMPvalueDisplayClass() + "\nMetric Date: " + getMetricDate();
    return message;
}

function getMPid() { return localStorage.getItem("mpId");}
function getBuildingName() { return localStorage.getItem("mpBuildingName"); }
function getMetricName() { return localStorage.getItem("mpName"); }
function getMetricGoal() { return localStorage.getItem("mpGoal"); }
function getMPvalueId() { return localStorage.getItem("mpValueId"); }
function getMPvalue() { return localStorage.getItem("mpValue"); }
function getMPvalueDisplayClass() { return localStorage.getItem("mpValueDisplayClass"); }
function getMetricDate() { return localStorage.getItem("mpValueDate"); }

function resetMetricValueVariables() {
    localStorage.setItem("mpId", "");
    localStorage.setItem("mpBuildingName", "");
    localStorage.setItem("mpName", "");
    localStorage.setItem("mpGoal", "");
    localStorage.setItem("mpValueId", "");
    localStorage.setItem("mpValue", "");
    localStorage.setItem("mpValueDisplayClass", "");
    localStorage.setItem("mpValueDate", "");
}

});

