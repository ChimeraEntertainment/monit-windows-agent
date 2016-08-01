﻿using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace ChMonitoring.Helpers
{
    public class UniqueWindowsId
    {
        /// <summary>
        /// 32 bit unique id, intially using the cpu and hdd id, then md5, then stored in ~/.monit.id, if hardware gets changed
        /// </summary>
        /// <returns></returns>
        public static string GetOrCreateUniqueId()
        {
            string monitIdFilePathName;
            var candidatePath = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            if (Directory.Exists(candidatePath))
            {
                // This won't exist if running as System
                monitIdFilePathName = Path.Combine(candidatePath, ".monit.id");
            }
            else
            {
                monitIdFilePathName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), ".monit.id");
            }

            string idValue;

            if (!File.Exists(monitIdFilePathName))
            {
                //idValue = GetUniqueWindowsIdByCPUAndHdd();
                idValue = GetUniqueWindowsIdByGuid();
                File.WriteAllText(monitIdFilePathName, idValue);
                return idValue;
            }

            idValue = File.ReadAllText(monitIdFilePathName).Trim();
            
            if (string.IsNullOrEmpty(idValue) || idValue.Length != 32)
            {
                // If read ID was invalid, create a new one...
                //idValue = GetUniqueWindowsIdByCPUAndHdd();
                idValue = GetUniqueWindowsIdByGuid();
                File.WriteAllText(monitIdFilePathName, idValue);
            }

            return idValue;
        }

        /// <summary>
        /// Uses the registry to store the key information in...
        /// </summary>
        /// <returns></returns>
        public static string GetOrCreateUniqueIdViaRegistry()
        {
            string subKeyName = "SOFTWARE\\MonitWindowsAgent";
            string idValueName = "id";
            
            string idValue = null;

            var idSubKey = Registry.LocalMachine.OpenSubKey(subKeyName, true);

            if (idSubKey == null)
            {
                idSubKey = Registry.LocalMachine.CreateSubKey(subKeyName);
                idValue = Guid.NewGuid().ToString();
                idSubKey.SetValue(idValueName, idValue);
            }
            else
            {
                var idValueObj = idSubKey.GetValue(idValueName);
                if (idValueObj == null)
                {
                    //idValue = MD5(GetUniqueWindowsIdByCPUAndHdd());
                    idValue = MD5(GetUniqueWindowsIdByGuid()); // was before: Guid.NewGuid().ToString();
                    idSubKey.SetValue(idValueName, idValue);
                }
                else
                {
                    idValue = idValueObj.ToString();
                }
            }

            return idValue;
        }

        private static string MD5(string str)
        {
            byte[] bytes = new UTF8Encoding().GetBytes(str);

            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(bytes);

            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }

        private static string GetUniqueWindowsIdByCPUAndHdd()
        {
            string cpuInfo = string.Empty;
            var mc = new ManagementClass("win32_processor");
            var moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                 cpuInfo = mo.Properties["processorID"].Value.ToString();
                 break;
            }

            string drive = "C";
            var dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
            dsk.Get();
            string volumeSerial = dsk["VolumeSerialNumber"].ToString();

            return MD5(cpuInfo + volumeSerial);
        }

        private static string GetUniqueWindowsIdByGuid()
        {
            return MD5(Guid.NewGuid().ToString());
        }
    }
}
