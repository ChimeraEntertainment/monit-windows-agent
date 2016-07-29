
namespace ChMonitoring.MonitData
{
    /** Defines event's up and down actions */

    public class EventAction_T
    {
        public Action_T failed; /**< Action in the case of failure down */
        public Action_T succeeded; /**< Action in the case of failure up */
    }
}
