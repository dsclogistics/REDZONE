using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.ModelViews
{
    public class dropDownItem
    {
        public string id { get; set; }
        public string Text { get; set; }
        public bool isSelected { get; set; }
        public string displayStyle { get; set; }
        //Class Constructor
        public dropDownItem(string ddl_ItemId, string text, string style, bool selected)
        {
            id = ddl_ItemId;             // Drop Down Item Id
            Text = text;                 // Drop Down Item Display Text
            displayStyle = style;        // Drop Down Item display type: "display:none" or blank (to display it in the drop down or not)
            isSelected = selected;       // Drop Down Item "selected" Indicator
        }
    }
}