using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    //Building Class Used to represent a single DSC LC Building
    public class Building
    {
        public string buildingName { get; set; }
        public string buildingCode { get; set; }
        public string metricPeriodValueID { get; set; }
        public string metricPeriodValue { get; set; }
        public string buildingLC { get; set; }
        public bool isEditable { get; set; }
        public bool isManual { get; set; }
        public string validationMsg(string type) {
            string valMsg = String.Empty;
            if (metricPeriodValue.GetType().ToString().Equals("Int16")) {

                valMsg = "There is an error";
            }
            return valMsg;
        }

        //-------- Empty Constructor -----------
        public Building() { }


    }

}