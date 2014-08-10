using System;
using System.Runtime.InteropServices;

namespace ChMonitoring.Helpers
{
    public static class PerformanceInfo
    {
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation,
            [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static uint GetPhysicalAvailableMemoryInKiB()
        {
            var pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToUInt32((pi.PhysicalAvailable.ToInt64()*pi.PageSize.ToInt64()/1024));
            }
            else
            {
                return 0;
            }

        }

        public static uint GetTotalMemoryInKiB()
        {
            var pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToUInt32((pi.PhysicalTotal.ToInt64()*pi.PageSize.ToInt64()/1024));
            }
            else
            {
                return 0;
            }

        }
    }
}
