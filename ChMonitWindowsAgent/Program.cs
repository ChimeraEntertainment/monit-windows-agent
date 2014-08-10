using System;
using System.Diagnostics;
using System.ServiceProcess;
using ChMonitoring.Helpers;

namespace ChMonitoring
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// See MonitWindowsAgentService class comments for TODOs!
        /// See docs/create_service.bat on how to install this as a service.
        /// </summary>
        static void Main(string[] args)
        {
            // create the Service and go!
            var serviceToRun = new MonitWindowsAgentService();

            if (Environment.UserInteractive)
            {
                // TODO check for elevated privileges
                // Must be executed as administrator/with elevated privs
                serviceToRun.Init();

                serviceToRun.CustomEventLog = new EventLog("MonitWindowsAgent");

                serviceToRun.Start();

                Console.ReadLine();
                serviceToRun.Stop();
            }
            else
            {
                ServiceBase.Run(serviceToRun.Init());
            }
        }
    }
}
