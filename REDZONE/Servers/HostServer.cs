using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.Servers
{
    public class HostServer
    {
        public string HostName { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string IP_Address { get; set; }
        public string Test1 { get; set; }

        public HostServer()
        {
        }
    }

    public class APIServer : HostServer
    {
        //const string PROD_APIURL = "http://dscapi.dsccorp.net/dscrest/api/v1/getobsemp/";
        //const string DEV_APIURL = "http://dscapidev.dsccorp.net/dscrest/api/v1/getobsemp/";
        //const string QA_APIURL = "http://192.168.43.191.dsccorp.net/dscrest/api/v1/getobsemp/";
        const string PROD_APIURL = "http://dscapi/dscmtrc/api/v1/metric/";
        const string DEV_APIURL = "http://dscapidev/dscmtrc/api/v1/metric/";
        const string QA_APIURL = "http://dscapiqa/dscmtrc/api/v1/metric/";
        const string API_PROD_IP = "192.168.1.114";
        const string API_DEV_IP = "192.168.43.119";
        const string API_QA_IP = "192.168.43.191";

        public string APIbaseURL { get; set; }

        public APIServer(string environment)
        {
            switch (environment)
            {
                case "PROD":
                    HostName = "DSCAPI";
                    Description = "DSC Production API Server";
                    IP_Address = API_PROD_IP;
                    APIbaseURL = PROD_APIURL;
                    break;
                case "DEV":
                    HostName = "DSCAPIDEV";
                    Description = "DSC Development API Server";
                    IP_Address = API_DEV_IP;
                    APIbaseURL = DEV_APIURL;
                    break;
                case "QA":
                    HostName = "DSCAPIQA";
                    Description = "DSC QA API Server";
                    IP_Address = API_QA_IP;
                    APIbaseURL = QA_APIURL;
                    break;
                default:
                    throw new Exception("Server Environment Is invalid and it can't be instantiated.");
            }
            Type = "API";   //All API Environments are the same Server Type
        }
    }

    public class SQLServer : HostServer
    {
        const string PROD_IP = "192.168.1.182";
        const string DEV_IP = "Undefined";
        const string QA_IP = "Undefined";

        //private string APIbaseURL { get; set; }

        public SQLServer(string environment)
        {
            switch (environment)
            {
                case "PROD":
                    HostName = "dscsqlappsprod1";
                    Description = "DSC SQL Production Server";
                    IP_Address = PROD_IP;
                    break;
                case "DEV":
                    HostName = "DSCAPIDEV";
                    Description = "DSC SQL Development Server";
                    IP_Address = DEV_IP;
                    break;
                case "QA":
                    HostName = "DSCAPIQA";
                    Description = "DSC SQL QA Server";
                    IP_Address = QA_IP;
                    break;
                default:
                    throw new Exception("Server Environment Is invalid and it can't be instantiated.");
            }
            Type = "API";   //All API Environments are the same Server Type
        }
    }

}