using System;
using System.Collections.Generic;
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
                        if (res < min || res > max ||  Convert.ToInt16(maxDecPlaces)<decDigits)
                        {
                            return "Value must be between [" + min + "] and [" + max + " ] and have no more than [" + maxDecPlaces + " ] digit(s) after decimal point";
                        }
                        else
                        {
                            return "True";
                        }
                    }
                    else
                    {
                        return  "Value:[ " + value + " ] is invalid for this metric";
                    }                  
                case "int":
                    int number;
                    if (int.TryParse(value, out number))
                    {
                        if (number < Convert.ToInt16(mtrcMinVal) || number > Convert.ToInt16(mtrcMaxVal))
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
                        return "Value: [ " + value + " ] is invalid for this metric";
                    } 
                case "char":
                    return value.Length==1? "True" : "Value: [ " + value + " ] is invalid for this metric"; 
                case "str":
                    return value.Length>=maxStrSize.Length? "Value for this metric cannot be more than ["+ maxStrSize.Length+"] characters long " : "True";
                default: return "True";
            } 
        }

    }
}