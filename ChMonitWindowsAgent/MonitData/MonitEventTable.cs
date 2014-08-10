
namespace ChMonitoring.MonitLogic
{
    public enum MonitEventType
    {
        Event_Null       = 0x0,
        Event_Checksum   = 0x1,
        Event_Resource   = 0x2,
        Event_Timeout    = 0x4,
        Event_Timestamp  = 0x8,
        Event_Size       = 0x10,
        Event_Connection = 0x20,
        Event_Permission = 0x40,
        Event_Uid        = 0x80,
        Event_Gid        = 0x100,
        Event_Nonexist   = 0x200,
        Event_Invalid    = 0x400,
        Event_Data       = 0x800,
        Event_Exec       = 0x1000,
        Event_Fsflag     = 0x2000,
        Event_Icmp       = 0x4000,
        Event_Content    = 0x8000,
        Event_Instance   = 0x10000,
        Event_Action     = 0x20000,
        Event_Pid        = 0x40000,
        Event_PPid       = 0x80000,
        Event_Heartbeat  = 0x100000,
        Event_Status     = 0x200000,
        Event_Uptime     = 0x400000,
        Event_All        = 0x7FFFFFFF
    }

    class MonitEventTable
    {
        /* TODO
         * 
        EventTable_T Event_Table[] = {
          {Event_Action,     "Action done",             "Action done",                "Action done",              "Action done"},
          {Event_Checksum,   "Checksum failed",         "Checksum succeeded",         "Checksum changed",         "Checksum not changed"},
          {Event_Connection, "Connection failed",       "Connection succeeded",       "Connection changed",       "Connection not changed"},
          {Event_Content,    "Content failed",          "Content succeeded",          "Content match",            "Content doesn't match"},
          {Event_Data,       "Data access error",       "Data access succeeded",      "Data access changed",      "Data access not changed"},
          {Event_Exec,       "Execution failed",        "Execution succeeded",        "Execution changed",        "Execution not changed"},
          {Event_Fsflag,     "Filesystem flags failed", "Filesystem flags succeeded", "Filesystem flags changed", "Filesystem flags not changed"},
          {Event_Gid,        "GID failed",              "GID succeeded",              "GID changed",              "GID not changed"},
          {Event_Heartbeat,  "Heartbeat failed",        "Heartbeat succeeded",        "Heartbeat changed",        "Heartbeat not changed"},
          {Event_Icmp,       "ICMP failed",             "ICMP succeeded",             "ICMP changed",             "ICMP not changed"},
          {Event_Instance,   "Monit instance failed",   "Monit instance succeeded",   "Monit instance changed",   "Monit instance not changed"},
          {Event_Invalid,    "Invalid type",            "Type succeeded",             "Type changed",             "Type not changed"},
          {Event_Nonexist,   "Does not exist",          "Exists",                     "Existence changed",        "Existence not changed"},
          {Event_Permission, "Permission failed",       "Permission succeeded",       "Permission changed",       "Permission not changed"},
          {Event_Pid,        "PID failed",              "PID succeeded",              "PID changed",              "PID not changed"},
          {Event_PPid,       "PPID failed",             "PPID succeeded",             "PPID changed",             "PPID not changed"},
          {Event_Resource,   "Resource limit matched",  "Resource limit succeeded",   "Resource limit changed",   "Resource limit not changed"},
          {Event_Size,       "Size failed",             "Size succeeded",             "Size changed",             "Size not changed"},
          {Event_Status,     "Status failed",           "Status succeeded",           "Status changed",           "Status not changed"},
          {Event_Timeout,    "Timeout",                 "Timeout recovery",           "Timeout changed",          "Timeout not changed"},
          {Event_Timestamp,  "Timestamp failed",        "Timestamp succeeded",        "Timestamp changed",        "Timestamp not changed"},
          {Event_Uid,        "UID failed",              "UID succeeded",              "UID changed",              "UID not changed"},
          {Event_Uptime,     "Uptime failed",           "Uptime succeeded",           "Uptime changed",           "Uptime not changed"},
          // Virtual events 
          {Event_Null,       "No Event",                "No Event",                   "No Event",                 "No Event"}
        };
         * */
    }
}
