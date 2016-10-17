using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REDZONE.Controllers
{
    public class BuildingController : Controller
    {
        //Unrelated test changes merge

        //// GET: Building
        //public ActionResult Index()
        //{
        //    return View();
        //}

        // GET: Building/APSummary
        public ActionResult _APSummary(string month, string building, string apData)
        {  //Display a specific Month Action Plan Summary
           // appData contains a list of metrics to display separated by "~"
            //each metric has comma separated values for Metric "Name", "Status", "Meeting Date", "Owner", "Last Updated"
            string viewId = "ap_" + month.Replace(",","").Replace(" ","");
            Models.monthActionPlan historyAP = new Models.monthActionPlan() {displayId=viewId, apMonth = month, apBuilding = building};
            
            string[] metrics = apData.Split(new string[] {"~"}, StringSplitOptions.RemoveEmptyEntries );
            foreach(string metric in metrics){
                string[] metricProperties = metric.Split(',');
                Models.ActionPlan redMetricAP;

                if (metricProperties.Length < 5) { redMetricAP = new Models.ActionPlan() { apMetric = "Invalid Data" }; }
                else {
                    redMetricAP = new Models.ActionPlan()
                    {
                        apMetric = metricProperties[0],
                        apStatus = metricProperties[1],
                        apMeetingDate = metricProperties[2],
                        submittedBy = metricProperties[3],
                        apLastUpdt = metricProperties[4]
                    };                
                }
                historyAP.metricActPlans.Add(redMetricAP);
            }
            
            
            //string apId, string mMonth, string metric, string status, string meetingDt, string owner, string lastUpdt


            //Models.ActionPlan ap = new Models.ActionPlan {apd_id=apId, apMetric=metric, apStatus=status, apMeetingDate = meetingDt, submittedBy = owner, apLastUpdt = lastUpdt};


            return PartialView(historyAP);
        }


    }
}