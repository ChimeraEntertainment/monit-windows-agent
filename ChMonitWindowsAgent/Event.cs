using System;
using System.Collections.Generic;
using System.IO;
using ChMonitoring.Helpers;
using ChMonitoring.MonitData;

namespace ChMonitoring
{
    internal class Event
    {
        #region Public Methods

        public static void Post(Service_T service, MonitEventType id, MonitStateType state, EventAction_T action,
            string s, params object[] args)
        {
            var message = string.Format(s, args);

            Event_T E = null;
            if (service.eventlist != null)
            {
                service.eventlist.ForEach(e =>
                {
                    if (e.action == action && e.id == id)
                    {
                        e.collected = DateTime.UtcNow;
                        e.state_map <<= 1;
                        e.state_map |= ((state == MonitStateType.State_Succeeded ||
                                         state == MonitStateType.State_ChangedNot)
                            ? 0
                            : 1);
                        e.message = message;
                        E = e;
                    }
                });
            }

            if (E == null)
            {
                if (state == MonitStateType.State_Succeeded || state == MonitStateType.State_ChangedNot)
                {
                    Logger.Log.DebugFormat("'{0}' {1}", service.name, message);
                    return;
                }

                E = new Event_T();
                E.id = id;
                E.collected = DateTime.UtcNow;
                E.source = service.name;
                E.mode = service.mode;
                E.type = service.type;
                E.state = MonitStateType.State_Init;
                E.state_map = 1;
                E.action = action;
                E.message = message;
                if (service.eventlist == null)
                    service.eventlist = new List<Event_T>();
                service.eventlist.Insert(0, E);
            }

            E.state_changed = CheckState(E, state);

            if (E.state_changed)
            {
                E.state = state;
                E.count = 1;
            }
            else
                E.count++;

            handleEvent(service, E);
        }

        #endregion

        #region Helper

        public static bool IsEventSet(int value, MonitEventType mask)
        {
            return (value & (int) mask) != 0;
        }

        #endregion

        #region Properties

        public static Service_T GetSource(Event_T E)
        {
            Service_T S = null;

            if ((S = Util.GetService(E.source)) == null)
                Logger.Log.Error(string.Format("Service {0} not found in monit configuration", E.source));

            return S;
        }

        public static bool CheckState(Event_T E, MonitStateType S)
        {
            var count = 0;
            var state = (S == MonitStateType.State_Succeeded || S == MonitStateType.State_ChangedNot)
                ? MonitStateType.State_Succeeded
                : MonitStateType.State_Failed; /* translate to 0/1 class */
            Action_T action;
            Service_T service;
            long flag;

            if ((service = GetSource(E)) == null)
                return true;

            /* Only true failed/changed state condition can change the initial state */
            if (state == MonitStateType.State_Succeeded && E.state == MonitStateType.State_Init &&
                (service.error & (int) E.id) == 0)
                return false;

            action = state == MonitStateType.State_Succeeded ? E.action.succeeded : E.action.failed;

            /* Compare as many bits as cycles able to trigger the action */
            for (var i = 0; i < action.cycles; i++)
            {
                /* Check the state of the particular cycle given by the bit position */
                flag = (E.state_map >> i) & 0x1;

                /* Count occurences of the posted state */
                if (flag == (int) state)
                    count++;
            }

            /* the internal instance and action events are handled as changed any time since we need to deliver alert whenever it occurs */
            if (E.id == MonitEventType.Event_Instance || E.id == MonitEventType.Event_Action ||
                (count >= action.count && (S != E.state || S == MonitStateType.State_Changed)))
            {
                E.state_map = (int) state;
                    // Restart state map on state change, so we'll not flicker on multiple-failures condition (next state change requires full number of cycles to pass)
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Get a textual description of actual event type.
        /// </summary>
        /// <param name="E">An event object</param>
        /// <returns>A string describing the event type in clear text. If the event type is not found NULL is returned.</returns>
        public static string GetDescription(Event_T E)
        {
            var et = MonitEventTable.Event_Table;

            foreach (var ete in et)
            {
                if (E.id == ete.id)
                {
                    switch (E.state)
                    {
                        case MonitStateType.State_Succeeded:
                            return ete.description_succeeded;
                        case MonitStateType.State_Failed:
                            return ete.description_failed;
                        case MonitStateType.State_Init:
                            return ete.description_failed;
                        case MonitStateType.State_Changed:
                            return ete.description_changed;
                        case MonitStateType.State_ChangedNot:
                            return ete.description_changednot;
                        default:
                            break;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Events the get action.
        /// </summary>
        /// <param name="E">The e.</param>
        /// <returns></returns>
        public static MonitActionType GetAction(Event_T E)
        {
            Action_T A = null;

            switch (E.state)
            {
                case MonitStateType.State_Succeeded:
                case MonitStateType.State_ChangedNot:
                    A = E.action.succeeded;
                    break;
                case MonitStateType.State_Failed:
                case MonitStateType.State_Changed:
                case MonitStateType.State_Init:
                    A = E.action.failed;
                    break;
                default:
                    Logger.Log.ErrorFormat("Invalid event state: {0}", E.state);
                    return MonitActionType.Action_Ignored;
            }
            if (A == null)
                return MonitActionType.Action_Ignored;
            /* In the case of passive mode we replace the description of start, stop or restart action for alert action, because these actions are passive in this mode */
            return (E.mode == MonitMonitorModeType.Monitor_Passive &&
                    ((A.id == MonitActionType.Action_Start) || (A.id == MonitActionType.Action_Stop) ||
                     (A.id == MonitActionType.Action_Restart)))
                ? MonitActionType.Action_Alert
                : A.id;
        }

        /// <summary>
        ///     TODO
        ///     Events the queue process.
        /// </summary>
        public static void EventQueueProcess()
        {
            /* return in the case that the eventqueue is not enabled or empty */
            if (string.IsNullOrEmpty(MonitWindowsAgent.Run.eventlist_dir) ||
                (!MonitWindowsAgent.Run.handler_init &&
                 MonitWindowsAgent.Run.handler_queue[(int) MonitHandlerType.Handler_Alert] == 0 &&
                 MonitWindowsAgent.Run.handler_queue[(int) MonitHandlerType.Handler_Mmonit] == 0))
                return;

            //DIR *dir = opendir(Run.eventlist_dir);
            //if (! dir) {
            //        if (errno != ENOENT)
            //                LogError("Cannot open the directory %s -- %s\n", Run.eventlist_dir, STRERROR);
            //        return;
            //}

            //struct dirent *de = readdir(dir);
            //if (de)
            //        DEBUG("Processing postponed events queue\n");

            //Action_T a;
            //EventAction_T ea;

            //while (de) {
            //        int handlers_passed = 0;

            //        /* In the case that all handlers failed, skip the further processing in this cycle. Alert handler is currently defined anytime (either explicitly or localhost by default) */
            //        if ( (Run.mmonits && FLAG(Run.handler_flag, Handler_Mmonit) && FLAG(Run.handler_flag, Handler_Alert)) || FLAG(Run.handler_flag, Handler_Alert))
            //                break;

            //        char file_name[PATH_MAX];
            //        snprintf(file_name, sizeof(file_name), "%s/%s", Run.eventlist_dir, de->d_name);

            //        if (File_isFile(file_name)) {
            //                LogInfo("Processing queued event %s\n", file_name);

            //                FILE *file = fopen(file_name, "r");
            //                if (! file) {
            //                        LogError("Queued event processing failed - cannot open the file %s -- %s\n", file_name, STRERROR);
            //                        goto error1;
            //                }

            //                size_t size;

            //                /* read event structure version */
            //                int *version = file_readQueue(file, &size);
            //                if (! version) {
            //                        LogError("skipping queued event %s - unknown data format\n", file_name);
            //                        goto error2;
            //                }
            //                if (size != sizeof(int)) {
            //                        LogError("Aborting queued event %s - invalid size %lu\n", file_name, (unsigned long)size);
            //                        goto error3;
            //                }
            //                if (*version != EVENT_VERSION) {
            //                        LogError("Aborting queued event %s - incompatible data format version %d\n", file_name, *version);
            //                        goto error3;
            //                }

            //                /* read event structure */
            //                Event_T e = file_readQueue(file, &size);
            //                if (! e)
            //                        goto error3;
            //                if (size != sizeof(*e))
            //                        goto error4;

            //                /* read source */
            //                if (! (e->source = file_readQueue(file, &size)))
            //                        goto error4;

            //                /* read message */
            //                if (! (e->message = file_readQueue(file, &size)))
            //                        goto error5;

            //                /* read event action */
            //                Action_Type *action = file_readQueue(file, &size);
            //                if (! action)
            //                        goto error6;
            //                if (size != sizeof(Action_Type))
            //                        goto error7;
            //                a->id = *action;
            //                switch (e->state) {
            //                        case State_Succeeded:
            //                        case State_ChangedNot:
            //                                ea->succeeded = a;
            //                                break;
            //                        case State_Failed:
            //                        case State_Changed:
            //                        case State_Init:
            //                                ea->failed = a;
            //                                break;
            //                        default:
            //                                LogError("Aborting queue event %s -- invalid state: %d\n", file_name, e->state);
            //                                goto error7;
            //                }
            //                e->action = ea;

            //                /* Retry all remaining handlers */

            //                /* alert */
            //                if (e->flag & Handler_Alert) {
            //                        if (Run.handler_init)
            //                                Run.handler_queue[Handler_Alert]++;
            //                        if ((Run.handler_flag & Handler_Alert) != Handler_Alert) {
            //                                if ( handle_alert(e) != Handler_Alert ) {
            //                                        e->flag &= ~Handler_Alert;
            //                                        Run.handler_queue[Handler_Alert]--;
            //                                        handlers_passed++;
            //                                } else {
            //                                        LogError("Alert handler failed, retry scheduled for next cycle\n");
            //                                        Run.handler_flag |= Handler_Alert;
            //                                }
            //                        }
            //                }

            //                /* mmonit */
            //                if (e->flag & Handler_Mmonit) {
            //                        if (Run.handler_init)
            //                                Run.handler_queue[Handler_Mmonit]++;
            //                        if ((Run.handler_flag & Handler_Mmonit) != Handler_Mmonit) {
            //                                if ( handle_mmonit(e) != Handler_Mmonit ) {
            //                                        e->flag &= ~Handler_Mmonit;
            //                                        Run.handler_queue[Handler_Mmonit]--;
            //                                        handlers_passed++;
            //                                } else {
            //                                        LogError("M/Monit handler failed, retry scheduled for next cycle\n");
            //                                        Run.handler_flag |= Handler_Mmonit;
            //                                }
            //                        }
            //                }

            //                /* If no error persists, remove it from the queue */
            //                if (e->flag == Handler_Succeeded) {
            //                        DEBUG("Removing queued event %s\n", file_name);
            //                        if (unlink(file_name) < 0)
            //                                LogError("Failed to remove queued event file '%s' -- %s\n", file_name, STRERROR);
            //                } else if (handlers_passed > 0) {
            //                        DEBUG("Updating queued event %s (some handlers passed)\n", file_name);
            //                        Event_queue_update(e, file_name);
            //                }

            //        error7:
            //                FREE(action);
            //        error6:
            //                FREE(e->message);
            //        error5:
            //                FREE(e->source);
            //        error4:
            //                FREE(e);
            //        error3:
            //                FREE(version);
            //        error2:
            //                fclose(file);
            //        }
            //error1:
            //        de = readdir(dir);
            //}
            MonitWindowsAgent.Run.handler_init = false;
            //closedir(dir);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Handles the event.
        /// </summary>
        /// <param name="S">The s.</param>
        /// <param name="E">The e.</param>
        private static void handleEvent(Service_T S, Event_T E)
        {
            /* We will handle only first succeeded event, recurrent succeeded events
             * or insufficient succeeded events during failed service state are
             * ignored. Failed events are handled each time. */
            if (!E.state_changed &&
                (E.state == MonitStateType.State_Succeeded || E.state == MonitStateType.State_ChangedNot ||
                 ((E.state_map & 0x1) ^ 0x1) == 1))
            {
                Logger.Log.DebugFormat("'{0}' {1}%s", S.name, E.message);
                return;
            }

            if (!string.IsNullOrEmpty(E.message))
            {
                /* In the case that the service state is initializing yet and error
                 * occured, log it and exit. Succeeded events in init state are not
                 * logged. Instance and action events are logged always with priority
                 * info. */
                if (E.state != MonitStateType.State_Init || (E.state_map & 0x1) == 1)
                {
                    if (E.state == MonitStateType.State_Succeeded || E.state == MonitStateType.State_ChangedNot ||
                        E.id == MonitEventType.Event_Instance || E.id == MonitEventType.Event_Action)
                        Logger.Log.InfoFormat("'{0}' {1}", S.name, E.message);
                    else
                        Logger.Log.ErrorFormat("'{0}' {1}", S.name, E.message);
                }
                if (E.state == MonitStateType.State_Init)
                    return;
            }

            if (E.state == MonitStateType.State_Failed || E.state == MonitStateType.State_Changed)
            {
                if (E.id != MonitEventType.Event_Instance && E.id != MonitEventType.Event_Action)
                {
                    // We are not interested in setting error flag for instance and action events
                    S.error |= (int) E.id;
                    /* The error hint provides second dimension for error bitmap and differentiates between failed/changed event states (failed=0, chaged=1) */
                    if (E.state == MonitStateType.State_Changed)
                        S.error_hint |= (int) E.id;
                    else
                        S.error_hint &= ~(int) E.id;
                }
                handleAction(E, E.action.failed);
            }
            else
            {
                S.error &= ~(int) E.id;
                handleAction(E, E.action.succeeded);
            }

            /* Possible event state change was handled so we will reset the flag. */
            E.state_changed = false;
        }

        /// <summary>
        ///     TODO
        ///     Handles the action.
        /// </summary>
        /// <param name="E">The e.</param>
        /// <param name="A">a.</param>
        private static void handleAction(Event_T E, Action_T A)
        {
            Service_T s;

            E.flag = MonitHandlerType.Handler_Succeeded;

            if (A.id == MonitActionType.Action_Ignored)
                return;

            /* Alert and mmonit event notification are common actions */
            E.flag |= Collector.HandleMmonit(E);
            E.flag |= Alert.HandleAlert(E);

            /* In the case that some subhandler failed, enqueue the event for
             * partial reprocessing */
            if (E.flag != MonitHandlerType.Handler_Succeeded)
            {
                if (!string.IsNullOrEmpty(MonitWindowsAgent.Run.eventlist_dir))
                    eventQueueAdd(E);
                else
                    Logger.Log.Error("Aborting event");
            }

            if ((s = GetSource(E)) == null)
            {
                Logger.Log.Error("Event action handling aborted");
                return;
            }

            /* Action event is handled already. For Instance events
             * we don't want actions like stop to be executed
             * to prevent the disabling of system service monitoring */
            if (A.id == MonitActionType.Action_Alert || E.id == MonitEventType.Event_Instance)
            {
            }
            if (A.id == MonitActionType.Action_Exec)
            {
                Logger.Log.InfoFormat("'{0}'", s.name);
                //spawn(s, A.exec, E);
            }
            else
            {
                if (s.actionratelist != null &&
                    (A.id == MonitActionType.Action_Start || A.id == MonitActionType.Action_Restart))
                    s.nstart++;

                if (s.mode == MonitMonitorModeType.Monitor_Passive &&
                    (A.id == MonitActionType.Action_Start || A.id == MonitActionType.Action_Stop ||
                     A.id == MonitActionType.Action_Restart))
                    return;

                Control.ControlService(s.name, A.id);
            }
        }

        /// <summary>
        ///     TODO
        ///     Events the queue add.
        /// </summary>
        /// <param name="E">The e.</param>
        private static void eventQueueAdd(Event_T E)
        {
            //        if (! file_checkQueueDirectory(Run.eventlist_dir)) {
            //                LogError("Aborting event - cannot access the directory %s\n", Run.eventlist_dir);
            //                return;
            //        }

            //        if (! file_checkQueueLimit(Run.eventlist_dir, Run.eventlist_slots)) {
            //                LogError("Aborting event - queue over quota\n");
            //                return;
            //        }

            //        /* compose the file name of actual timestamp and service name */
            //        char file_name[PATH_MAX];
            //        snprintf(file_name, PATH_MAX, "%s/%lld_%lx", Run.eventlist_dir, (long long)Time_now(), (long unsigned)E->source);

            //        LogInfo("Adding event to the queue file %s for later delivery\n", file_name);

            //        FILE *file = fopen(file_name, "w");
            //        if (! file) {
            //                LogError("Aborting event - cannot open the event file %s -- %s\n", file_name, STRERROR);
            //                return;
            //        }

            //        boolean_t  rv;

            //        /* write event structure version */
            //        int version = EVENT_VERSION;
            //        if (! (rv = file_writeQueue(file, &version, sizeof(int))))
            //                goto error;

            //        /* write event structure */
            //        if (! (rv = file_writeQueue(file, E, sizeof(*E))))
            //                goto error;

            //        /* write source */
            //        if (! (rv = file_writeQueue(file, E->source, E->source ? strlen(E->source) + 1 : 0)))
            //                goto error;

            //        /* write message */
            //        if (! (rv = file_writeQueue(file, E->message, E->message ? strlen(E->message) + 1 : 0)))
            //                goto error;

            //        /* write event action */
            //        Action_Type action = Event_get_action(E);
            //        if (! (rv = file_writeQueue(file, &action, sizeof(Action_Type))))
            //                goto error;

            //error:
            //        fclose(file);
            //        if (! rv) {
            //                LogError("Aborting event - unable to save event information to %s\n",  file_name);
            //                if (unlink(file_name) < 0)
            //                        LogError("Failed to remove event file '%s' -- %s\n", file_name, STRERROR);
            //        } else {
            //                if (! Run.handler_init && E->flag & Handler_Alert)
            //                        Run.handler_queue[Handler_Alert]++;
            //                if (! Run.handler_init && E->flag & Handler_Mmonit)
            //                        Run.handler_queue[Handler_Mmonit]++;
            //        }
        }

        /// <summary>
        ///     TODO
        ///     Event_queue_updates the specified e.
        /// </summary>
        /// <param name="E">The e.</param>
        /// <param name="file_name">The file_name.</param>
        private static void Event_queue_update(Event_T E, string file_name)
        {
            var version = Event_T.EVENT_VERSION;
            var action = GetAction(E);
            bool rv;

            //        if (! file_checkQueueDirectory(Run.eventlist_dir)) {
            //                LogError("Aborting event - cannot access the directory %s\n", Run.eventlist_dir);
            //                return;
            //        }

            //        DEBUG("Updating event in the queue file %s for later delivery\n", file_name);

            var file = File.OpenWrite(file_name);
            //        FILE *file = fopen(file_name, "w");
            //        if (! file) {
            //                LogError("Aborting event - cannot open the event file %s -- %s\n", file_name, STRERROR);
            //                return;
            //        }

            //        /* write event structure version */
            //        if (! (rv = file_writeQueue(file, &version, sizeof(int))))
            //                goto error;

            //        /* write event structure */
            //        if (! (rv = file_writeQueue(file, E, sizeof(*E))))
            //                goto error;

            //        /* write source */
            //        if (! (rv = file_writeQueue(file, E->source, E->source ? strlen(E->source) + 1 : 0)))
            //                goto error;

            //        /* write message */
            //        if (! (rv = file_writeQueue(file, E->message, E->message ? strlen(E->message) + 1 : 0)))
            //                goto error;

            //        /* write event action */
            //        if (! (rv = file_writeQueue(file, &action, sizeof(Action_Type))))
            //                goto error;

            //error:
            //        fclose(file);
            //        if (! rv) {
            //                LogError("Aborting event - unable to update event information to %s\n", file_name);
            //                if (unlink(file_name) < 0)
            //                        LogError("Failed to remove event file '%s' -- %s\n", file_name, STRERROR);
            //        }
        }

        #endregion
    }
}