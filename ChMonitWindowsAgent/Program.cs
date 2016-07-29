using ChMonitoring.Helpers;
using ChMonitoring.Http;
using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace ChMonitoring
{
    static class Program
    {
        private static int argMarker = 0;

        #region Public Methods

        public static void Main(string[] args)
        {
            ChMonitoring.Helpers.Logger.Log.Info("MonitWindowsAgent Main");

            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "MonitWindowsAgent");

            if (ctl == null)
            {
                if (args.Length > 0 && args[0] == "--install")
                {
                    ChMonitoring.Helpers.Logger.Log.Debug("Installing MonitWindowsAgent...");
                    ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                    argMarker++;
                }
                else
                    Console.WriteLine("Error - Monit not yet installed. Run monit with parameter '--install' first.");
                return;
            }

            if (args.Length > 0 && args[0] == "--uninstall")
            {
                ChMonitoring.Helpers.Logger.Log.Info("Uninstalling MonitWindowsAgent...");
                ctl.Close();
                ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                argMarker++;
                return;
            }

            handle_options(args);

            do_init();

            do_action(ctl, args);

            ctl.Close();
        }

        #endregion

        #region Private Methods

        private static void _validateOnce()
        {
            //TODO
        }

        private static void do_init()
        {
            ControlFileParser.parse();
        }

        private static void do_reinit(ServiceController ctl)
        {
            try
            {
                ctl.Stop();
                ctl.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(5));
            }
            catch (Exception e)
            {
                ChMonitoring.Helpers.Logger.Log.Error("Couldn't stop MonitWindowsAgent service");
                return;
            }

            ControlFileParser.parse();

            try
            {
                ctl.Start(new string[] { "reloaded" });
                ctl.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(5));
            }
            catch (Exception e)
            {
                ChMonitoring.Helpers.Logger.Log.Error("Couldn't start MonitWindowsAgent service");
                return;
            }
        }

        private static void do_action(ServiceController ctl, string[] args)
        {
            if (argMarker >= args.Length)
            {
                do_default(ctl);
                return;
            }

            string action = args[argMarker];

            switch (action)
            {
                case "start":
                case "stop":
                case "monitor":
                case "unmonitor":
                case "restart":
                    {
                        string service = string.Empty;
                        if (argMarker < args.Length - 1)
                            service = args[++argMarker];
                        if (!string.IsNullOrEmpty(MonitWindowsAgent.Run.mygroup) || !string.IsNullOrEmpty(service))
                        {
                            int errors = 0;
                            List<string> services = new List<string>();
                            if (!string.IsNullOrEmpty(MonitWindowsAgent.Run.mygroup))
                            {
                                foreach (var sg in MonitWindowsAgent.servicegrouplist)
                                    if (MonitWindowsAgent.Run.mygroup == sg.name)
                                    {
                                        foreach (var m in sg.members)
                                            services.Add(m.name);
                                        break;
                                    }
                                if (services.Count == 0)
                                {
                                    ChMonitoring.Helpers.Logger.Log.ErrorFormat("Group '{0}' not found\n", MonitWindowsAgent.Run.mygroup);
                                    throw new Exception("MyGroup not found");
                                }
                            }
                            else if (service == "all")
                            {
                                foreach (var s in MonitWindowsAgent.servicelist)
                                    services.Add(s.name);
                            }
                            else
                            {
                                services.Add(service);
                            }
                            errors = ctl.Status == ServiceControllerStatus.Running ? (Client.HttpClient_action(action, services) ? 0 : 1) : Control.ControlServiceString(services, action);
                            if (errors > 0)
                                throw new Exception("HttpClient terminated with an error");
                        }
                        else
                        {
                            ChMonitoring.Helpers.Logger.Log.ErrorFormat("Please specify a service name or 'all' after {0}\n", action);
                            throw new Exception("No service name specified");
                        }
                        break;
                    }
                case "reload":
                    {
                        ChMonitoring.Helpers.Logger.Log.Info("Reinitializing daemon");
                        do_reinit(ctl);
                        break;
                    }
                case "status":
                    {
                        string service = string.Empty;
                        if (argMarker < args.Length - 1)
                            service = args[++argMarker];
                        if (!Client.HttpClient_status(MonitWindowsAgent.Run.mygroup, service))
                            throw new Exception("HttpClient terminated with an error");
                        break;
                    }
                case "summary":
                    {
                        string service = string.Empty;
                        if (argMarker < args.Length - 1)
                            service = args[++argMarker];
                        if (!Client.HttpClient_summary(MonitWindowsAgent.Run.mygroup, service))
                            throw new Exception("HttpClient terminated with an error");
                        break;
                    }
                case "report":
                    {
                        string type = string.Empty;
                        if (argMarker < args.Length - 1)
                            type = args[++argMarker];
                        if (!Client.HttpClient_report(type))
                            throw new Exception("HttpClient terminated with an error");
                        break;
                    }
                case "procmatch":
                    {
                        string service = string.Empty;
                        if (argMarker < args.Length - 1)
                            service = args[++argMarker];
                        throw new Exception("Not implemented yet");
                        break;
                    }
                case "quit":
                    {
                        if (ctl.Status != ServiceControllerStatus.Running)
                            return;

                        try
                        {
                            ctl.Stop();
                            ctl.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(5));
                        }
                        catch (Exception e)
                        {
                            ChMonitoring.Helpers.Logger.Log.Error("Couldn't stop MonitWindowsAgent service");
                            return;
                        }
                        break;
                    }
                case "validate":
                    {
                        string service = string.Empty;
                        if (argMarker < args.Length - 1)
                            service = args[++argMarker];
                        throw new Exception("Not implemented yet");
                        Client.HttpClient_status(MonitWindowsAgent.Run.mygroup, service);
                        _validateOnce();
                        break;
                    }
                default:
                    {
                        ChMonitoring.Helpers.Logger.Log.ErrorFormat("Invalid argument -- {0}  (-h will show valid arguments)", action);
                        throw new Exception("Invalid argument");
                    }
            }
        }

        private static void do_default(ServiceController ctl)
        {
            if (ctl.Status == ServiceControllerStatus.Running)
            {
                _validateOnce();
                return;
            }

            if (!System.Environment.UserInteractive)
            {
                // create the Service and go!
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] {
                    new MonitWindowsAgentService()
                };
                ServiceBase.Run(ServicesToRun);
                return;
            }

            try
            {
                ctl.Start(new string[] { "default" });
                ctl.WaitForStatus(ServiceControllerStatus.Running);
            }
            catch (Exception e)
            {
                ChMonitoring.Helpers.Logger.Log.Error("Couldn't start MonitWindowsAgent service");
                return;
            }

        }

        private static void handle_options(string[] args)
        {
            //TODO
            while (argMarker < args.Length && args[argMarker][0] == '-')
            {
                switch (args[argMarker].Substring(1))
                {
                    case "h":
                        help();
                        break;
                    default:
                        break;
                }
                argMarker++;
            }
        }

        /**
         * Print the program's help message
         */
        private static void help()
        {
            Console.Write(@"Usage: ([install_option] | [options]+ [command])
               Install options are as follows:
                --install     Install the MonitWindowsAgent service
                --uninstall   Uninstall the MonitWindowsAgent service
               Options are as follows:
                -c file       Use this control file
                -d n          Run as a daemon once per n seconds
                -g name       Set group name for monit commands
                -l logfile    Print log information to this file
                -p pidfile    Use this lock file in daemon mode
                -s statefile  Set the file monit should write state information to
                -I            Do not run in background (needed for run from init)
                --id          Print Monit's unique ID
                --resetid     Reset Monit's unique ID. Use with caution
                -B            Batch command line mode (do not output tables or colors)
                -t            Run syntax check for the control file
                -v            Verbose mode, work noisy (diagnostic output)
                -vv           Very verbose mode, same as -v plus log stacktrace on error
                -H [filename] Print SHA1 and MD5 hashes of the file or of stdin if the
                              filename is omited; monit will exit afterwards
                -V            Print version number and patchlevel
                -h            Print this text
               Optional commands are as follows:
                start all             - Start all services
                start <name>          - Only start the named service
                stop all              - Stop all services
                stop <name>           - Stop the named service
                restart all           - Stop and start all services
                restart <name>        - Only restart the named service
                monitor all           - Enable monitoring of all services
                monitor <name>        - Only enable monitoring of the named service
                unmonitor all         - Disable monitoring of all services
                unmonitor <name>      - Only disable monitoring of the named service
                reload                - Reinitialize monit
                status [name]         - Print full status information for service(s)
                summary [name]        - Print short status information for service(s)
                report [up|down|..]   - Report services state. See manual for options
                quit                  - Kill monit daemon process
                validate              - Check all services and start if not running
                procmatch <pattern>   - Test process matching pattern");
        }

        #endregion
    }
}
