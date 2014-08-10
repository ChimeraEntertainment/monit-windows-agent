using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using ChMonitoring.Helpers;

namespace ChMonitoring.Monitoring
{
    /// <summary>
    /// This is an implementation of Windows Service Monitoring which can be used to satisfy
    /// Monit's service monitoring needs. This is an implementation totally independent from Monit,
    /// and yet neither fully tested nor integrated: TODO
    /// </summary>
    class ServiceMonitoring
    {
        private Dictionary<string, List<string>> m_servicesToCheck;
        private List<string> m_stoppedServices;
        private static List<int> m_failCounters;
        private EventLog m_log;

        internal void Start(EventLog log)
        {
            m_stoppedServices = new List<string>();
            m_log = log;

            InitServiceMonitoring();
        }

        private void InitServiceMonitoring()
        {
            try
            {
                m_servicesToCheck = new Dictionary<string, List<string>>();
                m_failCounters = new List<int>();

                foreach (var svc in ConfigMgr.Config.Services)
                {
                    //if (servicesList.Count == 1 && node.InnerText == "x")
                    //    throw new Exception("New config file was generated, make new configuration before restart");

                    // and save the names of the services to check
                    if (!m_servicesToCheck.ContainsKey(svc.ServiceName))
                    {
                        m_servicesToCheck.Add(svc.ServiceName, new List<string>());

                        foreach (var dependentServiceName in svc.DependentServiceNames)
                        {
                            m_servicesToCheck[svc.ServiceName].Add(dependentServiceName);
                        }
                    }

                    m_failCounters.Add(0);
                }
            }
            catch (Exception ex)
            {
                m_log.WriteEntry("No Services in config file: " + ex, EventLogEntryType.Error);
            }
        }

        public void CheckServicesRunning()
        {
            var serviceList = InitServiceList();
            var dependingServices = new List<string>();

            // check each service its running
            int index = 0;
            foreach (ServiceController s in serviceList)
            {
                if (s.Status == ServiceControllerStatus.Stopped)
                {
                    // start and log, if a servic is not running
                    s.Start();

                    //get depending services to stop them later
                    if (m_servicesToCheck[s.ServiceName].Count > 0)
                    {
                        foreach (var depending in m_servicesToCheck[s.ServiceName])
                        {
                            dependingServices.Add(depending);
                        }
                    }

                    if (m_failCounters[index] == ConfigMgr.Config.FailedStarts)
                    {
                        // SendEMail(s.ServiceName, true, true);
                    }
                    else if (m_failCounters[index] < ConfigMgr.Config.FailedStarts)
                    {
                        // SendEMail(s.ServiceName, true, false);
                        m_failCounters[index]++;
                    }

                    m_log.WriteEntry("Service wasn´t running: " + "[" + s.ServiceName + "]", EventLogEntryType.Error);
                    if (!m_stoppedServices.Contains(s.ServiceName))
                        m_stoppedServices.Add(s.ServiceName);

                }
                else if (m_stoppedServices.Contains(s.ServiceName))
                {
                    //when service has stopped and is running now, send mail
                    if (s.Status == ServiceControllerStatus.Running)
                    {
                        // SendEMail(s.ServiceName, false, false);
                        m_failCounters[index] = 0;
                        m_log.WriteEntry("Service was started: " + "[" + s.ServiceName + "]", EventLogEntryType.Information);
                        m_stoppedServices.Remove(s.ServiceName);
                    }
                }
                index++;
            }

            //if gateserver stopped, but not the corresponding worldserver, stop this worldserver for reconnection!
            foreach (var s in serviceList)
            {
                if (dependingServices.Contains(s.ServiceName))
                {
                    if (s.Status == ServiceControllerStatus.Running)
                        s.Stop();
                }
            }

        }

        private List<ServiceController> InitServiceList()
        {
            // get all services, which should be watched
            var monitoredServices = new List<ServiceController>();
            var services = ServiceController.GetServices();

            foreach (ServiceController controller in services)
            {
                foreach (var servicePack in m_servicesToCheck)
                {
                    if (controller.ServiceName == servicePack.Key)
                    {
                        monitoredServices.Add(controller);
                    }
                }
            }

            return monitoredServices;
        }
    }
}
