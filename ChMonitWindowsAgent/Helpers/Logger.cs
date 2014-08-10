using System;
using Common.Logging;

namespace ChMonitoring.Helpers
{
    class Logger
    {
        internal static ILog Log { get; private set; }

        internal static void Init()
        {
            Log = LogManager.GetCurrentClassLogger();
        }

    }
}
