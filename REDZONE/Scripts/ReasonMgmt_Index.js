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
        //placeholder: 'sort-state-highlight',
        placeholder: {
            element: function (currentItem) {
                return $('<tr><td colspan="8"><div style="background-color:#deecff; text-align:center; font-weight:bold; padding: 10px">DROP ITEM HERE<br/></div></td></tr>')[0];
            },
            update: function (container, p) {
                return;
            }
        },

        helper: fixHelper,            //used to prevent the columns from collapsing while dragging the row
        //items: "tr:not(.sort-state-disabled)"
        //update: function (event, ui) { },  //triggers when the "sort" drag/drop movement completes/updates successfully,
        stop: function (event, ui) {
            //When the "sort" drag/drop completes(even if not successfully), Reset the look and feel of the dragged row
            ui.item.css("color", "black");
            ui.item.css("font-weight", "normal");
            updateOrderNumbers();
        }
    }).disableSelection();
}

//Disable the table sortability capabilities 
//(We might no longer need this function as we can just reload the page to reset/discard any changes)
function reasonSortDisable() {
    $("#sortTable tbody").sortable('disable');
    //Perform the removal of the UI styling 
    $('.reasonListRow').removeClass('sortableRow');
    $('.firstCol').css("border-radius", "0px");
    $('.lastCol').css("border-radius", "0px");
}

//Update the number in the "Order" column to reflect the new order after a drag and drop action in the reorder table.
function updateOrderNumbers() {
    $('.stdReasonRow #reason_order').each(function (i, value) {
        $(this).text(i + 1);
    });
}

function getNextOrder() {
    var maxValue = null;

    $('.stdReasonRow #reason_order').each(function () {
        var value = parseFloat($(this).text());
        maxValue = (value > maxValue) ? value : maxValue;
    });

    maxValue = (maxValue == null) ? 1 : maxValue + 1;
    return maxValue;
}

$(document).ready(function () {

    // This is the New Branch

});
