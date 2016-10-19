using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace REDZONE.AppCode
{
    public class Util
    {
        //-------------------------------------------------------------------------------------------------------------------------
        public static bool matchesSearchCriteria(string search_for_string, string search_in, string search_criteria)
        {
            string[] splitterm = { " " };
            string[] splitted_search_string = search_for_string.Split(splitterm, StringSplitOptions.RemoveEmptyEntries);
            switch (search_criteria)
            {
                case "Any":
                    foreach (string s in splitted_search_string)
                    {
                        if (search_in.ToLower().Contains(s.ToLower())) { return true; }
                        else { continue; }
                    }
                    return false;

                case "All":
                    foreach (string s in splitted_search_string)
                    {
                        if (search_in.ToLower().Contains(s.ToLower())) { continue; }
                        else { return false; }
                    }
                    return true;

                case "Exact":
                    if (search_in.ToLower().Contains(search_for_string.ToLower())) { return true; }
                    else return false;

                default:
                    return false;
            }

        }
        //-------------------------------------------------------------------------------------------------------------------------
        public static string isValidDataType(string dataType, string value,  string mtrcMinVal, string mtrcMaxVal, string maxDecPlaces, string maxStrSize)
        {
            if (String.IsNullOrEmpty(value))
            {
                return "True";
            }
            if (value.ToUpper().Equals("N/A") || value.ToUpper().Equals("NA"))
            {
                return  "True";
            }
            switch (dataType)
            {
                case "pct":
                case "dec":                   
                case "cur":
                    float res;
                    int decDigits = 0;
                    double min = Convert.ToDouble(mtrcMinVal);
                    double max = Convert.ToDouble(mtrcMaxVal);                   
                    try
                    {
                        decDigits = value.IndexOf(".")==-1? decDigits:value.Substring(value.IndexOf(".")+1).Length;
                    }
                    catch { }
                    if (float.TryParse(value, out res))
                    {
                        //if (res < min || res > max ||  Convert.ToInt16(maxDecPlaces)<decDigits)
                        if (res < min || res > max)
                        {
                            //return "Value must be between [" + min + "] and [" + max + " ] and have no more than [" + maxDecPlaces + " ] digit(s) after decimal point";
                            return "Value must be between [" + min + "] and [" + max + " ]";
                        }
                        else
                        {
                            return "True";
                        }
                    }
                    else
                    {
                        // return "Value:[ " + value + " ] is invalid for this metric. Value must be between [" + min + "] and [" + max + " ] and have no more than [" + maxDecPlaces + " ] digit(s) after decimal point";
                        return "Value:[ " + value + " ] is invalid for this metric. Value must be between [" + min + "] and [" + max + " ] ";

                    }
                case "int":
                    int number;
                    if (int.TryParse(value, out number))
                    {
                        if (number < (int)(Convert.ToDouble(mtrcMinVal)) || number > (int)(Convert.ToDouble(mtrcMaxVal)))
                        {
                            return "Value must be between [ " + mtrcMinVal + "] and " + mtrcMaxVal + " ]";
                        }
                        else
                        {
                            return "True";
                        }
                    }
                    else
                    {
                        return "Value: [ " + value + " ] is invalid for this metric Value must be between [ " + mtrcMinVal + "] and " + mtrcMaxVal + " ]";
                    } 
                case "char":
                    return value.Length==1? "True" : "Value: [ " + value + " ] is invalid for this metric"; 
                case "str":
                    return value.Length>=maxStrSize.Length? "Value for this metric cannot be more than ["+ maxStrSize.Length+"] characters long " : "True";
                default: return "True";
            } 
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public static bool isValidBuilding(string[] list_of_buildings, string building_to_check)
        {
            
            foreach (string s in list_of_buildings)
            {
                if (s.ToUpper().Equals(building_to_check.ToUpper())) { return true; }
                else { continue; }
            }
            return false;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        // This function reads the current Server Webconfig File and retrieves the value of the Appsetting Variable passed as a parameter "key"
        public static string ReadSetting(string key)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string result = appSettings[key] ?? "";
            return result;
        }

        // ===== Retrieve the API URL USed for the application based on the current Environment's Server Name ==========
        public static string getAPIurl()
        {
            string serverName = Environment.MachineName.ToUpper();
            string applicationAPIurl = String.Empty;

            switch (serverName)
            {
                case "DSCAPPSQA1":
                    //QA Server
                    applicationAPIurl = ReadSetting("apiBaseURLQA");
                    break;
                case "DSCAPPSPROD1":
                    //PROD Server  192.168.1.181,  192.168.1.183 and 192.168.1.184
                    applicationAPIurl = ReadSetting("apiBaseURLPROD");
                    break;
                case "RASULMACHINENAME":
                    //Local API URL for Development Testing using Local Host API
                    applicationAPIurl = ReadSetting("apiBaseURLLocal");
                    break;
                case "L-9L28F12":
                    applicationAPIurl = ReadSetting("apiBaseURLLocal");
                    break;
                default:
                    //Default to the Development Server   192.168.43.43
                    applicationAPIurl = ReadSetting("apiBaseURL");
                    break;
            }
            return applicationAPIurl;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public static string getUserRoles(string username)
        {
            //Get User Role from DB or from harcoded List
            string appUserRoles = String.Empty;
            //appUserRoles = "1;2;3;4;5;6;7;8";          //Temp Hardcoding
            switch (username.ToUpper())
            {
                // Set ADMIN Group Level
                // Ed, John, Kevin G, Tracey White, Chris Boughey, Darrell, Jennifer Krueger, me, Giri, and all developers.

                case "POGANY_KEVIN":
                case "GOPAL_GIRI":
                case "ZUISS_EDWARD":
                case "OCALLAGHAN_JOHN":            //John.OCallaghan@dsc-logistics.com
                case "GLYNN_KEVIN":                //kevin.glynn@dsc-logistics.com
                case "WHITE_TRACEY":
                case "BOUGHEY_CHRISTOPHER":        //Chris.Boughey@dsc-logistics.com
                case "REED_DARRELL":               //darrell.reed@dsc-logistics.com
                case "FROSETH_ERICK":
                case "KRUEGER_JENNIFER":           //jennifer.krueger@dsc-logistics.com
                    appUserRoles = "ADMIN;REVIEWER";
                    break;
                case "DELGADO_FELICIANO":
                    appUserRoles = "ADMIN;";
                    break;
                case "CHEN_ALEX":
                    appUserRoles = "ADMIN;REVIEWER";
                    break;
                case "ABDUGUEV_RASUL":
                    appUserRoles = "ADMIN;REVIEWER;SUPERAPPROVER";
                    break;
                case "...":
                    appUserRoles = "SUPER";
                    break;
                default:
                    appUserRoles = "USER";
                    break;
            }
            return appUserRoles;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public static string getMetricMeetingOwner(string metricName){
            string meetingOwner = String.Empty;
            switch (metricName) { 
                case "Net FTE":
                case "OT":
                    meetingOwner = "Christina Dempsey";
                    break;
                case "Turnover":
                case "Trainees":
                    meetingOwner = "Mona Mounts";
                    break;
                case "Safety":
                    meetingOwner = "Joe Smiesko";
                    break;
                case "IT Tickets":
                    meetingOwner = "Pete Kuranchie";
                    break;
                case "Financial":
                    meetingOwner = "Steve Pignataro";
                    break;
                default:
                    meetingOwner = "TBD";
                    break;
            }
            return meetingOwner;
        }

        #region GEN_PURPOSE_FUNCTIONS
        //==========================================================================================================================\
        //================= GENERAL PURPOSE FUNCTIONS THAT CAN BE APPLICABLE TO ANY APPLICATION/PROGRAM ============================|
        //==========================================================================================================================|

        //------------ This function accepts a Date String and converts it to a new format --------------------------------------
        public static string formatDate(string dateString, string newFormat)
        {
            string tempDate = String.Empty;
            try { tempDate = DateTime.Parse(dateString).ToString(newFormat); }
            catch { tempDate = dateString; }            //If convertion fails, return the original String
            return tempDate;
        }
        //------------- Convert a Month's "Short Name" to it's Full Long Name ---------------------------------------------------
        public static string getMonthLongName(string shortName)
        {
            string longName = String.Empty;
            switch (shortName)
            {
                case "Jan": longName = "January"; break;
                case "Feb": longName = "February"; break;
                case "Mar": longName = "March"; break;
                case "Apr": longName = "April"; break;
                //case "May":      longName = "May";   break;
                case "Jun": longName = "June"; break;
                case "Jul": longName = "July"; break;
                case "Aug": longName = "August"; break;
                case "Sep": longName = "September"; break;
                case "Sept": longName = "September"; break;
                case "Oct": longName = "October"; break;
                case "Nov": longName = "November"; break;
                case "Dec": longName = "December"; break;
                default: longName = shortName; break;
            }

            return longName;
        }
        //------------- Convert a Month's "Long Name" to it's Short Abreviated Name ---------------------------------------------
        public static string getMonthShortName(string longName)
        {
            string shortName = String.Empty;
            switch (longName.ToUpper())
            {
                case "JANUARY": shortName = "Jan"; break;
                case "FEBRUARY": shortName = "Feb"; break;
                case "MARCH": shortName = "Mar"; break;
                case "APRIL": shortName = "Apr"; break;
                //case "May":      longName = "May";   break;
                case "JUNE": shortName = "Jun"; break;
                case "JULY": shortName = "Jul"; break;
                case "AUGUST": shortName = "Aug"; break;
                case "SEPTEMBER": shortName = "Sep"; break;
                case "OCTOBER": shortName = "Oct"; break;
                case "NOVEMBER": shortName = "Nov"; break;
                case "DECEMBER": shortName = "Dec"; break;
                default: shortName = longName; break;
            }
            return shortName;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public static int monthToInt(string monthName)
        {
            int monthNo = 0;
            switch (monthName)
            {
                case "January": monthNo = 1;
                    break;
                case "February": monthNo = 2;
                    break;
                case "March": monthNo = 3;
                    break;
                case "April": monthNo = 4;
                    break;
                case "May": monthNo = 5;
                    break;
                case "June": monthNo = 6;
                    break;
                case "July": monthNo = 7;
                    break;
                case "August": monthNo = 8;
                    break;
                case "September": monthNo = 9;
                    break;
                case "October": monthNo = 10;
                    break;
                case "November": monthNo = 11;
                    break;
                case "December": monthNo = 12;
                    break;
                default:
                    break;
            }
            return monthNo;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public static string intToMonth(int monthNo)
        {
            string monthName = "";
            switch (monthNo)
            {
                case 1:
                    monthName = "January";
                    break;
                case 2:
                    monthName = "February";
                    break;
                case 3:
                    monthName = "March";
                    break;
                case 4:
                    monthName = "April";
                    break;
                case 5:
                    monthName = "May";
                    break;
                case 6:
                    monthName = "June";
                    break;
                case 7:
                    monthName = "July";
                    break;
                case 8:
                    monthName = "August";
                    break;
                case 9:
                    monthName = "September";
                    break;
                case 10:
                    monthName = "October";
                    break;
                case 11:
                    monthName = "November";
                    break;
                case 12:
                    monthName = "December";
                    break;
                default:
                    break;
            }
            return monthName;
        }
        //----------- This function will Uppercase (Capitalize) the first Letter of each word in the input string ------------------
        public static string Capitalize(string inputString, bool allWords = true) {
            // If we want to capitalize all words in the input
            if (allWords)
            {  
                return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(inputString);
            }

            // If "allWords" flag is 'false', capitalize only the first Word
            char[] array = inputString.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0])) { array[0] = char.ToUpper(array[0]); }
            }
            //// Scan through all the input string letters looking for spaces. Change (if needed) to Uppercase the letters that follow an spaces.
            //for (int i = 1; i < array.Length; i++)
            //{
            //    if (array[i - 1] == ' ')
            //    {
            //        if (char.IsLower(array[i])) { array[i] = char.ToUpper(array[i]); }
            //    }
            //}
            return new string(array);
        }
        //-------------------------------------------------------------------------------------------------------------------------
        
        //-------------------------------------------------------------------------------------------------------------------------
        
        //-------------------------------------------------------------------------------------------------------------------------
        
        //-------------------------------------------------------------------------------------------------------------------------
        
        //-------------------------------------------------------------------------------------------------------------------------
        
        //-------------------------------------------------------------------------------------------------------------------------
        //======================================= END OF GENERAL PURPOSE FUNCTIONS =================================================|
        //==========================================================================================================================/
        #endregion
    }
}