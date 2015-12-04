using System.Collections.Generic;
using ChMonitoring.Helpers;
using ChMonitoring.MonitData;

namespace ChMonitoring
{
    internal class Alert
    {
        public const string SMTP_CLIENT = "smtp.live.com";
        public const int SMTP_PORT = 25;
        public const string CREDENTIAL_USERNAME = "Test@test.de";
        public const string CREDENTIAL_PASSWORD = "test";
        public const string ALERT_FROM = "monit@$HOST";
        public const string ALERT_SUBJECT = "monit alert -- $EVENT $SERVICE";

        public const string ALERT_MESSAGE =
            @"$EVENT Service $SERVICE \r\n
            \r\n
            \tDate:        $DATE\r\n
            \tAction:      $ACTION\r\n
            \tHost:        $HOST\r\n
            \tDescription: $DESCRIPTION\r\n
            \r\n
            Your faithful employee,\r\n
            Monit\r\n";

        public List<string> Maillist = new List<string>();
        /**
         * Notify registred users about the event
         * @param E An Event object
         * @return If failed, return Handler_Alert flag or Handler_Succeeded if succeeded
         */

        public static MonitHandlerType HandleAlert(Event_T E)
        {
            var rv = MonitHandlerType.Handler_Succeeded;

            var s = Event.GetSource(E);
            if (s == null)
            {
                Logger.Log.Error("Aborting alert\n");
                return rv;
            }
            if (s.maillist != null || MonitWindowsAgent.Run.maillist != null)
            {
                var list = new List<Mail_T>();
                /*
                 * Build a mail-list with local recipients that has registered interest
                 * for this event.
                 */
                foreach (var m in s.maillist)
                {
                    if (
                        /* particular event notification type is allowed for given recipient */
                        Event.IsEventSet((int) m.events, E.id) &&
                        (
                            /* state change notification is sent always */
                            E.state_changed ||
                            /* in the case that the state is failed for more cycles we check
                         * whether we should send the reminder */
                            (E.state == MonitStateType.State_Failed && m.reminder > 0 && E.count%m.reminder == 0)
                            )
                        )
                    {
                        var tmp = new Mail_T();
                        copy_mail(tmp, m);
                        substitute(tmp, E);
                        escape(tmp);
                        list.Insert(0, tmp);
                        Logger.Log.DebugFormat("{0} notification is sent to {1}", Event.GetDescription(E), m.to);
                    }
                }
                /*
                 * Build a mail-list with global recipients that has registered interest
                 * for this event. Recipients which are defined in the service localy
                 * overrides the same recipient events which are registered globaly.
                 */
                foreach (var m in MonitWindowsAgent.Run.maillist)
                {
                    var skip = false;
                    foreach (var n in s.maillist)
                    {
                        if (m.to == n.to)
                        {
                            skip = true;
                            break;
                        }
                    }
                    if (
                        /* the local service alert definition has not overrided the global one */
                        !skip &&
                        /* particular event notification type is allowed for given recipient */
                        Event.IsEventSet((int) m.events, E.id) &&
                        (
                            /* state change notification is sent always */
                            E.state_changed ||
                            /* in the case that the state is failed for more cycles we check
                         * whether we should send the reminder */
                            (E.state == MonitStateType.State_Failed && m.reminder > 0 && E.count%m.reminder == 0)
                            )
                        )
                    {
                        var tmp = new Mail_T();
                        copy_mail(tmp, m);
                        substitute(tmp, E);
                        escape(tmp);
                        list.Insert(0, tmp);
                        Logger.Log.DebugFormat("{0} notification is sent to {1}", Event.GetDescription(E), m.to);
                    }
                }
                if (list != null)
                {
                    if (Sendmail.Send(list))
                        rv = MonitHandlerType.Handler_Alert;
                }
            }
            return rv;
        }

        private static void substitute(Mail_T m, Event_T e)
        {
            m.from = m.from.Replace("$HOST", MonitWindowsAgent.Run.system[0].name);
            m.subject = m.subject.Replace("$HOST", MonitWindowsAgent.Run.system[0].name);
            m.message = m.message.Replace("$HOST", MonitWindowsAgent.Run.system[0].name);

            m.subject = m.subject.Replace("$DATE", Timing.GetTimestamp(e.collected).ToString());
            m.message = m.message.Replace("$DATE", Timing.GetTimestamp(e.collected).ToString());

            m.subject = m.subject.Replace("$SERVICE", e.source);
            m.message = m.message.Replace("$SERVICE", e.source);

            m.subject = m.subject.Replace("$EVENT", Event.GetDescription(e));
            m.message = m.message.Replace("$EVENT", Event.GetDescription(e));

            m.subject = m.subject.Replace("$DESCRIPTION", e.message); //NVLSTR(e.message));
            m.message = m.message.Replace("$DESCRIPTION", e.message); //NVLSTR(e.message));

            m.subject = m.subject.Replace("$ACTION", MonitWindowsAgent.actionnames[(int) Event.GetAction(e)]);
            m.message = m.message.Replace("$ACTION", MonitWindowsAgent.actionnames[(int) Event.GetAction(e)]);
        }

        private static void copy_mail(Mail_T n, Mail_T o)
        {
            n.to = o.to;
            n.from = !string.IsNullOrEmpty(o.from)
                ? o.from
                : !string.IsNullOrEmpty(MonitWindowsAgent.Run.MailFormat.from)
                    ? MonitWindowsAgent.Run.MailFormat.from
                    : ALERT_FROM;
            n.replyto = !string.IsNullOrEmpty(o.replyto)
                ? o.replyto
                : !string.IsNullOrEmpty(MonitWindowsAgent.Run.MailFormat.replyto)
                    ? MonitWindowsAgent.Run.MailFormat.replyto
                    : "";
            n.subject = !string.IsNullOrEmpty(o.subject)
                ? o.subject
                : !string.IsNullOrEmpty(MonitWindowsAgent.Run.MailFormat.subject)
                    ? MonitWindowsAgent.Run.MailFormat.subject
                    : ALERT_SUBJECT;
            n.message = !string.IsNullOrEmpty(o.message)
                ? o.message
                : !string.IsNullOrEmpty(MonitWindowsAgent.Run.MailFormat.message)
                    ? MonitWindowsAgent.Run.MailFormat.message
                    : ALERT_MESSAGE;
        }

        private static void escape(Mail_T m)
        {
            // replace bare linefeed
            m.message = m.message.Replace("\r\n", "\n");
            m.message = m.message.Replace("\n", "\r\n");
            // escape ^.
            m.message = m.message.Replace("\n.", "\n..");
            // drop any CR|LF from the subject
            m.subject = m.subject.Replace("\r", "").Replace("\n", "");
        }
    }
}