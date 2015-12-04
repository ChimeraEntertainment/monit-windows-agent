using System.Collections.Generic;

namespace ChMonitoring.MonitData
{
    public enum URLCommands
    {
        HOME,
        TEST,
        ABOUT,
        PING,
        GETID,
        STATUS,
        STATUS2,
        RUN,
        VIEWLOG,
        DOACTION,
        FAVICON,
        NONE
    }

    internal class MonitURLCommands
    {
        private static readonly Dictionary<string, URLCommands> URLCommandStrings = new Dictionary<string, URLCommands>
        {
            {"_monit", URLCommands.TEST},
            {"_about", URLCommands.ABOUT},
            {"_ping", URLCommands.PING},
            {"_getid", URLCommands.GETID},
            {"_status", URLCommands.STATUS},
            {"_status2", URLCommands.STATUS2},
            {"_runtime", URLCommands.RUN},
            {"_viewlog", URLCommands.VIEWLOG},
            {"_doaction", URLCommands.DOACTION},
            {"favicon.ico", URLCommands.FAVICON}
        };

        public static URLCommands GetURLCommand(string requestString)
        {
            if (string.IsNullOrEmpty(requestString))
                return URLCommands.NONE;

            var trimmed = requestString.TrimStart('/');

            if (string.IsNullOrEmpty(trimmed))
                return URLCommands.HOME;

            foreach (var command in URLCommandStrings)
            {
                if (trimmed.StartsWith(command.Key))
                    return command.Value;
            }

            return URLCommands.NONE;
        }
    }
}