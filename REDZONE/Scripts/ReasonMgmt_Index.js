//alert("Javascript file is connected");
var fixHelper = function (e, ui) {
    ui.children().each(function () {
        $(this).width($(this).width());
    });
    ui.css("color", "blue");
    ui.css("font-weight", "bold");
    return ui;
};

//Enable the table sortability capabilities
function reasonSortEnable() {
    if ($("#sortTable tbody").sortable())
    {
        $("#sortTable tbody").sortable('enable');
    }
    $("#sortTable tbody").sortable({
        placeholder: 'sort-state-highlight',
        helper: fixHelper,            //used to prevent the columns from collapsing while dragging the row
        //items: "tr:not(.sort-state-disabled)"
        update: function (event, ui) {
            //Reset the look and feel of the dragged row
            ui.item.css("color", "black");
            ui.item.css("font-weight", "normal");
            //alert("Item was sorted.");
        }
    }).disableSelection();
}

//Disable the table sortability capabilities
function reasonSortDisable() {
    $("#sortTable tbody").sortable('disable');
    //Perform the removal of the UI styling 
    $('.reasonListRow').removeClass('sortableRow');
    $('.firstCol').css("border-radius", "0px");
    $('.lastCol').css("border-radius", "0px");
}


$(document).ready(function () {

    // This is the New Branch

});
