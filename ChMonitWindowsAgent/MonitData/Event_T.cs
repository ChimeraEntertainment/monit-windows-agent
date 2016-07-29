using System;

namespace ChMonitoring.MonitData
{
    /// <summary>
    /// Events
    /// </summary>
    public class Event_T
    {
        public const int EVENT_VERSION = 4; /**< The event structure version */
        public EventAction_T action; /**< Description of the event action */
        public DateTime collected; /**< When the event occured */
        public uint count; /**< The event rate */
        public MonitHandlerType flag; /**< The handlers state flag */
        public MonitEventType id; /**< The event identification */
        public string message; /**< Optional message describing the event */
        public MonitMonitorModeType mode; /**< Monitoring mode for the service */
        public string source; /**< Event source service name */
        public MonitStateType state; /**< Test state */
        public bool state_changed; /**< true if state changed */
        public long state_map; /**< Event bitmap for last cycles */
        public MonitServiceType type; /**< Monitored service type */
    }
}
