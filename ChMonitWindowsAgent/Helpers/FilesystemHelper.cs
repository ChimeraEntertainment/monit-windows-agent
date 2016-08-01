using System;
using System.IO;
using System.Linq;
using ChMonitoring.MonitData;

namespace ChMonitoring.Helpers
{
    internal class FilesystemHelper : ServiceHelper
    {
        public static void AddFilesystem(Service_T service)
        {
            var drives = DriveInfo.GetDrives();

            if (service.type == MonitServiceType.Service_Filesystem)
            {
                var foundServices =
                    drives.Where(
                        sc =>
                            (sc.IsReady && sc.DriveType == DriveType.Fixed)
                                ? sc.VolumeLabel.ToLower() == service.name
                                : false).ToList();
                if (foundServices != null && foundServices.Count() > 0)
                {
                    foundServices.ForEach(sc =>
                    {
                        service.name = sc.VolumeLabel.ToLower() + " (" + sc.DriveFormat.ToUpper() + ")";
                        service.inf = new FileSystemInfo_T
                        {
                            f_blocks = sc.TotalSize,
                            f_blocksfreetotal = sc.TotalFreeSpace,
                            space_total = sc.TotalSize,
                            space_percent = Convert.ToInt16((sc.TotalSize == 0) ? 10 : ((sc.TotalFreeSpace/sc.TotalSize)*10))
                                
                        };
                        AddService(service);
                    });
                }
            }
        }
    }
}