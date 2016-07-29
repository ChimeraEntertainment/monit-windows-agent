using System;

namespace ChMonitoring.MonitData
{
    /// <summary>
    /// Defines data for systemwide statistic
    /// </summary>
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
}
