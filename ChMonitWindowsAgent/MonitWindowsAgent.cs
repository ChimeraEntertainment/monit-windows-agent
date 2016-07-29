using ChMonitoring.MonitData;
using System.Collections.Generic;

namespace ChMonitoring
{
    /**
     * TODOS:
     *  - Cleanup all TODO-Commented Sections
     *  - Add Comments
     *  - Add Logging
     *  - Refactor to pattern
     *  - Implement a monitrc - reader to fill config
     *      - Try to reuse yacc settings? -> p.y ? 
     *          --> http://gppg.codeplex.com/
     *          --> http://gplex.codeplex.com/
     *          --> http://simplescript.codeplex.com/
     *  
     * - Alternatively: Create a completely new config format (xml?) for the windows agent.
     *   - Question: Is Mmonit using the configfile (as it is contained in the controlfile parameter in the status xml?)
     *   
     * - In favor of testability etc, get rid of static methods and instances
     * **/

    static class MonitWindowsAgent
    {
        #region Fields

        public const string SERVER_VERSION = "5.6";
        public const string VERSION = "5.12.2";
        //Global Variables
        //TODO
        public static Run_T Run;
        public static List<Service_T> servicelist;
        public static List<Service_T> servicelist_conf;
        public static List<ServiceGroup_T> servicegrouplist;
        public static SystemInfo_T systeminfo;

        public static string[] actionnames =
        {
            "ignore", "alert", "restart", "stop", "exec", "unmonitor", "start",
            "monitor", ""
        };

        public static string[] modenames = {"active", "passive", "manual"};
        public static string[] checksumnames = {"UNKNOWN", "MD5", "SHA1"};

        public static Dictionary<string, string> operatornames = new Dictionary<string, string>
        {
            {">", "greater than"},
            {"<", "less than"},
            {"=", "equal to"},
            {"!=", "not equal to"},
            {"<>", "changed"}
        };

        public static string[] statusnames =
        {
            "Accessible", "Accessible", "Accessible", "Running",
            "Online with all services", "Running", "Accessible", "Status ok", "UP"
        };

        public static string[] servicetypes =
        {
            "Filesystem", "Directory", "File", "Process", "Remote Host", "System",
            "Fifo", "Program", "Network"
        };

        public static string[] pathnames = {"Path", "Path", "Path", "Pid file", "Path", "", "Path"};

        public static string[] icmpnames =
        {
            "Reply", "", "", "Destination Unreachable", "Source Quench", "Redirect", "",
            "", "Ping", "", "", "Time Exceeded", "Parameter Problem", "Timestamp Request", "Timestamp Reply",
            "Information Request", "Information Reply", "Address Mask Request", "Address Mask Reply"
        };

        public static string[] sslnames = {"auto", "v2", "v3", "tlsv1", "tlsv1.1", "tlsv1.2", "none"};

        #endregion
    }
}