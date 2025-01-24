using Microsoft.Win32;
using NvAPIWrapper;
using NvAPIWrapper.GPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoPcCleaner.Helpers
{
    public class Sysinfos
    {

        /// <summary>
        /// Get windows version
        /// </summary>
        /// <returns>Windows version string</returns>
        public string GetWinVer()
        {
            try
            {
                return Environment.OSVersion.ToString();
            }catch(Exception e)
            {
                return "Windows";
            }
        }



        /// <summary>
        /// Get computer hardware infos
        /// </summary>
        /// <returns>hardware infos string</returns>
        public string GetHardwareInfos()
        {
            StringBuilder sb = new StringBuilder();
            RegistryKey processor_name = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0",RegistryKeyPermissionCheck.ReadSubTree);
            if(processor_name != null)
            {
                sb.AppendLine($"{processor_name.GetValue("ProcessorNameString")}");
            }
            try
            {
                NVIDIA.Initialize();
                sb.AppendLine($"{PhysicalGPU.GetPhysicalGPUs()[0]}");
            }catch(Exception e)
            {

            }
           
            return sb.ToString();
        }
    }
}
