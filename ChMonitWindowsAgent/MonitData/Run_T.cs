using System;
using System.Collections.Generic;

namespace ChMonitoring.MonitData
{
    /// <summary>
    /// Defines data for application runtime
    /// </summary>
    public class Run_T
    {
        //FIXME: move files to sub-struct
        public string controlfile; /**< The file to read configuration from */
        //FIXME: create enum for Run flags and replace set of various boolean_t single-purpose flags with common flags where possible
        public bool debug; /**< Debug level */
        public bool doaction; /**< true if some service(s) has action pending */
        public bool dolog; /**< true if program should log actions, otherwise false */
        public bool dommonitcredentials; /**< true if M/Monit should receive credentials */
        public bool doprocess = true; /**< true if process status engine is used */
        public /*volatile*/ bool doreload; /**< true if a monit daemon should reinitialize */
        public /*volatile*/ bool dowakeup; /**< true if a monit daemon was wake up by signal */
        public myenvironment Env;
        public List<Event_T> eventlist; /** A list holding partialy handled events */
        public string eventlist_dir; /**< The event queue base directory */
        public int eventlist_slots; /**< The event queue size - number of slots */
        public int expectbuffer; /**< Generic protocol expect buffer - STRLEN by default */
        public int facility; /** The facility to use when running openlog() */
        public bool fipsEnabled; /** true if monit should use FIPS-140 mode */
        public MonitHandlerType handler_flag; /**< The handlers state flag */
        public bool handler_init; /**< The handlers queue initialization */
        public int[] handler_queue = new int[(int)MonitHandlerType.Handler_Max + 1]; /**< The handlers queue counter */
        /** An object holding Monit HTTP interface setup */
        public Httpd_T httpd;
        public string id; /**< Unique monit id */
        public string idfile; /**< The file with unique monit id */
        public DateTime incarnation; /**< Unique ID for running monit instance */
        public bool init; /**< true - don't background to run from init */
        public bool isdaemon; /**< true if program should run as a daemon */
        public string logfile; /**< The file to write logdata into */
        public string mail_hostname; /**< Used in HELO/EHLO/MessageID when sending mail */
        public myformat MailFormat;
        public List<Mail_T> maillist; /**< Global alert notification mailinglist */
        public int mailserver_timeout; /**< Connect and read timeout ms for a SMTP server */
        public List<MailServer_T> mailservers; /**< List of MTAs used for alert notification */
        public List<Mmonit_T> mmonits; /**< Event notification and status receivers list */
        public string mygroup; /**< Group Name of the Service */
        public bool once; /**< true - run only once */
        public string pidfile; /**< This programs pidfile */
        public int polltime; /**< In deamon mode, the sleeptime (sec) between run */
        public int startdelay; /**< the sleeptime (sec) after startup */
        public string statefile; /**< The file with the saved runtime state */
        public /*volatile*/ bool stopped; /**< true if monit was stopped. Flag used by threads */
        public List<Service_T> system; /**< The general system service */
        public bool use_syslog; /**< If true write log to syslog */
    }

    public class Httpd_T
    {
        public string address;
        public List<Auth_T> credentials;
        //public Httpd_Flags flags;
        public int port;
        public bool ssl;
    }

    public class Auth_T
    {
        public string groupname; /**< PAM group name */
        //public Digest_Type digesttype;                /**< How did we store the password */
        public bool is_readonly; /**< true if this is a read-only authenticated user*/
        public string passwd; /**< The users password data */
        public string uname; /**< User allowed to connect to monit httpd */
    }

    /** User selected standard mail format */
    public class myformat
    {
        public string from; /**< The standard mail from address */
        public string message; /**< The standard mail message */
        public string replyto; /**< Optional reply-to header */
        public string subject; /**< The standard mail subject */
    }

    /** An object holding program relevant "environment" data, see: env.c */
    public class myenvironment
    {
        public string cwd; /**< Current working directory */
        public string home; /**< Users home directory */
        public string user; /**< The the effective user running this program */
    }
}
