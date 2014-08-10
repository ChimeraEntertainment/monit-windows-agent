
namespace ChMonitoring.MonitData
{
    /*
     * @see https://mmonit.com/monit/documentation/
     * Monit supports three monitoring modes per service: active, passive and manual.
     * See also the example section below for usage of the mode statement.
     *
     * In active mode, Monit will pro-actively monitor a service and in case of problems 
     * Monit will raise alerts and/or restart the service. Active mode is the default mode.
     *
     * In passive mode, Monit will passively monitor a service and will raise alerts, but 
     * will not try to fix a problem.
     *
     * In manual mode, Monit will enter active mode only if a service was started via Monit:
     *
     * monit start <servicename>
     * Use "monit stop <servicename>" to stop the service and take it out of Monit control. 
     * The manual mode can be used to build simple cluster with active/passive HA-services.
     *
     * A service's monitoring state is persistent across Monit restart.
     *
     * If you use Monit in a HA-cluster you should place the state file in a temporary 
     * filesystem so if the machine which runs HA-services should crash and the stand-by machine 
     * take over its services, the HA-services won't be started after the crashed node will boot again:
     * 
     * set statefile /tmp/monit.state
     * */
    public enum MonitMonitorModeType
    {
        MODE_ACTIVE = 0, // active is default
        MODE_PASSIVE = 1,
        MODE_MANUAL = 2
    }
}
