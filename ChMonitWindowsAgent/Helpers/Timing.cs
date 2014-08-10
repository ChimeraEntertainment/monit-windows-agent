using System;

namespace ChMonitoring.Helpers
{
    class TimingFraction
    {
        public double Timestamp { get; set; }
        public uint Usec { get; set; }
    }
    
    internal class Timing
    {

        public static double GetTimestamp(DateTime dt)
        {
            return Math.Ceiling((dt.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }

        public static TimingFraction GetTimingFraction()
        {
            var tf = new TimingFraction();
            var snapshotTime = DateTime.UtcNow;

            tf.Timestamp = GetTimestamp(snapshotTime);
            tf.Usec = UInt32.Parse(snapshotTime.ToString("ffffff"));
            return tf;
        }
    }
}
