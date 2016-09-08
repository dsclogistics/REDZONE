//alert("Javascript file is connected");
var fixHelper = function (e, ui) {
    ui.children().each(function () {
        $(this).width($(this).width());
    });
    return ui;
};

//Enable the table sortability capabilities
function reasonSortEnable() {
    if ($("#sortTable tbody").sortable())
    {
        $("#sortTable tbody").sortable('enable');
    }
    //$("#sortTable tbody").sortable('enable');
    $("#sortTable tbody").sortable({
        placeholder: 'ui-state-highlight',
        helper: fixHelper            //used to prevent the columns from collapsing while dragging the row
    }).disableSelection();
}

//Disable the table sortability capabilities
function reasonSortDisable() {
    $("#sortTable tbody").sortable('disable');
}


$(document).ready(function () {

    // This is the New Branch

});
