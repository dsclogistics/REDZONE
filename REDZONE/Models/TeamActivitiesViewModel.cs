using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class TeamActivitiesViewModel
    {
        public List<TeamActivityPeriod> periodList { get; set; }
        public TeamActivitiesViewModel()
        {
            periodList = new List<TeamActivityPeriod>();
        }
    }

    public class TeamActivityPeriod
    {
        public string month { get; set; }
        public string monthName { get; set; }
        public string year { get; set; }
        public string periodName { get; set; }
        public List<TeamActivityPeriodBuilding> periodBldgList { get; set; }
        public TeamActivityPeriod()
        {
            periodBldgList = new List<TeamActivityPeriodBuilding>();
        }
    }

    public class TeamActivityPeriodBuilding
    {
        public string bldgName { get; set; }
        public string bldgId { get; set; }
        public List<TeamActivity> activityList { get; set; }
        public TeamActivityPeriodBuilding()
        {
            activityList = new List<TeamActivity>();
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
        public string rzBapmStartDate { get; set; }
    }

    //COUNT MODEL
    public class TeamActivityCount
    {
        public List<TeamActivityBuildingCount> buildingActivityList { get; set; }
        public int totalTasks { get { return buildingActivityList.Sum(x => x.submitCount) + buildingActivityList.Sum(x => x.reviewCount); } }
        public TeamActivityCount()
        {
            buildingActivityList = new List<TeamActivityBuildingCount>();
        }
    }

    public class TeamActivityBuildingCount
    {
        public string bldgId { get; set; }
        public string bldgName { get; set; }
        public int submitCount { get; set; }
        public int reviewCount { get; set; }
    }
}