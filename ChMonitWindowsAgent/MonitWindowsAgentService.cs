using System;
using System.ServiceProcess;

namespace ChMonitoring
{
    public sealed partial class MonitWindowsAgentService : ServiceBase
    {
        #region Fields

        private MonitWindowsAgent m_watcher;

        #endregion

        #region Public Methods

        public MonitWindowsAgentService()
        {
            // init the service
            ServiceName = "MonitWindowsAgent";

            CanHandlePowerEvent = true;
            CanHandleSessionChangeEvent = true;
            CanPauseAndContinue = true;
            CanShutdown = true;
            CanStop = true;

            // and create the watcher 
            m_watcher = new MonitWindowsAgent();

            // to delete
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            InitializeComponent();
        }

        public void Start()
        {
            OnStart(null);
        }

        #endregion

        #region Events

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
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
            // stop the watcher
            m_watcher.Stop();
        }

        protected override void OnPause()
        {
            base.OnPause();
            //pause the watcher
            m_watcher.Pause();
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            // shut the watcher down
            m_watcher.Shutdown();
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return base.OnPowerEvent(powerStatus);
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
        }

        #endregion
    }
}
