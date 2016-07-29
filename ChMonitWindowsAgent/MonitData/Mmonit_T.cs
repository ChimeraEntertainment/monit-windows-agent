
namespace ChMonitoring.MonitData
{
    /** Defines an event notification and status receiver object */
    public class Mmonit_T
    {
        //SslOptions_T ssl;                                      /**< SSL definition */
        public int timeout; /**< The timeout to wait for connection or i/o */
        public URL_T url; /**< URL definition */
    }

    /** Defines an url object */
    public class URL_T
    {
        public string hostname; /**< URL hostname part */
        public string password; /**< URL password part */
        public string path; /**< URL path     part */
        public int port; /**< URL port     part */
        public string protocol; /**< URL protocol type */
        public string query; /**< URL query    part */
        public string url; /**< Full URL */
        public string user; /**< URL user     part */
    }   
}
