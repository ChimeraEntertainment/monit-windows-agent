using System;
using System.IO;
using System.Net;
using System.Text;
using ChMonitoring.Helpers;

namespace ChMonitoring.MonitLogic
{
    class MMonitClient
    {
        public void Start()
        {

        }

        public void Push()
        {
            if (string.IsNullOrEmpty(ConfigMgr.Config.MMonitCollectorUrl))
            {
                return; // TODO error log
            }

            var serialized = MonitWindowsAgent.Instance.GetStatusXmlData();
            // http://msdn.microsoft.com/en-us/library/debx8sh9(v=vs.110).aspx
            // send to mmonit
            var request = (HttpWebRequest)WebRequest.Create(ConfigMgr.Config.MMonitCollectorUrl);

            if (!string.IsNullOrEmpty(ConfigMgr.Config.MMonitCollectorUsername))
            {
                String username = ConfigMgr.Config.MMonitCollectorUsername;
                String password = ConfigMgr.Config.MMonitCollectorPassword;
                String encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);
            }

            request.Headers.Add("Pragma", "no-cache");
            request.UserAgent = "monit/5.8.1";
            request.Accept = "*/*";

            request.Method = "POST";
            request.ContentLength = serialized.SerializedString.Length;
            request.ContentType = "text/xml";
            
            try{
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(serialized.SerializedBytes, 0, serialized.SerializedBytes.Length);
                }

                HttpWebResponse response;
                response = request.GetResponse() as HttpWebResponse;
                Logger.Log.Debug("Sent to mmonit: " + serialized.SerializedString);
            }
            catch (Exception e)
            {
                Logger.Log.Error("Sent to mmonit: " + e);
            }
        }

    }
}
