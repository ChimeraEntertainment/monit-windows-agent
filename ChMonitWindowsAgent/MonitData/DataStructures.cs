using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace ChMonitoring.MonitData
{
    /** Defines data for application runtime */

    public class Run_T
    {
        //FIXME: move files to sub-struct
        public string controlfile; /**< The file to read configuration from */
        //FIXME: create enum for Run flags and replace set of various boolean_t single-purpose flags with common flags where possible
        public bool debug; /**< Debug level */
        public bool doaction; /**< true if some service(s) has action pending */
        public bool dolog; /**< true if program should log actions, otherwise false */
        public bool dommonitcredentials; /**< true if M/Monit should receive credentials */
        public bool doprocess; /**< true if process status engine is used */
        public /*volatile*/ bool doreload; /**< true if a monit daemon should reinitialize */
        public /*volatile*/ bool dowakeup; /**< true if a monit daemon was wake up by signal */
        public myenvironment Env;
        public List<Event_T> eventlist; /** A list holding partialy handled events */
        public string eventlist_dir; /**< The event queue base directory */
        public int eventlist_slots; /**< The event queue size - number of slots */
        public int expectbuffer; /**< Generic protocol expect buffer - STRLEN by default */
        public int facility; /** The facility to use when running openlog() */
        public bool fipsEnabled; /** true if monit should use FIPS-140 mode */
        public MonitHandlerType handler_flag; /**< The handlers state flag */
        public bool handler_init; /**< The handlers queue initialization */
        public int[] handler_queue = new int[(int) MonitHandlerType.Handler_Max + 1]; /**< The handlers queue counter */
        /** An object holding Monit HTTP interface setup */
        public Httpd_T httpd;
        public string id; /**< Unique monit id */
        public string idfile; /**< The file with unique monit id */
        public DateTime incarnation; /**< Unique ID for running monit instance */
        public bool init; /**< true - don't background to run from init */
        public bool isdaemon; /**< true if program should run as a daemon */
        public string logfile; /**< The file to write logdata into */
        public string mail_hostname; /**< Used in HELO/EHLO/MessageID when sending mail */
        public myformat MailFormat;
        public List<Mail_T> maillist; /**< Global alert notification mailinglist */
        public int mailserver_timeout; /**< Connect and read timeout ms for a SMTP server */
        public List<MailServer_T> mailservers; /**< List of MTAs used for alert notification */
        public List<Mmonit_T> mmonits; /**< Event notification and status receivers list */
        public string mygroup; /**< Group Name of the Service */
        public bool once; /**< true - run only once */
        public string pidfile; /**< This programs pidfile */
        public int polltime; /**< In deamon mode, the sleeptime (sec) between run */
        public int startdelay; /**< the sleeptime (sec) after startup */
        public string statefile; /**< The file with the saved runtime state */
        public /*volatile*/ bool stopped; /**< true if monit was stopped. Flag used by threads */
        public List<Service_T> system; /**< The general system service */
        public bool use_syslog; /**< If true write log to syslog */
    }

    /** User selected standard mail format */

    public class myformat
    {
        public string from; /**< The standard mail from address */
        public string message; /**< The standard mail message */
        public string replyto; /**< Optional reply-to header */
        public string subject; /**< The standard mail subject */
    }

    /** An object holding program relevant "environment" data, see: env.c */

    public class myenvironment
    {
        public string cwd; /**< Current working directory */
        public string home; /**< Users home directory */
        public string user; /**< The the effective user running this program */
    }

    public class Httpd_T
    {
        public string address;
        public List<Auth_T> credentials;
        //public Httpd_Flags flags;
        public int port;
        public bool ssl;
    }

    public class Auth_T
    {
        public string groupname; /**< PAM group name */
        //public Digest_Type digesttype;                /**< How did we store the password */
        public bool is_readonly; /**< true if this is a read-only authenticated user*/
        public string passwd; /**< The users password data */
        public string uname; /**< User allowed to connect to monit httpd */
    }

    /** Defines service data */
    //FIXME: use union for type-specific rules
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
        public Action restart; /**< The restart command for the service */
        public Action start; /**< The start command for the service */
        public Action stop; /**< The stop command for the service */
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

    /** Events */

    public class Event_T
    {
        public const int EVENT_VERSION = 4; /**< The event structure version */
        public EventAction_T action; /**< Description of the event action */
        public DateTime collected; /**< When the event occured */
        public uint count; /**< The event rate */
        public MonitHandlerType flag; /**< The handlers state flag */
        public MonitEventType id; /**< The event identification */
        public string message; /**< Optional message describing the event */
        public MonitMonitorModeType mode; /**< Monitoring mode for the service */
        public string source; /**< Event source service name */
        public MonitStateType state; /**< Test state */
        public bool state_changed; /**< true if state changed */
        public long state_map; /**< Event bitmap for last cycles */
        public MonitServiceType type; /**< Monitored service type */
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

    /** Defines an event action object */

    public class Action_T
    {
        public int count; /**< Event count needed to trigger the action */
        public int cycles; /**< Cycles during which count limit can be reached */
        public Action exec; /**< Optional command to be executed */
        public MonitActionType id; /**< Action to be done */
    }

    /** Defines event's up and down actions */

    public class EventAction_T
    {
        public Action_T failed; /**< Action in the case of failure down */
        public Action_T succeeded; /**< Action in the case of failure up */
    }

    /** Defines when to run a check for a service. This type suports both the old cycle based every statement and the new cron-format version */

    public abstract class Every_T
    {
        public DateTime last_run;
        public MonitEveryType type; /**< 0 = not set, 1 = cycle, 2 = cron, 3 = negated cron */
    }

    public class CycleEvery_T : Every_T
    {
        public int counter; /**< Counter for number. When counter == number, check */
        public int number; /**< Check this program at a given cycles */
    }

    public class CronEvery_T : Every_T
    {
        public string cron; /* A crontab format string */
    }

    public interface Info_T
    {
    }

    public class FileSystemInfo_T : Info_T
    {
        public int _flags; /**< Filesystem flags from last cycle */
        public long f_blocks; /**< Total data blocks in filesystem */
        public long f_blocksfree; /**< Free blocks available to non-superuser */
        public long f_blocksfreetotal; /**< Free blocks in filesystem */
        public long f_bsize; /**< Transfer block size */
        public long f_files; /**< Total file nodes in filesystem */
        public long f_filesfree; /**< Free file nodes in filesystem */
        public int flags; /**< Filesystem flags from actual cycle */
        public int gid; /**< Owner's gid */
        public short inode_percent; /**< Used inode percentage * 10 */
        public long inode_total; /**< Used inode total objects */
        public ushort mode;
        public short space_percent; /**< Used space percentage * 10 */
        public long space_total; /**< Used space total blocks */
        public int uid; /**< Owner's uid */
    }

    public class FileInfo_T : Info_T
    {
        public string cs_sum;
        public int gid; /**< Owner's gid */
        public ulong inode; /**< Inode */
        public ulong inode_prev; /**< Previous inode for regex matching */
        public ushort mode; /**< Permission */
        public int readpos; /**< Position for regex matching */
        public int size; /**< Size */
        public DateTime timestamp; /**< Timestamp */
        public int uid; /**< Owner's uid */
    }

    public class DirectoryInfo_T : Info_T
    {
        public int gid;
        public ushort mode; /**< Permission */
        public DateTime timestamp; /**< Timestamp */
        public int uid; /**< Owner's uid */
    }

    public class FiFoInfo_T : Info_T
    {
        public int gid;
        public ushort mode; /**< Permission */
        public DateTime timestamp; /**< Timestamp */
        public int uid; /**< Owner's uid */
    }

    public class ProcessInfo_T : Info_T
    {
        public int _pid; /**< Process PID from last cycle */
        public int _ppid; /**< Process parent PID from last cycle */
        public int children;
        public short cpu_percent; /**< percentage * 10 */
        public int euid; /**< Effective Process UID */
        public int gid; /**< Process GID */
        public long mem_kbyte;
        public short mem_percent; /**< percentage * 10 */
        public int pid; /**< Process PID from actual cycle */
        public int ppid; /**< Process parent PID from actual cycle */
        public short total_cpu_percent; /**< percentage * 10 */
        public long total_mem_kbyte;
        public short total_mem_percent; /**< percentage * 10 */
        public int uid; /**< Process UID */
        public TimeSpan uptime;
        public bool zombie;
    }

    public class NetInfo_T : Info_T
    {
    }

    /** Defines an url object */

    public class URL_T
    {
        public string hostname; /**< URL hostname part */
        public string password; /**< URL password part */
        public string path; /**< URL path     part */
        public int port; /**< URL port     part */
        public string protocol; /**< URL protocol type */
        public string query; /**< URL query    part */
        public string url; /**< Full URL */
        public string user; /**< URL user     part */
    }

    /** Defines an event notification and status receiver object */

    public class Mmonit_T
    {
        //SslOptions_T ssl;                                      /**< SSL definition */
        public int timeout; /**< The timeout to wait for connection or i/o */
        public URL_T url; /**< URL definition */
    }

    /** Defines data for systemwide statistic */

    public class SystemInfo_T
    {
        public DateTime collected; /**< When were data collected */
        public int cpus; /**< Number of CPUs */
        public double[] loadavg = new double[3]; /**< Load average triple */
        public uint mem_kbyte_max; /**< Maximal system real memory */
        public uint swap_kbyte_max; /**< Swap size */
        public short total_cpu_syst_percent; /**< Total CPU in use in kernel space (pct.)*/
        public short total_cpu_user_percent; /**< Total CPU in use in user space (pct.)*/
        public short total_cpu_wait_percent; /**< Total CPU in use in waiting (pct.)*/
        public uint total_mem_kbyte; /**< Total real memory in use in the system */
        public short total_mem_percent; /**< Total real memory in use in the system */
        public uint total_swap_kbyte; /**< Total swap in use in the system */
        public short total_swap_percent; /**< Total swap in use in the system */
        public utsname uname; /**< Platform information provided by uname() */
    }

    public class utsname
    {
        public string machine;
        public string release;
        public string sysname;
        public string version;
    }

    public class ServiceGroupMember_T
    {
        public string name; /**< name of service */
    }

    public class ServiceGroup_T
    {
        public List<ServiceGroupMember_T> members; /**< Service group members */
        public string name; /**< name of service group */
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

    public class ActionRate_T
    {
        public EventAction_T action; /**< Description of the action upon matching rate */
        public int count; /**< Action counter */
        public int cycle; /**< Cycle counter */
    }

    public class Resource_T
    {
        public EventAction_T action; /**< Description of the action upon event occurence */
        public string compOperator; /**< Comparison operator */
        public long limit; /**< Limit of the resource */
        public MonitResourceType resource_id; /**< Which value is checked */
    }

    public class Filesystem_T
    {
        public EventAction_T action; /**< Description of the action upon event occurence */
        public string compOperator; /**< Comparison operator */
        public long limit_absolute; /**< Watermark - blocks */
        public short limit_percent; /**< Watermark - percent */
        public MonitResourceType resource; /**< Whether to check inode or space */
    }

    public class Mail_T
    {
        public uint events; /*< Events for which this mail object should be sent */
        public string from; /**< The mail from address */
        public string message; /**< The mail message */
        public uint reminder; /*< Send error reminder each Xth cycle */
        public string replyto; /**< Optional reply-to address */
        public string subject; /**< The mail subject */
        public string to; /**< Mail address for alert notification */
    }

    public class MailServer_T
    {
        public string host; /**< Server host address, may be a IP or a hostname string */
        public string password; /** < Password for SMTP_AUTH */
        public int port; /**< Server port */
        public string username; /** < Username for SMTP_AUTH */
    }

    public class Uptime_T
    {
        public EventAction_T action; /**< Description of the action upon event occurence */
        public string compOperator; /**< Comparison operator */
        public TimeSpan uptime; /**< Uptime watermark */
    }

    public class Timestamp_T
    {
        public EventAction_T action; /**< Description of the action upon event occurence */
        public string compOperator; /**< Comparison operator */
        public bool test_changes; /**< true if we only should test for changes */
        public TimeSpan time; /**< Timestamp watermark */
        public DateTime timestamp; /**< The original last modified timestamp for this object*/
    }
}