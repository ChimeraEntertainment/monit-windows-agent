namespace ChMonitoring.Http
{
    public enum HttpStatusCode
    {
        SC_CONTINUE = 100,
        SC_SWITCHING_PROTOCOLS = 101,
        SC_PROCESSING = 102,
        SC_OK = 200,
        SC_CREATED = 201,
        SC_ACCEPTED = 202,
        SC_NON_AUTHORITATIVE = 203,
        SC_NO_CONTENT = 204,
        SC_RESET_CONTENT = 205,
        SC_PARTIAL_CONTENT = 206,
        SC_MULTI_STATUS = 207,
        SC_MULTIPLE_CHOICES = 300,
        SC_MOVED_PERMANENTLY = 301,
        SC_MOVED_TEMPORARILY = 302,
        SC_SEE_OTHER = 303,
        SC_NOT_MODIFIED = 304,
        SC_USE_PROXY = 305,
        SC_TEMPORARY_REDIRECT = 307,
        SC_BAD_REQUEST = 400,
        SC_UNAUTHORIZED = 401,
        SC_PAYMENT_REQUIRED = 402,
        SC_FORBIDDEN = 403,
        SC_NOT_FOUND = 404,
        SC_METHOD_NOT_ALLOWED = 405,
        SC_NOT_ACCEPTABLE = 406,
        SC_PROXY_AUTHENTICATION_REQUIRED = 407,
        SC_REQUEST_TIMEOUT = 408,
        SC_CONFLICT = 409,
        SC_GONE = 410,
        SC_LENGTH_REQUIRED = 411,
        SC_PRECONDITION_FAILED = 412,
        SC_REQUEST_ENTITY_TOO_LARGE = 413,
        SC_REQUEST_URI_TOO_LARGE = 414,
        SC_UNSUPPORTED_MEDIA_TYPE = 415,
        SC_RANGE_NOT_SATISFIABLE = 416,
        SC_EXPECTATION_FAILED = 417,
        SC_UNPROCESSABLE_ENTITY = 422,
        SC_LOCKED = 423,
        SC_FAILED_DEPENDENCY = 424,
        SC_INTERNAL_SERVER_ERROR = 500,
        SC_NOT_IMPLEMENTED = 501,
        SC_BAD_GATEWAY = 502,
        SC_SERVICE_UNAVAILABLE = 503,
        SC_GATEWAY_TIMEOUT = 504,
        SC_VERSION_NOT_SUPPORTED = 505,
        SC_VARIANT_ALSO_VARIES = 506,
        SC_INSUFFICIENT_STORAGE = 507,
        SC_NOT_EXTENDED = 510
    }

    internal class HttpStatusCodes
    {
        public static string GetStatusString(int statusCode)
        {
            return GetStatusString((HttpStatusCode) statusCode);
        }

        public static string GetStatusString(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.SC_OK:
                    return "OK";
                case HttpStatusCode.SC_ACCEPTED:
                    return "Accepted";
                case HttpStatusCode.SC_BAD_GATEWAY:
                    return "Bad Gateway";
                case HttpStatusCode.SC_BAD_REQUEST:
                    return "Bad Request";
                case HttpStatusCode.SC_CONFLICT:
                    return "Conflict";
                case HttpStatusCode.SC_CONTINUE:
                    return "Continue";
                case HttpStatusCode.SC_CREATED:
                    return "Created";
                case HttpStatusCode.SC_EXPECTATION_FAILED:
                    return "Expectation Failed";
                case HttpStatusCode.SC_FORBIDDEN:
                    return "Forbidden";
                case HttpStatusCode.SC_GATEWAY_TIMEOUT:
                    return "Gateway Timeout";
                case HttpStatusCode.SC_GONE:
                    return "Gone";
                case HttpStatusCode.SC_VERSION_NOT_SUPPORTED:
                    return "HTTP Version Not Supported";
                case HttpStatusCode.SC_INTERNAL_SERVER_ERROR:
                    return "Internal Server Error";
                case HttpStatusCode.SC_LENGTH_REQUIRED:
                    return "Length Required";
                case HttpStatusCode.SC_METHOD_NOT_ALLOWED:
                    return "Method Not Allowed";
                case HttpStatusCode.SC_MOVED_PERMANENTLY:
                    return "Moved Permanently";
                case HttpStatusCode.SC_MOVED_TEMPORARILY:
                    return "Moved Temporarily";
                case HttpStatusCode.SC_MULTIPLE_CHOICES:
                    return "Multiple Choices";
                case HttpStatusCode.SC_NO_CONTENT:
                    return "No Content";
                case HttpStatusCode.SC_NON_AUTHORITATIVE:
                    return "Non-Authoritative Information";
                case HttpStatusCode.SC_NOT_ACCEPTABLE:
                    return "Not Acceptable";
                case HttpStatusCode.SC_NOT_FOUND:
                    return "Not Found";
                case HttpStatusCode.SC_NOT_IMPLEMENTED:
                    return "Not Implemented";
                case HttpStatusCode.SC_NOT_MODIFIED:
                    return "Not Modified";
                case HttpStatusCode.SC_PARTIAL_CONTENT:
                    return "Partial Content";
                case HttpStatusCode.SC_PAYMENT_REQUIRED:
                    return "Payment Required";
                case HttpStatusCode.SC_PRECONDITION_FAILED:
                    return "Precondition Failed";
                case HttpStatusCode.SC_PROXY_AUTHENTICATION_REQUIRED:
                    return "Proxy Authentication Required";
                case HttpStatusCode.SC_REQUEST_ENTITY_TOO_LARGE:
                    return "Request Entity Too Large";
                case HttpStatusCode.SC_REQUEST_TIMEOUT:
                    return "Request Timeout";
                case HttpStatusCode.SC_REQUEST_URI_TOO_LARGE:
                    return "Request URI Too Large";
                case HttpStatusCode.SC_RANGE_NOT_SATISFIABLE:
                    return "Requested Range Not Satisfiable";
                case HttpStatusCode.SC_RESET_CONTENT:
                    return "Reset Content";
                case HttpStatusCode.SC_SEE_OTHER:
                    return "See Other";
                case HttpStatusCode.SC_SERVICE_UNAVAILABLE:
                    return "Service Unavailable";
                case HttpStatusCode.SC_SWITCHING_PROTOCOLS:
                    return "Switching Protocols";
                case HttpStatusCode.SC_UNAUTHORIZED:
                    return "Unauthorized";
                case HttpStatusCode.SC_UNSUPPORTED_MEDIA_TYPE:
                    return "Unsupported Media Type";
                case HttpStatusCode.SC_USE_PROXY:
                    return "Use Proxy";
                default:
                {
                    return "Unknown HTTP status";
                }
            }
        }
    }
}