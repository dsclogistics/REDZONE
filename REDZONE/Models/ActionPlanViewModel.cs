using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class ActionPlanViewModel
    {
        public List<MPReason> reasonList { get; set; }
        public string bapm_id { get; set; }
        public string bapmStatus { get; set; }
        public List<ActionPlan> actionPlanList { get; set; }

        public ActionPlanViewModel()
        {
            reasonList = new List<MPReason>();
            actionPlanList = new List<ActionPlan>();
        }

    }
}