using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class ActionPlanViewModel
    {
        //User Authorization
        public bool canAccessAP { get; set; }
        public bool canEditReasons { get; set; }
        public bool canViewFinishedAP { get; set; }
        public bool canViewWipAP { get; set; }
        public bool canSubmitAP { get; set; }
        public bool canReviewAP { get; set; }

        //Model
        public List<MPReason> reasonList { get; set; }
        public string bapm_id { get; set; }
        public string bapmStatus { get; set; }
        public string mpv_id { get; set; }
        public string bldgName { get; set; }
        public string bldgId { get; set; }
        public string mtrcDisplayText { get; set; }
        public string apMonth { get; set; }
        public string apMonthName { get; set; }
        public string apYear { get; set; }
        public string goalText { get; set; }
        public string mtrcPeriodValue { get; set; }

        public List<ActionPlan> actionPlanList { get; set; }
        public List<PriorActionPlan> priorActionPlanList { get; set; }

        public string pageHeader { get; set; }
        public string currentAPVersion { get; set; }
        public string currentAPStatus { get; set; }
        public string displayStatus { get; set; }
        public string displayColor { get; set; }
        public string AP_EditableStatus { get; set; }
        public string AP_EditableColor { get; set; }
        public string review_EditableStatus { get; set; }

        public ActionPlanViewModel()
        {
            canAccessAP = false;
            canEditReasons = false;
            canViewFinishedAP = false;
            canViewWipAP = false;
            canSubmitAP = false;
            canReviewAP = false;
            reasonList = new List<MPReason>();
            actionPlanList = new List<ActionPlan>();
            priorActionPlanList = new List<PriorActionPlan>();
        }

    }

}