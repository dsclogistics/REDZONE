﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace REDZONE.AppCode
{
    public class Util
    {
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

        public static bool isValidBuilding(string[] list_of_buildings, string building_to_check)
        {
            
            foreach (string s in list_of_buildings)
            {
                if (s.ToUpper().Equals(building_to_check.ToUpper())) { return true; }
                else { continue; }
            }
            return false;
        }

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

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        // This function reads the current Server Webconfig File and retrieves the value of the Appsetting Variable passed as a parameter "key"
        public static string ReadSetting(string key)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string result = appSettings[key] ?? "";
            return result;
        }

        // ===== Retrieve the API URL USed for the application based on the current Environment's Server Name
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

        public static string getMonthLongName(string shortName)
        {
            string longName = String.Empty;
            switch (shortName)
            {
                case "Jan": longName = "January";
                    break;
                case "Feb": longName = "February";
                    break;
                //case "March":    monthShort = "March"; break;
                //case "April":    monthShort = "April"; break;
                //case "May":      monthShort = "May";   break;
                //case "June":     monthShort = "June";  break;
                //case "July":     monthShort = "July";  break;
                case "Aug": longName = "August";
                    break;
                case "Sept": longName = "September";
                    break;
                case "Oct": longName = "October";
                    break;
                case "Nov": longName = "November";
                    break;
                case "Dec": longName = "December";
                    break;
                default: longName = shortName;
                    break;
            }

            return longName;
        }

        public static string getUserRoles(string username)
        {
            //Get User Role from DB or from harcoded List
            string appUserRoles = String.Empty;
            //appUserRoles = "1;2;3;4;5;6;7;8";          //Temp Hardcoding
            switch (username.ToUpper())
            {
                // Set ADMIN Group Level
                case "DELGADO_FELICIANO":
                case "ABDUGUEV_RASUL":
                case "CHEN_ALEX":
                    appUserRoles = "ADMIN;";
                    break;
                case "GOPAL_GIRI":
                    appUserRoles = "SUPER";
                    break;
                default:
                    appUserRoles = "USER";
                    break;
            }
            return appUserRoles;
        }
    }


}