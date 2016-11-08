$(document).ready(function () {
    $(".menuItem").removeClass("menuSelected");    // Reset Menu Selections
    $('#mMyTasks').show();                      // Show a new Menu Item when this page is displayed
    $("#mMyTasks").addClass("menuSelected");


    $('.btn-toolbar').on('click', '.btn', function (e) {
        var $target = $(this).parentsUntil('btn-toolbar').next('.collapse');
        //alert($target.attr("aria-expanded"));
        $target.attr("aria-expanded") ? $target.collapse('toggle') : $target.collapse();
        $(this).children('.glyphicon').toggleClass('glyphicon-chevron-right glyphicon-chevron-down');
    });

});


//------------------------------------------------------------------------------------------------
//-------------------------------------------FUNCTIONS--------------------------------------------
//------------------------------------------------------------------------------------------------