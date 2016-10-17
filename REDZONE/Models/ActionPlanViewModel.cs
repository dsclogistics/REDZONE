﻿using System;
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
        public string mpv_id { get; set; }
        public string bldgName { get; set; }
        public string mtrcDisplayText { get; set; }
        public string apMonth { get; set; }
        public string apYear { get; set; }
        public List<ActionPlan> actionPlanList { get; set; }
        public List<PriorActionPlan> priorActionPlanList { get; set; }

        public ActionPlanViewModel()
        {
            reasonList = new List<MPReason>();
            actionPlanList = new List<ActionPlan>();
            priorActionPlanList = new List<PriorActionPlan>();
        }

    }

}