using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.ModelViews
{

    public class NonStdrReasonViewModel
    {
        public string checkBoxName { get; set; }    //Must be unique for each checkbox reason
        public string wasUpdated { get; set; }      //"Y" or "N" If it'a a newly added Reason, then "Y"
        public string originalStatus { get; set; }  //"checked" or blank
        public string reasonId { get; set; }        // reason Id Key Field     for DD Value
        public string reasonText { get; set; }      // reason Text to display  for DD Text
        public string reasonComment { get; set; }      // reason Text to display  for DD Text
        public string valueReasonId { get; set; }   // Metric Period Value Assigned Reason Id (For use in ajax calls)
        public List<dropDownItem> ddItems = new List<dropDownItem>();  //List of all non-standard items to display on the Dropdown List
        //public List<string> ddItems = new List<string>();
        // Class Constructor
        public NonStdrReasonViewModel() {
            //ddItems.Add("Item 1");
            //ddItems.Add("Item 2");
            //ddItems.Add("Item 3");
            //ddItems.Add("Item 4");
        }

    }
    
}