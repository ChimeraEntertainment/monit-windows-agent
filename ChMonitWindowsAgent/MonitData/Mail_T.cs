
namespace ChMonitoring.MonitData
{
    public class Mail_T
    {
        public uint events; /*< Events for which this mail object should be sent */
        public string from; /**< The mail from address */
        public string message; /**< The mail message */
        public uint reminder; /*< Send error reminder each Xth cycle */
        public string replyto; /**< Optional reply-to address */
        public string subject; /**< The mail subject */
        public string to; /**< Mail address for alert notification */
    }

    public class MailServer_T
    {
        public string host; /**< Server host address, may be a IP or a hostname string */
        public string password; /** < Password for SMTP_AUTH */
        public int port; /**< Server port */
        public string username; /** < Username for SMTP_AUTH */
    }
}
