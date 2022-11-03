using System;
using System.Diagnostics;

namespace DeskTime
{
    internal static class RealProcess
    {
        public static Process realProcess;

        public static int GetWindowProcessId(IntPtr hwnd)
        {
            Win32.GetWindowThreadProcessId(hwnd, out var lpdwProcessId);
            return lpdwProcessId;
        }

        public static IntPtr GetforegroundWindow()
        {
            return Win32.GetForegroundWindow();
        }

        public static Process GetRealProcess(Process foregroundProcess)
        {
            Win32.EnumChildWindows(foregroundProcess.MainWindowHandle, ChildWindowCallback, IntPtr.Zero);
            return realProcess;
        }

        public static bool ChildWindowCallback(IntPtr hwnd, IntPtr lparam)
        {
            Process processById = Process.GetProcessById(GetWindowProcessId(hwnd));
            if (processById.ProcessName != "ApplicationFrameHost")
            {
                realProcess = processById;
            }
            return true;
        }
    }
}
