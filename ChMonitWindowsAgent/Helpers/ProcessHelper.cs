using System.Linq;
using System.ServiceProcess;
using ChMonitoring.MonitData;

namespace ChMonitoring.Helpers
{
    internal class ProcessHelper : ServiceHelper
    {
        public static void AddProcess(Service_T service)
        {
            var services = ServiceController.GetServices();

            if (service.type == MonitServiceType.Service_Process)
            {
                var foundServices = services.Where(sc => sc.ServiceName.ToLower() == service.name).ToList();
                if (foundServices != null && foundServices.Count() > 0)
                {
                    foundServices.ForEach(sc =>
                    {
                        service.start = sc.Start;
                        service.stop = sc.Stop;
                        AddService(service);
                    });
                }
            }
        }

        public static ServiceController GetController(string serviceName)
        {
            var services = ServiceController.GetServices();
            var controller = services.Where(s => s.ServiceName.ToLower() == serviceName.ToLower()).FirstOrDefault();
            if (controller != null)
            {
                return controller;
            }
            return null;
        }
    }
}