namespace ChMonitoring.MonitLogic
{
    /// <summary>
    /// From monit.h
    /// </summary>
    public enum MonitServiceType : byte
    {
        TYPE_FILESYSTEM = 0,
        TYPE_DIRECTORY = 1,
        TYPE_FILE = 2,
        TYPE_PROCESS = 3,
        TYPE_HOST = 4,
        TYPE_SYSTEM = 5,
        TYPE_FIFO = 6,
        TYPE_PROGRAM = 7
    }
}
