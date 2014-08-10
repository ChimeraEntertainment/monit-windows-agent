using System;
using System.Diagnostics;
using System.ServiceProcess;


namespace ChMonitoring
{
    public sealed partial class MonitWindowsAgentService : ServiceBase
    {
        private MonitWindowsAgent m_watcher;

        private EventLog m_eventLog;
        public EventLog CustomEventLog
        {
            get { return m_eventLog ?? EventLog; }
            set { m_eventLog = value; }
        }

        public MonitWindowsAgentService()
        {

        }

        public MonitWindowsAgentService Init()
        {
            // init the service
            ServiceName = "MonitWindowsAgent";
            CustomEventLog.Log = "Application";

            CanHandlePowerEvent = true;
            CanHandleSessionChangeEvent = true;
            CanPauseAndContinue = true;
            CanShutdown = true;
            CanStop = true;

            // and create the watcher 
            m_watcher = new MonitWindowsAgent(CustomEventLog);

            // to delete
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            InitializeComponent();
            
            return this;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
        }

        public void Start()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            // start the watcher
           m_watcher.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return base.OnPowerEvent(powerStatus);
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
        }

    }
}
