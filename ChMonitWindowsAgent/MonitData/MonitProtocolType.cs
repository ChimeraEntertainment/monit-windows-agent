
namespace ChMonitoring.MonitData
{
    public enum MonitProtocolType : int
    {
        Protocol_DEFAULT = 0,
        Protocol_HTTP,
        Protocol_FTP,
        Protocol_SMTP,
        Protocol_POP,
        Protocol_IMAP,
        Protocol_NNTP,
        Protocol_SSH,
        Protocol_DWP,
        Protocol_LDAP2,
        Protocol_LDAP3,
        Protocol_RDATE,
        Protocol_RSYNC,
        Protocol_GENERIC,
        Protocol_APACHESTATUS,
        Protocol_NTP3,
        Protocol_MYSQL,
        Protocol_DNS,
        Protocol_POSTFIXPOLICY,
        Protocol_TNS,
        Protocol_PGSQL,
        Protocol_CLAMAV,
        Protocol_SIP,
        Protocol_LMTP,
        Protocol_GPS,
        Protocol_RADIUS,
        Protocol_MEMCACHE,
        Protocol_WEBSOCKET
    }
}
