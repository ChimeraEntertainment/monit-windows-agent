using System;
using System.IO;
using System.Net;
using System.Text;
using ChMonitoring.Helpers;
using NHttp;

namespace ChMonitoring.MonitLogic
{
    class MonitServer
    {
        /*
         * Possible requests from M/Monit:
         * #define HOME        "/"
            #define TEST        "/_monit"
            #define ABOUT       "/_about"
            #define PING        "/_ping"
            #define GETID       "/_getid"
            #define STATUS      "/_status"
            #define STATUS2     "/_status2"
            #define RUN         "/_runtime"
            #define VIEWLOG     "/_viewlog"
            #define DOACTION    "/_doaction"
            #define FAVICON     "/favicon.ico"
         * */
        internal void Start()
        {
            var server = new HttpServer();

            server.EndPoint = new IPEndPoint(IPAddress.Parse(ConfigMgr.Config.HttpdBindIp), ConfigMgr.Config.HttpdPort);
            
            server.RequestReceived += (sender, eventArgs) =>
            {
                string responseString = String.Empty;
                string reqUrl = eventArgs.Request.RawUrl;
                
                Logger.Log.Debug("Nhttpd.HttpServer: Received URL request to " + reqUrl);

                if (reqUrl.Equals("/_ping"))
                {
                    // pong is the expected answer
                    // see do_ping in cervlet.c
                    responseString = "pong";
                }

                else if (reqUrl.Equals("/_getid"))
                {
                    responseString = UniqueWindowsId.GetOrCreateUniqueId();
                }

                else if (reqUrl.Equals("/_status?format=xml"))
                {
                    if (!CheckCredentials(eventArgs.Request.Headers))
                    {
                        eventArgs.Response.StatusCode = 403;
                        eventArgs.Response.Status = "Forbidden";
                        return;
                    }

                    eventArgs.Response.ContentType = "text/xml";
                    responseString = MonitWindowsAgent.Instance.GetStatusXmlData().SerializedString;
                }
                else
                {
                    eventArgs.Response.StatusCode = 404;
                    eventArgs.Response.Status = "Not Found";
                }

                using (var writer = new StreamWriter(eventArgs.Response.OutputStream))
                {
                    writer.Write(responseString);
                }

                Logger.Log.Debug("Nhttpd.HttpServer: Responding with status " + eventArgs.Response.Status + ": " + responseString);
            };

            server.Start();
        }

        private bool CheckCredentials(System.Collections.Specialized.NameValueCollection nameValueCollection)
        {
            // no check if no credentials are set up
            if (string.IsNullOrEmpty(ConfigMgr.Config.HttpdUsername))
                return true;

            if (nameValueCollection == null)
                return false;

            if (nameValueCollection["Authorization"] == null)
                return false;

            string credentials;
            try
            {
                credentials = Encoding.UTF8.GetString(Convert.FromBase64String(nameValueCollection["Authorization"]));
            }
            catch
            {
                return false;
            }

            var credentialValues = credentials.Split(':');

            if (credentialValues.Length < 2)
                return false;

            return (credentialValues[0].Equals(ConfigMgr.Config.HttpdUsername) &&
                    credentialValues[1].Equals(ConfigMgr.Config.HttpdPassword));

        }

    }
}
