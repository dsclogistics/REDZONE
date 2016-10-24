using Newtonsoft.Json.Linq;
using REDZONE.AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class dscUser
    {
        public string dbUserId { get; set; }
        public bool isValidUser { get; set; }
        public string userStatusMsg { get; set; }
        public string SSO { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string emailAddress { get; set; }
        public string fullName { get{ return (FirstName + " " + LastName).Trim(); } }    //Read Only Property
        public string other2 { get; set; }
        public List<userRole> roles = new List<userRole>();
        public List<building> buildings = new List<building>();
        #region Constructors
        //--------------- Constructors -------------------------------
        public dscUser() {}
        public dscUser(string userSSO) {
            SSO = userSSO;
            string jsonData = getUserJsonData();    //Retrieve all (JSON) User Info from DB using the SSO

            //Routine to parse the First Last Name
            if (userSSO.Contains("_"))
            {
                string[] names = userSSO.Split('_');
                FirstName = Util.Capitalize(names[0].Trim());
                LastName = Util.Capitalize(names[1].Trim());
            }
            else {
                FirstName = userSSO;
            }

            try           // -------- Try Parsing the User Data properties
            {
                JObject parsed_result = JObject.Parse(jsonData);
                //Verify that the Data Retrieval was successful before attempting to parse any data
                if (parsed_result["result"].ToString().Equals("Success") && !parsed_result["user_id"].ToString().Equals("0"))
                {
                    dbUserId = parsed_result["user_id"].ToString();
                    // ------- Retrieve all the User ROLES --------
                    JArray jRoles = (JArray)parsed_result["roles"];
                    if (jRoles.HasValues){
                        int roleIndex = 0;
                        foreach (var jRole in jRoles)
                        {
                            userRole uRole = new userRole();
                            uRole.roleDesc = (string)jRole["role_desc"];
                            uRole.id = (string)jRole["role_id"];
                            uRole.roleName = (string)jRole["role_name"];
                            uRole.appProduct = (string)jRole["prod_name"];
                            
                            //Add all the metrics for this role
                            foreach (var rMetric in jRole["metrics"]) {
                                roleMetric roleMetricInfo = new roleMetric();
                                roleMetricInfo.mpId = (string)rMetric["metric_period_id"];
                                roleMetricInfo.mpName = (string)rMetric["metric_period_name"];
                                uRole.roleMetrics.Add(roleMetricInfo);  //Add a metric entry to this Role's Metrics
                            }

                            roles.Add(uRole); //Add a "Role" Entry to this user Roles
                            roleIndex++;      //Get Index of the Next Role in the Json Data
                        }
                    
                    }

                    // ------ Retrieve all the User BUILDINGS ----------
                    JArray jbldgList = (JArray)parsed_result["buildings"];
                    if (jbldgList!= null && jbldgList.HasValues) {
                        foreach (var building in jbldgList)
                        {
                            building userBuilding = new building();
                            userBuilding.id = (string)building["dsc_mtrc_lc_bldg_id"];      //Not Implemented yet. Name might change !!!!!!!!!!!!!!!
                            userBuilding.buildingName = (string)building["dsc_mtrc_lc_bldg_name"];      //Not Implemented yet. Name might change !!!!!!!!!!!!!!!
                            buildings.Add(userBuilding);
                        }                    
                    }

                    isValidUser = true;
                    userStatusMsg = "User Information Loaded Successfully";
                }
                else {
                    isValidUser = false;
                    dbUserId = "0";
                    userStatusMsg = "User Roles Information Not Found in the Database";
                }
            }
            catch (Exception ex)
            {
                isValidUser = false;
                dbUserId = "0";
                userStatusMsg = ex.Message;
            }
        } 
        //---------- Constructors Section Ends-------------------------------
        #endregion
        #region classMethods
        public string getUserJsonData(string userSSO="") {
            DataRetrieval api = new DataRetrieval();
            if (String.IsNullOrEmpty(userSSO)) { userSSO = SSO; }
            return api.getMockUserRoles(userSSO);
        }
        public List<string> getUserRolesList() {
            return roles.Select(x => x.roleName).ToList();
        }
        public string getUserRoles()
        {
            return String.Join(";", roles.Select(x => x.roleName).ToList());
        }
        #endregion
    }

    public class userRole {
        public string id { get; set; }
        public string appProduct { get; set; }
        public string roleName { get; set; }
        public string roleDesc { get; set; }
        public List<roleMetric> roleMetrics = new List<roleMetric>();
        public string metrics { get { return String.Join(", ", roleMetrics.Select(x => x.mpName).ToList()); } }
    }

    public class roleMetric {
        public string mpId { get; set; }             //Metric Period
        public string mpName { get; set; }
    }

    public class building {
        public string id { get; set; }
        public string buildingName { get; set; }
    }

}