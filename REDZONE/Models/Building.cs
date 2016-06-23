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
        public string buildingViewClass { get; set; }
        public string buildingErrorMsg { get; set; }
        public string valueErrorMsg { get; set; }
        public string saveFlag { get; set; }

        //-------- Empty Constructor -----------
        public Building()
        {
            saveFlag  = "N";
        }


    }

}