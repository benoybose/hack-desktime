#define TRACE
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DeskTime
{
    internal static class Tools
    {
        internal static string GetPrettyTime(int secDiff)
        {
            int num = secDiff % 3600;
            string text = "";
            if (secDiff < 60)
            {
                return "Just started";
            }
            if (secDiff < 120)
            {
                return "1 minute";
            }
            if (secDiff < 3600)
            {
                return $"{Math.Floor((double)secDiff / 60.0)} minutes";
            }
            if (secDiff < 7200)
            {
                if (num > 60)
                {
                    text = " " + GetPrettyTime(num);
                }
                return "1 hour" + text;
            }
            if (secDiff < 86400)
            {
                if (num > 60)
                {
                    text = " " + GetPrettyTime(num);
                }
                return $"{Math.Floor((double)secDiff / 3600.0)} hours" + text;
            }
            return null;
        }

        internal static string LimitByteLength(string s, int n, string p = "")
        {
            StringInfo stringInfo = new StringInfo(s);
            if (stringInfo.LengthInTextElements > n)
            {
                return stringInfo.SubstringByTextElements(0, n - 1) + p;
            }
            return s;
        }

        internal static string GetUrlHost(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            if (Regex.Match(url, "^view-source:").Success)
            {
                url = Regex.Replace(url, "^view-source:", "");
            }
            if (!Regex.Match(url, "^https*://|^ftp://|^chrome://|^chrome-extension://|^file://|^about:").Success)
            {
                url = "http://" + url;
            }
            try
            {
                Uri uri = new Uri(url);
                switch (uri.Scheme)
                {
                    case "about":
                        return url;
                    case "file":
                    case "chrome":
                    case "chrome-extension":
                        return uri.Scheme + "://" + uri.Host;
                    default:
                        return uri.Host;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static string GetWindowString(IntPtr hWnd)
        {
            int windowTextLength = Win32.GetWindowTextLength(hWnd);
            if (windowTextLength > 0)
            {
                StringBuilder stringBuilder = new StringBuilder(windowTextLength + 1);
                Win32.GetWindowText(hWnd, stringBuilder, windowTextLength + 1);
                return stringBuilder.ToString();
            }
            return "";
        }

        internal static string GetClassNameString(IntPtr hWnd)
        {
            StringBuilder stringBuilder = new StringBuilder(100);
            Win32.GetClassName(hWnd, stringBuilder, 100);
            return stringBuilder.ToString();
        }

        internal static string ToQueryString(NameValueCollection nvc)
        {
            string[] value2 = (from key in nvc.AllKeys
                               from value in nvc.GetValues(key)
                               select $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}").ToArray();
            return "?" + string.Join("&", value2);
        }

        internal static DateTime ConvertFromUnixTimestamp(this long timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToLocalTime();
        }

        internal static long ConvertToUnixTimestamp(this DateTime date)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return (long)(date.ToUniversalTime() - dateTime).TotalSeconds;
        }

        internal static IntPtr GetWindowHandle(IntPtr hWnd, string path)
        {
            string[] array = path.Split('/');
            IntPtr intPtr = hWnd;
            string[] array2 = array;
            foreach (string lpszClass in array2)
            {
                intPtr = Win32.FindWindowEx(intPtr, IntPtr.Zero, lpszClass, null);
                if (intPtr == IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }
            }
            return intPtr;
        }

        internal unsafe static string GetWindowString(IntPtr hWnd, string path)
        {
            IntPtr windowHandle = GetWindowHandle(hWnd, path);
            if (windowHandle == IntPtr.Zero)
            {
                return null;
            }
            int num = Win32.SendMessage(windowHandle, 14, 0, IntPtr.Zero);
            if (num < 1)
            {
                return null;
            }
            char* ptr = stackalloc char[num + 1];
            num = Win32.SendMessage(windowHandle, 13, num + 1, (IntPtr)ptr);
            if (num < 1)
            {
                return null;
            }
            return new string(ptr);
        }

        internal static bool GetScreenSaverActive()
        {
            bool lpvParam = false;
            Win32.SystemParametersInfo(114, 0, ref lpvParam, 0);
            return lpvParam;
        }

        internal static string[] GetPebStrings(int pid)
        {
            IntPtr intPtr = Win32.OpenProcess((Win32.ProcessAccessFlags)1040u, bInheritHandle: false, pid);
            if (intPtr == IntPtr.Zero)
            {
                return null;
            }
            uint pebAddress = GetPebAddress(intPtr);
            if (pebAddress == 0)
            {
                Win32.CloseHandle(intPtr);
                return null;
            }
            string[] result = new string[3]
            {
                GetPebByOffset(intPtr, pebAddress, PebOffset.ImagePathName),
                GetPebByOffset(intPtr, pebAddress, PebOffset.CurrentDirectoryPath),
                GetPebByOffset(intPtr, pebAddress, PebOffset.CommandLine)
            };
            Win32.CloseHandle(intPtr);
            return result;
        }

        public static string GetMainModuleFileName(Process process, int buffer = 1024)
        {
            StringBuilder stringBuilder = new StringBuilder(buffer);
            uint lpdwSize = (uint)(stringBuilder.Capacity + 1);
            string result = null;
            try
            {
                result = (Win32.QueryFullProcessImageName(process.Handle, 0u, stringBuilder, ref lpdwSize) ? stringBuilder.ToString() : null);
                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "GetWindowData error: " + ex.ToString());
                return result;
            }
        }

        internal static string GetPebString(int pid, PebOffset offset)
        {
            IntPtr intPtr = Win32.OpenProcess((Win32.ProcessAccessFlags)1040u, bInheritHandle: false, pid);
            if (intPtr == IntPtr.Zero)
            {
                return null;
            }
            uint pebAddress = GetPebAddress(intPtr);
            if (pebAddress == 0)
            {
                Win32.CloseHandle(intPtr);
                return null;
            }
            string pebByOffset = GetPebByOffset(intPtr, pebAddress, offset);
            Win32.CloseHandle(intPtr);
            return pebByOffset;
        }

        private unsafe static uint GetPebAddress(IntPtr handle)
        {
            Win32.PROCESS_BASIC_INFORMATION pbi = default(Win32.PROCESS_BASIC_INFORMATION);
            if (Win32.NtQueryInformationProcess(handle, 0u, out pbi, pbi.Size, out var pSize) != 0)
            {
                return 0u;
            }
            byte* ptr = stackalloc byte[(int)(uint)IntPtr.Size];
            if (!Win32.ReadProcessMemory(handle, pbi.PebBaseAddress + 16, new IntPtr(ptr), 4, out pSize) || pSize != 4)
            {
                return 0u;
            }
            return *(uint*)ptr;
        }

        private unsafe static string GetPebByOffset(IntPtr handle, uint ppaddr, PebOffset offset)
        {
            Win32.UNICODE_STRING uNICODE_STRING = default(Win32.UNICODE_STRING);
            if (!Win32.ReadProcessMemory(handle, (uint)(ppaddr + offset), new IntPtr(&uNICODE_STRING), 8, out var lpNumberOfBytesRead) || lpNumberOfBytesRead != 8)
            {
                return null;
            }
            if (uNICODE_STRING.Length > 0)
            {
                byte[] array = new byte[uNICODE_STRING.Length];
                if (!Win32.ReadProcessMemory(handle, uNICODE_STRING.Buffer, array, uNICODE_STRING.Length, out lpNumberOfBytesRead) || lpNumberOfBytesRead != uNICODE_STRING.Length)
                {
                    return null;
                }
                return Encoding.Unicode.GetString(array);
            }
            return null;
        }

        internal static float getScalingFactor()
        {
            IntPtr hdc = Graphics.FromHwnd(IntPtr.Zero).GetHdc();
            int deviceCaps = Win32.GetDeviceCaps(hdc, 10);
            return (float)Win32.GetDeviceCaps(hdc, 117) / (float)deviceCaps;
        }

        internal static Image CaptureScreen(int monitorIndex = 0)
        {
            Screen screen = Screen.AllScreens[monitorIndex];
            Size size = screen.Bounds.Size;
            float num = ((monitorIndex == 0) ? getScalingFactor() : 1f);
            int width = (int)((float)size.Width * num);
            int height = (int)((float)size.Height * num);
            Bitmap bitmap = new Bitmap(width, height);
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            graphics.Dispose();
            return bitmap;
        }

        internal static Bitmap CaptureWindow(IntPtr handle)
        {
            IntPtr windowDC = Win32.GetWindowDC(handle);
            Win32.GetWindowRect(handle, out var lpRect);
            int nWidth = lpRect.Right - lpRect.Left;
            int nHeight = lpRect.Bottom - lpRect.Top;
            IntPtr intPtr = Win32.CreateCompatibleDC(windowDC);
            IntPtr intPtr2 = Win32.CreateCompatibleBitmap(windowDC, nWidth, nHeight);
            IntPtr hObject = Win32.SelectObject(intPtr, intPtr2);
            Win32.BitBlt(intPtr, 0, 0, nWidth, nHeight, windowDC, 0, 0, 13369376);
            Win32.SelectObject(intPtr, hObject);
            Win32.DeleteDC(intPtr);
            Win32.ReleaseDC(handle, windowDC);
            Bitmap result = Image.FromHbitmap(intPtr2);
            Win32.DeleteObject(intPtr2);
            return result;
        }

        internal static void About()
        {
            MessageBox.Show(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat("" + "DeskTime version: " + MainWin.Version?.ToString() + "\n", "Operating system version: ", DeviceInfo.OSVersion, "\n"), "Operating system name: ", DeviceInfo.OsName, "\n"), "Computer name: ", DeviceInfo.MachineName, "\n"), "User name: ", Environment.UserName, "\n"), "CLR runtime version: ", Environment.Version.ToString(), "\n"), Get462PlusFromRegistry(), "\n"), "About", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        internal static int GetDotNetReleaseFromRegistry()
        {
            int result = 0;
            try
            {
                using RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\");
                if (registryKey != null)
                {
                    if (registryKey.GetValue("Release") != null)
                    {
                        result = (int)registryKey.GetValue("Release");
                        return result;
                    }
                    return result;
                }
                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "GetDotNetReleaseFromRegistry Exception: " + ex.ToString());
                return result;
            }
        }

        private static string Get462PlusFromRegistry()
        {
            try
            {
                return string.Format(".NET Framework Version: " + CheckFor462PlusVersion(GetDotNetReleaseFromRegistry()));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Get462PlusFromRegistry Exception: " + ex.ToString());
                return ".Net Framework not detected!";
            }
        }

        internal static string CheckFor462PlusVersion(int releaseKey)
        {
            if (releaseKey >= 528040)
            {
                return "4.8";
            }
            if (releaseKey >= 461808)
            {
                return "4.7.2";
            }
            if (releaseKey >= 461308)
            {
                return "4.7.1";
            }
            if (releaseKey >= 460798)
            {
                return "4.7";
            }
            if (releaseKey >= 394802)
            {
                return "4.6.2";
            }
            if (releaseKey >= 394254)
            {
                return "4.6.1";
            }
            if (releaseKey >= 393295)
            {
                return "4.6";
            }
            if (releaseKey >= 379893)
            {
                return "4.5.2";
            }
            if (releaseKey >= 378675)
            {
                return "4.5.1";
            }
            if (releaseKey >= 378389)
            {
                return "4.5";
            }
            return "No 4.5 or later version detected";
        }
    }
}
