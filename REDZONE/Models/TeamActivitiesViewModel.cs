using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class TeamActivitiesViewModel
    {
        public List<TeamActivity> teamActivityList { get; set; }
        public TeamActivitiesViewModel()
        {
            teamActivityList = new List<TeamActivity>();
        }
    }

    public class TeamActivity
    {
        public string month { get; set; }
        public string monthName { get; set; }
        public string year { get; set; }
        [Display(Name = "Period")]
        public string periodName { get; set; }
        [Display(Name = "Building")]
        public string bldgName { get; set; }
        public string bldgId { get; set; }
        [Display(Name = "Metric")]
        public string mtrcName { get; set; }
        public string mtrcPeriodId { get; set; }
        public string rzBapmId { get; set; }
        [Display(Name = "Status")]
        public string rzBapmStatus { get; set; }
    }
}