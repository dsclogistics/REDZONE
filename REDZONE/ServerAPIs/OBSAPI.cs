using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using REDZONE.Servers;
using System.Net;
using System.IO;

namespace REDZONE.ServerAPIs
{
    // Common Payload Item that can be used by all API calls when definign their Payload data (innermost item on any Json Payload)
    public class payloadItem
    {
        string desc { get; set; }
        string name { get; set; }
        string valueType { get; set; }
        string value { get; set; }
        bool itemSet
        {
            get
            {
                return (desc != "" && name != "" && value != "" && valueType != "") ? true : false;
            }
        }
        // Constructor Initialize Items
        public payloadItem(string jName, string jDesc, string jType,string jValue)
        {
            name = jName;
            desc = jDesc;
            valueType = jType;
            value = jValue;
        }
        public override string ToString() 
        { 
            string quote = (valueType.ToUpper() == "INT") ? "" : "\"";
            return String.Format("\"{0}\":{1}{2}{1}",name,quote,value);
        }
    }
    public class OBSAPI
    {
        public APIServer apiServer;
        public string APIName = "";
        public string APIURL;
        public bool paramsSet = false;
        public string requestMethod = "";
        List<payloadItem> parameters = new List<payloadItem>();

        // ------- CONSTRUCTOR-----------
        public OBSAPI()
        {
        }// ------- END OF CONSTRUCTOR-----------

        public void addParameter(string pName, string pDesc, string pType, string pValue) {
            payloadItem parameter = new payloadItem(pName, pDesc, pType, pValue);
            parameters.Add(parameter);
        }

        public string jParamPayload() {
            StringBuilder sb = new StringBuilder("");
            sb.Append("{");
            foreach (payloadItem parameter in parameters) {
                sb.Append(parameter.ToString()).Append(",");
            }
            sb.Length--;   //sb.Remove(sb.Length-1, 1);       //Rempove the last character from the Stringbuilder Object
            sb.Append("}");
            return sb.ToString();
        }

        //public string getData() {
        //    string result = "";
        //    if (!paramsSet) {
        //        throw new Exception("API missing the required Parameters");
        //    }
        //// Add Logic to execute and get the API Json Response (result)
        //    return result;
        //}
        public string getJasonData() {
            string postPayload = jParamPayload();
            string JsonStringResponse = String.Empty;
            try
            {
                WebRequest request = WebRequest.Create(APIURL);
                request.Method = requestMethod.Equals("POST") ? "POST" : "GET";       // Default Request Method is always "GET"
                request.ContentType = "application/json";

                if (requestMethod.Equals("POST"))
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    Byte[] bytes = encoding.GetBytes(postPayload);
                    Stream newStream = request.GetRequestStream();
                    newStream.Write(bytes, 0, bytes.Length);
                    newStream.Close();
                }
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonStringResponse = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                JsonStringResponse = e.Message;
            }
            return JsonStringResponse;
        }
    }
    //=======================================================================================================================


    //======================================================================================================================||
    //============================          OBSERVATIONS DEFINED SERVER APIs       =========================================||
    //======================================================================================================================||


    //===============================  "DSCAuthenticationSrv" API (Authentication) =========================================//
    public class API_DSCAuthenticationSrv : OBSAPI
    {
        const string APINAME = "DSCAuthenticationSrv";       // <--- Important (This is the URL API Endpoint)
        // ------- CONSTRUCTOR-----------
        public API_DSCAuthenticationSrv(string apiEnvironment, string pUserName, string pUserPwd)
        {
            requestMethod = "POST";                          // <---Important; It will determine if there is an http "getRequest"            
            apiServer = new APIServer(apiEnvironment);
            APIURL = apiServer.APIbaseURL + APINAME;

            addParameter("username", "User Name", "STRING", pUserName);
            addParameter("password", "User Password", "STRING", pUserPwd);
            addParameter("appid", "Aplication Id", "STRING", "DM");
        }// ------- END OF CONSTRUCTOR-----------
    }
    //===============================  "DSCAuthenticationSrv" API (Authentication) =========================================//
    public class API_RZ_MetricProducs : OBSAPI
    {
        const string APINAME = "metricperiod";       // <--- Important (This is the URL API Endpoint)
        // ------- CONSTRUCTOR-----------
        public API_RZ_MetricProducs(string apiEnvironment, string test)
        {
            requestMethod = "POST";                          // <---Important; It will determine if there is an http "getRequest"            
            apiServer = new APIServer(apiEnvironment);
            APIURL = apiServer.APIbaseURL + APINAME;

            addParameter("test", "Dummy Description", "STRING", "test");
        }// ------- END OF CONSTRUCTOR-----------
    }
    //===============================  "Retrieve a list of Metric NAmes" API  =========================================//
    public class API_RZ_metricname : OBSAPI
    {
        const string APINAME = "metricname";       // <--- Important (This is the URL API Endpoint)
        // ------- CONSTRUCTOR-----------
        public API_RZ_metricname(string apiEnvironment, string productname, string tptname)
        {
            requestMethod = "POST";                          // <---Important; It will determine if there is an http "getRequest"            
            apiServer = new APIServer(apiEnvironment);
            APIURL = apiServer.APIbaseURL + APINAME;

            addParameter("productname", "Product Name", "STRING", productname);
            addParameter("tptname", "Product Time", "STRING", tptname);
        }// ------- END OF CONSTRUCTOR-----------
    }
    //=======================================================================================================================
    public class API_obs_getObserver : OBSAPI
    {
        const string APINAME = "obs_getObserver";       // <--- Important (This is the URL API Endpoint)
        // ------- CONSTRUCTOR-----------
        public API_obs_getObserver(string apiEnvironment, string pObserverFirstName, string pObserverLastName, string pObserverEmail)
        {
            requestMethod = "POST";                          // <---Important; It will determine if there is an http "getRequest"            
            apiServer = new APIServer(apiEnvironment);
            APIURL = apiServer.APIbaseURL + APINAME;

            //{"dsc_observer_emp_first_name":"Obs","dsc_observer_emp_last_name":"User","dsc_observer_emp_email_addr":"obs.user@dsc-logistics.com"}
            addParameter("dsc_observer_emp_first_name", "Observer First Name", "STRING", pObserverFirstName);
            addParameter("dsc_observer_emp_last_name", "Observer Last Name", "STRING", pObserverLastName);
            addParameter("dsc_observer_emp_email_addr", "Observer Email Address", "STRING", pObserverEmail);
        }// ------- END OF CONSTRUCTOR-----------
    }
    //=======================================================================================================================
    public class API_obs_getOpenReady : OBSAPI
    {
        // ------- CONSTRUCTOR-----------
        public API_obs_getOpenReady(string apiEnvironment)
        {

        }// ------- END OF CONSTRUCTOR-----------
    }
    //=======================================================================================================================
    public class API_obs_getLC : OBSAPI
    {
        const string APINAME = "obs_getLC";                                      // Important (This is the URL API Endpoint)

        // ------- CONSTRUCTOR-----------
        public API_obs_getLC(string apiEnvironemnt)
        {
            requestMethod = "GET";                          // <---Important; It will determine if there is an http "getRequest"            
            apiServer = new APIServer(apiEnvironemnt);
            APIURL = apiServer.APIbaseURL + APINAME;
            // No "Parameters" needed
        }// ------- END OF CONSTRUCTOR-----------
    }
    //=======================================================================================================================
    public class API_obs_getEmployees : OBSAPI
    {
        // ------- CONSTRUCTOR-----------
        public API_obs_getEmployees(string apiEnvironment)
        {

        }// ------- END OF CONSTRUCTOR-----------
    }
    //=======================================================================================================================
    public class API_obs_getFunctions : OBSAPI
    {
        // ------- CONSTRUCTOR-----------
        public API_obs_getFunctions(string apiEnvironment)
        {

        }// ------- END OF CONSTRUCTOR-----------
    }
    //=======================================================================================================================
    public class API_obs_getformslist : OBSAPI
    {
        // ------- CONSTRUCTOR-----------
        public API_obs_getformslist(string apiEnvironment)
        {

        }// ------- END OF CONSTRUCTOR-----------
    }
    //=======================================================================================================================
    public class API_obs_NewCollform : OBSAPI
    {
        // ------- CONSTRUCTOR-----------
        public API_obs_NewCollform(string apiEnvironment)
        {

        }// ------- END OF CONSTRUCTOR-----------
    }
    //=======================================================================================================================
    public class API_obs_Save : OBSAPI
    {
        // ------- CONSTRUCTOR-----------
        public API_obs_Save(string apiEnvironment)
        {

        }// ------- END OF CONSTRUCTOR-----------
    }
    //=======================================================================================================================
    public class API_obs_getCollform : OBSAPI
    {
        // ------- CONSTRUCTOR-----------
        public API_obs_getCollform(string apiEnvironment)
        {

        }// ------- END OF CONSTRUCTOR-----------
    }
    //=======================================================================================================================

}