using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace REDZONE.App_Code
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
    }
}