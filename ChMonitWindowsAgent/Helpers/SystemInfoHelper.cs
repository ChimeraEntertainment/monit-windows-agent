using System;
using ChMonitoring.MonitData;

namespace ChMonitoring.Helpers
{
    internal class SystemInfoHelper
    {
        public static SystemInfo_T GetSystemInfo()
        {
            SystemStats systemStats = new SystemStats();
            var totalMem = PerformanceInfo.GetTotalMemoryInKiB();
            var systeminfo = new SystemInfo_T();

            systeminfo.uname = new utsname();
            systeminfo.uname.sysname = Environment.OSVersion.Platform.ToString();
            systeminfo.uname.machine = CPUAchitecture.IsOS64Bit() ? "x86_64" : "x86";
            systeminfo.uname.release = Environment.OSVersion.Version.ToString();
                // Environment.OSVersion.Version.Build.ToString();
            systeminfo.uname.version = Environment.OSVersion.Version.Build.ToString();

            systeminfo.collected = DateTime.UtcNow;

            systeminfo.cpus = SystemStats.GetCPUCoreCount();
            systeminfo.total_cpu_syst_percent = systemStats.GetCPULoadPercentage();
            systeminfo.mem_kbyte_max = totalMem;
            systeminfo.total_mem_percent = (1 - ((float)PerformanceInfo.GetPhysicalAvailableMemoryInKiB() / (float)totalMem)) * 10;
            systeminfo.total_mem_kbyte = totalMem - PerformanceInfo.GetPhysicalAvailableMemoryInKiB();

            systeminfo.swap_kbyte_max = 0;
            systeminfo.total_swap_kbyte = 0;

            systeminfo.loadavg[0] = 0.07;
            systeminfo.loadavg[1] = 0.03;
            systeminfo.loadavg[2] = 0.08;

            return systeminfo;
        }
    }
}