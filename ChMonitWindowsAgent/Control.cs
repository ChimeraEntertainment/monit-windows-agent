using ChMonitoring.Helpers;
using ChMonitoring.MonitData;
using System.Collections.Generic;

namespace ChMonitoring
{
    /// <summary>
    ///     The class responsible for all controlling Actions
    /// </summary>
    internal class Control
    {
        #region Public Methods

        /// <summary>
        ///     Check to see if we should try to start/stop service
        /// </summary>
        /// <param name="S">A service name as stated in the config file</param>
        /// <param name="A">A string describing the action to execute</param>
        /// <returns>false for error, otherwise true</returns>
        public static int ControlServiceString(List<string> services, string action)
        {
            MonitActionType a;
            if ((a = Util.GetAction(action)) == MonitActionType.Action_Ignored)
            {
                Logger.Log.ErrorFormat("invalid action {0}", action);
                return 1;
            }
            int errors = 0;
            foreach (var s in services)
            {
                if (ControlService(s, a) == false)
                    errors++;
            }
            return errors;
        }


        /// <summary>
        ///     Check to see if we should try to start/stop service
        /// </summary>
        /// <param name="S">A service name as stated in the config file</param>
        /// <param name="A">An action id describing the action to execute</param>
        /// <returns>false for error, otherwise true</returns>
        public static bool ControlService(string S, MonitActionType A)
        {
            Service_T s = null;
            if ((s = Util.GetService(S)) == null)
            {
                Logger.Log.ErrorFormat("Service '{0}' -- doesn't exist", S);
                return false;
            }

            switch (A)
            {
                case MonitActionType.Action_Start:
                    _doDepend(s, MonitActionType.Action_Stop, false);
                    _doStart(s);
                    _doDepend(s, MonitActionType.Action_Start, false);
                    break;

                case MonitActionType.Action_Stop:
                    _doDepend(s, MonitActionType.Action_Stop, true);
                    _doStop(s, true);
                    break;

                case MonitActionType.Action_Restart:
                    Logger.Log.InfoFormat("'{0}' trying to restart", s.name);
                    _doDepend(s, MonitActionType.Action_Stop, false);
                    if (s.restart != null)
                    {
                        _doRestart(s);
                        _doDepend(s, MonitActionType.Action_Start, false);
                    }
                    else
                    {
                        if (_doStop(s, false))
                        {
                            /* Only start if stop succeeded */
                            _doStart(s);
                            _doDepend(s, MonitActionType.Action_Start, false);
                        }
                        else
                        {
                            /* enable monitoring of this service again to allow the restart retry in the next cycle up to timeout limit */
                            Util.MonitorSet(s);
                        }
                    }
                    break;

                case MonitActionType.Action_Monitor:
                    /* We only enable monitoring of this service and all prerequisite services. Chain of services which depends on this service keep its state */
                    _doMonitor(s, false);
                    break;

                case MonitActionType.Action_Unmonitor:
                    /* We disable monitoring of this service and all services which depends on it */
                    _doDepend(s, MonitActionType.Action_Unmonitor, false);
                    _doUnmonitor(s, false);
                    break;

                default:
                    Logger.Log.ErrorFormat("Service '{0}' -- invalid action {1}", S, A);
                    return false;
            }
            return true;
        }

        /*
         * Reset the visited flags used when handling dependencies
         */

        public static void ResetDepend()
        {
            foreach (var s in MonitWindowsAgent.servicelist)
            {
                s.visited = false;
                s.depend_visited = false;
            }
        }

        #endregion

        #region Private Methods

        /*
         * This is a post- fix recursive function for starting every service
         * that s depends on before starting s.
         * @param s A Service_T object
         */

        private static void _doStart(Service_T s)
        {
            if (s.visited)
                return;
            s.visited = true;
            if (s.dependantlist != null)
            {
                foreach (var d in s.dependantlist)
                {
                    var parent = Util.GetService(d);
                    _doStart(parent);
                }
            }
            if (s.start != null)
            {
                if (s.type != MonitServiceType.Service_Process || Util.IsProcessRunning(s, false) == 0)
                {
                    Logger.Log.InfoFormat("'{0}' start", s.name);
                    if (s.type == MonitServiceType.Service_Process)
                    {
                        if (s.start())
                        {
                            Event.Post(s, MonitEventType.Event_Exec, MonitStateType.State_Succeeded, s.action_EXEC,
                                "started");
                        }
                        else
                        {
                            Event.Post(s, MonitEventType.Event_Exec, MonitStateType.State_Failed, s.action_EXEC,
                                "failed to start");
                        }
                    }
                }
            }
            else
            {
                Logger.Log.DebugFormat("'{0}' start skipped -- method not defined", s.name);
            }
            Util.MonitorSet(s);
        }


        /*
         * This function simply stops the service p.
         * @param s A Service_T object
         * @param flag true if the monitoring should be disabled or false if monitoring should continue (when stop is part of restart)
         * @return true if the service was stopped otherwise false
         */

        private static bool _doStop(Service_T s, bool flag)
        {
            var rv = true;
            if (s.depend_visited)
                return rv;
            s.depend_visited = true;
            if (s.stop != null)
            {
                if (s.type != MonitServiceType.Service_Process || Util.IsProcessRunning(s, false) != 0)
                {
                    Logger.Log.InfoFormat("'{0}' stop", s.name);
                    var pid = Util.IsProcessRunning(s, true);
                    if (s.type == MonitServiceType.Service_Process)
                    {
                        if (s.stop())
                        {
                            Event.Post(s, MonitEventType.Event_Exec, MonitStateType.State_Succeeded, s.action_EXEC,
                                "stopped");
                        }
                        else
                        {
                            rv = false;
                            Event.Post(s, MonitEventType.Event_Exec, MonitStateType.State_Failed, s.action_EXEC,
                                "failed to stop");
                        }
                    }
                }
            }
            else
            {
                Logger.Log.DebugFormat("'{0}' stop skipped -- method not defined", s.name);
            }
            if (flag)
                Util.MonitorUnset(s);
            else
                Util.ResetInfo(s);

            return rv;
        }


        /*
         * This function simply restarts the service s.
         * @param s A Service_T object
         */

        private static void _doRestart(Service_T s)
        {
            if (s.restart != null)
            {
                Logger.Log.InfoFormat("'{0}' restart", s.name);
                Util.ResetInfo(s);
                if (s.type == MonitServiceType.Service_Process)
                {
                    if (s.restart())
                    {
                        Event.Post(s, MonitEventType.Event_Exec, MonitStateType.State_Succeeded, s.action_EXEC,
                            "restarted");
                    }
                    else
                    {
                        Event.Post(s, MonitEventType.Event_Exec, MonitStateType.State_Failed, s.action_EXEC,
                            "failed to restart");
                    }
                }
            }
            else
            {
                Logger.Log.DebugFormat("'{0}' restart skipped -- method not defined", s.name);
            }
            Util.MonitorSet(s);
        }

        /*
         * This is a post- fix recursive function for enabling monitoring every service
         * that s depends on before monitor s.
         * @param s A Service_T object
         * @param flag A Custom flag
         */

        private static void _doMonitor(Service_T s, bool flag)
        {
            if (s.visited)
                return;
            s.visited = true;
            if (s.dependantlist != null)
            {
                foreach (var d in s.dependantlist)
                {
                    var parent = Util.GetService(d);
                    _doMonitor(parent, flag);
                }
            }
            Util.MonitorSet(s);
        }


        /*
         * This is a function for disabling monitoring
         * @param s A Service_T object
         * @param flag A Custom flag
         */

        private static void _doUnmonitor(Service_T s, bool flag)
        {
            if (s.depend_visited)
                return;
            s.depend_visited = true;
            Util.MonitorUnset(s);
        }


        /*
         * This is an in-fix recursive function called before s is started to
         * stop every service that depends on s, in reverse order *or* after s
         * was started to start again every service that depends on s. The
         * action parametere controls if this function should start or stop
         * the procceses that depends on s.
         * @param s A Service_T object
         * @param action An action to do on the dependant services
         * @param flag A Custom flag
         */

        private static void _doDepend(Service_T s, MonitActionType action, bool flag)
        {
            foreach (var child in MonitWindowsAgent.servicelist)
            {
                if (child.dependantlist != null)
                {
                    foreach (var d in child.dependantlist)
                    {
                        if (d == s.name)
                        {
                            if (action == MonitActionType.Action_Start)
                                _doStart(child);
                            else if (action == MonitActionType.Action_Monitor)
                                _doMonitor(child, flag);
                            _doDepend(child, action, flag);
                            if (action == MonitActionType.Action_Stop)
                                _doStop(child, flag);
                            else if (action == MonitActionType.Action_Unmonitor)
                                _doUnmonitor(child, flag);
                            break;
                        }
                    }
                }
            }
        }

        #endregion
    }
}