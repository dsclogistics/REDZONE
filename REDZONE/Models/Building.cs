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
        public string buildingValue { get; set; }
        public string buildingLC { get; set; }
        public bool isEditable { get; set; }
        public bool isManual { get; set; }
        //-------- Empty Constructor -----------
        public Building() { }
        //-------- END Constructor -----------
        //--------  Constructor Initializer -----------
        public Building(string building_Name, string building_MetricValue, string building_Code, bool Is_editable)
        {
            buildingName = building_Name;
            buildingCode = building_Code;
            buildingValue = building_MetricValue;
            isEditable = Is_editable;
            //isManual = Is_Manual;
        } //-------- END of Initializer Constructor -----------
    }

}