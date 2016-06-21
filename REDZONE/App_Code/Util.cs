using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.App_Code
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

        public static bool isValidDataType(string dataType, string value)
        {

            switch (dataType)
            {
                case "dec":                   
                case "cur":
                case "pct":
                    float res;
                    return float.TryParse(value, out res);
                case "int":
                    int number;
                    return int.TryParse(value, out number);
                case "char":
                    return value.Length==1?true:false;
                case "str":
                    return true;
                default: return false;
            } 
        }

    }
}