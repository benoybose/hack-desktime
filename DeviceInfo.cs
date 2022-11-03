#define TRACE
using System;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace DeskTime
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    internal struct DeviceInfo
    {
        public static string MachineName = Environment.MachineName;

        public static string UserName = Environment.UserName;

        public static string Version = Environment.Version.ToString();

        public static string CommandLine = Environment.CommandLine;

        public static string OSVersion = Environment.OSVersion.ToString();

        public static string OsName = GetOSFriendlyName();

        public static string SerialNumber = GetSN();

        public static string Model = GetComputerSystem("model");

        public static string Manufacturer = GetComputerSystem("manufacturer");

        public static string Framework = Tools.CheckFor462PlusVersion(Tools.GetDotNetReleaseFromRegistry());

        private static string GetOSFriendlyName()
        {
            string result = string.Empty;
            try
            {
                using ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = new ManagementObjectSearcher("select Caption from Win32_OperatingSystem").Get().GetEnumerator();
                if (managementObjectEnumerator.MoveNext())
                {
                    result = ((ManagementObject)managementObjectEnumerator.Current)["Caption"].ToString();
                    return result;
                }
                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "GetComputerOs Exception: " + ex.ToString());
                return result;
            }
        }

        private static string GetComputerSystem(string property)
        {
            string text = string.Empty;
            try
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("select * from Win32_ComputerSystem");
                managementObjectSearcher.Get();
                foreach (ManagementObject item in managementObjectSearcher.Get())
                {
                    text += item.GetPropertyValue(property).ToString();
                }
                return text;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "GetComputerSystem Exception: " + ex.ToString());
                return text;
            }
        }

        private static string GetSN()
        {
            string text = string.Empty;
            try
            {
                foreach (ManagementObject item in new ManagementObjectSearcher("select * from Win32_Processor")
                {
                    Query = new ObjectQuery("select * from Win32_BIOS")
                }.Get())
                {
                    text += item.GetPropertyValue("SerialNumber").ToString();
                }
                return text;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "SerialNumber Exception: " + ex.ToString());
                return text;
            }
        }

        public static string GerIPAddress()
        {
            string result = string.Empty;
            try
            {
                NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface networkInterface in allNetworkInterfaces)
                {
                    IPInterfaceProperties iPProperties = networkInterface.GetIPProperties();
                    if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet || networkInterface.OperationalStatus != OperationalStatus.Up || networkInterface.Description.ToLower().Contains("virtual") || networkInterface.Description.ToLower().Contains("pseudo"))
                    {
                        continue;
                    }
                    foreach (UnicastIPAddressInformation unicastAddress in iPProperties.UnicastAddresses)
                    {
                        if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(unicastAddress.Address))
                        {
                            result = unicastAddress.Address.ToString();
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "SerialNumber Exception: " + ex.ToString());
                return result;
            }
        }
    }
}
