using REDZONE.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;


namespace REDZONE.AppCode
{
    public class DataRetrieval
    {
        //private DSC_MTRC_DEV_Entities db = new DSC_MTRC_DEV_Entities();
        private string api_url = AppCode.Util.getAPIurl();

        public string getMetricname( string productName, string tptName)
        {
            string endPoint = "metricname";
            WebRequest request = WebRequest.Create(api_url + endPoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            string parsedContent = "{\"productname\":\"" + productName + "\",\"tptname\":\"" + tptName + "\"}";
            Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string getAllBuildings(string productName, string tptName, string year)
        {
            string endPoint = "buildinglc";
            WebRequest request = WebRequest.Create(api_url + endPoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            string parsedContent = "{\"productname\":\"" + productName + "\",\"tptname\":\"" + tptName + "\",\"calyear\":\"" + year + "\"}";
            Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string getMetricperiod(string productName, string tptName, string mtrcid, string calmonth, string calyear)
        {
            // {"productname":"Red Zone", "tptname":"Month","mtrcid":3,"calmonth":"June","calyear":2016}
            string endPoint = "metricperiod";
            WebRequest request = WebRequest.Create(api_url + endPoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            string parsedContent = "{\"productname\":\"" + productName + "\",\"tptname\":\"" + tptName + "\",\"mtrcid\":\""+ mtrcid+
                                     "\",\"calmonth\":\""+ calmonth+ "\",\"calyear\":\""+ calyear+"\"}";
            Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return "ERROR: " +  e.Message;
            }
        }

        public string saveRZMetric(string raw_json)
        {
            string endPoint = "metricperiodsave";
            WebRequest request = WebRequest.Create(api_url + endPoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            string parsedContent = raw_json;
            Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string closePeriod(string productName, string tptName, string mtrcid, string calmonth, string calyear, string userId, string metricPeriodId)
        {
            string endPoint = "metricperiodclose";
            WebRequest request = WebRequest.Create(api_url + endPoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            string parsedContent = "{\"productname\":\"" + productName + "\",\"tptname\":\"" + tptName + "\",\"mtrcid\":\"" + mtrcid +
                                     "\",\"mtrc_period_id\":\""+ metricPeriodId +"\",\"calmonth\":\"" + calmonth + "\",\"calyear\":\"" + calyear + "\",\"user_id\":\"" + userId + "\"}";
            Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string authorizeUser(string userName)
        {
            string endPoint = "metricauthorization";
            WebRequest request = WebRequest.Create(api_url + endPoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            string parsedContent = "{\"username\":\"" + userName + "\"}";
            Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string getExecSummary(string productName, string tptName, string mtrcid, string calmonth, string calyear, string buildingID)
        {
            // {"productname":"Red Zone", "tptname":"Month","mtrcid":3,"calmonth":"June","calyear":2016}
            string endPoint = "summary";
            WebRequest request = WebRequest.Create(api_url + endPoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            string parsedContent = "{\"productname\":\"" + productName + "\",\"tptname\":\"" + tptName + "\",\"mtrcid\":\"" + mtrcid +
                                     "\",\"calmonth\":\"" + calmonth + "\",\"calyear\":\"" + calyear + "\",\"dsc_mtrc_lc_bldg_id\":\""+ buildingID + "\"}";
            Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return "ERROR: " + e.Message;
            }
        }
        public string getBuildingSummary(string productName, string tptName, string mtrcid, string calmonth, string calyear, string buildingID)
        {
            // {"productname":"Red Zone", "tptname":"Month","mtrcid":3,"calmonth":"June","calyear":2016}
            string endPoint = "buildingsummary";
            WebRequest request = WebRequest.Create(api_url + endPoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            string parsedContent = "{\"productname\":\"" + productName + "\",\"tptname\":\"" + tptName + "\",\"mtrcid\":\"" + mtrcid +
                                     "\",\"calmonth\":\"" + calmonth + "\",\"calyear\":\"" + calyear + "\",\"dsc_mtrc_lc_bldg_id\":\"" + buildingID + "\"}";
            Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return "ERROR: " + e.Message;
            }
        }
        public string getMetricSummary(string productName, string tptName, string mtrcid, string calyear)
        {
            
            string endPoint = "metricsummary";
            WebRequest request = WebRequest.Create(api_url + endPoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            // {"productname":"Red Zone", "tptname":"Month","calyear":2016 , "mtrc_id":"3"  }
            string parsedContent = "{\"productname\":\"" + productName + "\",\"tptname\":\"" + tptName + "\",\"mtrc_id\":\"" + mtrcid +
                                     "\",\"calyear\":\"" + calyear + "\"}";
            Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return "ERROR: " + e.Message;
            }
        }

        public string reloadMetricValues(string packageName, string tptName, string mtrcid, string calmonth, string calyear)
        {
            // {"packagename":"volume", "tptname":"Month", "calmonth":"July", "calyear":2016,"mtrc_id": "6" }
            string endPoint = "autouploadmetric";
            WebRequest request = WebRequest.Create(api_url + endPoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            string parsedContent = "{\"packagename\":\"" + packageName + "\",\"tptname\":\"" + tptName + "\",\"mtrc_id\":\"" + mtrcid +
                                     "\",\"calmonth\":\"" + calmonth + "\",\"calyear\":\"" + calyear + "\"}";
            Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return "ERROR: " + e.Message;
            }
        }
        //--------------------------------------------------------------------------------------------------------
        //Json Data Retrieval for Metric Period Reasons
        public string getMetricPeriodReasons(string metricPeriodId)
        {
            string endPoint = "reasons";
            WebRequest request = WebRequest.Create(api_url + endPoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            string parsedContent = "{\"mtrc_period_id\":\"" + metricPeriodId + "\"}"; 
            Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        //--------------------------------------------------------------------------------------------------------
        public string getAssignedMetricPeriodReasons(string mtrc_period_val_id)
        {
            string endPoint = "assignedreasons";
            WebRequest request = WebRequest.Create(api_url + endPoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            string parsedContent = "{\"mtrc_period_val_id\":\"" + mtrc_period_val_id + "\"}";
            Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        //--------------------------------------------------------------------------------------------------------
        public string getMetricPeriods()
        {
            string endPoint = "mmperiods";
            WebRequest request = WebRequest.Create(api_url + endPoint);
            request.Method = "GET";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            //string parsedContent = "{\"productname\":\"" + productName + "\",\"tptname\":\"" + tptName + "\",\"calyear\":\"" + year + "\"}";
            //Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                //Stream newStream = request.GetRequestStream();
                //newStream.Write(bytes, 0, bytes.Length);
                //newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        //Add Reason
        public string saveMPReason(string raw_json)
        {
            string endPoint = "savereason";
            WebRequest request = WebRequest.Create(api_url + endPoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            string parsedContent = raw_json;
            Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        //Update Reason
        public string updateMPReason(string raw_json)
        {
            string endPoint = "updatereason";
            WebRequest request = WebRequest.Create(api_url + endPoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            string parsedContent = raw_json;
            Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string getMPVReasons(string mtrc_period_val_id)
        {
            //This function returns a jSon Object with an array of all the Reasons Assigned for the "Metric Period Value Id" passed as a parameter

            string endPoint = "reasons";
            WebRequest request = WebRequest.Create(api_url + endPoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            ASCIIEncoding encoding = new ASCIIEncoding();
            // {"productname":"Red Zone", "tptname":"Month","calyear":2016 , "mtrc_id":"3"  }
            string parsedContent = "{\"mtrc_period_val_id\":\"" + mtrc_period_val_id + "\"}";
            Byte[] bytes = encoding.GetBytes(parsedContent);
            string JsonString = String.Empty;
            try
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                    return JsonString;
                }
            }
            catch (Exception e)
            {
                return "ERROR: " + e.Message;
            }
        }


    }
}