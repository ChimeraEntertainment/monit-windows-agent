using ChMonitoring.Configuration;
using ChMonitoring.Http;
using ChMonitoring.MonitData;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ChMonitoring
{
    public sealed partial class MonitWindowsAgentService : ServiceBase
    {
        #region Fields

        private Timer m_timer;
        private bool heartbeatRunning;
        private Thread m_updateThread;
        private bool m_monitAgentRunning = false;

        #endregion

        #region Public Methods

        public MonitWindowsAgentService()
        {
            m_timer = new Timer(ConfigMgr.Config.Period);
            m_timer.Elapsed += DoPeriodicCheck;
            m_timer.AutoReset = true;

            // to delete
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            InitializeComponent();
        }

        #endregion

        #region Events

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ChMonitoring.Helpers.Logger.Log.Fatal(e.ExceptionObject);
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            string startType = "default";

            // start the watcher
            if (args != null && args.Length > 0)
                startType = args[0];

            ChMonitoring.Helpers.Logger.Log.Debug("Starting Service");
            ChMonitoring.Helpers.Logger.Log.DebugFormat("StartType: {0}", startType);
            ChMonitoring.Helpers.Logger.Log.DebugFormat("MonitWindowsAgent Run: {0}", MonitWindowsAgent.Run == null);
            Server.Start();

            if (startType == "default")
            {
                Event.Post(MonitWindowsAgent.Run.system[0], MonitEventType.Event_Instance, MonitStateType.State_Changed,
                    MonitWindowsAgent.Run.system[0].action_MONIT_START, "Monit started");
            }
            else if (startType == "reloaded")
            {
                Event.Post(MonitWindowsAgent.Run.system[0], MonitEventType.Event_Instance, MonitStateType.State_Changed,
                    MonitWindowsAgent.Run.system[0].action_MONIT_START, "Monit reloaded");
            }
            else
            {
                ChMonitoring.Helpers.Logger.Log.Warn("Monit startType not recognized");
            }

            // start the timer
            m_timer.Start();

            DoPeriodicCheck(m_timer, null);

            m_monitAgentRunning = true;
            m_updateThread = new Thread(Heartbeat);
            m_updateThread.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();

            // stop the watcher
            m_monitAgentRunning = false;

            Server.Stop();

            Event.Post(MonitWindowsAgent.Run.system[0], MonitEventType.Event_Instance, MonitStateType.State_Changed,
                MonitWindowsAgent.Run.system[0].action_MONIT_STOP, "Monit stopped");
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            OnStop();
        }

        #endregion

        private void DoPeriodicCheck(object sender, ElapsedEventArgs e)
        {
            heartbeatRunning = true;
            MMonit.HandleMmonit(null);
            heartbeatRunning = false;
        }

        private void Heartbeat()
        {
            while (m_monitAgentRunning)
            {
                if (!heartbeatRunning)
                    Validate.validate();

                if (!MonitWindowsAgent.Run.doaction)
                    Thread.Sleep(MonitWindowsAgent.Run.polltime);

                //if(Run.stopped)
                //    do_exit();
                //else if(Run.doreload)
                //    do_reinit();
            }
        }
    }
}
