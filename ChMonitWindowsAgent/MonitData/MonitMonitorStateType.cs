namespace ChMonitoring.MonitData
{
    /// <summary>
    /// From monit.h
    /// </summary>
    public enum MonitMonitorStateType : byte
    {
        MONITOR_NOT        = 0x0,
        MONITOR_YES        = 0x1,
        MONITOR_INIT       = 0x2,
        MONITOR_WAITING    = 0x4,
    }
}
