using System.Collections.Generic;

namespace ChMonitoring.MonitData
{
    internal class MonitEventTable
    {
        public static List<EventTable_T> Event_Table = new List<EventTable_T>
        {
            new EventTable_T(MonitEventType.Event_Action, "Action done", "Action done", "Action done", "Action done"),
            new EventTable_T(MonitEventType.Event_Checksum, "Checksum failed", "Checksum succeeded", "Checksum changed",
                "Checksum not changed"),
            new EventTable_T(MonitEventType.Event_Connection, "Connection failed", "Connection succeeded",
                "Connection changed", "Connection not changed"),
            new EventTable_T(MonitEventType.Event_Content, "Content failed", "Content succeeded", "Content match",
                "Content doesn't match"),
            new EventTable_T(MonitEventType.Event_Data, "Data access error", "Data access succeeded",
                "Data access changed", "Data access not changed"),
            new EventTable_T(MonitEventType.Event_Exec, "Execution failed", "Execution succeeded", "Execution changed",
                "Execution not changed"),
            new EventTable_T(MonitEventType.Event_Fsflag, "Filesystem flags failed", "Filesystem flags succeeded",
                "Filesystem flags changed", "Filesystem flags not changed"),
            new EventTable_T(MonitEventType.Event_Gid, "GID failed", "GID succeeded", "GID changed", "GID not changed"),
            new EventTable_T(MonitEventType.Event_Heartbeat, "Heartbeat failed", "Heartbeat succeeded",
                "Heartbeat changed", "Heartbeat not changed"),
            new EventTable_T(MonitEventType.Event_Icmp, "ICMP failed", "ICMP succeeded", "ICMP changed",
                "ICMP not changed"),
            new EventTable_T(MonitEventType.Event_Instance, "Monit instance failed", "Monit instance succeeded",
                "Monit instance changed", "Monit instance not changed"),
            new EventTable_T(MonitEventType.Event_Invalid, "Invalid type", "Type succeeded", "Type changed",
                "Type not changed"),
            new EventTable_T(MonitEventType.Event_Nonexist, "Does not exist", "Exists", "Existence changed",
                "Existence not changed"),
            new EventTable_T(MonitEventType.Event_Permission, "Permission failed", "Permission succeeded",
                "Permission changed", "Permission not changed"),
            new EventTable_T(MonitEventType.Event_Pid, "PID failed", "PID succeeded", "PID changed", "PID not changed"),
            new EventTable_T(MonitEventType.Event_PPid, "PPID failed", "PPID succeeded", "PPID changed",
                "PPID not changed"),
            new EventTable_T(MonitEventType.Event_Resource, "Resource limit matched", "Resource limit succeeded",
                "Resource limit changed", "Resource limit not changed"),
            new EventTable_T(MonitEventType.Event_Size, "Size failed", "Size succeeded", "Size changed",
                "Size not changed"),
            new EventTable_T(MonitEventType.Event_Status, "Status failed", "Status succeeded", "Status changed",
                "Status not changed"),
            new EventTable_T(MonitEventType.Event_Timeout, "Timeout", "Timeout recovery", "Timeout changed",
                "Timeout not changed"),
            new EventTable_T(MonitEventType.Event_Timestamp, "Timestamp failed", "Timestamp succeeded",
                "Timestamp changed", "Timestamp not changed"),
            new EventTable_T(MonitEventType.Event_Uid, "UID failed", "UID succeeded", "UID changed", "UID not changed"),
            new EventTable_T(MonitEventType.Event_Uptime, "Uptime failed", "Uptime succeeded", "Uptime changed",
                "Uptime not changed"),
            /* Virtual events */
            new EventTable_T(MonitEventType.Event_Null, "No Event", "No Event", "No Event", "No Event")
        };

        public struct EventTable_T
        {
            public string description_changed;
            public string description_changednot;
            public string description_failed;
            public string description_succeeded;
            public MonitEventType id;

            public EventTable_T(MonitEventType _id, string _description_failed, string _description_succeeded,
                string _description_changed, string _description_changednot)
            {
                id = _id;
                description_failed = _description_failed;
                description_succeeded = _description_succeeded;
                description_changed = _description_changed;
                description_changednot = _description_changednot;
            }
        }
    }
}