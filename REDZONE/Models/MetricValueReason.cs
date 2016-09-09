using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class MetricValueReason
    {
        public string rText { get; set; }
        public string rComment { get; set; }
        public bool isAssigned { get; set; }

        //--- Constructors---
        public MetricValueReason() { }
        public MetricValueReason(string text, string comment, bool assigned)
        {
            rText = text;
            rComment = comment;
            isAssigned = assigned;
        }
    }
}