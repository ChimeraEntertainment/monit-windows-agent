using System;

namespace ChMonitoring.MonitData
{
    /** Defines when to run a check for a service. This type suports both the old cycle based every statement and the new cron-format version */

    public abstract class Every_T
    {
        public DateTime last_run;
        public MonitEveryType type; /**< 0 = not set, 1 = cycle, 2 = cron, 3 = negated cron */
    }

    public class CycleEvery_T : Every_T
    {
        public int counter; /**< Counter for number. When counter == number, check */
        public int number; /**< Check this program at a given cycles */
    }

    public class CronEvery_T : Every_T
    {
        public string cron; /* A crontab format string */
    }
}
