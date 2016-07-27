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
        //private string api_url = Common.ReadSetting("apiBaseURL");
        private const string api_url =  "http://dscapidev/dscmtrc/api/v1/metric/";
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
    }
}