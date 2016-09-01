using System;

namespace ChMonitoring.MonitData
{
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
        public float inode_percent; /**< Used inode percentage */
        public long inode_total; /**< Used inode total objects */
        public ushort mode;
        public float space_percent; /**< Used space percentage */
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
        public float cpu_percent; /**< percentage */
        public int euid; /**< Effective Process UID */
        public int gid; /**< Process GID */
        public ulong mem_kbyte;
        public float mem_percent; /**< percentage */
        public int pid; /**< Process PID from actual cycle */
        public int ppid; /**< Process parent PID from actual cycle */
        public float total_cpu_percent; /**< percentage */
        public ulong total_mem_kbyte;
        public float total_mem_percent; /**< percentage */
        public int uid; /**< Process UID */
        public TimeSpan uptime;
        public bool zombie;
    }

    public class NetInfo_T : Info_T
    {
    }
}
