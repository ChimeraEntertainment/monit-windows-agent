using System;

namespace ChMonitoring.MonitData
{
    /** Defines an event action object */

    public class Action_T
    {
        public int count; /**< Event count needed to trigger the action */
        public int cycles; /**< Cycles during which count limit can be reached */
        public Func<bool> exec; /**< Optional command to be executed */
        public MonitActionType id; /**< Action to be done */
    }
}
