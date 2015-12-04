namespace ChMonitoring.MonitData
{
    public enum MonitActionType
    {
        Action_Ignored = 0,
        Action_Alert,
        Action_Restart,
        Action_Stop,
        Action_Exec,
        Action_Unmonitor,
        Action_Start,
        Action_Monitor
    }

    public enum MonitEveryType
    {
        Every_Cycle = 0,
        Every_SkipCycles,
        Every_Cron,
        Every_NotInCron
    }

    public enum MonitLevelType
    {
        Level_Full = 0,
        Level_Summary
    }

    public enum MonitStateType
    {
        State_Succeeded = 0,
        State_Failed,
        State_Changed,
        State_ChangedNot,
        State_Init
    }

    public enum MonitServiceType
    {
        Service_Filesystem = 0,
        Service_Directory,
        Service_File,
        Service_Process,
        Service_Host,
        Service_System,
        Service_Fifo,
        Service_Program,
        Service_Net
    }

    public enum MonitResourceType
    {
        Resource_CpuPercent = 1,
        Resource_MemoryPercent,
        Resource_MemoryKbyte,
        Resource_LoadAverage1m,
        Resource_LoadAverage5m,
        Resource_LoadAverage15m,
        Resource_Children,
        Resource_MemoryKbyteTotal,
        Resource_MemoryPercentTotal,
        Resource_Inode,
        Resource_Space,
        Resource_CpuUser,
        Resource_CpuSystem,
        Resource_CpuWait,
        Resource_CpuPercentTotal,
        Resource_SwapPercent,
        Resource_SwapKbyte
    }

    public enum MonitProtocolType
    {
        Protocol_DEFAULT = 0,
        Protocol_HTTP,
        Protocol_FTP,
        Protocol_SMTP,
        Protocol_POP,
        Protocol_IMAP,
        Protocol_NNTP,
        Protocol_SSH,
        Protocol_DWP,
        Protocol_LDAP2,
        Protocol_LDAP3,
        Protocol_RDATE,
        Protocol_RSYNC,
        Protocol_GENERIC,
        Protocol_APACHESTATUS,
        Protocol_NTP3,
        Protocol_MYSQL,
        Protocol_DNS,
        Protocol_POSTFIXPOLICY,
        Protocol_TNS,
        Protocol_PGSQL,
        Protocol_CLAMAV,
        Protocol_SIP,
        Protocol_LMTP,
        Protocol_GPS,
        Protocol_RADIUS,
        Protocol_MEMCACHE,
        Protocol_WEBSOCKET
    }

    public enum MonitProcessStatusType
    {
        Process_Stopped = 0,
        Process_Started = 1
    }

    public enum MonitMonitorStateType
    {
        Monitor_Not = 0x0,
        Monitor_Yes = 0x1,
        Monitor_Init = 0x2,
        Monitor_Waiting = 0x4
    }

    /*
     * @see https://mmonit.com/monit/documentation/
     * Monit supports three monitoring modes per service: active, passive and manual.
     * See also the example section below for usage of the mode statement.
     *
     * In active mode, Monit will pro-actively monitor a service and in case of problems 
     * Monit will raise alerts and/or restart the service. Active mode is the default mode.
     *
     * In passive mode, Monit will passively monitor a service and will raise alerts, but 
     * will not try to fix a problem.
     *
     * In manual mode, Monit will enter active mode only if a service was started via Monit:
     *
     * monit start <servicename>
     * Use "monit stop <servicename>" to stop the service and take it out of Monit control. 
     * The manual mode can be used to build simple cluster with active/passive HA-services.
     *
     * A service's monitoring state is persistent across Monit restart.
     *
     * If you use Monit in a HA-cluster you should place the state file in a temporary 
     * filesystem so if the machine which runs HA-services should crash and the stand-by machine 
     * take over its services, the HA-services won't be started after the crashed node will boot again:
     * 
     * set statefile /tmp/monit.state
     * */

    public enum MonitMonitorModeType
    {
        Monitor_Active = 0, // active is default
        Monitor_Passive,
        Monitor_Manual
    }

    public enum MonitHandlerType
    {
        Handler_Succeeded = 0x0,
        Handler_Alert = 0x1,
        Handler_Mmonit = 0x2,
        Handler_Max = Handler_Mmonit
    }

    public enum MonitEventType
    {
        Event_Null = 0x0,
        Event_Checksum = 0x1,
        Event_Resource = 0x2,
        Event_Timeout = 0x4,
        Event_Timestamp = 0x8,
        Event_Size = 0x10,
        Event_Connection = 0x20,
        Event_Permission = 0x40,
        Event_Uid = 0x80,
        Event_Gid = 0x100,
        Event_Nonexist = 0x200,
        Event_Invalid = 0x400,
        Event_Data = 0x800,
        Event_Exec = 0x1000,
        Event_Fsflag = 0x2000,
        Event_Icmp = 0x4000,
        Event_Content = 0x8000,
        Event_Instance = 0x10000,
        Event_Action = 0x20000,
        Event_Pid = 0x40000,
        Event_PPid = 0x80000,
        Event_Heartbeat = 0x100000,
        Event_Status = 0x200000,
        Event_Uptime = 0x400000,
        Event_All = 0x7FFFFFFF
    }
}