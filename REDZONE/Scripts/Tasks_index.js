$(document).ready(function () {
    setDisplay();

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
function setDisplay() {
    var display = $('#hdnDisplay').val();

    if (display == 'ap') {
        $('#divSubmitterTasks').find('.glyphicon').toggleClass('glyphicon-chevron-right glyphicon-chevron-down');
    }
    else if(display == 'mtrc') {

    }
}