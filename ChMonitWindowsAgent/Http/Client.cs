using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using ChMonitoring.MonitData;

namespace ChMonitoring.Http
{
    class Client
    {

        #region Private Methods


        private static void _argument(StringBuilder data, string name, string value)
        {
            string _value = WebUtility.UrlEncode(value);
            data.AppendFormat("{0}{1}={2}", data.Length > 0 ? "&" : "", name, _value);
        }

        private static bool _client(string request, StringBuilder data)
        {
            bool status = false;
            //if (! exist_daemon()) {
            //        LogError("Monit: the monit daemon is not running\n");
            //        return status;
            //}
            HttpWebRequest req = null;
            // if (MonitWindowsAgent.Run.httpd.flags & Httpd_Net) {
            // FIXME: Monit HTTP support IPv4 only currently ... when IPv6 is implemented change the family to Socket_Ip
            //SslOptions_T options = {
            //        .flags = (Run.httpd.flags & Httpd_Ssl) ? SSL_Enabled : SSL_Disabled,
            //        .clientpemfile = MonitWindowsAgent.Run.httpd.socket.net.ssl.clientpem,
            //        .allowSelfSigned = MonitWindowsAgent.Run.httpd.flags & Httpd_AllowSelfSignedCertificates
            //};
            string uri = string.Format("http://{0}:{1}{2}", !string.IsNullOrEmpty(MonitWindowsAgent.Run.httpd.address) ? MonitWindowsAgent.Run.httpd.address : "localhost", MonitWindowsAgent.Run.httpd.port, request);
            req = (HttpWebRequest)WebRequest.Create(uri);
            //} else if (MonitWindowsAgent.Run.httpd.flags & Httpd_Unix) {
            //        req = Socket_createUnix(Run.httpd.socket.unix.path, Socket_Tcp, Run.limits.networkTimeout);
            //} else {
            //        LogError("Monit: the monit HTTP interface is not enabled, please add the 'set httpd' statement and use the 'allow' option to allow monit to connect\n");
            //}
            if (req != null)
            {
                _argument(data, "format", "text");
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = data.Length;
                req.Credentials = new NetworkCredential(MonitWindowsAgent.Run.httpd.credentials[0].uname, MonitWindowsAgent.Run.httpd.credentials[0].passwd);
                using (var writer = new StreamWriter(req.GetRequestStream()))
                {
                    writer.Write(data.ToString());
                }
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                if (res == null || res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    ChMonitoring.Helpers.Logger.Log.ErrorFormat("{0}", "ServerRequest had an error");
                }
                else
                {
                    //boolean_t strip = (MonitWindowsAgent.Run.flags & Run_Batch || ! Color_support()) ? true : false;
                    //while (Socket_readLine(S, buf, sizeof(buf))) {
                    //        if (strip)
                    //                Color_strip(Box_strip(buf));
                    //        printf("%s", buf);
                    //}
                    status = true;
                }
            }
            return status;
        }

        #endregion


        #region Public Methods


        public static bool HttpClient_action(string action, List<string> services)
        {
            if (Util.GetAction(action) == MonitActionType.Action_Ignored)
            {
                ChMonitoring.Helpers.Logger.Log.ErrorFormat("Invalid action {0}\n", action);
                return false;
            }
            StringBuilder data = new StringBuilder();
            _argument(data, "action", action);
            foreach (var s in services)
            {
                _argument(data, "service", s);
            }
            bool rv = _client("/_doaction", data);
            return rv;
        }


        public static bool HttpClient_report(string type)
        {
            StringBuilder data = new StringBuilder();
            if (!string.IsNullOrEmpty(type))
                _argument(data, "type", type);
            bool rv = _client("/_report", data);
            return rv;
        }


        public static bool HttpClient_status(string group, string service)
        {
            StringBuilder data = new StringBuilder();
            if (!string.IsNullOrEmpty(service))
                _argument(data, "service", service);
            if (!string.IsNullOrEmpty(group))
                _argument(data, "group", group);
            bool rv = _client("/_status", data);
            return rv;
        }


        public static bool HttpClient_summary(string group, string service)
        {
            StringBuilder data = new StringBuilder();
            if (!string.IsNullOrEmpty(service))
                _argument(data, "service", service);
            if (!string.IsNullOrEmpty(group))
                _argument(data, "group", group);
            bool rv = _client("/_summary", data);
            return rv;
        }

        #endregion
    }

}
