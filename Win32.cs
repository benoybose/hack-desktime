using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace DeskTime
{
    internal static class Win32
    {
        public delegate bool WindowEnumProc(IntPtr hwnd, IntPtr lparam);

        private struct ScreenRect
        {
            public int left;

            public int top;

            public int right;

            public int bottom;
        }

        private delegate bool MonitorEnumProc(IntPtr hDesktop, IntPtr hdc, ref ScreenRect pRect, int dwData);

        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
            LOGPIXELSY = 90
        }

        public delegate IntPtr DdeDelegate(uint uType, uint uFmt, IntPtr hconv, IntPtr hsz1, IntPtr hsz2, IntPtr hdata, UIntPtr dwData1, UIntPtr dwData2);

        internal struct LASTINPUTINFO
        {
            internal uint cbSize;

            internal uint dwTime;
        }

        internal delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        internal struct RECT
        {
            private int _Left;

            private int _Top;

            private int _Right;

            private int _Bottom;

            internal int X
            {
                get
                {
                    return _Left;
                }
                set
                {
                    _Left = value;
                }
            }

            internal int Y
            {
                get
                {
                    return _Top;
                }
                set
                {
                    _Top = value;
                }
            }

            internal int Left
            {
                get
                {
                    return _Left;
                }
                set
                {
                    _Left = value;
                }
            }

            internal int Top
            {
                get
                {
                    return _Top;
                }
                set
                {
                    _Top = value;
                }
            }

            internal int Right
            {
                get
                {
                    return _Right;
                }
                set
                {
                    _Right = value;
                }
            }

            internal int Bottom
            {
                get
                {
                    return _Bottom;
                }
                set
                {
                    _Bottom = value;
                }
            }

            internal int Height
            {
                get
                {
                    return _Bottom - _Top;
                }
                set
                {
                    _Bottom = value - _Top;
                }
            }

            internal int Width
            {
                get
                {
                    return _Right - _Left;
                }
                set
                {
                    _Right = value + _Left;
                }
            }

            internal Point Location
            {
                get
                {
                    return new Point(Left, Top);
                }
                set
                {
                    _Left = value.X;
                    _Top = value.Y;
                }
            }

            internal Size Size
            {
                get
                {
                    return new Size(Width, Height);
                }
                set
                {
                    _Right = value.Width + _Left;
                    _Bottom = value.Height + _Top;
                }
            }

            internal RECT(Rectangle Rectangle)
                : this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
            {
            }

            internal RECT(int Left, int Top, int Right, int Bottom)
            {
                _Left = Left;
                _Top = Top;
                _Right = Right;
                _Bottom = Bottom;
            }

            public static implicit operator Rectangle(RECT Rectangle)
            {
                return new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
            }

            public static implicit operator RECT(Rectangle Rectangle)
            {
                return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
            }

            public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
            {
                return Rectangle1.Equals(Rectangle2);
            }

            public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
            {
                return !Rectangle1.Equals(Rectangle2);
            }

            public override string ToString()
            {
                return $"{{Left: {_Left}; Top: {_Top}; Right: {_Right}; Bottom: {_Bottom}}}";
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            public bool Equals(RECT Rectangle)
            {
                if (Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right)
                {
                    return Rectangle.Bottom == _Bottom;
                }
                return false;
            }

            public override bool Equals(object Object)
            {
                if (Object is RECT)
                {
                    return Equals((RECT)Object);
                }
                if (Object is Rectangle)
                {
                    return Equals(new RECT((Rectangle)Object));
                }
                return false;
            }
        }

        internal delegate bool EnumWindowsProc(IntPtr hWnd, ref IntPtr lParam);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct PROCESS_BASIC_INFORMATION
        {
            internal uint ExitStatus;

            internal uint PebBaseAddress;

            internal uint AffinityMask;

            internal uint BasePriority;

            internal uint UniqueProcessId;

            internal uint InheritedFromUniqueProcessId;

            internal int Size => 24;
        }

        internal struct UNICODE_STRING
        {
            internal ushort Length;

            internal ushort MaximumLength;

            internal uint Buffer;
        }

        internal enum ProcessAccessFlags : uint
        {
            All = 2035711u,
            Terminate = 1u,
            CreateThread = 2u,
            VMOperation = 8u,
            VMRead = 0x10u,
            VMWrite = 0x20u,
            DupHandle = 0x40u,
            SetInformation = 0x200u,
            QueryInformation = 0x400u,
            Synchronize = 0x100000u
        }

        internal const int SRCCOPY = 13369376;

        internal const int INTERNET_OPTION_END_BROWSER_SESSION = 42;

        internal const int INTERNET_COOKIE_HTTPONLY = 8192;

        internal const int APPCMD_CLIENTONLY = 16;

        internal const int APPCLASS_STANDARD = 0;

        internal const int CP_WINUNICODE = 1200;

        internal const int CF_TEXT = 1;

        internal const int XTYP_REQUEST = 8368;

        internal const int GWL_EXSTYLE = -20;

        internal const int WS_EX_TOOLWINDOW = 128;

        internal const int WS_EX_APPWINDOW = 262144;

        internal const int WH_MOUSE_LL = 14;

        internal const int WH_KEYBOARD_LL = 13;

        internal const int WM_LBUTTONDOWN = 513;

        internal const int WM_RBUTTONDOWN = 516;

        internal const int WM_MBUTTONDOWN = 519;

        internal const int WM_LBUTTONUP = 514;

        internal const int WM_RBUTTONUP = 517;

        internal const int WM_MBUTTONUP = 520;

        internal const int WM_LBUTTONDBLCLK = 515;

        internal const int WM_RBUTTONDBLCLK = 518;

        internal const int WM_MBUTTONDBLCLK = 521;

        internal const int WM_MOUSEWHEEL = 522;

        internal const int WM_KEYDOWN = 256;

        internal const int WM_KEYUP = 257;

        internal const int WM_SYSKEYDOWN = 260;

        internal const int WM_SYSKEYUP = 261;

        internal const int WM_CREATE = 1;

        internal const int WM_DDE_INITIATE = 992;

        internal const int WM_DDE_ACK = 996;

        internal const int SPI_GETSCREENSAVERRUNNING = 114;

        internal const int HWND_BROADCAST = 65535;

        internal const int WM_GETTEXTLENGTH = 14;

        internal const int WM_GETTEXT = 13;

        [DllImport("wininet.dll", CharSet = CharSet.Unicode, EntryPoint = "InternetSetCookieExW", SetLastError = true)]
        public static extern bool InternetSetCookieEx(string lpszURL, string lpszCookieName, string lpszCookieData, uint dwFlags, IntPtr dwReserved);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc callback, IntPtr lParam);

        [DllImport("inetcpl.cpl", SetLastError = true)]
        public static extern int LaunchConnectionDialog(IntPtr hWnd);

        [DllImport("user32")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lpRect, MonitorEnumProc callback, int dwData);

        public static int GetMonitorCount()
        {
            int monCount = 0;
            MonitorEnumProc callback = delegate
            {
                return ++monCount > 0;
            };
            if (EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, 0))
            {
                Console.WriteLine("You have {0} monitors", monCount);
            }
            else
            {
                Console.WriteLine("An error occured while enumerating monitors");
            }
            return monCount;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "DdeInitializeW")]
        internal static extern uint DdeInitialize(ref uint pidInst, DdeDelegate pfnCallback, uint afCmd, uint ulRes);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "DdeCreateStringHandleW")]
        internal static extern IntPtr DdeCreateStringHandle(uint idInst, string psz, int iCodePage);

        [DllImport("user32.dll")]
        internal static extern IntPtr DdeConnect(uint idInst, IntPtr hszService, IntPtr hszTopic, IntPtr pCC);

        [DllImport("user32.dll")]
        internal static extern IntPtr DdeClientTransaction(IntPtr pData, uint cbData, IntPtr hConv, IntPtr hszItem, uint wFmt, uint wType, uint dwTimeout, ref uint pdwResult);

        [DllImport("user32.dll")]
        internal static extern bool DdeFreeStringHandle(uint idInst, IntPtr hsz);

        [DllImport("user32.dll")]
        internal static extern uint DdeGetData(IntPtr hData, [Out] byte[] pDst, uint cbMax, uint cbOff);

        [DllImport("user32.dll")]
        internal static extern bool DdeDisconnect(IntPtr hConv);

        [DllImport("user32.dll")]
        internal static extern bool DdeUninitialize(uint idInst);

        [DllImport("user32.dll")]
        internal static extern bool DdeFreeDataHandle(IntPtr hData);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr window, int index, int value);

        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr window, int index);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowTextLengthW", SetLastError = true)]
        internal static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetWindowTextW")]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetClassNameW", SetLastError = true)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendNotifyMessageW", SetLastError = true)]
        internal static extern int SendNotifyMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "FindWindowExW", SetLastError = true)]
        internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegisterWindowMessageW", SetLastError = true)]
        internal static extern int RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SystemParametersInfo(int uAction, int uParam, ref bool lpvParam, int flags);

        [DllImport("user32.dll")]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsIconic(IntPtr hWnd);

        [DllImport("ntdll.dll")]
        internal static extern int NtQueryInformationProcess(IntPtr hProcess, uint pic, out PROCESS_BASIC_INFORMATION pbi, int cb, out int pSize);

        [DllImport("Kernel32.dll")]
        internal static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, [In][Out] ref uint lpdwSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, [Out] IntPtr lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("user32.dll")]
        internal static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int UnhookWindowsHookEx(int idHook);

        [DllImport("User32.dll")]
        internal static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW", SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("wininet.dll", SetLastError = true)]
        internal static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern bool DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern ushort GlobalAddAtom(string lpString);
    }
}
