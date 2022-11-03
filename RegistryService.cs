using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DeskTime
{
    internal class RegistryService
    {
        public static RegistryKey rootRegisterKey;

        public static RegistryKey getRootRegisterKey()
        {
            if (rootRegisterKey == null)
            {
                rootRegisterKey = GetParent(Application.UserAppDataRegistry).OpenSubKey("1.3", writable: true);
                if (rootRegisterKey == null)
                {
                    GetParent(GetParent(Application.UserAppDataRegistry)).OpenSubKey(Application.ProductName, writable: true).CreateSubKey("1.3").Close();
                    rootRegisterKey = GetParent(Application.UserAppDataRegistry).OpenSubKey("1.3", writable: true);
                }
            }
            return rootRegisterKey;
        }

        public static object GetValue(string name)
        {
            return getRootRegisterKey().GetValue(name);
        }

        public static object GetValue(string name, object defaultValue)
        {
            return getRootRegisterKey().GetValue(name, defaultValue);
        }

        public static void SetValue(string name, object value)
        {
            getRootRegisterKey().SetValue(name, value);
        }

        public static void DeleteValue(string name)
        {
            getRootRegisterKey().DeleteValue(name);
        }

        public static void DeleteValue(string name, bool throwOnMissingValue)
        {
            getRootRegisterKey().DeleteValue(name, throwOnMissingValue);
        }

        public static RegistryKey OpenSubKey(string name)
        {
            return getRootRegisterKey().OpenSubKey(name);
        }

        public static RegistryKey OpenSubKey(string name, bool writable)
        {
            return getRootRegisterKey().OpenSubKey(name, writable);
        }

        public static RegistryKey CreateSubKey(string subkey)
        {
            return getRootRegisterKey().CreateSubKey(subkey);
        }

        internal static void MigrateRegistry(RegistryKey userAppDataRegistry)
        {
            try
            {
                if (userAppDataRegistry.GetValue("proxystyle", "") as string != "")
                {
                    RegistryKey registryKey = userAppDataRegistry.OpenSubKey("Proxy", writable: true);
                    if (registryKey == null)
                    {
                        registryKey = userAppDataRegistry.CreateSubKey("Proxy");
                    }
                    int num = int.Parse(userAppDataRegistry.GetValue("proxystyle", "1") as string);
                    string value = userAppDataRegistry.GetValue("proxyaddress", "") as string;
                    int num2 = int.Parse(userAppDataRegistry.GetValue("proxyport", "0") as string);
                    string value2 = userAppDataRegistry.GetValue("proxyuser", "") as string;
                    string value3 = userAppDataRegistry.GetValue("proxypassword", "") as string;
                    registryKey.SetValue("proxystyle", num.ToString());
                    registryKey.SetValue("proxypassword", value3);
                    registryKey.SetValue("proxyuser", value2);
                    registryKey.SetValue("proxyport", num2.ToString());
                    registryKey.SetValue("proxyaddress", value);
                    registryKey.Close();
                    userAppDataRegistry.DeleteValue("proxystyle", throwOnMissingValue: false);
                    userAppDataRegistry.DeleteValue("proxypassword", throwOnMissingValue: false);
                    userAppDataRegistry.DeleteValue("proxyuser", throwOnMissingValue: false);
                    userAppDataRegistry.DeleteValue("proxyport", throwOnMissingValue: false);
                    userAppDataRegistry.DeleteValue("proxyaddress", throwOnMissingValue: false);
                }
            }
            catch (Exception)
            {
            }
        }

        private static RegistryKey GetParent(RegistryKey value)
        {
            int num = value.Name.IndexOf("\\");
            int num2 = value.Name.LastIndexOf("\\");
            string name = value.Name.Substring(num + 1, num2 - num);
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(name);
            if (registryKey != null)
            {
                return registryKey;
            }
            return null;
        }

        private static bool CopyKey(RegistryKey parentKey, string keyNameToCopy, string newKeyName)
        {
            RegistryKey registryKey;
            try
            {
                registryKey = parentKey.OpenSubKey(newKeyName, writable: true);
            }
            catch (Exception)
            {
                registryKey = null;
            }
            if (registryKey == null)
            {
                try
                {
                    registryKey = parentKey.CreateSubKey(newKeyName);
                }
                catch (Exception)
                {
                    registryKey = null;
                }
            }
            if (registryKey == null)
            {
                return false;
            }
            RecurseCopyKey(parentKey.OpenSubKey(keyNameToCopy), registryKey);
            return true;
        }

        private static void RecurseCopyKey(RegistryKey sourceKey, RegistryKey destinationKey)
        {
            string[] valueNames = sourceKey.GetValueNames();
            foreach (string name in valueNames)
            {
                object value = sourceKey.GetValue(name);
                RegistryValueKind valueKind = sourceKey.GetValueKind(name);
                destinationKey.SetValue(name, value, valueKind);
            }
            valueNames = sourceKey.GetSubKeyNames();
            foreach (string text in valueNames)
            {
                RegistryKey sourceKey2 = sourceKey.OpenSubKey(text);
                RegistryKey destinationKey2 = destinationKey.CreateSubKey(text);
                RecurseCopyKey(sourceKey2, destinationKey2);
            }
        }
    }
}
