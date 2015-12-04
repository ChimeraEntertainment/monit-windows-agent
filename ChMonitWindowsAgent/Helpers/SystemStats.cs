﻿using System;
using System.Management;

namespace ChMonitoring.Helpers {
    internal class SystemStats {

        internal static string GetHostname() {
            return Environment.MachineName.ToLower();
        }

        internal static string GetVersion() {
            return Environment.OSVersion.VersionString;
        }

        internal static long GetWorkingSet() {
            return Environment.WorkingSet;
        }

        internal static uint GetMemorySizeKB() {
            double totalCapacity = 0;
            foreach(var val in new ManagementObjectSearcher("select * from Win32_PhysicalMemory").Get()) {
                var obj = val.GetPropertyValue("Capacity");
                totalCapacity += Convert.ToDouble(obj);
            }

            return Convert.ToUInt32(totalCapacity / 1024);
        }

        internal static byte GetCPUCoreCount() {
            byte coreCount = 0;
            foreach(var item in new ManagementObjectSearcher("Select * from Win32_Processor").Get()) {
                coreCount += byte.Parse(item["NumberOfCores"].ToString());
            }
            return coreCount;
        }

        internal static short GetCPULoadPercentage() {
            byte coreCount = 0;
            foreach(var item in new ManagementObjectSearcher("Select * from Win32_Processor").Get()) {
                var lp = item["LoadPercentage"];
                if(lp == null)
                    continue;
                coreCount += byte.Parse(item["LoadPercentage"].ToString());
            }
            return coreCount;

            //private PerformanceCounter theCPUCounter = 
            // private PerformanceCounter theMemCounter = new PerformanceCounter("Memory", "Available MBytes");

            /*
            systemService.system = new monitServiceSystem();
            systemService.system.cpu = new monitServiceSystemCpu
            {
                user = Convert.ToDecimal(new PerformanceCounter("Processor", "% Processor Time", "_Total").NextValue())
            };
            systemService.system.memory = new monitServiceSystemMemory
            {
                kilobyte = Convert.ToUInt32(new PerformanceCounter("Memory", "Available KBytes").NextValue())
            };
            */
        }

        internal static DateTime ProcessRunningInSec() {
            var current = System.Diagnostics.Process.GetCurrentProcess();
            return current.StartTime;
        }
    }
}
