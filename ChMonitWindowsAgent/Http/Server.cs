using System.Net;
using NHttp;

namespace ChMonitoring.Http
{
    internal class Server
    {
        private static HttpServer m_server;
        private static Cervlet m_cervlet;

        public static void Start()
        {
            m_cervlet = new Cervlet();
            m_cervlet.Start();

            m_server = new HttpServer();
            m_server.EndPoint = new IPEndPoint(IPAddress.Parse(MonitWindowsAgent.Run.httpd.address),
                MonitWindowsAgent.Run.httpd.port);
            m_server.RequestReceived += Processor.ServerRequestReceived;
            m_server.StateChanged += Processor.OnServerStateChanged;
            m_server.Start();
        }

        public static void Stop()
        {
            m_server.Stop();
            m_server.RequestReceived -= Processor.ServerRequestReceived;
            m_server.StateChanged -= Processor.OnServerStateChanged;

            m_server.Dispose();
            m_server = null;

            m_cervlet.Stop();
            m_cervlet = null;
        }
    }
}