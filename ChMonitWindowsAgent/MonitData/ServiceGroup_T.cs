using System.Collections.Generic;

namespace ChMonitoring.MonitData
{
    public class ServiceGroup_T
    {
        public List<ServiceGroupMember_T> members; /**< Service group members */
        public string name; /**< name of service group */
    }

    public class ServiceGroupMember_T
    {
        public string name; /**< name of service */
    }
}
