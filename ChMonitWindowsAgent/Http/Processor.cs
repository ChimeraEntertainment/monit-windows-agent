using System;
using System.IO;
using System.Linq;
using System.Text;
using ChMonitoring.Helpers;
using NHttp;

namespace ChMonitoring.Http
{
    /// <summary>
    ///     The Http-Processor
    /// </summary>
    internal class Processor
    {
        public const string SERVER_VERSION = MonitWindowsAgent.SERVER_VERSION;
        public const string SERVER_PROTOCOL = "HTTP/1.0";
        public const string SERVER_NAME = "monit";
        public const string SERVER_URL = "http://mmonit.com/monit/";
        public static Action<HttpRequest, HttpResponse> doGet;
        public static Action<HttpRequest, HttpResponse> doPost;
        public static bool ServerRunning { private set; get; }

        public static void ServerRequestReceived(object sender, HttpRequestEventArgs eventArgs)
        {
            Logger.Log.Debug("Nhttpd.HttpServer: Received URL request to " + eventArgs.Request.RawUrl);

            // no check if no credentials are set up
            if (!string.IsNullOrEmpty(MonitWindowsAgent.Run.httpd.credentials[0].uname) &&
                !CheckCredentials(eventArgs.Request))
            {
                SendError(eventArgs.Response, HttpStatusCode.SC_UNAUTHORIZED,
                    "You are not authorized to access monit. Either you supplied the wrong credentials (e.g. bad password), or your browser doesn't understand how to supply the credentials required");
                eventArgs.Response.Headers.Add("WWW-Authenticate", "Basic realm=\"monit\"");
                return;
            }

            if (eventArgs.Request.HttpMethod == "GET" && doGet != null)
                doGet(eventArgs.Request, eventArgs.Response);
            else if (eventArgs.Request.HttpMethod == "POST" && doPost != null)
                doPost(eventArgs.Request, eventArgs.Response);
            else
                SendError(eventArgs.Response, HttpStatusCode.SC_NOT_IMPLEMENTED, "Method not implemented");
        }

        public static void OnServerStateChanged(object sender, EventArgs e)
        {
            var state = (sender as HttpServer).State;
            ServerRunning = state == HttpServerState.Started;
            Logger.Log.WarnFormat("Nhttpd.HttpServer: Server changed state to: {0}", state);
        }

        public static bool CheckCredentials(HttpRequest req)
        {
            if (req.Headers == null)
                return false;

            if (req.Headers["Authorization"] == null)
                return false;

            var credentials = req.Headers["Authorization"];
            try
            {
                credentials = Encoding.UTF8.GetString(Convert.FromBase64String(credentials.Substring(6)));
            }
            catch
            {
                return false;
            }

            var credentialValues = credentials.Split(':');

            if (credentialValues.Length < 2)
                return false;

            return (credentialValues[0].Equals(MonitWindowsAgent.Run.httpd.credentials[0].uname) &&
                    credentialValues[1].Equals(MonitWindowsAgent.Run.httpd.credentials[0].passwd));
        }

        public static string EscapeHTML(string s)
        {
            var sb = new StringBuilder(s.Length);

            foreach (var c in s)
            {
                if (c == '<')
                    sb.Append("&lt;");
                else if (c == '>')
                    sb.Append("&gt;");
                else if (c == '&')
                    sb.Append("&amp;");
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

        public static void SendError(HttpResponse res, HttpStatusCode code, string s, params object[] args)
        {
            var msg = string.Format(s, args);
            var err = HttpStatusCodes.GetStatusString(code);
            res.ContentType = "text/html";
            SetStatus(res, code);

            var responseString = new StringBuilder();
            responseString.AppendFormat("<html><head><title>{0} {1}</title></head><body bgcolor=#FFFFFF><h2>{2}</h2>",
                (int) code, err, err);
            responseString.Append(EscapeHTML(msg));
            Logger.Log.Error(string.Format("HttpRequest error: {0} {1}", (int) code, msg));
            responseString.AppendFormat("<hr><a href='{0}'><font size=-1>{1}</font></a></body></html>", SERVER_URL,
                GetServer());

            using (var writer = new StreamWriter(res.OutputStream))
            {
                writer.Write(responseString.ToString());
            }
        }

        public static void SetStatus(HttpResponse res, HttpStatusCode code)
        {
            res.StatusCode = (int) code;
            res.StatusDescription = HttpStatusCodes.GetStatusString(code);
        }

        public static string GetParameter(HttpRequest req, string name)
        {
            if (req.Params.AllKeys.Contains(name))
                return req.Params[name];
            return "";
        }

        public static string GetServer()
        {
            return string.Format("{0} {1}", SERVER_NAME, SERVER_VERSION);
        }
    }
}