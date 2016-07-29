using Common.Logging;

namespace ChMonitoring.Helpers
{
    class Logger
    {
        private static ILog _log = null;
        internal static ILog Log {
            get {
                if(_log == null)
                    _log = LogManager.GetLogger("MonitWindowsAgent");
                return _log;
            }
        }
    }
}
