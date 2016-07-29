using System;
using System.IO;
using System.Net;
using System.Text;
using ChMonitoring.Helpers;
using ChMonitoring.MonitData;

namespace ChMonitoring
{
    internal class MMonit
    {
        public static MonitHandlerType HandleMmonit(Event_T E)
        {
            var rv = MonitHandlerType.Handler_Mmonit;

            if (MonitWindowsAgent.Run.mmonits == null || (E != null && !E.state_changed))
                return MonitHandlerType.Handler_Succeeded;

            foreach (var C in MonitWindowsAgent.Run.mmonits)
            {
                var xml = Xml.StatusXml(E, E != null ? MonitLevelType.Level_Summary : MonitLevelType.Level_Full, 2,
                    "TODO: LOCALHOST");

                var request = sendData(C, xml);
                if (request != null)
                    if (!checkData(request))
                        Logger.Log.Error("Pushing Data to M/Monit had an Error!");
                    else
                        rv = MonitHandlerType.Handler_Succeeded;
            }
            return rv;
        }

        private static HttpWebRequest sendData(Mmonit_T config, string D)
        {
            // http://msdn.microsoft.com/en-us/library/debx8sh9(v=vs.110).aspx
            // send to mmonit
            var request = (HttpWebRequest) WebRequest.Create(config.url.url);

            AddBasicAuthHeaderMonit(config, request);

            request.Headers.Add("Pragma", "no-cache");
            request.UserAgent = string.Format("Monit/{0}", MonitWindowsAgent.VERSION);
            request.Accept = "*/*";

            request.Method = "POST";
            request.ContentType = "text/xml";

            var encoding = Encoding.GetEncoding("ISO-8859-1");
            var byteData = encoding.GetBytes(D);

            request.ContentLength = byteData.Length;

            try
            {
                using (var reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteData, 0, byteData.Length);
                }
                Logger.Log.Debug("Sent to mmonit: " + D);
                return request;
            }
            catch
            {
                return null;
            }
        }

        private static bool checkData(HttpWebRequest req)
        {
            HttpWebResponse response = null;

            try
            {
                response = req.GetResponse() as HttpWebResponse;
            }
            catch
            {
                return false;
            }

            if (response == null)
                return false;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Logger.Log.Debug("Response from mmonit: OK");
            }
            else
            {
                try
                {
                    using (var respStream = response.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(respStream))
                        {
                            Logger.Log.Debug("Response from mmonit: " + streamReader.ReadToEnd());
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        private static void AddBasicAuthHeaderMonit(Mmonit_T config, HttpWebRequest request)
        {
            if (!string.IsNullOrEmpty(config.url.user))
            {
                var username = config.url.user;
                var password = config.url.password;
                var encoded =
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password)));
                request.Headers.Add("Authorization", "Basic " + encoded);
            }
        }
    }
}