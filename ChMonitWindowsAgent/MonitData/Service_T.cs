using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace ChMonitoring.MonitData
{
    /// <summary>
    /// Defines service data
    /// FIXME: use union for type-specific rules
    /// </summary>
    public class Service_T
    {
        public EventAction_T action_ACTION; /**< Action requested by CLI or GUI */
        //LinkStatus_T linkstatuslist;                 /**< Network link status list */
        //LinkSpeed_T linkspeedlist;                    /**< Network link speed list */
        //LinkSaturation_T linksaturationlist;     /**< Network link saturation list */
        //Bandwidth_T uploadbyteslist;                  /**< Upload bytes check list */
        //Bandwidth_T uploadpacketslist;              /**< Upload packets check list */
        //Bandwidth_T downloadbyteslist;              /**< Download bytes check list */
        //Bandwidth_T downloadpacketslist;          /**< Download packets check list */

        /** General event handlers */
        public EventAction_T action_DATA; /**< Description of the action upon event */
        public EventAction_T action_EXEC; /**< Description of the action upon event */
        public EventAction_T action_INVALID; /**< Description of the action upon event */
        public EventAction_T action_MONIT_RELOAD; /**< Monit instance reload action */
        /** Internal monit events */
        public EventAction_T action_MONIT_START; /**< Monit instance start action */
        public EventAction_T action_MONIT_STOP; /**< Monit instance stop action */
        /** Test rules and event handlers */
        public List<ActionRate_T> actionratelist; /**< ActionRate check list */
        public Func<Service_T, bool> check; /**< Service verification function */
        public DateTime collected; /**< When were data collected */
        public bool depend_visited; /**< Depend visited flag, set if dependencies are used */
        public List<string> dependantlist; /**< Dependant service list */
        public MonitActionType doaction; /**< Action scheduled by http thread */
        /** Runtime parameters */
        public int error; /**< Error flags bitmap */
        public int error_hint; /**< Failed/Changed hint for error bitmap */
        public Uid_T euid; /**< Effective Uid check */
        public List<Event_T> eventlist; /**< Pending events list */
        public Every_T every; /**< Timespec for when to run check of service */
        //Checksum_T checksum;                                  /**< Checksum check */
        public List<Filesystem_T> filesystemlist; /**< Filesystem check list */
        //Status_T statuslist;           /**< Program execution status check list */
        public List<EventAction_T> fsflaglist; /**< Action upon filesystem flags change */
        public Gid_T gid; /**< Gid check */
        public Info_T inf; /**< Service check result */
        public List<Mail_T> maillist; /**< Alert notification mailinglist */
        public MonitMonitorModeType mode; /**< Monitoring mode for the service */
        public MonitMonitorStateType monitor; /**< Monitor state flag */
        /** Common parameters */
        public string name = ""; /**< Service descriptive name */
        public int ncycle; /**< The number of the current cycle */
        public List<EventAction_T> nonexistlist; /**< Action upon test subject existence failure */
        public int nstart; /**< The number of current starts with this service */
        /** Context specific parameters */
        public string path; /**< Path to the filesys, file, directory or process pid file */
        public List<Pid_T> pidlist; /**< Pid check list */
        public List<Pid_T> ppidlist; /**< PPid check list */
        public Program_T program; /**< Program execution check */
        //Icmp_T icmplist;                                 /**< ICMP check list */
        //Perm_T perm;                                    /**< Permission check */
        //Port_T portlist;                            /**< Portnumbers to check */
        //Port_T socketlist;                         /**< Unix sockets to check */
        public List<Resource_T> resourcelist; /**< Resouce check list */
        public Func<bool> restart; /**< The restart command for the service */
        public Func<bool> start; /**< The start command for the service */
        public Func<bool> stop; /**< The stop command for the service */
        //Match_T matchlist;                             /**< Content Match list */
        //Match_T matchignorelist;                /**< Content Match ignore list */
        public List<Timestamp_T> timestamplist; /**< Timestamp check list */
        public string token; /**< Action token */
        public MonitServiceType type; /**< Monitored service type */
        public Uid_T uid; /**< Uid check */
        //Size_T sizelist;                                 /**< Size check list */
        public List<Uptime_T> uptimelist; /**< Uptime check list */
        public bool visited; /**< Service visited flag, set if dependencies are used */
    }

    public class Program_T
    {
        //public Command_T C;          /**< A Command_T object for creating the sub-process */
        public string args; /**< Program arguments */
        public int exitStatus; /**< Sub-process exit status for reporting */
        public StringBuilder output; /**< Last program output */
        public ServiceController P; /**< A Process_T object representing the sub-process */
        public DateTime started; /**< When the sub-process was started */
        public int timeout; /**< Seconds the program may run until it is killed */
    }

    public class Resource_T
    {
        public EventAction_T action; /**< Description of the action upon event occurence */
        public string compOperator; /**< Comparison operator */
        public double limit; /**< Limit of the resource */
        public MonitResourceType resource_id; /**< Which value is checked */
    }

    public class Timestamp_T
    {
        public EventAction_T action; /**< Description of the action upon event occurence */
        public string compOperator; /**< Comparison operator */
        public bool test_changes; /**< true if we only should test for changes */
        public TimeSpan time; /**< Timestamp watermark */
        public DateTime timestamp; /**< The original last modified timestamp for this object*/
    }

    public class Uptime_T
    {
        public EventAction_T action; /**< Description of the action upon event occurence */
        public string compOperator; /**< Comparison operator */
        public TimeSpan uptime; /**< Uptime watermark */
    }

    public class Filesystem_T
    {
        public EventAction_T action; /**< Description of the action upon event occurence */
        public string compOperator; /**< Comparison operator */
        public long limit_absolute; /**< Watermark - blocks */
        public float limit_percent; /**< Watermark - percent */
        public MonitResourceType resource; /**< Whether to check inode or space */
    }

    public class ActionRate_T
    {
        public EventAction_T action; /**< Description of the action upon matching rate */
        public int count; /**< Action counter */
        public int cycle; /**< Cycle counter */
    }

    public class Uid_T
    {
        public EventAction_T action; /**< Description of the action upon event occurence */
        public uint uid; /**< Owner's uid */
    }

    public class Gid_T
    {
        public EventAction_T action; /**< Description of the action upon event occurence */
        public uint gid; /**< Owner's gid */
    }

    public class Pid_T
    {
        public EventAction_T action; /**< Description of the action upon event occurence */
    }
}
