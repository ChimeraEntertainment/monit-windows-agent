using System;
using System.Collections.Generic;
using ChMonitoring.Configuration;
using ChMonitoring.MonitData;

namespace ChMonitoring.Helpers
{
    internal class ServiceHelper
    {
        public static Service_T GetHostService()
        {
            // TODO
            return null;
        }

        public static Service_T GetSystemService()
        {
            var systemService = CreateService(null, SystemStats.GetHostname());
            systemService.collected = MonitWindowsAgent.systeminfo.collected;

            systemService.inf = new ProcessInfo_T
            {
                total_cpu_percent = MonitWindowsAgent.systeminfo.total_cpu_syst_percent,
                total_mem_kbyte = MonitWindowsAgent.systeminfo.total_mem_kbyte,
                // calculating free mem, then substracting this from 100 to get used mem percentage
                total_mem_percent = MonitWindowsAgent.systeminfo.total_mem_percent
            };

            //systemService.system = new monitServiceSystem();
            //// CPU
            //systemService.system.cpu = new monitServiceSystemCpu
            //{
            //    system = SystemStats.GetCPULoadPercentage(),
            //    user = 0,
            //};
            //// MEMORY
            //var totalMem = PerformanceInfo.GetTotalMemoryInKiB();
            //systemService.system.memory = new monitServiceSystemMemory()
            //{
            //    kilobyte = totalMem,
            //    // calculating free mem, then substracting this from 100 to get used mem percentage
            //    percent = Math.Round(100 - ((decimal)PerformanceInfo.GetPhysicalAvailableMemoryInKiB() / (decimal)totalMem) * 100, 1),
            //};
            //// LOAD
            //systemService.system.load = new monitServiceSystemLoad()
            //{
            //    avg01 = 0.07,
            //    avg05 = 0.03,
            //    avg15 = 0.08,
            //};
            //// SWAP
            //// Possible to use this on windows?
            //systemService.system.swap = new monitServiceSystemSwap()
            //{
            //    kilobyte = 0,
            //    percent = 0,
            //};

            return systemService;
        }

        //public void CheckServicesRunning() {
        //    var serviceList = InitServiceList();
        //    var dependingServices = new List<string>();

        //    // check each service its running
        //    foreach(var s in serviceList) {
        //        switch(s.Status) {
        //            case ServiceControllerStatus.Running:
        //                if(MonitoredServices[s.ServiceName] <= 0) {
        //                    //when service has stopped and is running now, send mail
        //                    Logger.Log.Info("Service running again: " + "[" + s.ServiceName + "]");
        //                    MonitoredServices[s.ServiceName] = 1;
        //                    // SendEMail(s.Key.ServiceName, false, false);
        //                }
        //                break;
        //            case ServiceControllerStatus.Stopped:
        //                if(MonitoredServices[s.ServiceName] >= 0) {
        //                    Logger.Log.Warn("Service stopped running: " + "[" + s.ServiceName + "]");
        //                    MonitoredServices[s.ServiceName] = 0;
        //                } else if(MonitoredServices[s.ServiceName] <= -ConfigMgr.Config.FailedStarts) {
        //                    if(MonitoredServices[s.ServiceName] == -ConfigMgr.Config.FailedStarts) {
        //                        Logger.Log.Error("Service is stuck in Status Stopped: " + "[" + s.ServiceName + "]");
        //                        MonitoredServices[s.ServiceName]--;
        //                        // SendEMail(s.Key.ServiceName, true, true);
        //                    }
        //                    continue;
        //                }

        //                //get depending services to stop them later
        //                dependingServices.AddRange(s.DependentServices.Select(sc => s.ServiceName));

        //                try {
        //                    s.Start();
        //                    MonitoredServices[s.ServiceName]--;
        //                } catch(Exception e) {
        //                    Logger.Log.Error("Exception occured while trying to start service: " + "[" + s.ServiceName + "]\n" + e);
        //                }
        //                break;
        //            default:
        //                if(MonitoredServices[s.ServiceName] != 0) {
        //                    Logger.Log.Warn("Service is in an unknown state: " + "[" + s.ServiceName + "]");
        //                    MonitoredServices[s.ServiceName] = 0;
        //                    // SendEMail(s.Key.ServiceName, true, false);
        //                }
        //                break;
        //        }
        //    }

        //    //if gateserver stopped, but not the corresponding worldserver, stop this worldserver for reconnection!
        //    foreach(var s in serviceList) {
        //        if(dependingServices.Contains(s.ServiceName)) {
        //            if(s.Status == ServiceControllerStatus.Running)
        //                s.Stop();
        //        }
        //    }
        //}

        /*
         * Create a new service object and add any current objects to the
         * service list.
         */

        public static Service_T CreateService(ServiceConfig sc, string name, string command = null)
        {
            var current = new Service_T();
            var type = MonitServiceType.Service_System;
            var value = "";
            Func<Service_T, bool> check = null;

            if (sc == null)
            {
                check = Validate.CheckSystem;
                type = MonitServiceType.Service_System;
                current.every = new CycleEvery_T {type = MonitEveryType.Every_Cycle, number = 0};
            }
            else
            {
                if (sc is ProcessConfig)
                {
                    check = Validate.CheckProcess;
                    type = MonitServiceType.Service_Process;
                    current.resourcelist = new List<Resource_T>();
                    var processConf = sc as ProcessConfig;
                    if (processConf.Resources != null && processConf.Resources.Count > 0)
                    {
                        foreach (var resource in processConf.Resources)
                        {
                            AddResource(current,
                                new Resource_T
                                {
                                    resource_id = (MonitResourceType) resource.Type,
                                    compOperator = resource.ComparisonOperator,
                                    limit = resource.Limit,
                                    action =
                                        GetEventAction((MonitActionType) resource.ActionType,
                                            MonitActionType.Action_Ignored)
                                });
                        }
                    }
                    else
                    {
                        Logger.Log.ErrorFormat("error: No Resources defined for Process: {0}", sc.Name);
                    }
                }
                else if (sc is FilesystemConfig)
                {
                    check = Validate.CheckFilesystem;
                    type = MonitServiceType.Service_Filesystem;
                    current.filesystemlist = new List<Filesystem_T>();
                    var filesystemConf = sc as FilesystemConfig;
                    if (filesystemConf.Filesystems != null && filesystemConf.Filesystems.Count > 0)
                    {
                        foreach (var filesystem in filesystemConf.Filesystems)
                        {
                            AddFilesystem(current,
                                new Filesystem_T
                                {
                                    resource = (MonitResourceType) filesystem.Type,
                                    compOperator = filesystem.ComparisonOperator,
                                    limit_absolute = filesystem.LimitAbsolute,
                                    limit_percent = filesystem.LimitPercent,
                                    action =
                                        GetEventAction((MonitActionType) filesystem.ActionType,
                                            MonitActionType.Action_Ignored)
                                });
                        }
                    }
                    else
                    {
                        Logger.Log.ErrorFormat("error: No Filesystems defined for Drive: {0}", sc.Name);
                    }
                }
                else
                    return null;

                current.mode = (MonitMonitorModeType) sc.MonitoringMode;

                if (sc.ActionRate != null)
                {
                    AddActionRate(current,
                        new ActionRate_T
                        {
                            count = sc.ActionRate.Count,
                            cycle = sc.ActionRate.Cycle,
                            action =
                                GetEventAction((MonitActionType)sc.ActionRate.ActionType, MonitActionType.Action_Alert)
                        });
                }
                else
                {
                    Logger.Log.ErrorFormat("warning: No ActionRate defined for Service: {0}", sc.Name);
                }

                if (sc.Every != null)
                {
                    current.every = new CycleEvery_T
                    {
                        type = (MonitEveryType)sc.Every.Type,
                        number = sc.Every.CycleNumber
                    };
                }
                else
                {
                    current.every = new CycleEvery_T { type = MonitEveryType.Every_Cycle, number = 0 };
                }
            }

            if (Util.ExistService(name))
            {
                Logger.Log.ErrorFormat("Service name conflict, {0} already defined", name);
                return null;
            }

            current.type = type;

            switch (type)
            {
                case MonitServiceType.Service_Filesystem:
                    current.inf = new FileSystemInfo_T();
                    break;
                case MonitServiceType.Service_File:
                    current.inf = new FileInfo_T();
                    break;
                case MonitServiceType.Service_Directory:
                    current.inf = new DirectoryInfo_T();
                    break;
                case MonitServiceType.Service_Fifo:
                    current.inf = new FiFoInfo_T();
                    break;
                case MonitServiceType.Service_Process:
                    current.inf = new ProcessInfo_T();
                    break;
                case MonitServiceType.Service_Net:
                    current.inf = new NetInfo_T();
                    break;
                default:
                    break;
            }
            Util.ResetInfo(current);

            if (type == MonitServiceType.Service_Program)
            {
                current.program = new Program_T();
                current.program.args = command;
                current.program.timeout = 5*60;
            }

            /* Set default values */
            current.monitor = MonitMonitorStateType.Monitor_Init;
            current.mode = MonitMonitorModeType.Monitor_Active;
            current.name = name;
            current.check = check;
            current.path = value;

            /* Initialize general event handlers */
            current.action_DATA = GetEventAction(MonitActionType.Action_Alert, MonitActionType.Action_Alert);
            current.action_EXEC = GetEventAction(MonitActionType.Action_Alert, MonitActionType.Action_Alert);
            current.action_INVALID = GetEventAction(MonitActionType.Action_Restart, MonitActionType.Action_Alert);

            current.action_MONIT_START = GetEventAction(MonitActionType.Action_Start, MonitActionType.Action_Ignored);
            current.action_MONIT_STOP = GetEventAction(MonitActionType.Action_Stop, MonitActionType.Action_Ignored);
            current.action_MONIT_RELOAD = GetEventAction(MonitActionType.Action_Start, MonitActionType.Action_Ignored);
            current.action_ACTION = GetEventAction(MonitActionType.Action_Alert, MonitActionType.Action_Ignored);

            current.collected = DateTime.UtcNow;
            current.eventlist = new List<Event_T>();

            return current;
        }

        /*
         * Add a service object to the servicelist
         */

        public static void AddService(Service_T s)
        {
            // Test sanity check
            switch (s.type)
            {
                case MonitServiceType.Service_Host:
                    //// Verify that a remote service has a port or an icmp list
                    //if (!s.portlist && !s.icmplist)
                    //{
                    //    Logger.Log.ErrorFormat("'check host' statement is incomplete: Please specify a port number to test\n or an icmp test at the remote host: '{0}'", s.name);
                    //    cfg_errflag++;
                    //}
                    break;
                case MonitServiceType.Service_Program:
                    //// Verify that a program test has a status test
                    //if (!s.statuslist)
                    //{
                    //    Logger.Log.ErrorFormat("'check program {0}' is incomplete: Please add an 'if status != n' test", s.name);
                    //    cfg_errflag++;
                    //}
                    //// Create the Command object
                    //s.program.C = Command_new(s.path, NULL);
                    //// Append any arguments
                    //for (int i = 1; i < s.program.args.length; i++)
                    //    Command_appendArgument(s.program.C, s.program.args.arg[i]);
                    //if (s.program.args.has_uid)
                    //    Command_setUid(s.program.C, s.program.args.uid);
                    //if (s.program.args.has_gid)
                    //    Command_setGid(s.program.C, s.program.args.gid);
                    break;
                case MonitServiceType.Service_Net:
                    //if (!s.linkstatuslist)
                    //{
                    //    // Add link status test if not defined
                    //    addeventaction(&(linkstatusset).action, Action_Alert, Action_Alert);
                    //    addlinkstatus(s, &linkstatusset);
                    //}
                    break;
                case MonitServiceType.Service_Filesystem:
                    if (s.fsflaglist == null)
                    {
                        // Add filesystem flags change test if not defined
                        s.fsflaglist = new List<EventAction_T>();
                        s.fsflaglist.Add(GetEventAction(MonitActionType.Action_Alert, MonitActionType.Action_Ignored));
                    }
                    break;
                case MonitServiceType.Service_Directory:
                case MonitServiceType.Service_Fifo:
                case MonitServiceType.Service_File:
                case MonitServiceType.Service_Process:
                    if (s.nonexistlist == null)
                    {
                        // Add existence test if not defined
                        s.nonexistlist = new List<EventAction_T>();
                        s.nonexistlist.Insert(0,
                            GetEventAction(MonitActionType.Action_Restart, MonitActionType.Action_Alert));
                    }
                    break;
                default:
                    break;
            }

            MonitWindowsAgent.servicelist.Add(s);
            MonitWindowsAgent.servicelist_conf.Add(s);
        }

        /*
         * Add a new object to the current service actionrate list
         */

        public static void AddActionRate(Service_T s, ActionRate_T ar)
        {
            if (ar.count > ar.cycle)
                Logger.Log.Error("The number of restarts must be less than poll cycles");
            if (ar.count <= 0 || ar.cycle <= 0)
                Logger.Log.Error("Zero or negative values not allowed in a action rate statement");

            if (s.actionratelist == null)
                s.actionratelist = new List<ActionRate_T>();
            s.actionratelist.Insert(0, ar);
        }

        /*
         * Add a new filesystem to the current service's filesystem list
         */

        public static void AddFilesystem(Service_T s, Filesystem_T ds)
        {
            if (s.filesystemlist == null)
                s.filesystemlist = new List<Filesystem_T>();
            s.filesystemlist.Insert(0, ds);
        }

        public static void AddResource(Service_T s, Resource_T rr)
        {
            if (s.resourcelist == null)
                s.resourcelist = new List<Resource_T>();
            s.resourcelist.Insert(0, rr);
        }

        public static EventAction_T GetEventAction(MonitActionType failed, MonitActionType succeeded,
            Func<bool> command1 = null, Func<bool> command2 = null)
        {
            var ea = new EventAction_T();

            ea.failed = new Action_T();
            ea.succeeded = new Action_T();

            ea.failed.id = failed;
            ea.failed.count = 1;
            ea.failed.cycles = 1;
            if (failed == MonitActionType.Action_Exec)
                ea.failed.exec = command1;

            ea.succeeded.id = succeeded;
            ea.succeeded.count = 1;
            ea.succeeded.cycles = 1;
            if (succeeded == MonitActionType.Action_Exec)
                ea.succeeded.exec = command2;

            return ea;
        }
    }
}