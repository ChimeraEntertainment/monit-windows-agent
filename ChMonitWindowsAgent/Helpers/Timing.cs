using System;

namespace ChMonitoring.Helpers {
    internal class Timing {

        public static double GetTimestamp(DateTime dt) {
            return Math.Ceiling((dt.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }

        public static uint GetUsec(DateTime dt) {
            return UInt32.Parse(dt.ToString("ffffff"));
        }
    }
}