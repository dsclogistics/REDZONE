
function showAlert(msg, msgStyle) {
    var msgClass = "alert-" + msgStyle;
    if (msgStyle == null || msgStyle == "") { msgClass = "";}
    $("#msgFormBodyData").removeClass("alert-warning");
    $("#msgFormBodyData").removeClass("alert-danger");
    $("#msgFormBodyData").addClass(msgClass);
    $("#msgFormBodyData").html(msg);
    $('#msgForm').modal('show');
}
function getMPid() { return localStorage.getItem("mpId"); }
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
function rsList_ItemChangeId(rsId_toRemove, idAction) {
    //Actions can be "show" or "hide" //Do not remove Reason Item, just set it's display property to "hidden"
    var displayClass = "";
    if (idAction == "hide") { displayClass = "display:none"; }
    var sOldList = $('#nsItemList').val();
    var startIndex = sOldList.indexOf(rsId_toRemove);
    var endIndex = sOldList.indexOf("~", startIndex);
    var idStringOld = sOldList.substring(startIndex, endIndex);
    //alert("Item string found is: " + idStringOld);
    var rsArray = idStringOld.split(",");
    var idStringNew = rsArray[0] + "," + rsArray[1] + "," + displayClass + ",false";
    var sNewList = sOldList.replace(idStringOld, idStringNew);
    $('#nsItemList').val(sNewList);

    //alert("nsItemList: " + sOldList + "\n\nRemoving Id: " + rsId_toRemove + "\n\nFinal List is: " + sNewList);
}
function rsList_ItemAddId(rsId_toAdd, rsText) {
    $('#nsItemList').val($('#nsItemList').val() + rsId_toAdd + "," + rsText + ",,false~");
}

function getURLid() {
    var sPageURL = window.location.href;
    sPageURL = sPageURL.substring(0, sPageURL.indexOf("?"));         //Remove all the params from the URL
    var indexOfLastSlash = sPageURL.lastIndexOf("/");
    var urlId = sPageURL.substring(indexOfLastSlash + 1);

    return urlId;
    //return sPageURL.match(/\d+\.?\d*/g);
}
function getURL_mpid() {
    var sPageURL = window.location.href;
    sPageURL = sPageURL.substring(sPageURL.indexOf("mpId="), sPageURL.indexOf("&"));         //Remove all the params from the URL
    return sPageURL.replace("mpId=", "");
    //return sPageURL.match(/\d+\.?\d*/g);
}
function cleanString(inputString) {
    //alert("Input String: " + inputString);
    var outputString = inputString;
    //Remove the ending Character Separator
    if (outputString != "") { outputString = outputString.slice(0, -1); }
    //Remove unneeded html tags
    outputString = outputString.replaceAll("<br>", "").replaceAll("<p>", "").replaceAll("</p>", "");
    //alert("Output String: " + outputString);
    return outputString;
}

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.split(search).join(replacement);
};
//=============== Section of Methods/Attach Events to Perform only after document is ready =====================
$(document).ready(function () {
    //Global Variables
    var originalComment = "";
    $('#fancy-checkbox-default-999').prop('disabled', true);
    $('#nonStdRsnDropDown').find('.ghost').first().css('background-color', 'gray');
    $("#buildingName").html(getBuildingName());
    $("#metricDate").html(getMetricDate());
    $("#metricDate1").html(getMetricDate());
    $("#mValueCell").addClass(getMPvalueDisplayClass());
    $(".mNameCell").html(getMetricName());
    $("#mGoalCell").html(getMetricGoal());
    $("#mValueCell").html(getMPvalue());
    
    //resetMetricValueVariables(); After document has loaded we do not need to keep the variables cached in local Storage
    //============================ SAVE PROGRESS ACTION ====================================================
    $('#btnSaveProg').click(function () {
        $(this).prop("disabled", true);      //Disable the save button until processing completes
        // This process will compile three lists that will be submitted to the server via ajax for processing:
        // List 1: DELETE LIST - A list all reason codes currently assigned that need to be deleted
        // List 2: UPDATE LIST - A list of reason codes already assigned that need to be updated (New comment for example)
        // List 3: ASSIGN LIST - A list of reason codes that need to be assigned (Added)
        //if ($('#nsItemsToDelete').val() != "") { alert("Item to Delete: " + $('#nsItemsToDelete').val()); }

        if (!getMPvalueId()) {
            showAlert("Warning!! Your session has timed out. Some of the changes might not be saved.\nGo back to building dashboard and try again.");
            //-------------------- TO DO --------------------------------------------------------------------
            // -- Restore all Local Storage Values from the cached webpage labels so user is not kicked out
            //-----------------------------------------------------------------------------------------------
        }
        else {   // Do Saving procedure
        }


        // Parameters that will be used for all three actions and for both static and dynamic reason sections
        var rMPValueId = getURLid();     //This is the Cell Value Id      //or  getMPvalueId() if we want to retrieve value from Local storage (Global value for all items);
        var rMPValueReasonId = "";       //For existing assigned Reasons (Delete/Update). New reasons do not have a Value Reason Id (Generated by DB after the insert)
        var rMPReasonId = "";
        var rMPValueReasonComment = "";
        var reasonsToDelete_List = "";
        var reasonsToUpdate_List = "";
        var reasonsToAdd_List = "";
        //var rMPValueReasonText = "";     (The reason text is not needed, API just needs the reason Id)

        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        // --- Loop through each Standard Reason (In the 'standardReasonsDiv' DIV section) to compile the DELETE/ADD/Update lists ---
        //alert("Checking Existing Items");
        $('.stdrReason').each(function () {
            var $curReason = $(this);

            //Check whether the item in question was changed. If not changed Skip it
            if ($curReason.find('#wasUpdated').val() == "Y") {
                //Retrieve it's original status and variable values needed for add/update/delete
                var originalSTatus = $curReason.find('#origSts').first().val();        //Either "checked" or ""
                rMPValueReasonId = $curReason.find('#valueReasonId').first().val();
                rMPReasonId = $curReason.find('#reasonId').first().val();
                $curReason.find('.rCommentBox').first().find('span:first').remove();   //Remove the unneeded 'span' tag inside the comments.
                rMPValueReasonComment = $curReason.find('.rCommentBox').first().html().trim();

                //If the check Box Status was changed, then the item either needs to be added or deleted
                if ($curReason.find('.rsCheckBox').first().prop("checked")) {//Item is checked.
                    if (originalSTatus == "checked") {//If the item was originally checked then this is an update (The comment might have been changed)
                        // ~~~~~~~~~ THIS IS AN UPDATE ~~~~~~~~~~~~
                        // Add the current Item to the "UPDATE" list
                        reasonsToUpdate_List = reasonsToUpdate_List + rMPValueReasonId + "," + rMPValueId + "," + rMPReasonId + "," + rMPValueReasonComment + "~";
                    }
                    else {
                        //// ~~~~~~~~~ THIS IS AN ADD ~~~~~~~~~~~~
                        // Add the current Item to the "ADD" list
                        reasonsToAdd_List = reasonsToAdd_List + rMPValueId + "," + rMPReasonId + "," + rMPValueReasonComment + "~";
                    }
                }
                else {//Box is not checked.
                    if (originalSTatus == "checked") { //If it was originally checked, then this is a delete
                        // ~~~~~~~~~ THIS IS A DELETE ~~~~~~~~~~~~
                        // Add the current Item to the "DELETE" list
                        reasonsToDelete_List = reasonsToDelete_List + rMPValueReasonId + ",";
                    }
                 }
            }//Else, If not changed, skip it (no need to save anything for this reason)
        });
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        // --- Loop through all non-standard reason DIV (Both the curent and the newly added non-stdr reasons) to compile the DELETE/ADD/Update lists ---
        $('#nonStdrReasonsDiv .nsReason, #nonStdrReasonsDivAdded .nsReason').each(function () {
            //There is nothing to Delete on these sections (all delete actions have already been handled by the uncheck action)
            //At this point all non-standard reasons found should be checked (Unchecked Items were removed already)
            
            var rMPReasonId_Original = $(this).find('#origReasonId').first().val();
            rMPReasonId = $(this).find('#reasonId').first().val();
            rMPValueReasonId = $(this).find('#valueReasonId').first().val();      //Assigned Value Reason Id

            $(this).find('.rCommentBox').first().find('span:first').remove();   //Remove the unneeded 'span' tag inside the comments.
            rMPValueReasonComment = $(this).find('.rCommentBox').first().html().trim();

            //var textTagId = "#li_" + $(this).find('#reasonId').first().val();
            //var assignedRsnText = $(textTagId).html();

            if (rMPValueReasonId != "") { // This is an Existing Assigned MPV Reason 
                //This Assigned Reason Id is in the database so it is a "change"                
                //We must determine whether it was updated or not (If not updated there is no need to submit)
                if ($(this).find('#wasUpdated').first().val() == "Y") {                    
                    //Final Check: If the Original Reason Id and the New Reason Id are not the same, then it means that 
                    // the user has reselected a new Reason id from the drop down. In that case, the Original Reason Id 
                    //must be Deleted and the New must be added. Otherwise we process it as a regulat Update
                    if (rMPReasonId_Original != rMPReasonId) {
                        //This is a dual operation: "DELETE" of the Old Reason Id and an "ADD" of the new Reason Id 
                        reasonsToDelete_List = reasonsToDelete_List + rMPValueReasonId + ",";
                        reasonsToAdd_List = reasonsToAdd_List + rMPValueId + "," + rMPReasonId + "," + rMPValueReasonComment + "~";
                    } else {
                        // ~~~~~~~~~ THIS IS AN UPDATE (Add the current Item to the "UPDATE" list) ~~~~~~~~~~~~
                        reasonsToUpdate_List = reasonsToUpdate_List + rMPValueReasonId + "," + rMPValueId + "," + rMPReasonId + "," + rMPValueReasonComment + "~";
                    }
                }//Else there is nothing to do, as the reason was not modified (No changes to save)
            }
            else {// This is a new assigned Assigned MPV Reason to ADD to the database
                //// ~~~~~~~~~ THIS IS AN ADD [Add the current Item to the "ADD" list] ~~~~~~~~~~~~
                reasonsToAdd_List = reasonsToAdd_List + rMPValueId + "," + rMPReasonId + "," + rMPValueReasonComment + "~";
            }

        });

        //Finally, clean the input string from non valid characters and ending separator character
        reasonsToDelete_List = reasonsToDelete_List + $("#nsItemsToDelete").val();
        reasonsToDelete_List = cleanString(reasonsToDelete_List);
        reasonsToUpdate_List = cleanString(reasonsToUpdate_List);
        reasonsToAdd_List = cleanString(reasonsToAdd_List);

        //alert("Posting Data to Server:\nADDING: [" + reasonsToAdd_List + "]\nDELETING: [" + reasonsToDelete_List + "]\nUPDATING: [" + reasonsToUpdate_List + "]" );

        if (reasonsToDelete_List == "" && reasonsToUpdate_List == "" && reasonsToAdd_List == "") {
            //There is Nothing to post
            showAlert("There are no changes to Save");
        }
        else {
            //alert("Submitting action to Server:\nADD:" + reasonsToAdd_List + "\nDELETE:" + reasonsToDelete_List + "\nUPDATE: " + reasonsToUpdate_List);
            //Perform Ajax Call to post/save changes: Parameters to Post= string addList, string deleteList, string updateList
            $.ajax({
                url: '/MPVreasons/modifyMPVReasons',
                method: "POST",
                cache: false,
                //type: "POST",
                //data: payload,
                //string mpvr_id, string mtrc_period_val_id, string mpr_id, string mpvr_comment
                data: { addList: reasonsToAdd_List, deleteList: reasonsToDelete_List, updateList: reasonsToUpdate_List },
                //contentType: "application/json; charset=utf-8",
                //dataType: "json",
                error: function (jqXHR, textStatus, errorThrown) {
                    showAlert("Failed to Save Data. Ajax Failed!!\nError:" + textStatus + "," + errorThrown, "danger");  //<-- Trap and alert of any errors if they occurred
                }
            }).done(function (d) {
                //alert("Update Operation completed:\n\n=========== OPERATION RESULTS ===============\n" + d);
                $("#msgFormBodyData").html("Update Operation completed:<br /><br />=========== OPERATION RESULTS ===============<br />" + d.replaceAll("\n", "<br />"));
                $('#reloadAfterPopup').val("Y");  //To trigger a relaod when popup is Closed
                $('#msgForm').modal('show');
            });
        }


        ////Make ajax call to save (Add) Data (New MP Value Assigned Reason)
        //$.ajax({
        //    url: '/MPVreasons/addUpdateMPVReasons',
        //    method: "POST",
        //    cache: false,
        //    //type: "POST",
        //    //data: payload,
        //    //string mpvr_id, string mtrc_period_val_id, string mpr_id, string mpvr_comment
        //    data: { mtrc_period_val_id: rMPValueId, mpr_id: rMPReasonId, mpvr_comment: rMPValueReasonComment },
        //    //contentType: "application/json; charset=utf-8",
        //    //dataType: "json",
        //    error: function (jqXHR, textStatus, errorThrown) {
        //        alert("Failed to Save Data. Ajax Failed!!\nError:" + textStatus + "," + errorThrown);  //<-- Trap and alert of any errors if they occurred
        //    }
        //}).done(function (d) {
        //    alert("Ajax Call result string: " + d);
        //});


        //alert("Progress Saved Successfully");
    });
    //============================ SAVE PROGRESS ACTION FUNCTION ENDS ============================================

    $("#btnCancelMsgForm").click(function () {
        if ($('#reloadAfterPopup').val() == "Y") { window.location.reload(); }
    });
    $('#btnBeginAction').click(function () {
        showAlert("New Action Plan Actions are not enabled yet<br\>Check again later.");
    });

    $('.stdrCheckBox').change(function () {
        var $parentDiv = $(this).parents(".stdrReason").first();
        var $thisComment = $parentDiv.find(".rCommentBox").first();
        if ($(this).prop("checked")) {
            $thisComment.show();
            $parentDiv.find('#nsReasonText').first().css("font-weight", "bold");
        }
        else {
            $thisComment.hide();
            $parentDiv.find('#nsReasonText').first().css("font-weight", "normal");
        }
    });


    //------- When Non Standard Chexkbox is checked or unchecked ------------------------------
    $('#reasonListSection').on('change', '.nsCheckBox', function () {
        $nsReasonDiv = $(this).parents('.nsReason').first();
        $nsReasonDiv.find('#wasUpdated').first().val("Y");       //Flag current reason as "changed"
        //Toogle on-off the check mark icon
        $OKbox = $(this).parents('.form-group').first().find('.glyphicon-ok').first();
        $ghostBox = $(this).parents('.form-group').first().find('.ghost').first();
        if ($(this).prop("checked")) {
            //alert("Checked!");
            $OKbox.show();
            $ghostBox.hide();
            //----------- TO DO-----------------------------------
            //   Open the dropdown for a value to be selected
            //$(this).parent().find('.dropdown-menu').first().toggle();
            //$(this).parent().find('.dropdown-toggle').first().trigger('click.bs.dropdown');    //Not tested
            //$(this).parent().find('.dropdown-menu').first().trigger('click.bs.dropdown');    //Not tested
            // Find a way to close it when an item is selected
            //----------------------------------------------------
        }
        else {
            //alert("You unchecked me!");
            var nsRsnMPValue_old_id = $nsReasonDiv.find('#valueReasonId').first().val();
            var rsId_old = $nsReasonDiv.find('#reasonId').first().val();
            var oldLiId = "#li_" + rsId_old;                         //Save the 'Li' tag Id of the Reason Id that was unchecked

            //If the ns Assigned Reason was an existing assigned reason (in the database, then get the mpvalueReasonId to delete it)
            // otherwise (the original mpvalueReasonId is blank, it means was recently added but not save so there is no need to delete it)

            //When unchecked, verify if this item's Old mp valueReasonId is not blank (Which means it was originally assigned in the database)
            //If so, it needs to be flagged for deletion
            if (nsRsnMPValue_old_id != "") {
                //alert("MP Value Reason Id '" + nsRsnMPValue_old_id + "' has been flagged for Deletion");
                $('#nsItemsToDelete').val($('#nsItemsToDelete').val() + nsRsnMPValue_old_id + ",");
            }

            //$OKbox.hide();
            //$ghostBox.show();
            ////If unchecked, reset all the values to empty
            //$nsReasonDiv.find('#reasonId').first().val("");          //Reset to blank the current selected Reason Id
            //$nsReasonDiv.find('#nsReasonText').first().html("[ Select other Reason ... ]");     //Reset The Text

            $('.nsReason').each(function () {      //loop through all the Existing Drop down boxes
                //Read (show) the Old (Now Unselected) Reason Id so it can be added back again by the user if needed
                $(this).find(oldLiId).first().parents('li').first().show();
            });
            rsList_ItemChangeId(rsId_old, "show");        // Reset the Reason from the Page List so it is shown and included in New Drop Downs

            //Finally remove the whole control so it can be reused Again
            $nsReasonDiv.remove();
        }
        //Reset the Save Button
        $('#btnSaveProg').prop("disabled", false);

    });
    // -----------------------------------------------------------------------------------------------------
    $(".rsCheckBox").change(function () {
        // When a Standard Reason check box is changed, flag the reason Item as "modified"
        $stdrReasonDiv = $(this).parents('.stdrReason').first();
        $stdrReasonDiv.find('#wasUpdated').first().val("Y");       //Flag current reason as "changed"
        //Toogle on-off the check mark icon
        $OKbox = $(this).parents('.form-group').first().find('.glyphicon-ok').first();
        $ghostBox = $(this).parents('.form-group').first().find('.ghost').first();
        if ($(this).prop("checked")) {
            //alert("Checked!");
            $OKbox.show();
            $ghostBox.hide();
        }
        else {
            //alert("You unchecked me!");
            var stdrRsnMPValue_old_id = $stdrReasonDiv.find('#valueReasonId').first().val();
            var rsId_old = $stdrReasonDiv.find('#reasonId').first().val();


            //If the ns Assigned Reason was an existing assigned reason (in the database, then get the mpvalueReasonId to delete it)
            // otherwise (the original mpvalueReasonId is blank, it means was recently added but not save so there is no need to delete it)
            //When unchecked, verify if this item's Old mp valueReasonId is not blank (Which means it was originally assigned in the database)
            //If so, it needs to be flagged for deletion

            $OKbox.hide();
            $ghostBox.show();
            //If unchecked, reset all the values to empty
            //$stdrReasonDiv.find('#reasonId').first().val("");          //Reset to blank the current selected Reason Id
            //$nsReasonDiv.find('#nsReasonText').first().html("[ Select other Reason ... ]");     //Reset The Text
        }
        //Reset the Save Button
        $('#btnSaveProg').prop("disabled", false);

















    });
    // -----------------------------------------------------------------------------------------------------
    //$('.reasonComment').change(function () {
    //    // When a Comment box is changed, flag the reason Item as "modified"
    //    alert("Comment chnaged");
    //    $(this).parents('.stdrReason').first().find('#wasUpdated').first().val("Y");
    //});
    // -----------------------------------------------------------------------------------------------------
    $('#reasonListSection').on('focus', '.rCommentBox', function () {
        $(this).find(".cmLabel").remove();      // Remove the Label <span> tag since it is not part of the comment text
        originalComment = $(this).text().replace("&nbsp;", "").trim();   //Save the Original comment text to determine if it was modified
        $(this).html(originalComment);        //Resave the comment in a cleaned/trimmed way
    });
    // -----------------------------------------------------------------------------------------------------
    $('#reasonListSection').on('keyup', '.rCommentBox', function () {
        $('#btnSaveProg').prop("disabled", false);
    });

    // -----------------------------------------------------------------------------------------------------
    $('#reasonListSection').on('focusout', '.rCommentBox', function () {
        var updatedComment = $(this).text().replace("&nbsp;", "").trim();      //Get the new comment Text
        $(this).html(updatedComment);    //Reset the comment box data
        $(this).prepend('<span class="cmLabel" style="color:darkgray;">Comment: </span>');    //add the <span> label tag back
        if (updatedComment != originalComment) {
            //The comment text was changed. Update the 'updated' flag for this reason
            $(this).parents('.mpvReason').first().find('#wasUpdated').first().val("Y");   
            //alert("Comment was updated.\nNew Comment: " + updatedComment);
        }
        //$(this).find(".cmLabel").show();      //Put the Comment label back after leaving the comment field
    });
    // -----------------------------------------------------------------------------------------------------
    $('#reasonListSection').on('click', '.selNewReason', function () {
        //Reset the New Reason text box and show the dialog form
        $('#inputReasonText').val("");
        $('#puErrorMsg').hide();               //Hide the Pupup form error message if visible before displaying the form
        $('#addReasonForm').modal('show');
    });

    //-------------- When an Item from the Static (ADD New) dropdown is selected --------------------------
    $('#nonStdRsnDropDown').on('click', '.ddlReasonItem', function () {
        $('.noData').hide();  //Just in case it is still visible
        $('#ddlWait').show();
        var elNameIndex = parseInt($('#newReasonCounter').val()) + 1;  //Get last elementCounter and increase it by One to be used an part of the unique name

        //Perform Ajax Call to get a new drop down added with the Reason Id and text that was selected
        //Controller called has Parameters:
        //int? nsChkNameIndex, string reasonId, string valRsnId, string reasonText, string reasonComment, string wasUpdated, string ddItems
        $('#newReasonCounter').val(elNameIndex);      //Store the increased new Index to be used later
        var selReasonId = $(this).prop("id").replace("li_", "");
        var selReasonText = $(this).html();
        var ddlListItems = $('#nsItemList').val();
        //alert("Generating drop down for:\nReason Id: " + selReasonId + "\nReason Text: [" + selReasonText + "]");

        $.ajax({
            url: '/MPVreasons/_assignedNoStdrReason',
            method: "POST",
            cache: false,
            //type: "POST",
            //data: payload,
            //string mpvr_id, string mtrc_period_val_id, string mpr_id, string mpvr_comment
            data: { nsChkNameIndex: elNameIndex, reasonId: selReasonId, valRsnId: "", reasonText: selReasonText, reasonComment: "", wasUpdated: "", ddItems: ddlListItems },
            //contentType: "application/json; charset=utf-8",
            //dataType: "json",
            error: function (jqXHR, textStatus, errorThrown) {
                showAlert("Failed Retrieve template for New reason. Please try again!!\nError:" + textStatus + "," + errorThrown);  //<-- Trap and alert of any errors if they occurred
                $('#ddlWait').hide();
            }
        }).done(function (d) {
            $("#nonStdrReasonsDivAdded").append(d);
            $('html, body').animate({ scrollTop: $(document).height() }, "slow");        //Scroll to the botttom of the page to avoid hidding the newly added element
            $('#btnSaveProg').prop("disabled", false);
            //alert("Reason Id is: " + selReasonId);
            var curLiId = "#li_" + selReasonId;
            rsList_ItemChangeId(selReasonId, "hide");   //Hide the Reason from the Page List so it is not included in New Drop Downs
            //Remove (hide) the Selected Reason Id from all the Existing Drop down boxes so it cannot be added back again anywhere
            $('.nsReason').each(function () {
                $(this).find(curLiId).first().parents('li').first().hide();
            });

            $('#ddlWait').hide();
            $('#nonStdrReasonsDivAdded').find(".rCommentBox").last().focus();        //Position to the last added Reason Control's Comment Box
        });
    }); //----- End of When an Item from the Static dropdown is selected ----------

    // ------- Event for when an Item from one of the Non-Standard drop Down Boxes (Except the Static ddl used to add new reasons) is selected -----------
    $('#nonStdrReasonsDiv, #nonStdrReasonsDivAdded').on('click', '.ddlReasonItem', function () {
        // A currently selected Non-Standard Reason was reselected. Updated flags as needed
        $('.noData').hide();  //Just in case it is still visible
        var $nsReason = $(this).parents('.nsReason').first();
        var rsId_old = $nsReason.find("#reasonId").val();
        var rsId_selected = $(this).prop("id").replace("li_", "");
        var curLiId = "#" + $(this).prop("id");
        var oldLiId = "#li_" + rsId_old;

        $('.nsReason').each(function () {      //loop through all the Existing Drop down boxes
            //Remove (hide) the Selected Reason Id so it cannot be added back again anywhere
            $(this).find(curLiId).first().parents('li').first().hide();
            //Readd (show) the Old (Now Unselected) Reason Id so it can be added back again by user
            $(this).find(oldLiId).first().parents('li').first().show();
        });
        rsList_ItemChangeId(rsId_selected, "hide");   //Hide the Reason from the Page List so it is not included in New Drop Downs
        rsList_ItemChangeId(rsId_old, "show");        // Reset the Reason from the Page List so it is shown and included in New Drop Downs

        //Reset the Hidden Fields to the new values
        $nsReason.find("#wasUpdated").first().val("Y");
        $nsReason.find("#reasonId").first().val(rsId_selected);
        $nsReason.find("#nsReasonText").first().html($(this).html());
        $('html, body').animate({ scrollTop: $(document).height() }, "fast");        //Scroll to the botttom of the page to avoid hidding the newly added element
        $('#btnSaveProg').prop("disabled", false);
        //alert("Non Standard Reason Drop Down List Item Action Performed:\nSelection Changed from Id: " + rsId_old + " to: " + rsId_selected);
    });

    $('#inputReasonText').keydown(function (e) {
        $('#puErrorMsg').html("").hide();  //Hide the error message if visible
        $(this).removeClass("hasError");
        $('#btnAddReasonDetail').prop("disabled", false);
    });

    $('#btnCancelReasons').click(function () {
        //alert("Returning back to URL: " + returnUrl);
        window.location.href = decodeURIComponent($('#urlReturn').val());
    });

    $('#btnAddReasonDetail').click(function () {
        //Create the jason Payload topost to the server
        var reasonText = $('#inputReasonText').val();
        //------------ Form Data Validation -------------------
        //verify that user did not enter invalid characters
        if ((reasonText.indexOf("<") != -1) || (reasonText.indexOf("'") != -1) || (reasonText.indexOf("\"") != -1)) {
            $('#inputReasonText').addClass("hasError");
            $('#puErrorMsg').html("Reason Text has invalid characters.\nPlease correct and try again.").show();  //Display the error message
            return false;
        }
        if (reasonText == "") {
            $('#inputReasonText').addClass("hasError");
            $('#puErrorMsg').html("Reason Text Cannot be blank").show();
            //showAlert("Reason Text Cannot be blank");
            return false;
        }
        // --------- End of Form Validation -----------------------

        var mpID = getURL_mpid();
        var reasonText = $('#inputReasonText').val();
        var userId = $('#ssoName').val();
        var jsonPayload = '{"reasons": [{"mtrc_period_id": "' + mpID + '", "mpr_display_text": "' + reasonText + '", "mpr_desc": "", "mpr_std_yn": "N", "mpr_display_order": "", "user_id": "' + userId + '" }]}';
        //alert(jsonPayload);
        //Make ajax call to save (Add) Data (New MP Reason)
        $.ajax({
            url: '/ReasonMgmt/addMPReason',
            //url: 'http://dscapidev/dscmtrc/api/v1/metric/savereason',
            method: "POST",
            cache: false,
            //type: "POST",
            //data: payload,
            data: { raw_json: jsonPayload },
            //contentType: "application/json; charset=utf-8",
            //dataType: "json",
            error: function (jqXHR, textStatus, errorThrown) {
                showAlert("Failed to Save Data. Ajax Failed!!\nError:" + textStatus + "," + errorThrown);  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            // Check for Error Message response
            if (d.substring(0, 1) == "E") {
                $('#inputReasonText').addClass("hasError");
                showAlert(d);
            }
            else if (d != "0") {      // The "Add" Ajax Call is successful and we received an new reason_id back
                //alert("Reason Addedd Successfully\nJason result mpr_Id: " + d);
                $('.noData').hide();        //Hide the "No Data" Dashboard Message (If it happened to be visible)
                $('#puSuccessMsg').show().fadeOut(1000, "linear", function () { $('#addReasonForm').modal('hide'); });   //Show Success Message and after 1 second autoclose the popup

                //Add the newly added Reason to the Drop Down List
                var newReasonId = d;
                var insertionIndex = ($("#nonStdRsnDropDown").find("#reasonDropdown li").length) - 3;
                var reasonListItem = '<li style="padding:0px; "><a class="ddlReasonItem " id="li_' + newReasonId + '" style="padding:0px; margin-left:15px;" href="#">' + reasonText + '</a></li>';

                //Loop through all the drop down boxes to add the new Reason Element
                $('.nsReason').each(function () {      //loop through all the Existing Drop down boxes and add the new Reason
                    $(this).find("#reasonDropdown li:eq(" + insertionIndex + ")").after($(reasonListItem));
                });

                rsList_ItemAddId(d, reasonText);   //Add the New Reason to the Page List so it can be included in New Drop Downs
                //alert("Reason id: " + d + " [" + reasonText + "] was added.")
                $('#btnSaveProg').prop("disabled", false);    //Re-Enable the Save progress button
                //location.reload();
            } else {
                showAlert("Error Saving the data./nThe Reason entered could not be added to the Database.");
                //alert("Error Saving the data!\n" + JSON.stringify(d));
            }
        });
    });

    $("#showTable").dblclick(function () {
        $('#listTable').show();
    });


});