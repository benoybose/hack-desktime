#define TRACE
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Windows.Forms;
using DeskTime.Properties;
using DeskTime.Sources;
using Microsoft.Win32;
using static DeskTime.Sources.EncodeDecodeString;

namespace DeskTime
{
    internal class MainWin : Form
    {
        public struct POINT
        {
            public int X;

            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        internal struct Win32LastInputInfo
        {
            internal uint cbSize;

            internal uint dwTime;

            internal static readonly int SizeOf = Marshal.SizeOf(typeof(Win32LastInputInfo));
        }

        private const int TIME_IDLE = 3;

        private const int MAX_MEMREC = 500;

        private const int TIME_CYCLE = 7;

        private const int TIME_OFFSLEEP = 62;

        private const int TIME_ALIVE = 60;

        public static bool DEV;

        public static readonly Version Version;

        private readonly object mutex = new object();

        private string currentBrowserUrl = "";

        private LogDataList dataList = new LogDataList();

        private static readonly RSACryptoServiceProvider RsaKey;

        public static string Backtrace;

        public static int ProxyStyle;

        public static string ProxyAddress;

        public static int ProxyPort;

        public static string ProxyUser;

        public static string ProxyPassword;

        private DTLogData lastWin;

        public createProjectWin cf;

        public ProjectWindow projWin;

        public Onboarding onboardingForm;

        public bool projWinShown;

        private volatile string session;

        public static int ActiveUserId;

        private volatile bool mustRelogin;

        private readonly bool isSilentVersion;

        private readonly bool isMsi;

        private readonly string authProvider;

        private volatile bool skipIntro;

        private volatile bool terminalServices;

        private volatile bool inOffice = true;

        private volatile bool isTracking = true;

        private volatile bool ipLimited;

        private volatile bool isOnline = true;

        private volatile bool isScreenlock;

        private volatile bool isSlacking;

        private volatile bool allowScreenCapture;

        private volatile int screenCaptureTimeout = 600;

        private volatile int screenCaptureRandomTime = 300;

        private Random screenCaptureRand = new Random();

        private System.Timers.Timer screenCaptureTimer = new System.Timers.Timer();

        private long lastScreenCapture = DateTime.UtcNow.ConvertToUnixTimestamp();

        private volatile int screenCaptureInterval;

        private volatile int noop;

        private int secondsFromStart;

        private int timeCycle = 7;

        public long launchTime = DateTime.UtcNow.ConvertToUnixTimestamp();

        private volatile int idleTimeInterval = 3;

        private bool disablePrivateTime;

        private volatile int privateTimeRemind = 15;

        private DateTime privateTimeStarted = DateTime.UtcNow;

        private bool ignorePrivateTimeFromServer;

        private volatile int breakRemind;

        private DateTime breakRemindStarted = DateTime.UtcNow;

        private bool disableWindowTitle;

        private bool disableMouseClick;

        private bool disableMouseMovement;

        private bool disableLogOut = true;

        public int currentProjectId;

        public int currentProjectTime;

        public string currentProjectName;

        public string lastProjectName;

        public string currentTaskName;

        public string lastTaskName;

        public int lastProjectUiUpdate;

        public int lastAppSeconds;

        public int recentProjectsUpdate = -1;

        public int lastProjectListUpdate;

        public List<ProjectItem> allProjects = new List<ProjectItem>();

        public List<ProjectItem> recentProjects = new List<ProjectItem>();

        private bool balloonTrShown;

        private bool balloonPmShown;

        private bool idleRegistered;

        private bool restartRegistered;

        private static DateTime LastInput;

        private static string LastInputBecause;

        private static readonly object LastInputLock;

        private readonly object notifyMenulock = new object();

        private long lastMonday;

        private int mousehook;

        private int mouseDiff = 5;

        private int keyhook;

        private Win32.HookProc hookProcDelegate;

        private IntPtr hModule;

        private IntPtr MainWinHandle = (IntPtr)0;

        public static RESTHosts hosts;

        private volatile Browsers browsers;

        private long nextUpdate;

        private long currentInputTime;

        private long lastInputTime;

        private string prev_error = "";

        public static bool fatalError;

        private byte[] prevKeyState = new byte[256];

        private byte[] curKeyState = new byte[256];

        private POINT prevMousePos;

        private POINT curMousePos;

        private List<DTLogData> _zwindata = new List<DTLogData>();

        private int NextItemId;

        private int loginWinHeight;

        private int loginWinWidth;

        private IContainer components;

        private NotifyIcon notifyIcon;

        private ContextMenuStrip notifyMenu;

        private ToolStripMenuItem statusToolStripMenuItem;

        private ToolStripMenuItem exitToolStripMenuItem;

        private Panel browserCancelPanel;

        private Button browserCancelButton;

        private ToolStripMenuItem userToolStripMenuItem;

        private ToolStripMenuItem currentProjectMenuItem;

        private ToolStripSeparator stopProjectSep;

        private ToolStripMenuItem viewStatisticsToolStripMenuItem;

        private ToolStripSeparator toolStripSeparator2;

        private ToolStripMenuItem logoutToolStripMenuItem;

        private ToolStripMenuItem stopProjectMenuItem;

        private ToolStripMenuItem pauseToolStripMenuItem;

        private System.Windows.Forms.Timer timerHooker;

        private System.Windows.Forms.Timer timerUp;

        private ToolStripMenuItem settingsToolStripMenuItem;

        private ToolStripSeparator toolStripSeparator3;

        private ToolStripMenuItem checkForUpdateToolStripMenuItem;

        private System.Windows.Forms.Timer timer;

        private ToolStripMenuItem startWithWindowsToolStripMenuItem;

        private ToolStripMenuItem internetPropertiesToolStripMenuItem;

        internal ToolStripMenuItem hideTimeWindowToolStripMenuItem;

        private ToolStripMenuItem proxySettingsToolStripMenuItem;

        private ToolStripMenuItem privateTimeReminder;

        private ToolStripMenuItem privateTimeRemind15;

        private ToolStripMenuItem privateTimeRemind30;

        private ToolStripMenuItem privateTimeRemind45;

        private ToolStripMenuItem privateTimeRemindDisable;

        private ToolStripMenuItem devVersionToolStripMenuItem;

        private ToolStripSeparator toolStripSeparator1;

        private ToolStripMenuItem createNewProjectMenuItem;

        private ToolStripMenuItem toolStripMenuRecentProjects;

        private ToolStripSeparator toolStripSeparator4;

        private ToolStripSeparator privateTimeToolStripSeparator;

        private ToolStripMenuItem pingToolStripMenuItem;

        private ToolStripMenuItem reminderToTakeBreak;

        private ToolStripMenuItem reminderToTakeBreak52;

        private ToolStripMenuItem reminderToTakeBreak60;

        private ToolStripMenuItem reminderToTakeBreak90;

        private ToolStripMenuItem reminderToTakeBreakDisabled;

        private ToolStripMenuItem aboutDeskTimeClientToolStripMenuItem;

        private Panel loginPanel;

        private Button dt_login;

        private Button gg_login;

        private Button linkedin_login;

        private Button native_login;

        private LinkLabel forgotPassword;

        private TextBox nativeEmail;

        private TextBox nativePassword;

        private Label label2;

        private Label label1;

        private Button twi_login;

        private Button fb_login;

        private Panel IE_panel;

        private System.Windows.Forms.Timer timerMagicLink;

        private ToolStripMenuItem loginToolStripMenuItem;

        internal int WmDesktime { get; set; }

        private bool ScreenSaverActive => Tools.GetScreenSaverActive();

        private TimeSpan IdleTime
        {
            get
            {
                lock (LastInputLock)
                {
                    return DateTime.UtcNow - LastInput;
                }
            }
        }

        static MainWin()
        {
            DEV = false;
            Backtrace = "";
            ProxyStyle = 1;
            ProxyAddress = "";
            ProxyPort = 0;
            ProxyUser = "";
            ProxyPassword = "";
            ActiveUserId = 0;
            LastInputBecause = "";
            LastInputLock = new object();
            fatalError = false;
            Version = new Version("1.3.613");
            RsaKey = new RSACryptoServiceProvider();
            RsaKey.FromXmlString("<RSAKeyValue><Modulus>rAdx5xSvGU9amQquWyJELRbwk9NnZAbloL0/eoC+hefnOkXpjWya9mlF8HhE3Nnmj/1FBnt6FFGI/X6+nZPF/fa6ZarTY9cIc6hHTSL8b70trgJ91LBBP+eW96+rOGxlRyvwOL+K+p+SDIlIzpvevhE90B26kG9qNxvNFJcxryU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
            LastInput = DateTime.UtcNow;
            LastInputBecause = "";
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff "));
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Operating system version: " + DeviceInfo.OSVersion);
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Operating system name: " + DeviceInfo.OsName);
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Computer name: " + DeviceInfo.MachineName);
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "User name: " + DeviceInfo.UserName);
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "CLR runtime version: " + DeviceInfo.Version);
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Command line: " + DeviceInfo.CommandLine);
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Args: " + Environment.GetCommandLineArgs().Length);
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Arg 1: " + Environment.GetCommandLineArgs()[1]);
            }
        }

        internal MainWin()
        {
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "MainWin: DeskTime " + Version.ToString());
            InitializeComponent();
            projWin = new ProjectWindow();
            projWin.parentMainForm = this;
            projWin.update_time_Tick(null, null);
            projWin.Visible = false;
            Text = "DeskTime " + Version.ToString();
            terminalServices = SystemInformation.TerminalServerSession;
            RegistryService.MigrateRegistry(Application.UserAppDataRegistry);
            RegistryService.SetValue("lastRun", DateTime.Today.Date.ConvertToUnixTimestamp());
            RegistryService.SetValue("version", Version.ToString());
            if (RegistryService.GetValue("dev", "0") as string != "0")
            {
                DEV = true;
            }
            browsers = new Browsers();
            isSilentVersion = RegistryService.GetValue("silent", "0") as string != "0";
            mustRelogin = RegistryService.GetValue("relogin", "0") as string != "0";
            isMsi = RegistryService.GetValue("msi", "0") as string != "0";
            skipIntro = RegistryService.GetValue("skipIntro", "0") as string != "0";
            privateTimeRemind = int.Parse(RegistryService.GetValue("privateTimeRemind", "15") as string);
            breakRemind = int.Parse(RegistryService.GetValue("breakRemind", "0") as string);
            authProvider = RegistryService.GetValue("authProvider", "") as string;
            RegistryKey registryKey = RegistryService.OpenSubKey("Proxy");
            if (registryKey == null)
            {
                registryKey = RegistryService.CreateSubKey("Proxy");
            }
            ProxyStyle = int.Parse(registryKey.GetValue("proxystyle", "1") as string);
            ProxyAddress = registryKey.GetValue("proxyaddress", "") as string;
            ProxyPort = int.Parse(registryKey.GetValue("proxyport", "0") as string);
            ProxyUser = registryKey.GetValue("proxyuser", "") as string;
            ProxyPassword = registryKey.GetValue("proxypassword", "") as string;
            registryKey.Close();
            try
            {
                NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "NetworkChange Exception: " + ex.ToString());
            }
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            base.WindowState = FormWindowState.Minimized;
            base.Visible = false;
            base.ShowInTaskbar = false;
            int windowLong = Win32.GetWindowLong(base.Handle, -20);
            Win32.SetWindowLong(base.Handle, -20, windowLong | 0x80);
            try
            {
                lastMonday = Convert.ToInt64(RegistryService.GetValue("lastMonday", 0));
            }
            catch (Exception ex2)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "lastMonday Exception: " + ex2.ToString());
                lastMonday = 0L;
            }
            if (!isSilentVersion)
            {
                notifyIcon.Visible = true;
            }
            hosts = new RESTHosts(DEV);
            hosts.CheckServersAvailability(0L);
        }

        private void MainWin_Load(object sender, EventArgs e)
        {
            MagicLogin();
            object value = RegistryService.GetValue("Session");
            if (DEV)
            {
                devVersionToolStripMenuItem.Text = "DeskTime " + Version.ToString();
                devVersionToolStripMenuItem.Available = true;
            }
            InfoIcon();
            hookProcDelegate = HookProc;
            using (Process process = Process.GetCurrentProcess())
            {
                hModule = Win32.GetModuleHandle(process.MainModule.ModuleName);
                process.MainModule.Dispose();
            }
            timerHookerTick(null, EventArgs.Empty);
            timerHooker.Enabled = true;
            Thread thread = new Thread(KeepTime);
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
            Thread thread2 = new Thread(TrackingData);
            thread2.SetApartmentState(ApartmentState.STA);
            thread2.IsBackground = true;
            thread2.Start();
            Thread thread3 = new Thread(DataSender);
            thread3.SetApartmentState(ApartmentState.STA);
            thread3.IsBackground = true;
            thread3.Start();
            timerUpTick(null, EventArgs.Empty);
            timerMagicLink.Enabled = true;
            if (value != null)
            {
                session = value.ToString();
                string text = RegistryService.GetValue("Name") as string;
                if (text != null)
                {
                    userToolStripMenuItem.Text = string.Format(Resources.UserName, text);
                    userToolStripMenuItem.Available = true;
                }
                if (!skipIntro)
                {
                    skipIntro = true;
                }
            }
            else
            {
                ClearSession();
            }
        }

        private string StringFromSecureString(SecureString secureString)
        {
            IntPtr intPtr = default(IntPtr);
            try
            {
                char[] array = new char[secureString.Length];
                intPtr = Marshal.SecureStringToCoTaskMemUnicode(secureString);
                Marshal.Copy(intPtr, array, 0, secureString.Length);
                return new string(array);
            }
            finally
            {
                Marshal.ZeroFreeCoTaskMemUnicode(intPtr);
            }
        }

        [Obsolete("InitScreenCapture is deprecated!!!")]
        private void InitScreenCapture()
        {
            //screenCaptureTimer.Elapsed += screenCaptureTimerEventProcessor;
            screenCaptureInterval = screenCaptureRand.Next(30, 90);
            if (DEV)
            {
                screenCaptureInterval = 5;
            }
            int num = 5000;
            screenCaptureTimer.Interval = num;
            screenCaptureTimer.Enabled = true;
            screenCaptureTimer.AutoReset = true;
            screenCaptureTimer.Start();
        }

        //[Obsolete("screenCaptureTimerEventProcessor is deprecated!!!")]
        //public void screenCaptureTimerEventProcessor(object myObject, EventArgs myEventArgs)
        //{
        //    bool flag = isInActiveMode();
        //    int fortimerinterval = 5000;
        //    screenCaptureTimer.Stop();
        //    screenCaptureTimer.Interval = fortimerinterval;
        //    if (lastScreenCapture + screenCaptureInterval >= DateTime.UtcNow.ConvertToUnixTimestamp())
        //    {
        //        screenCaptureTimer.Start();
        //        return;
        //    }
        //    if (allowScreenCapture && flag && !isScreenlock && MustTracking())
        //    {
        //        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Start screen capture");
        //        try
        //        {
        //            lastScreenCapture += 60L;
        //            DTLogData dTLogData = null;
        //            int monitorCount = Win32.GetMonitorCount();
        //            IList<byte[]> list = new List<byte[]>();
        //            for (int i = 0; i < monitorCount; i++)
        //            {
        //                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Monitor: " + i);
        //                using MemoryStream memoryStream = new MemoryStream();
        //                Image image;
        //                double num;
        //                try
        //                {
        //                    image = Tools.CaptureScreen(i);
        //                    num = 1024.0 / (double)image.Width;
        //                }
        //                catch (Exception)
        //                {
        //                    image = new Bitmap(1, 1);
        //                    num = 1.0;
        //                }
        //                int num2 = (int)((double)image.Width * num);
        //                int num3 = (int)((double)image.Height * num);
        //                using (Bitmap bitmap = new Bitmap(num2, num3))
        //                {
        //                    using Graphics graphics = Graphics.FromImage(bitmap);
        //                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
        //                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        //                    graphics.DrawImage(image, new Rectangle(0, 0, num2, num3));
        //                    bitmap.Save(memoryStream, ImageFormat.Jpeg);
        //                    list.Add(memoryStream.ToArray());
        //                    bitmap.Dispose();
        //                    graphics.Dispose();
        //                }
        //                memoryStream.Dispose();
        //                image.Dispose();
        //            }
        //            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Get screenshots activeApp - started!");
        //            IntPtr foregroundWindow = Win32.GetForegroundWindow();
        //            string windowString = Tools.GetWindowString(foregroundWindow);
        //            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "title: " + windowString);
        //            int lpdwProcessId = 0;
        //            Win32.GetWindowThreadProcessId(foregroundWindow, out lpdwProcessId);
        //            try
        //            {
        //                Process processById;
        //                Process process = (processById = Process.GetProcessById(lpdwProcessId));
        //                if (process.ProcessName == "ApplicationFrameHost")
        //                {
        //                    process = RealProcess.GetRealProcess(processById);
        //                }
        //                using (process)
        //                {
        //                    try
        //                    {
        //                        dTLogData = GetWindowData(foregroundWindow, lpdwProcessId, process, windowString, processById);
        //                    }
        //                    catch (Exception ex2)
        //                    {
        //                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "where is acitve app??? " + ex2.ToString());
        //                    }
        //                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "activeApp: " + dTLogData.App);
        //                }
        //            }
        //            catch (Exception ex3)
        //            {
        //                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "proc Exception: " + ex3.ToString());
        //            }
        //            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Get screenshots activeApp - finished!");
        //            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Get screenshots BACKGROUND APPS - started!");
        //            try
        //            {
        //                PrepareDisplayWinData();
        //            }
        //            catch (Exception ex4)
        //            {
        //                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "proc Exception: " + ex4.ToString());
        //            }
        //            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Get screenshots BACKGROUND APPS - ended!");
        //            int num4 = 0;
        //            foreach (byte[] item in list)
        //            {
        //                num4++;
        //                NameValueCollection nameValueCollection = new NameValueCollection();
        //                nameValueCollection.Add("app_id", "22");
        //                nameValueCollection.Add("now", DateTime.UtcNow.ConvertToUnixTimestamp().ToString());
        //                nameValueCollection.Add("launch", launchTime.ToString());
        //                nameValueCollection.Add("version", Version.ToString() + (isSilentVersion ? " s" : "") + (isMsi ? " m" : ""));
        //                nameValueCollection.Add("type", "DESKTOP");
        //                nameValueCollection.Add("name", HttpUtility.UrlEncode(DeviceInfo.MachineName));
        //                nameValueCollection.Add("seconds", secondsFromStart.ToString());
        //                nameValueCollection.Add("monitor", num4.ToString());
        //                nameValueCollection.Add("session", session);
        //                nameValueCollection.Add("epr", HttpUtility.UrlEncode(hosts.ActiveEndPointRegion()));
        //                if (dTLogData != null)
        //                {
        //                    nameValueCollection.Add("log[app]", dTLogData.App);
        //                    nameValueCollection.Add("log[win]", HttpUtility.UrlEncode(dTLogData.Text));
        //                    if (!string.IsNullOrEmpty(dTLogData.Url))
        //                    {
        //                        nameValueCollection.Add("log[url]", dTLogData.Url);
        //                    }
        //                }
        //                if (_zwindata.Count > 0)
        //                {
        //                    for (int j = 0; j < _zwindata.Count; j++)
        //                    {
        //                        if (_zwindata[j].Visible && !string.IsNullOrEmpty(_zwindata[j].Text))
        //                        {
        //                            if (!string.IsNullOrEmpty(_zwindata[j].App))
        //                            {
        //                                nameValueCollection.Add("apps[" + j + "][app]", _zwindata[j].App);
        //                            }
        //                            if (!string.IsNullOrEmpty(_zwindata[j].Text))
        //                            {
        //                                nameValueCollection.Add("apps[" + j + "][win]", HttpUtility.UrlEncode(_zwindata[j].Text));
        //                            }
        //                            if (!string.IsNullOrEmpty(_zwindata[j].Url))
        //                            {
        //                                nameValueCollection.Add("apps[" + j + "][url]", _zwindata[j].Url);
        //                            }
        //                        }
        //                    }
        //                }
        //                Uri uri = new Uri(string.Format(hosts.ActiveApiEndPoint() + "screenshot"));
        //                try
        //                {
        //                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "RESTService.UploadFileResponse starts");
        //                    RESTService.UploadFileResponse(uri, nameValueCollection, item, AppDomain.CurrentDomain.BaseDirectory + "\\screenshot.jpg", "image/jpeg", delegate (Response Response)
        //                    {
        //                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "RESTService.UploadFileResponse callback");
        //                        if (Response == null)
        //                        {
        //                            fortimerinterval = 180000;
        //                            screenCaptureTimer.Interval = fortimerinterval;
        //                            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + " No response! Next retray: " + fortimerinterval);
        //                            prev_error = "Error: Screenshot Exception 0: No server response!";
        //                        }
        //                        else if (Response.Error != null)
        //                        {
        //                            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Error: " + Response.Error.Code + " " + Response.Error.Description);
        //                            fortimerinterval = 180000;
        //                            screenCaptureTimer.Interval = fortimerinterval;
        //                            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + " Response error! Next retray: " + fortimerinterval);
        //                            prev_error = "Error: Screenshot Exception 0: Error: " + Response.Error.Code + " " + Response.Error.Description;
        //                            if (!(Response.Error.Code == "401"))
        //                            {
        //                                isOnline = false;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            isOnline = true;
        //                            lastScreenCapture = DateTime.UtcNow.ConvertToUnixTimestamp();
        //                            screenCaptureInterval = screenCaptureRand.Next(screenCaptureTimeout, screenCaptureTimeout + screenCaptureRandomTime);
        //                            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Screenshot created! Next sceeenshot after " + screenCaptureInterval + "s");
        //                        }
        //                        screenCaptureTimer.Start();
        //                    }).ConfigureAwait(continueOnCapturedContext: false);
        //                }
        //                catch (Exception ex5)
        //                {
        //                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Screenshot Exception 1: " + ex5.ToString());
        //                    prev_error = "Error: Screenshot Exception 1: " + ex5.ToString();
        //                }
        //            }
        //        }
        //        catch (InvalidOperationException ex6)
        //        {
        //            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Screenshot InvalidOperationException: " + ex6.ToString());
        //            prev_error = "Error Screenshot InvalidOperationException: " + ex6.ToString();
        //        }
        //        catch (Exception ex7)
        //        {
        //            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Screenhot Exception 2: " + ex7.ToString());
        //            prev_error = "Error: Screenhot Exception 2: " + ex7.ToString();
        //        }
        //        finally
        //        {
        //            if (prev_error != "")
        //            {
        //                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "prev_error: " + prev_error);
        //            }
        //        }
        //    }
        //    screenCaptureTimer.Start();
        //}

        //public void ScreenCapture()
        //{
        //    bool flag = isInActiveMode();
        //    int fortimerinterval = 5000;
        //    if (lastScreenCapture + screenCaptureInterval >= DateTime.UtcNow.ConvertToUnixTimestamp() || !(allowScreenCapture && flag) || isScreenlock || !MustTracking())
        //    {
        //        return;
        //    }
        //    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Start screen capture");
        //    try
        //    {
        //        lastScreenCapture += 60L;
        //        DTLogData dTLogData = null;
        //        int monitorCount = Win32.GetMonitorCount();
        //        IList<byte[]> list = new List<byte[]>();
        //        for (int i = 0; i < monitorCount; i++)
        //        {
        //            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Monitor: " + i);
        //            using MemoryStream memoryStream = new MemoryStream();
        //            Image image;
        //            double num;
        //            try
        //            {
        //                image = Tools.CaptureScreen(i);
        //                num = 1024.0 / (double)image.Width;
        //            }
        //            catch (Exception)
        //            {
        //                image = new Bitmap(1, 1);
        //                num = 1.0;
        //            }
        //            int num2 = (int)((double)image.Width * num);
        //            int num3 = (int)((double)image.Height * num);
        //            using (Bitmap bitmap = new Bitmap(num2, num3))
        //            {
        //                using Graphics graphics = Graphics.FromImage(bitmap);
        //                graphics.SmoothingMode = SmoothingMode.AntiAlias;
        //                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        //                graphics.DrawImage(image, new Rectangle(0, 0, num2, num3));
        //                bitmap.Save(memoryStream, ImageFormat.Jpeg);
        //                list.Add(memoryStream.ToArray());
        //                bitmap.Dispose();
        //                graphics.Dispose();
        //            }
        //            memoryStream.Dispose();
        //            image.Dispose();
        //        }
        //        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Get screenshots activeApp - started!");
        //        IntPtr foregroundWindow = Win32.GetForegroundWindow();
        //        string windowString = Tools.GetWindowString(foregroundWindow);
        //        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "title: " + windowString);
        //        int lpdwProcessId = 0;
        //        Win32.GetWindowThreadProcessId(foregroundWindow, out lpdwProcessId);
        //        try
        //        {
        //            Process processById;
        //            Process process = (processById = Process.GetProcessById(lpdwProcessId));
        //            if (process.ProcessName == "ApplicationFrameHost")
        //            {
        //                process = RealProcess.GetRealProcess(processById);
        //            }
        //            using (process)
        //            {
        //                try
        //                {
        //                    dTLogData = GetWindowData(foregroundWindow, lpdwProcessId, process, windowString, processById);
        //                }
        //                catch (Exception ex2)
        //                {
        //                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "where is acitve app??? " + ex2.ToString());
        //                }
        //                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "activeApp: " + dTLogData.App);
        //            }
        //        }
        //        catch (Exception ex3)
        //        {
        //            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "proc Exception: " + ex3.ToString());
        //        }
        //        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Get screenshots activeApp - finished!");
        //        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Get screenshots BACKGROUND APPS - started!");
        //        try
        //        {
        //            PrepareDisplayWinData();
        //        }
        //        catch (Exception ex4)
        //        {
        //            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "proc Exception: " + ex4.ToString());
        //        }
        //        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Get screenshots BACKGROUND APPS - ended!");
        //        int num4 = 0;
        //        foreach (byte[] item in list)
        //        {
        //            num4++;
        //            NameValueCollection nameValueCollection = new NameValueCollection();
        //            nameValueCollection.Add("app_id", "22");
        //            nameValueCollection.Add("now", DateTime.UtcNow.ConvertToUnixTimestamp().ToString());
        //            nameValueCollection.Add("launch", launchTime.ToString());
        //            nameValueCollection.Add("version", Version.ToString() + (isSilentVersion ? " s" : "") + (isMsi ? " m" : ""));
        //            nameValueCollection.Add("type", "DESKTOP");
        //            nameValueCollection.Add("name", HttpUtility.UrlEncode(DeviceInfo.MachineName));
        //            nameValueCollection.Add("seconds", secondsFromStart.ToString());
        //            nameValueCollection.Add("monitor", num4.ToString());
        //            nameValueCollection.Add("session", session);
        //            nameValueCollection.Add("epr", HttpUtility.UrlEncode(hosts.ActiveEndPointRegion()));
        //            if (dTLogData != null)
        //            {
        //                nameValueCollection.Add("log[app]", dTLogData.App);
        //                nameValueCollection.Add("log[win]", HttpUtility.UrlEncode(dTLogData.Text));
        //                if (!string.IsNullOrEmpty(dTLogData.Url))
        //                {
        //                    nameValueCollection.Add("log[url]", dTLogData.Url);
        //                }
        //            }
        //            if (_zwindata.Count > 0)
        //            {
        //                for (int j = 0; j < _zwindata.Count; j++)
        //                {
        //                    if (_zwindata[j].Visible && !string.IsNullOrEmpty(_zwindata[j].Text))
        //                    {
        //                        if (!string.IsNullOrEmpty(_zwindata[j].App))
        //                        {
        //                            nameValueCollection.Add("apps[" + j + "][app]", _zwindata[j].App);
        //                        }
        //                        if (!string.IsNullOrEmpty(_zwindata[j].Text))
        //                        {
        //                            nameValueCollection.Add("apps[" + j + "][win]", HttpUtility.UrlEncode(_zwindata[j].Text));
        //                        }
        //                        if (!string.IsNullOrEmpty(_zwindata[j].Url))
        //                        {
        //                            nameValueCollection.Add("apps[" + j + "][url]", _zwindata[j].Url);
        //                        }
        //                    }
        //                }
        //            }
        //            Uri uri = new Uri(string.Format(hosts.ActiveApiEndPoint() + "screenshot"));
        //            try
        //            {
        //                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "RESTService.UploadFileResponse starts");
        //                RESTService.UploadFileResponse(uri, nameValueCollection, item, AppDomain.CurrentDomain.BaseDirectory + "\\screenshot.jpg", "image/jpeg", delegate (Response Response)
        //                {
        //                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "RESTService.UploadFileResponse callback");
        //                    if (Response == null)
        //                    {
        //                        fortimerinterval = 180000;
        //                        screenCaptureTimer.Interval = fortimerinterval;
        //                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + " No response! Next retray: " + fortimerinterval);
        //                        prev_error = "Error: Screenshot Exception 0: No server response!";
        //                    }
        //                    else if (Response.Error != null)
        //                    {
        //                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Error: " + Response.Error.Code + " " + Response.Error.Description);
        //                        fortimerinterval = 180000;
        //                        screenCaptureTimer.Interval = fortimerinterval;
        //                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + " Response error! Next retray: " + fortimerinterval);
        //                        prev_error = "Error: Screenshot Exception 0: Error: " + Response.Error.Code + " " + Response.Error.Description;
        //                        if (!(Response.Error.Code == "401"))
        //                        {
        //                            isOnline = false;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        isOnline = true;
        //                        lastScreenCapture = DateTime.UtcNow.ConvertToUnixTimestamp();
        //                        screenCaptureInterval = screenCaptureRand.Next(screenCaptureTimeout, screenCaptureTimeout + screenCaptureRandomTime);
        //                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Screenshot created! Next sceeenshot after " + screenCaptureInterval + "s");
        //                    }
        //                    screenCaptureTimer.Start();
        //                }).ConfigureAwait(continueOnCapturedContext: false);
        //            }
        //            catch (Exception ex5)
        //            {
        //                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Screenshot Exception 1: " + ex5.ToString());
        //                prev_error = "Error: Screenshot Exception 1: " + ex5.ToString();
        //            }
        //        }
        //    }
        //    catch (InvalidOperationException ex6)
        //    {
        //        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Screenshot InvalidOperationException: " + ex6.ToString());
        //        prev_error = "Error Screenshot InvalidOperationException: " + ex6.ToString();
        //    }
        //    catch (Exception ex7)
        //    {
        //        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Screenhot Exception 2: " + ex7.ToString());
        //        prev_error = "Error: Screenhot Exception 2: " + ex7.ToString();
        //    }
        //    finally
        //    {
        //        if (prev_error != "")
        //        {
        //            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "prev_error: " + prev_error);
        //        }
        //    }
        //}

        private void MainWin_FormClosed(object sender, FormClosedEventArgs e)
        {
            StackTrace stackTrace = new StackTrace(fNeedFileInfo: true);
            Backtrace = "cl: " + stackTrace.ToString() + "\r\n msg: " + WmDesktime + "\r\n proxy: " + ProxyStyle;
            if (mousehook != 0)
            {
                Win32.UnhookWindowsHookEx(mousehook);
            }
            if (keyhook != 0)
            {
                Win32.UnhookWindowsHookEx(keyhook);
            }
        }

        private void MainWin_Resize(object sender, EventArgs e)
        {
            if (base.WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void ResetBreakRemindStarted()
        {
            if (pauseToolStripMenuItem.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText)
            {
                Trace.WriteLine("Reset breakRemindStarted (is private time): " + breakRemindStarted.ToString());
                breakRemindStarted = DateTime.UtcNow;
            }
            if (IdleTime.TotalMinutes > (double)(idleTimeInterval + 1))
            {
                breakRemindStarted = DateTime.UtcNow;
            }
            else if (isScreenlock && IdleTime.TotalMinutes > (double)(idleTimeInterval - 2))
            {
                breakRemindStarted = DateTime.UtcNow;
            }
        }

        private int HookProc(int nCode, int wParam, IntPtr lParam)
        {
            switch (wParam)
            {
                case 513:
                    if (disableMouseClick)
                    {
                        break;
                    }
                    goto case 257;
                case 515:
                    if (disableMouseClick)
                    {
                        break;
                    }
                    goto case 257;
                case 516:
                    if (disableMouseClick)
                    {
                        break;
                    }
                    goto case 257;
                case 518:
                    if (disableMouseClick)
                    {
                        break;
                    }
                    goto case 257;
                case 522:
                    if (disableMouseClick)
                    {
                        break;
                    }
                    goto case 257;
                case 519:
                    if (disableMouseClick)
                    {
                        break;
                    }
                    goto case 257;
                case 521:
                    if (disableMouseClick)
                    {
                        break;
                    }
                    goto case 257;
                case 257:
                case 261:
                    lock (LastInputLock)
                    {
                        LastInput = DateTime.UtcNow;
                        LastInputBecause = "hook: " + wParam;
                        currentInputTime = DateTime.UtcNow.ConvertToUnixTimestamp();
                    }
                    break;
            }
            return Win32.CallNextHookEx(0, nCode, wParam, lParam);
        }

        private void ClearSession(bool showLogin = true)
        {
            session = null;
            launchTime = DateTime.UtcNow.ConvertToUnixTimestamp();
            secondsFromStart = 0;
            RegistryService.DeleteValue("Name", throwOnMissingValue: false);
            RegistryService.DeleteValue("Session", throwOnMissingValue: false);
            if (!isSilentVersion && !mustRelogin)
            {
                RegistryService.DeleteValue("u", throwOnMissingValue: false);
                RegistryService.DeleteValue("p", throwOnMissingValue: false);
            }
            RegistryService.DeleteValue("relogin", throwOnMissingValue: false);
            RegistryService.DeleteValue("projects_update", throwOnMissingValue: false);
            Win32.InternetSetCookieEx("http://facebook.com", "xs", "\0", 8192u, IntPtr.Zero);
            Win32.InternetSetCookieEx("http://twitter.com", "_twitter_sess", "\0", 8192u, IntPtr.Zero);
            Win32.InternetSetCookieEx("http://linkedin.com", "leo_auth_token", "\0", 8192u, IntPtr.Zero);
            Win32.InternetSetCookieEx("http://google.com", "SID", "\0", 8192u, IntPtr.Zero);
            Win32.InternetSetOption(IntPtr.Zero, 42, IntPtr.Zero, 0);
            recentProjectsUpdate = -1;
            currentProjectName = null;
            lastProjectName = null;
            currentProjectId = 0;
            currentProjectTime = 0;
            currentTaskName = null;
            lastTaskName = null;
            setProject();
            if (projWin.InvokeRequired)
            {
                projWin.Invoke((Action<Form>)delegate (Form formInstance)
                {
                    formInstance.Hide();
                }, projWin);
            }
            else
            {
                projWin.Hide();
            }
            projWinShown = false;
            if (base.InvokeRequired)
            {
                Invoke((Action<MainWin>)delegate (MainWin instance)
                {
                    instance.logoutToolStripMenuItem.Enabled = true;
                    instance.exitToolStripMenuItem.Enabled = true;
                    instance.userToolStripMenuItem.Text = "";
                    instance.userToolStripMenuItem.Available = false;
                }, this);
            }
            else
            {
                logoutToolStripMenuItem.Enabled = true;
                exitToolStripMenuItem.Enabled = true;
                userToolStripMenuItem.Text = "";
                userToolStripMenuItem.Available = false;
            }
            if (showLogin)
            {
                PopUp();
            }
            InfoIcon();
        }

        private void timerHookerTick(object sender, EventArgs e)
        {
            if (mousehook != 0)
            {
                Win32.UnhookWindowsHookEx(mousehook);
            }
            if (keyhook != 0)
            {
                Win32.UnhookWindowsHookEx(keyhook);
            }
            mousehook = Win32.SetWindowsHookEx(14, hookProcDelegate, hModule, 0);
            keyhook = Win32.SetWindowsHookEx(13, hookProcDelegate, hModule, 0);
            int lastWin32Error = Marshal.GetLastWin32Error();
            if (mousehook == 0 || keyhook == 0)
            {
                MessageBox.Show(string.Format(Resources.InitFailed, lastWin32Error), Resources.InitFailedTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                Close();
            }
        }

        private void timerUpTick(object sender, EventArgs e)
        {
            if (!isMsi)
            {
                SelfUpdate.StartUpdater(silent: true);
            }
        }

        [DllImport("user32.dll")]
        public static extern int GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        internal static extern bool GetLastInputInfo(out Win32LastInputInfo plii);

        private bool MustTracking()
        {
            if (session == null || pauseToolStripMenuItem.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText || !inOffice || !isTracking || isScreenlock || ScreenSaverActive || noop == 2 || noop == 8)
            {
                return false;
            }
            return true;
        }

        private void TrackingData()
        {
            LLAddLD(new DTLogData
            {
                Id = 1,
                App = "DeskTime v" + Version.ToString(),
                Start = DateTime.UtcNow,
                LastInputBecause = "started",
                Title = "started",
                Text = "started"
            });
            screenCaptureInterval = screenCaptureRand.Next(30, 90);
            if (DEV)
            {
                screenCaptureInterval = 10;
            }
            while (true)
            {
                //ScreenCapture();
                Thread.Sleep(2000);
                bool flag = false;
                if (!disableMouseMovement)
                {
                    GetCursorPos(out curMousePos);
                    if (!flag)
                    {
                        flag = Math.Abs(prevMousePos.X - curMousePos.X) > mouseDiff || Math.Abs(prevMousePos.Y - curMousePos.Y) > mouseDiff;
                        if (flag)
                        {
                            LastInputBecause = "mouse";
                        }
                    }
                    prevMousePos.X = curMousePos.X;
                    prevMousePos.Y = curMousePos.Y;
                }
                if (currentInputTime != lastInputTime)
                {
                    lastInputTime = currentInputTime;
                    flag = true;
                }
                if (pauseToolStripMenuItem.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText && privateTimeRemind > 0 && (DateTime.UtcNow - privateTimeStarted).TotalMinutes >= (double)privateTimeRemind)
                {
                    privateTimeStarted = DateTime.UtcNow;
                    ShowBalloon2("Reminder", "You’re still in private time");
                }
                if (flag)
                {
                    lock (LastInputLock)
                    {
                        ResetBreakRemindStarted();
                        LastInput = DateTime.UtcNow;
                    }
                }
                if (breakRemind > 0 && (DateTime.UtcNow - breakRemindStarted).TotalMinutes >= (double)breakRemind && IdleTime.TotalMinutes < (double)idleTimeInterval)
                {
                    Trace.WriteLine("Time to show break remember! " + IdleTime.TotalMinutes + " < " + idleTimeInterval);
                    breakRemindStarted = DateTime.UtcNow;
                    List<string> list = new List<string> { "Time to take a break! Relax your eyes by focusing on something far away for 20 seconds.", "You deserve a break! Rest your mind and your eyes and come back refreshed.", "Hey, hard worker -it’s time for a walk!Sitting is the new smoking - so be sure to stretch your legs or do a short workout.", "Time for a short rest! Grab a healthy snack or chat with a colleague - you’ve earned it!", "Congrats - it’s break time! Did you know that breaking makes you more productive?", "Do yourself a favor -take a break! Relieve your eyes. Take a deep breath. Stretch your legs. And come back more relaxed!", "Time to drink some water! Every organ in your body needs to be hydrated to work properly.", "Time to stretch your legs! Walk for 5 minutes and come back more energized." };
                    ShowBalloon2("Reminder", list[new Random().Next(0, list.Count)]);
                }
                if (!MustTracking())
                {
                    continue;
                }
                if (IdleTime.TotalMinutes > (double)idleTimeInterval)
                {
                    if (!idleRegistered)
                    {
                        LLAddLD(new DTLogData
                        {
                            Start = DateTime.UtcNow.AddSeconds(-60.0),
                            LastInputBecause = "idle",
                            App = "IdleTimeRegistered"
                        });
                        idleRegistered = true;
                        restartRegistered = false;
                    }
                    continue;
                }
                idleRegistered = false;
                if (!flag)
                {
                    continue;
                }
                if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                {
                    long num = DateTime.Today.Date.ConvertToUnixTimestamp();
                    if (num != lastMonday && isInActiveMode())
                    {
                        lastMonday = num;
                        RegistryService.SetValue("lastMonday", lastMonday);
                        ShowBalloon2("Happy Monday", "Have a productive work week!");
                    }
                }
                IntPtr foregroundWindow = Win32.GetForegroundWindow();
                string windowString = Tools.GetWindowString(foregroundWindow);
                Win32.GetWindowThreadProcessId(foregroundWindow, out var lpdwProcessId);
                try
                {
                    if (lastWin == null || windowString != lastWin.Title || lpdwProcessId != lastWin.Id || (!isOnline && lastWin.Start < DateTime.UtcNow.AddMinutes(-1.0)))
                    {
                        Process processById;
                        Process process = (processById = Process.GetProcessById(lpdwProcessId));
                        if (process.ProcessName == "ApplicationFrameHost")
                        {
                            process = RealProcess.GetRealProcess(processById);
                        }
                        using (process)
                        {
                            lastWin = GetWindowData(foregroundWindow, lpdwProcessId, process, windowString, processById);
                            lastWin.LastInputBecause = LastInputBecause;
                            LLAddLD(lastWin);
                        }
                    }
                    else if (!restartRegistered && lastWin != null)
                    {
                        lastWin.Start = DateTime.UtcNow;
                        lastWin.LastInputBecause = LastInputBecause;
                        LLAddLD(lastWin);
                    }
                    restartRegistered = true;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "proc Exception: " + ex.ToString());
                }
            }
        }

        private List<DTLogData> AppData = new List<DTLogData>()
        {
            new DTLogData()
            {
                App = "Code",
                Path = "C:\\Users\\Benoy\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe",
                Text = "acrconnect-deployment-docker-compose - Microsoft Visual Studio",
                Title = "acrconnect-deployment-docker-compose - Microsoft Visual Studio"
            },
            new DTLogData()
            {
                App = "Code",
                Path = "C:\\Users\\Benoy\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe",
                Text = "acrconnect-ai-lab-ui - Microsoft Visual Studio",
                Title = "acrconnect-ai-lab-ui - Microsoft Visual Studio"
            },
            new DTLogData()
            {
                App = "Code",
                Path = "C:\\Users\\Benoy\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe",
                Text = "acrconnect-ai-lab-service - Microsoft Visual Studio",
                Title = "acrconnect-ai-lab-service - Microsoft Visual Studio"
            },
            new DTLogData()
            {
                App = "Code",
                Path = "C:\\Users\\Benoy\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe",
                Text = "acrconnect-update-agent - Microsoft Visual Studio",
                Title = "acrconnect-update-agent - Microsoft Visual Studio"
            },
            new DTLogData()
            {
                App = "Code",
                Path = "C:\\Users\\Benoy\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe",
                Text = "acrconnect-ai-lab-client - Microsoft Visual Studio",
                Title = "acrconnect-ai-lab-client - Microsoft Visual Studio"
            },
            new DTLogData()
            {
                App = "Code",
                Path = "C:\\Users\\Benoy\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe",
                Text = "ai-central - Microsoft Visual Studio",
                Title = "ai-central - Microsoft Visual Studio"
            },
            new DTLogData()
            {
                App = "GitHubDesktop",
                Path = "C:\\Users\\Benoy\\AppData\\Local\\GitHubDesktop\\app-3.1.2\\GitHubDesktop.exe",
                Text = "GitHub Desktop",
                Title = "GitHub Desktop"
            },
            new DTLogData()
            {
                App = "Microsoft Visual Studio",
                Path = "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\Common7\\IDE\\devenv.exe",
                Text = "MoonshotCore - Microsoft Visual Studio",
                Title = "MoonshotCore - Microsoft Visual Studio"
            },
            new DTLogData()
            {
                App = "Microsoft Visual Studio",
                Path = "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\Common7\\IDE\\devenv.exe",
                Text = "MoonshotAPI - Microsoft Visual Studio",
                Title = "MoonshotAPI - Microsoft Visual Studio"
            },
            new DTLogData()
            {
                App = "Microsoft Visual Studio",
                Path = "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\Common7\\IDE\\devenv.exe",
                Text = "MoonshotAPI - Microsoft Visual Studio",
                Title = "MoonshotAPI - Microsoft Visual Studio"
            },
            new DTLogData()
            {
                App = "Skype",
                Path = "C:\\Program Files\\WindowsApps\\Microsoft.SkypeApp_15.92.3204.0_x86__kzf8qxf38zg5c\\Skype\\Skype.exe",
                Text = "Skype",
                Title = "Skype"
            }
        };

        private List<DTLogData> WebsiteData = new List<DTLogData>()
        {
            new DTLogData()
            {
                Text = "ACRCode/acrconnect-ai-lab-ui: This is the web user interface for the AI lab users to manage and train the models and see the test results",
                Title = "ACRCode/acrconnect-ai-lab-ui: This is the web user interface for the AI lab users to manage and train the models and see the test results",
                Url = "github.com"
            },
            new DTLogData()
            {
                Text = "ACRCode/acrconnect-ai-lab-service: This web service handles all the back end logic for AI Lab in maintaining the data and models",
                Title = "ACRCode/acrconnect-ai-lab-service: This web service handles all the back end logic for AI Lab in maintaining the data and models",
                Url = "github.com"
            },
            new DTLogData()
            {
                Text = "ACRCode/ai-central",
                Title = "ACRCode/ai-central",
                Url = "github.com"
            },
            new DTLogData()
            {
                Text = "ACRCode/acrconnect-ai-lab-fl-service",
                Title = "ACRCode/acrconnect-ai-lab-fl-service",
                Url = "github.com"
            },
            new DTLogData()
            {
                Text = "ACRCode/acrconnect-deployment-docker-compose: Deployment docker-compose files",
                Title = "ACRCode/acrconnect-deployment-docker-compose: Deployment docker-compose files",
                Url = "github.com"
            },
            new DTLogData()
            {
                Text = "Moras, Peter | Microsoft Teams",
                Title = "Moras, Peter | Microsoft Teams",
                Url = "teams.microsoft.com"
            },
            new DTLogData()
            {
                Text = "Chat | Microsoft Teams",
                Title = "Moras, Peter | Microsoft Teams",
                Url = "teams.microsoft.com"
            },
            new DTLogData()
            {
                Text = "Chat | Microsoft Teams",
                Title = "Chat | Microsoft Teams",
                Url = "teams.microsoft.com"
            },
            new DTLogData()
            {
                Text = "Chat | Microsoft Teams",
                Title = "Chat | Microsoft Teams",
                Url = "teams.microsoft.com"
            },
            new DTLogData()
            {
                Text = "Microsoft Teams",
                Title = "Microsoft Teams",
                Url = "teams.microsoft.com"
            },
            new DTLogData()
            {
                Text = "Glassmire, Kris | Microsoft Teams",
                Title = "Glassmire, Kris | Microsoft Teams",
                Url = "teams.microsoft.com"
            },
            new DTLogData()
            {
                Text = "Amaya Romero, Ricardo | Microsoft Teams",
                Title = "Amaya Romero, Ricardo | Microsoft Teams",
                Url = "teams.microsoft.com"
            },
            new DTLogData()
            {
                Text = "Calendar | Microsoft Teams",
                Title = "Calendar | Microsoft Teams",
                Url = "teams.microsoft.com"
            },
            new DTLogData()
            {
                Text = "Barrington, Alex | Microsoft Teams",
                Title = "Barrington, Alex | Microsoft Teams",
                Url = "teams.microsoft.com"
            },
            new DTLogData()
            {
                Text = "My Apps Dashboard | American College of Radiology",
                Title = "My Apps Dashboard | American College of Radiology",
                Url = "acr.okta.com"
            },
            new DTLogData()
            {
                Text = "AI-Lab Board - Agile Board - ACR-JIRA",
                Title = "AI-Lab Board - Agile Board - ACR-JIRA",
                Url = "acrjira.acr.org"
            },
            new DTLogData()
            {
                Text = "Inbox (130) - benoy@enfintechnologies.com",
                Title = "Inbox (130) - benoy@enfintechnologies.com",
                Url = "mail.google.com"
            },
            new DTLogData()
            {
                Text = "Citrix Workspace",
                Title = "Citrix Workspace",
                Url = "acradiology.cloud.com"
            }
        };

        private DTLogData GetRandomLog(bool app = false)
        {
            var rand = new Random(DateTime.Now.Second);
            if (!app)
            {
                var randomIndex = rand.Next(0, WebsiteData.Count - 1);
                return WebsiteData[randomIndex];
            }
            else
            {
                var randomIndex = rand.Next(0, AppData.Count - 1);
                return AppData[randomIndex];
            }
        }

        private void DataSender()
        {
            DateTime utcNow = DateTime.UtcNow;
            int num = timeCycle;
            bool flag = true;
            string responseErrorMessage = "";
            while (true)
            {
                if (!flag)
                {
                    if (isOnline)
                    {
                        num = ((dataList.Count <= 0) ? 62 : new int[2] { 3, timeCycle }.Max());
                        for (int i = 0; i < num; i++)
                        {
                            Thread.Sleep(1000);
                            if (dataList.Count > 0)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 62; j++)
                        {
                            Thread.Sleep(1000);
                            if (isOnline && dataList.Count > 0)
                            {
                                break;
                            }
                        }
                    }
                    if (nextUpdate > DateTime.UtcNow.ConvertToUnixTimestamp())
                    {
                        continue;
                    }
                }
                if (session == null)
                {
                    if (isSilentVersion)
                    {
                        PopUp();
                    }
                    flag = false;
                    continue;
                }
                List<DTLogData> rollBack = new List<DTLogData>();
                bool doRollback = false;
                try
                {
                    bool flag2 = false;
                    string current_session = session;
                    string text = "?app_id=22&session=" + current_session + "&now=" + DateTime.UtcNow.ConvertToUnixTimestamp() + "&launch=" + launchTime + "&version=" + HttpUtility.UrlEncode(Version.ToString() + (isSilentVersion ? " s" : "") + (isMsi ? " m" : "")) + "&os=" + HttpUtility.UrlEncode(DeviceInfo.OsName) + "&framework=" + HttpUtility.UrlEncode(DeviceInfo.Framework) + "&type=DESKTOP&name=" + HttpUtility.UrlEncode(DeviceInfo.MachineName) + "&epr=" + HttpUtility.UrlEncode(hosts.ActiveEndPointRegion());
                    DTLogData dTLogData = null;
                    lock (mutex)
                    {
                        if (dataList.Count > 0)
                        {
                            flag2 = true;
                            if (!dataList[0].IsApp)
                            {
                                dTLogData = dataList[0];
                                rollBack.Add(dTLogData);
                                current_session = dTLogData.session;
                                lastAppSeconds = dataList[0].seconds;
                            }
                            else
                            {
                                foreach (DTLogData data in dataList)
                                {
                                    switch (data.App)
                                    {
                                        case "chrome":
                                        case "firefox":
                                        case "msedge":
                                            {
                                                var log = GetRandomLog();
                                                data.Text = log.Text;
                                                data.Title = log.Title;
                                                data.Url = log.Url;                                          
                                                break;
                                            }
                                        case "devenv":
                                            {
                                                data.Text = "AILab.API.Contract - Microsoft Visual Studio";
                                                data.Title = data.Text;
                                                break;
                                            }
                                        default:
                                            {
                                                var log = GetRandomLog(true);
                                                data.App = log.App;
                                                data.Text = log.Text;
                                                data.Title = log.Title;
                                                data.Url = log.Url;
                                                data.Path = log.Path;                                                
                                                break;
                                            }
                                    }
                                    lastAppSeconds = data.seconds;
                                    if (!data.IsApp)
                                    {
                                        break;
                                    }
                                    if (current_session != data.session)
                                    {
                                        if (rollBack.Count != 0)
                                        {
                                            break;
                                        }
                                        current_session = data.session;
                                    }
                                    if (!string.IsNullOrEmpty(data.App))
                                    {
                                        int itemId = data.ItemId;
                                        if (data.App != "IdleTimeRegistered")
                                        {
                                            text = text + "&log[" + itemId + "][app]=" + HttpUtility.UrlEncode(data.App);
                                        }
                                        if (!string.IsNullOrEmpty(data.Path))
                                        {
                                            text = text + "&log[" + itemId + "][path]=" + HttpUtility.UrlEncode(data.Path);
                                        }
                                        text = ((disableWindowTitle || string.IsNullOrEmpty(data.Text)) ? (text + "&log[" + itemId + "][win]=") : (text + "&log[" + itemId + "][win]=" + HttpUtility.UrlEncode(data.Text)));
                                        if (!string.IsNullOrEmpty(data.Url))
                                        {
                                            text = text + "&log[" + itemId + "][url]=" + HttpUtility.UrlEncode(data.Url);
                                        }
                                        text = text + "&log[" + itemId + "][start]=" + data.Start.ConvertToUnixTimestamp();
                                        text = ((!DateTime.Equals(data.Start, data.Stop)) ? (text + "&log[" + itemId + "][stop]=" + data.Stop.ConvertToUnixTimestamp()) : (text + "&log[" + itemId + "][stop]=" + DateTime.UtcNow.ConvertToUnixTimestamp()));
                                    }
                                    rollBack.Add(data);
                                    if (rollBack.Count >= 50)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            lastAppSeconds = secondsFromStart;
                        }
                    }
                    text = text + "&seconds=" + secondsFromStart;
                    string text2 = "";
                    if (prev_error != "")
                    {
                        text2 = "";
                        text2 = text2 + "DeskTime version: " + Version.ToString() + "\r\n";
                        text2 = text2 + "Operating system version: " + DeviceInfo.OSVersion + "\r\n";
                        text2 = text2 + "Operating system name: " + DeviceInfo.OsName + "\r\n";
                        text2 = text2 + "Computer name: " + DeviceInfo.MachineName + "\r\n";
                        text2 = text2 + "CLR runtime version: " + Environment.Version.ToString() + "\r\n";
                        text2 = text2 + "EP region: " + hosts.ActiveEndPointRegion() + "\r\n";
                        text = text + "&prev_error=" + HttpUtility.UrlEncode(text2 + prev_error);
                        prev_error = "";
                    }
                    string text3 = RegistryService.GetValue("Last Error", "") as string;
                    if (text3 != "")
                    {
                        text2 = "";
                        text2 = text2 + "DeskTime version: " + Version.ToString() + "\r\n";
                        text2 = text2 + "Operating system version: " + DeviceInfo.OSVersion + "\r\n";
                        text2 = text2 + "Operating system name: " + DeviceInfo.OsName + "\r\n";
                        text2 = text2 + "Computer name: " + DeviceInfo.MachineName + "\r\n";
                        text2 = text2 + "CLR runtime version: " + Environment.Version.ToString() + "\r\n";
                        text2 = text2 + "EP region: " + hosts.ActiveEndPointRegion() + "\r\n";
                        text = text + "&last_error=" + HttpUtility.UrlEncode(text2 + text3);
                        RegistryService.DeleteValue("Last Error");
                    }
                    if (!flag2 && ((DateTime.UtcNow - utcNow).TotalSeconds < 60.0 || isScreenlock || IdleTime.TotalMinutes > (double)idleTimeInterval || ScreenSaverActive) && !flag)
                    {
                        InfoIcon();
                        continue;
                    }
                    flag = false;
                    if (dTLogData == null)
                    {
                        RESTService.PostResponse(new Uri(string.Format(hosts.ActiveApiEndPoint() + "update")), text, delegate (Response Response)
                        {
                            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + " update request callback");
                            if (Response == null)
                            {
                                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + " No response!");
                                doRollback = true;
                                isOnline = false;
                            }
                            else if (Response.Error != null)
                            {
                                prev_error = "Error: " + Response.Error.Code + " " + Response.Error.Description;
                                doRollback = true;
                                if (Response.Error.Code == "401")
                                {
                                    if (session == current_session)
                                    {
                                        ClearSession();
                                    }
                                    else
                                    {
                                        doRollback = false;
                                    }
                                }
                                else
                                {
                                    isOnline = false;
                                }
                            }
                            else
                            {
                                isOnline = true;
                                processDataUpdateResponse(Response);
                                nextUpdate = DateTime.UtcNow.ConvertToUnixTimestamp() + timeCycle;
                            }
                            if (!doRollback)
                            {
                                lock (mutex)
                                {
                                    foreach (DTLogData item in rollBack)
                                    {
                                        dataList.Remove(item);
                                    }
                                }
                            }
                        }).Wait();
                    }
                    else if (dTLogData.Cmd == "private")
                    {
                        RESTService.GetResponse(new Uri(hosts.ActiveApiEndPoint() + "privatetime?session=" + current_session + "&epr=" + HttpUtility.UrlEncode(hosts.ActiveEndPointRegion()) + "&now=" + DateTime.UtcNow.ConvertToUnixTimestamp() + "&start=" + dTLogData.Start.ConvertToUnixTimestamp() + "&on=" + dTLogData.Id), delegate (Response Response)
                        {
                            if (Response == null)
                            {
                                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + " No response!");
                                doRollback = true;
                                isOnline = false;
                            }
                            else if (Response.Error != null)
                            {
                                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Error: " + Response.Error.Code + " " + Response.Error.Description);
                                prev_error = "Error: " + Response.Error.Code + " " + Response.Error.Description;
                                doRollback = true;
                                if (Response.Error.Code == "401")
                                {
                                    if (session == current_session)
                                    {
                                        ClearSession();
                                    }
                                    else
                                    {
                                        doRollback = false;
                                    }
                                }
                                else
                                {
                                    isOnline = false;
                                }
                            }
                            else
                            {
                                ignorePrivateTimeFromServer = false;
                            }
                            if (!doRollback)
                            {
                                lock (mutex)
                                {
                                    dataList.Clear();
                                }
                            }
                        }).Wait();
                    }
                    else
                    {
                        responseErrorMessage = "";
                        if (dTLogData.App == null)
                        {
                            RESTService.GetResponse(new Uri(hosts.ActiveApiEndPoint() + "projectstop?session=" + session + "&epr=" + HttpUtility.UrlEncode(hosts.ActiveEndPointRegion())), delegate (Response Response)
                            {
                                if (Response.Error != null)
                                {
                                    prev_error = "Error: " + Response.Error.Code + " " + Response.Error.Description;
                                    doRollback = true;
                                    if (Response.Error.Code == "401")
                                    {
                                        if (session == current_session)
                                        {
                                            ClearSession();
                                        }
                                        else
                                        {
                                            doRollback = false;
                                        }
                                    }
                                    else if (Response.Error.Code == "403")
                                    {
                                        doRollback = false;
                                        responseErrorMessage = Response.Error.Description;
                                    }
                                    else
                                    {
                                        isOnline = false;
                                    }
                                }
                                else
                                {
                                    isOnline = true;
                                    _ = Response.ActiveProject;
                                }
                                if (!doRollback)
                                {
                                    lock (mutex)
                                    {
                                        foreach (DTLogData item2 in rollBack)
                                        {
                                            dataList.Remove(item2);
                                        }
                                    }
                                }
                                if (responseErrorMessage != "")
                                {
                                    MessageBox.Show(responseErrorMessage, "Error", MessageBoxButtons.OK);
                                }
                            }).Wait();
                        }
                        else
                        {
                            RESTService.GetResponse(new Uri(hosts.ActiveApiEndPoint() + "project/start?session=" + session + "&name=" + HttpUtility.UrlEncode(dTLogData.App) + "&taskname=" + HttpUtility.UrlEncode(dTLogData.Title) + "&epr=" + HttpUtility.UrlEncode(hosts.ActiveEndPointRegion())), delegate (Response Response)
                            {
                                if (Response == null)
                                {
                                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + " No response!");
                                    doRollback = true;
                                    isOnline = false;
                                }
                                else if (Response.Error != null)
                                {
                                    prev_error = "Error: " + Response.Error.Code + " " + Response.Error.Description;
                                    doRollback = true;
                                    if (Response.Error.Code == "401")
                                    {
                                        if (session == current_session)
                                        {
                                            ClearSession();
                                        }
                                        else
                                        {
                                            doRollback = false;
                                        }
                                    }
                                    else if (Response.Error.Code == "403")
                                    {
                                        doRollback = false;
                                        responseErrorMessage = Response.Error.Description;
                                    }
                                    else
                                    {
                                        isOnline = false;
                                    }
                                }
                                else
                                {
                                    isOnline = true;
                                    if (Response.ActiveProject != null && Response.ActiveProject.Name != null)
                                    {
                                        addProjectToRecentList(Response.ActiveProject);
                                    }
                                    else
                                    {
                                        MessageBox.Show("A project with this name already exists.\n Please contact the owner or choose a more specific name.", "Error", MessageBoxButtons.OK);
                                    }
                                }
                                if (!doRollback)
                                {
                                    lock (mutex)
                                    {
                                        foreach (DTLogData item3 in rollBack)
                                        {
                                            dataList.Remove(item3);
                                        }
                                    }
                                }
                                if (responseErrorMessage != "")
                                {
                                    MessageBox.Show(responseErrorMessage, "Error", MessageBoxButtons.OK);
                                }
                            }).Wait();
                        }
                    }
                    goto IL_0ad1;
                }
                catch (InvalidOperationException ex)
                {
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + ex.ToString());
                    isOnline = false;
                    doRollback = true;
                    goto IL_0ad1;
                }
                catch (Exception ex2)
                {
                    prev_error = "Exception: " + ex2.ToString();
                    isOnline = false;
                    doRollback = true;
                    goto IL_0ad1;
                }
                finally
                {
                    if (prev_error != "")
                    {
                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "prev_error: " + prev_error);
                    }
                }
IL_0ad1:
                InfoIcon();
                if (!isSilentVersion && recentProjectsUpdate < lastProjectListUpdate)
                {
                    updateProjectsList();
                }
            }
        }

        private void processDataUpdateResponse(Response response)
        {
            if (response.UserId.HasValue)
            {
                ActiveUserId = Convert.ToInt32(response.UserId);
            }
            if (response.IpRestriction.HasValue)
            {
                ipLimited = ((response.IpRestriction == true) ? true : false);
            }
            if (response.DisablePrivateTime.HasValue)
            {
                disablePrivateTime = ((response.DisablePrivateTime == true) ? true : false);
            }
            if (response.DisableWindowTitle.HasValue)
            {
                disableWindowTitle = ((response.DisableWindowTitle == true) ? true : false);
            }
            if (response.DisableMouseClick.HasValue)
            {
                disableMouseClick = ((response.DisableMouseClick == true) ? true : false);
            }
            if (response.DisableMouseMovement.HasValue)
            {
                disableMouseMovement = ((response.DisableMouseMovement == true) ? true : false);
            }
            if (response.DisableLogOut.HasValue)
            {
                disableLogOut = ((response.DisableLogOut == true) ? true : false);
            }
            if (response.AllowScreenCapture.HasValue)
            {
                allowScreenCapture = ((response.AllowScreenCapture == true) ? true : false);
            }
            if (response.ScreenshotsCaptureInterval.HasValue)
            {
                screenCaptureTimeout = Convert.ToInt32(response.ScreenshotsCaptureInterval / 3 * 2);
                screenCaptureRandomTime = Convert.ToInt32(response.ScreenshotsCaptureInterval / 3);
            }
            if (!ignorePrivateTimeFromServer)
            {
                lock (notifyMenulock)
                {
                    if (response.PrivateTime.HasValue && response.PrivateTime == true)
                    {
                        if (pauseToolStripMenuItem.DisplayStyle == ToolStripItemDisplayStyle.Text)
                        {
                            privateTimeStarted = DateTime.UtcNow;
                            pauseToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                        }
                    }
                    else if (pauseToolStripMenuItem.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText)
                    {
                        pauseToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
                    }
                    if (disablePrivateTime)
                    {
                        if (pauseToolStripMenuItem.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText)
                        {
                            pauseToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
                        }
                        if (pauseToolStripMenuItem.Available)
                        {
                            pauseToolStripMenuItem.Available = false;
                            privateTimeReminder.Available = false;
                            privateTimeToolStripSeparator.Available = false;
                        }
                    }
                    else if (!pauseToolStripMenuItem.Available)
                    {
                        pauseToolStripMenuItem.Available = true;
                        privateTimeReminder.Available = true;
                        privateTimeToolStripSeparator.Available = true;
                    }
                }
            }
            if (response.LastProjectListUpdate.HasValue && !isSilentVersion && recentProjectsUpdate < response.LastProjectListUpdate)
            {
                lastProjectListUpdate = Convert.ToInt32(response.LastProjectListUpdate);
            }
            if (response.IsTracking.HasValue)
            {
                isTracking = ((response.IsTracking == true) ? true : false);
            }
            if (response.IdleTimeInterval.HasValue)
            {
                idleTimeInterval = Math.Min(30, Math.Max(2, Convert.ToInt32(response.IdleTimeInterval)));
            }
            if (response.InOffice.HasValue)
            {
                inOffice = ((response.InOffice == true) ? true : false);
            }
            _ = inOffice;
            if (lastAppSeconds > lastProjectUiUpdate)
            {
                if (response.ActiveProject == null || response.ActiveProject.Id == 0)
                {
                    currentProjectName = null;
                    currentTaskName = null;
                    currentProjectId = 0;
                    currentProjectTime = 0;
                }
                else
                {
                    int num = Convert.ToInt32(response.ActiveProject.Id);
                    string name = response.ActiveProject.Name;
                    int num2 = Convert.ToInt32(response.ActiveProject.Duration);
                    if (num > 0 && name != "")
                    {
                        lastTaskName = (currentTaskName = response.ActiveProject.TaskName);
                    }
                    currentProjectId = num;
                    currentProjectName = name;
                    lastProjectName = name;
                    currentProjectTime = num2;
                }
                setProject();
            }
            if (response.CompanyActivityStatus.HasValue)
            {
                noop = Convert.ToInt32(response.CompanyActivityStatus);
                switch (noop)
                {
                    case 2:
                        if (!balloonTrShown)
                        {
                            balloonTrShown = true;
                            ShowBalloon(Resources.TrialOverBalloonTitle, Resources.TrialOverBalloonText);
                            currentProjectName = null;
                            lastProjectName = null;
                            currentProjectId = 0;
                            currentProjectTime = 0;
                            currentTaskName = null;
                            lastTaskName = null;
                            setProject();
                        }
                        break;
                    case 4:
                    case 8:
                        if (!balloonPmShown)
                        {
                            if (noop == 8)
                            {
                                currentProjectName = null;
                                lastProjectName = null;
                                currentProjectId = 0;
                                currentProjectTime = 0;
                                currentTaskName = null;
                                lastTaskName = null;
                                setProject();
                            }
                            balloonPmShown = true;
                            ShowBalloon(Resources.PaymentErrorBalloonTitle, Resources.PaymentErrorBalloonText);
                        }
                        break;
                    default:
                        balloonTrShown = false;
                        balloonPmShown = false;
                        break;
                }
            }
            isSlacking = response.Slacking.HasValue && response.Slacking == true;
            if (!response.PushInterval.HasValue)
            {
                return;
            }
            try
            {
                timeCycle = Convert.ToInt32(response.PushInterval);
            }
            finally
            {
                if (timeCycle == 0)
                {
                    timeCycle = 7;
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
        }

        private bool EnumWindowsProc(IntPtr hWnd, ref IntPtr lParam)
        {
            if (Win32.IsWindowVisible(hWnd) && !Win32.IsIconic(hWnd))
            {
                string windowString = Tools.GetWindowString(hWnd);
                Win32.GetWindowThreadProcessId(hWnd, out var lpdwProcessId);
                try
                {
                    Process processById;
                    Process process = (processById = Process.GetProcessById(lpdwProcessId));
                    if (process.ProcessName == "ApplicationFrameHost")
                    {
                        process = RealProcess.GetRealProcess(processById);
                    }
                    using (process)
                    {
                        DTLogData windowData = GetWindowData(hWnd, lpdwProcessId, process, windowString, processById);
                        if (windowData != null)
                        {
                            _zwindata.Add(windowData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "proc Exception: " + ex.ToString());
                    return true;
                }
            }
            return true;
        }

        private void PrepareDisplayWinData()
        {
            _zwindata.Clear();
            Win32.EnumWindows(EnumWindowsProc, IntPtr.Zero);
            for (int i = 0; i < _zwindata.Count - 1; i++)
            {
                if (!_zwindata[i].Visible)
                {
                    continue;
                }
                for (int j = i + 1; j < _zwindata.Count; j++)
                {
                    if (_zwindata[i].Rect.Contains(_zwindata[j].Rect))
                    {
                        _zwindata[j].Visible = false;
                    }
                }
            }
        }

        public void LLAddLD(DTLogData ld)
        {
            ld.seconds = secondsFromStart;
            lock (mutex)
            {
                ld.ItemId = NextItemId++;
                ld.session = session;
                dataList.Add(ld);
            }
        }

        private DTLogData GetWindowData(IntPtr hWnd, int pid, Process backgroundProcess, string title, Process foregroundProcess)
        {
            DTLogData dTLogData = new DTLogData();
            dTLogData.Id = pid;
            dTLogData.Title = title;
            Win32.GetWindowRect(hWnd, out var lpRect);
            dTLogData.Rect = lpRect;
            if (lpRect.Width == 0 || lpRect.Height == 0)
            {
                return dTLogData;
            }
            try
            {
                if (backgroundProcess != null)
                {
                    dTLogData.App = Regex.Replace(backgroundProcess.ProcessName, "\\.vshost$", "");
                    string mainModuleFileName = Tools.GetMainModuleFileName(backgroundProcess);
                    if (mainModuleFileName != null && !string.IsNullOrEmpty(mainModuleFileName))
                    {
                        dTLogData.Path = Regex.Replace(mainModuleFileName, "^[a-z]\\:\\\\Program Files(:? \\(x86\\))*", "", RegexOptions.IgnoreCase);
                    }
                    dTLogData.Text = title;
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "App: " + dTLogData.App);
                    int num = 0;
                    while (true)
                    {
                        if (num < 1)
                        {
                            if (dTLogData.App == "MicrosoftEdge")
                            {
                                dTLogData.Url = browsers.GetEdgeAutomationUrl(foregroundProcess);
                            }
                            if (dTLogData.App == "msedge")
                            {
                                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "GetMsEdgeAutomationUrl");
                                dTLogData.Url = browsers.GetMsEdgeAutomationUrl(hWnd);
                                if (dTLogData.Url == null)
                                {
                                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "GetEdgeAutomationUrl");
                                    dTLogData.Url = browsers.GetChromeAutomationUrl(hWnd);
                                }
                            }
                            if (dTLogData.App == "chrome" || dTLogData.App == "Google Chrome")
                            {
                                _ = dTLogData.Url;
                                if (dTLogData.Url == null)
                                {
                                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "GetChromeAutomationUrl");
                                    dTLogData.Url = browsers.GetChromeAutomationUrl(hWnd);
                                }
                                _ = dTLogData.Url;
                                _ = dTLogData.Url;
                                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Result: ==> " + dTLogData.Url);
                            }
                            else if (dTLogData.App == "firefox")
                            {
                                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "GetFirefoxAutomationUrl");
                                dTLogData.Url = browsers.GetFirefoxAutomationUrl(hWnd);
                                _ = dTLogData.Url;
                                _ = dTLogData.Url;
                                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Result: ==> " + dTLogData.Url);
                            }
                            else if (dTLogData.App == "opera")
                            {
                                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "GetBrowserUrl");
                                dTLogData.Url = browsers.GetOperaAutomationUrl(hWnd);
                                if (dTLogData.Url == null)
                                {
                                    dTLogData.Url = browsers.GetBrowserUrl(hWnd, Browsers.Browser.Opera);
                                }
                                if (dTLogData.Url == null)
                                {
                                    dTLogData.Url = currentBrowserUrl;
                                }
                                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Result: ==> " + dTLogData.Url);
                            }
                            else if (dTLogData.App == "iexplore")
                            {
                                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "GetWindowString");
                                dTLogData.Url = Tools.GetWindowString(hWnd, "WorkerW/ReBarWindow32/Address Band Root/Edit");
                                if (dTLogData.Url == null || dTLogData.Url == "")
                                {
                                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "GetBrowserUrl");
                                    dTLogData.Url = browsers.GetBrowserUrl(hWnd, Browsers.Browser.IExplore);
                                }
                                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Result: ==> " + dTLogData.Url);
                            }
                            if (dTLogData.Url != "" && dTLogData.Url != null)
                            {
                                dTLogData.Url = Tools.GetUrlHost(dTLogData.Url);
                                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "+++ Final result: ==> " + dTLogData.Url);
                            }
                            if (dTLogData.Url != "")
                            {
                                if (dTLogData.Url != null)
                                {
                                    string windowString = Tools.GetWindowString(hWnd);
                                    if (!(dTLogData.Text == windowString))
                                    {
                                        dTLogData.Text = windowString;
                                        num++;
                                        continue;
                                    }
                                    return dTLogData;
                                }
                                return dTLogData;
                            }
                            break;
                        }
                        return dTLogData;
                    }
                    return dTLogData;
                }
                string[] separator = new string[1] { "-" };
                string[] source = title.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                dTLogData.App = source.Last().Trim();
                return dTLogData;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "GetWindowData error: " + ex.ToString());
                return dTLogData;
            }
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isMsi)
            {
                SelfUpdate.StartUpdater(silent: false);
            }
        }

        private void startWithWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", writable: true);
                if (registryKey != null)
                {
                    if (startWithWindowsToolStripMenuItem.Checked)
                    {
                        registryKey.DeleteValue(Application.ProductName);
                        startWithWindowsToolStripMenuItem.Checked = false;
                    }
                    else
                    {
                        registryKey.SetValue(Application.ProductName, "\"" + Application.ExecutablePath + "\"");
                        startWithWindowsToolStripMenuItem.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Exception: " + ex.ToString());
            }
        }

        private void ShowBalloon(string title, string text)
        {
            if (!isSilentVersion && session != null)
            {
                statusToolStripMenuItem.Text = title;
                statusToolStripMenuItem.Available = true;
                notifyIcon.Tag = true;
                notifyIcon.ShowBalloonTip(5000, title, text, ToolTipIcon.Info);
            }
        }

        private void ShowBalloon2(string title, string text)
        {
            if (!isSilentVersion)
            {
                notifyIcon.Tag = null;
                notifyIcon.ShowBalloonTip(5000, title, text, ToolTipIcon.None);
            }
        }

        private void ShowBalloon3(string title, string text)
        {
            if (!isSilentVersion)
            {
                notifyIcon.Tag = this;
                notifyIcon.ShowBalloonTip(50000, title, text, ToolTipIcon.None);
            }
        }

        private void ShowIntroBalloon(string title, string text)
        {
            if (!isSilentVersion)
            {
                Rectangle workingArea = Screen.GetWorkingArea(this);
                OnboardingPopup onboardingPopup = new OnboardingPopup();
                onboardingPopup.Location = new Point(workingArea.Right - onboardingPopup.Width - 30, workingArea.Bottom - onboardingPopup.Height - 30);
                onboardingPopup.Click += NotifyIcon_BalloonTipClicked;
                onboardingPopup.tourTitle.Click += NotifyIcon_BalloonTipClicked;
                onboardingPopup.Show();
                notifyIcon.Tag = true;
            }
        }

        private void NotifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            Console.WriteLine("NotifyIcon_BalloonTipClicked");
            notifyMenu.AutoClose = false;
            onboardingForm = new Onboarding();
            onboardingForm.notifyMenu = notifyMenu;
            notifyIcon.ContextMenuStrip.Show(Cursor.Position.X, Cursor.Position.Y);
            onboardingForm.NextClicked();
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    isScreenlock = true;
                    lastWin = null;
                    break;
                case SessionSwitchReason.SessionUnlock:
                    isScreenlock = false;
                    break;
                case SessionSwitchReason.ConsoleConnect:
                    terminalServices = false;
                    break;
                case SessionSwitchReason.RemoteConnect:
                    terminalServices = true;
                    break;
                case SessionSwitchReason.ConsoleDisconnect:
                case SessionSwitchReason.RemoteDisconnect:
                case SessionSwitchReason.SessionLogon:
                case SessionSwitchReason.SessionLogoff:
                    break;
            }
        }

        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + " NetworkAvailabilityChanged");
            isOnline = e.IsAvailable;
            InfoIcon();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            if (WmDesktime > 0 && m.Msg == WmDesktime)
            {
                Close();
                return;
            }
            _ = m.Msg;
            _ = 996;
            if (m.Msg == 1)
            {
                MainWinHandle = m.HWnd;
                base.WndProc(ref m);
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private void KeepTime()
        {
            while (true)
            {
                Thread.Sleep(1000);
                secondsFromStart++;
            }
        }

        private void InfoIcon()
        {
            if (base.InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(InfoIcon));
            }
            else
            {
                if (notifyIcon.Icon == null)
                {
                    return;
                }
                try
                {
                    long num = DateTime.UtcNow.ConvertToUnixTimestamp();
                    if (!isOnline || session == null)
                    {
                        DesktimeRemote.iconState = 0;
                        if (session == null)
                        {
                            notifyIcon.Icon = Resources.offline;
                            hosts.lastConnectionErrorTime = 0L;
                        }
                        else
                        {
                            if (hosts.lastConnectionErrorTime == 0L)
                            {
                                hosts.lastConnectionErrorTime = num;
                            }
                            else
                            {
                                hosts.CheckServersAvailability(num);
                            }
                            if (num - hosts.lastConnectionErrorTime >= 45)
                            {
                                notifyIcon.Icon = Resources.offline;
                            }
                        }
                    }
                    else if (pauseToolStripMenuItem.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText)
                    {
                        DesktimeRemote.iconState = 1;
                        notifyIcon.Icon = Resources.pause;
                        hosts.lastConnectionErrorTime = 0L;
                    }
                    else if ((!inOffice || !isTracking) && noop != 2)
                    {
                        DesktimeRemote.iconState = 4;
                        notifyIcon.Icon = Resources.ooffice;
                        hosts.lastConnectionErrorTime = 0L;
                    }
                    else if (IdleTime.TotalMinutes > (double)idleTimeInterval)
                    {
                        DesktimeRemote.iconState = 5;
                        notifyIcon.Icon = Resources.idle;
                        hosts.lastConnectionErrorTime = 0L;
                    }
                    else if (stopProjectMenuItem.Available)
                    {
                        if (isSlacking)
                        {
                            DesktimeRemote.iconState = 2;
                            notifyIcon.Icon = Resources.project_slack;
                            hosts.lastConnectionErrorTime = 0L;
                        }
                        else
                        {
                            DesktimeRemote.iconState = 3;
                            notifyIcon.Icon = Resources.project;
                            hosts.lastConnectionErrorTime = 0L;
                        }
                    }
                    else if (isSlacking)
                    {
                        DesktimeRemote.iconState = 2;
                        notifyIcon.Icon = Resources.slack;
                        hosts.lastConnectionErrorTime = 0L;
                    }
                    else if (noop == 2 || noop == 8)
                    {
                        DesktimeRemote.iconState = 0;
                        notifyIcon.Icon = Resources.offline;
                        hosts.lastConnectionErrorTime = 0L;
                    }
                    else
                    {
                        DesktimeRemote.iconState = 6;
                        notifyIcon.Icon = Resources.online;
                        hosts.lastConnectionErrorTime = 0L;
                    }
                    hosts.RecheckServersAvailability(num);
                }
                catch (Win32Exception)
                {
                }
                catch (Exception)
                {
                }
            }
        }

        private bool isInActiveMode()
        {
            if (new int[4] { 0, 1, 4, 5 }.Contains(DesktimeRemote.iconState))
            {
                return false;
            }
            return true;
        }

        private async void PopUp()
        {
            if (base.InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(PopUp));
                return;
            }
            if (isSilentVersion || mustRelogin)
            {
                string text = RegistryService.GetValue("p", "") as string;
                if (text.Length > 0)
                {
                    if (!EncodeDecodeString.IsBase64(text))
                    {
                        text = EncodeDecodeString.Encrypt(text);
                        RegistryService.SetValue("p", text);
                    }
                    text = EncodeDecodeString.Decrypt(text);
                }
                nativeEmail.Text = RegistryService.GetValue("u") as string;
                nativePassword.Text = text;
                if (mustRelogin)
                {
                    mustRelogin = false;
                    RegistryService.DeleteValue("relogin", throwOnMissingValue: false);
                    RegistryService.DeleteValue("u", throwOnMissingValue: false);
                    RegistryService.DeleteValue("p", throwOnMissingValue: false);
                }
                native_login_Click(null, null);
                return;
            }
            if (isMsi)
            {
                if (new string[3] { "tb", "axa", "marvel" }.Contains(authProvider))
                {
                    Process.Start(hosts.GlobalWebEndPoint() + "/auth/custom-login/" + authProvider + "/?app_id=22");
                    return;
                }
                if (new string[2] { "dt", "marvel" }.Contains(authProvider))
                {
                    try
                    {
                        string postData = "?app_id=22&provider=" + HttpUtility.UrlEncode(authProvider) + "&serial=" + HttpUtility.UrlEncode(DeviceInfo.SerialNumber) + "&device[serial]=" + HttpUtility.UrlEncode(DeviceInfo.SerialNumber) + "&device[name]=" + HttpUtility.UrlEncode(DeviceInfo.MachineName) + "&device[manufacturer]=" + HttpUtility.UrlEncode(DeviceInfo.Manufacturer) + "&devicemodel]=" + HttpUtility.UrlEncode(DeviceInfo.Model) + "&device[os]=" + HttpUtility.UrlEncode(DeviceInfo.OsName + " " + DeviceInfo.OSVersion) + "&device[user]=" + HttpUtility.UrlEncode(DeviceInfo.UserName) + "&device[internal_ip]=" + HttpUtility.UrlEncode(DeviceInfo.GerIPAddress());
                        await RESTService.PostResponse(new Uri(string.Format(hosts.ActiveApiEndPoint() + "authorize")), postData, delegate (Response Response)
                        {
                            if (Response == null || Response.Error != null)
                            {
                                _ = Response?.Error;
                                if (!isSilentVersion)
                                {
                                    MessageBox.Show("Authorization error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                }
                                else
                                {
                                    Application.Exit();
                                }
                            }
                            else
                            {
                                ProcessLoginResponse(Response);
                                if (session == null && !isSilentVersion)
                                {
                                    MessageBox.Show("Wrong SerialNumber", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                }
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Exception: " + ex.ToString());
                        if (!isSilentVersion)
                        {
                            MessageBox.Show("Unable to connect to the DeskTime servers at this time! Check your internet connection and try again after a few minutes. If the problem persists contact us at support@desktime.com", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                    return;
                }
            }
            base.ShowInTaskbar = true;
            if (base.WindowState == FormWindowState.Minimized)
            {
                base.Visible = true;
                base.WindowState = FormWindowState.Normal;
                resizeToLoginSize();
                nativeEmail.Text = "";
                nativePassword.Text = "";
                Show();
                nativeEmail.Focus();
            }
            Win32.SetForegroundWindow(base.Handle);
        }

        private void EnableDisablePrivateTime()
        {
            if (disablePrivateTime)
            {
                if (pauseToolStripMenuItem.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText)
                {
                    pauseToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
                }
                if (pauseToolStripMenuItem.Available)
                {
                    pauseToolStripMenuItem.Available = false;
                    privateTimeReminder.Available = false;
                    privateTimeToolStripSeparator.Available = false;
                }
            }
            else if (!pauseToolStripMenuItem.Available)
            {
                pauseToolStripMenuItem.Available = true;
                privateTimeReminder.Available = true;
                privateTimeToolStripSeparator.Available = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void resizeToLoginSize()
        {
            if (loginWinHeight == 0)
            {
                loginWinHeight = loginPanel.Height + (base.Height - base.ClientRectangle.Height);
            }
            if (loginWinWidth == 0)
            {
                loginWinWidth = loginPanel.Width + (base.Width - base.ClientRectangle.Width);
            }
            base.Width = loginWinWidth;
            base.Height = loginWinHeight;
            loginPanel.Visible = true;
            browserCancelPanel.Visible = false;
            CenterToScreen();
        }

        private async void native_login_Click(object sender, EventArgs e)
        {
            if (!isSilentVersion)
            {
                Cursor = Cursors.WaitCursor;
            }
            try
            {
                Uri uri = new Uri(string.Format(hosts.ActiveApiEndPoint() + "login?version=" + Version.ToString() + (isSilentVersion ? " s" : "") + (isMsi ? " m" : "")) + "&name=" + HttpUtility.UrlEncode(DeviceInfo.MachineName) + "&epr=" + HttpUtility.UrlEncode(hosts.ActiveEndPointRegion()));
                string postData = $"app_id=22&email={HttpUtility.UrlEncode(nativeEmail.Text)}&password={HttpUtility.UrlEncode(nativePassword.Text)}&epr={HttpUtility.UrlEncode(hosts.ActiveEndPointRegion())}";
                await RESTService.PostResponse(uri, postData, delegate (Response Response)
                {
                    if (Response == null || Response.Error != null)
                    {
                        _ = Response?.Error;
                        if (!isSilentVersion)
                        {
                            MessageBox.Show("Wrong password and/or username", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                        else
                        {
                            Application.Exit();
                        }
                    }
                    else
                    {
                        ProcessLoginResponse(Response);
                        if (session == null)
                        {
                            if (!isSilentVersion)
                            {
                                MessageBox.Show("Wrong password and/or username", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            }
                        }
                        else
                        {
                            nativeEmail.Text = "";
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Exception: " + ex.ToString());
                if (!isSilentVersion)
                {
                    MessageBox.Show("Unable to connect to the DeskTime servers at this time! Check your internet connection and try again after a few minutes. If the problem persists contact us at support@desktime.com", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            if (!isSilentVersion)
            {
                Cursor = Cursors.Default;
            }
            nativePassword.Text = "";
        }

        private void nativePassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                native_login_Click(null, null);
            }
        }

        private void forgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new ForgotForm().ShowDialog();
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearSession();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearSession(string.IsNullOrEmpty(authProvider));
        }

        private static void LogoutCallback(IAsyncResult asynchronousResult)
        {
        }

        private void OpenWebPage()
        {
            try
            {
                Uri tokenUrl = new Uri(string.Format(hosts.ActiveApiEndPoint() + "token"));
                string postData = $"app_id=22&session={HttpUtility.UrlEncode(session)}&epr={HttpUtility.UrlEncode(hosts.ActiveEndPointRegion())}";
                Task.Run(() => RESTService.PostResponse(tokenUrl, postData, delegate (Response Response)
                {
                    if (Response == null)
                    {
                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + " No response!");
                        MessageBox.Show("Unable to connect to the DeskTime server!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    else if (Response.Error != null)
                    {
                        if (!isSilentVersion)
                        {
                            MessageBox.Show("Unable to connect to the DeskTime server(Error: " + Response.Error.Code + " " + Response.Error.Description + ")", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                    else
                    {
                        if (Response.AccessToken == null || !Response.AccessTokenId.HasValue)
                        {
                            throw new Exception("Cannot get Access Token");
                        }
                        if (!skipIntro)
                        {
                            Process.Start(hosts.GlobalWebEndPoint() + "/app/my?intro&token=" + Response.AccessTokenId + ":" + Response.AccessToken);
                            skipIntro = true;
                        }
                        else
                        {
                            new Task(delegate
                            {
                                Process.Start(hosts.GlobalWebEndPoint() + "/app?token=" + Response.AccessTokenId + ":" + Response.AccessToken);
                            }).Start();
                        }
                    }
                })).Wait();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Exception: " + ex.ToString());
                if (!isSilentVersion)
                {
                    Process.Start(hosts.GlobalWebEndPoint() + "/app");
                }
            }
        }

        private void viewStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Task(delegate
            {
                OpenWebPage();
            }).Start();
        }

        private bool updatePrivateTime(DTLogData ld)
        {
            string current_session = session;
            bool successfulRequest = true;
            try
            {
                RESTService.GetResponse(new Uri(hosts.ActiveApiEndPoint() + "privatetime?session=" + current_session + "&now=" + DateTime.UtcNow.ConvertToUnixTimestamp() + "&start=" + ld.Start.ConvertToUnixTimestamp() + "&on=" + ld.Id + "&epr=" + HttpUtility.UrlEncode(hosts.ActiveEndPointRegion())), delegate (Response Response)
                {
                    if (Response == null)
                    {
                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + " No response!");
                        successfulRequest = false;
                        isOnline = false;
                    }
                    else if (Response.Error != null)
                    {
                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Error: " + Response.Error.Code + " " + Response.Error.Description);
                        if (Response.Error.Code == "401")
                        {
                            if (session == current_session)
                            {
                                ClearSession();
                            }
                        }
                        else
                        {
                            isOnline = false;
                        }
                        successfulRequest = false;
                    }
                    else
                    {
                        processDataUpdateResponse(Response);
                        ignorePrivateTimeFromServer = false;
                    }
                }).Wait();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "UpdatePrivateTime exception: " + ex.ToString());
                successfulRequest = false;
            }
            return successfulRequest;
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Task(delegate
            {
                if (pauseToolStripMenuItem.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText)
                {
                    pauseToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
                    DTLogData ld = new DTLogData
                    {
                        IsApp = false,
                        App = null,
                        Cmd = "private",
                        Id = 0
                    };
                    ignorePrivateTimeFromServer = false;
                    if (!updatePrivateTime(ld))
                    {
                        LLAddLD(ld);
                    }
                }
                else
                {
                    pauseToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                    privateTimeStarted = DateTime.UtcNow;
                    ignorePrivateTimeFromServer = true;
                    DTLogData ld2 = new DTLogData
                    {
                        IsApp = false,
                        App = null,
                        Cmd = "private",
                        Id = 1
                    };
                    if (!updatePrivateTime(ld2))
                    {
                        LLAddLD(ld2);
                    }
                }
                InfoIcon();
            }).Start();
        }

        private void internetPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Win32.LaunchConnectionDialog(IntPtr.Zero);
        }

        private void settingsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            checkForUpdateToolStripMenuItem.Enabled = !SelfUpdate.UpLock;
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", writable: true);
                if (registryKey != null)
                {
                    object value = registryKey.GetValue(Application.ProductName);
                    startWithWindowsToolStripMenuItem.Checked = value != null && value.ToString().ToUpperInvariant() == "\"" + Application.ExecutablePath.ToUpperInvariant() + "\"";
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Exception: " + ex.ToString());
            }
        }

        private void privateTimeRemind15_Click(object sender, EventArgs e)
        {
            privateTimeRemind = 15;
            RegistryService.SetValue("privateTimeRemind", privateTimeRemind.ToString());
        }

        private void privateTimeRemind30_Click(object sender, EventArgs e)
        {
            privateTimeRemind = 30;
            RegistryService.SetValue("privateTimeRemind", privateTimeRemind.ToString());
        }

        private void privateTimeRemind45_Click(object sender, EventArgs e)
        {
            privateTimeRemind = 45;
            RegistryService.SetValue("privateTimeRemind", privateTimeRemind.ToString());
        }

        private void privateTimeRemindDisable_Click(object sender, EventArgs e)
        {
            privateTimeRemind = 0;
            RegistryService.SetValue("privateTimeRemind", privateTimeRemind.ToString());
        }

        private void reminderToTakeBreak52_Click(object sender, EventArgs e)
        {
            breakRemind = 52;
            RegistryService.SetValue("breakRemind", breakRemind.ToString());
        }

        private void reminderToTakeBreak60_Click(object sender, EventArgs e)
        {
            breakRemind = 60;
            RegistryService.SetValue("breakRemind", breakRemind.ToString());
        }

        private void reminderToTakeBreak90_Click(object sender, EventArgs e)
        {
            breakRemind = 90;
            RegistryService.SetValue("breakRemind", breakRemind.ToString());
        }

        private void reminderToTakeBreakDisabled_Click(object sender, EventArgs e)
        {
            breakRemind = 0;
            RegistryService.SetValue("breakRemind", breakRemind.ToString());
        }

        private void NotifyMenuClearProjects()
        {
            for (int num = notifyMenu.Items.Count - 1; num >= 0; num--)
            {
                if (notifyMenu.Items[num].Name == "projectItem")
                {
                    notifyMenu.Items.RemoveAt(num);
                }
            }
        }

        private void notifyMenu_Opening(object sender, CancelEventArgs e)
        {
            lock (notifyMenulock)
            {
                privateTimeRemind15.Checked = privateTimeRemind == 15;
                privateTimeRemind30.Checked = privateTimeRemind == 30;
                privateTimeRemind45.Checked = privateTimeRemind == 45;
                privateTimeRemindDisable.Checked = privateTimeRemind == 0;
                reminderToTakeBreak52.Checked = breakRemind == 52;
                reminderToTakeBreak60.Checked = breakRemind == 60;
                reminderToTakeBreak90.Checked = breakRemind == 90;
                reminderToTakeBreakDisabled.Checked = breakRemind == 0;
                NotifyMenuClearProjects();
                if (disableLogOut)
                {
                    logoutToolStripMenuItem.Enabled = false;
                    exitToolStripMenuItem.Enabled = false;
                }
                else
                {
                    logoutToolStripMenuItem.Enabled = true;
                    exitToolStripMenuItem.Enabled = true;
                }
                if (session == null)
                {
                    viewStatisticsToolStripMenuItem.Enabled = false;
                    pauseToolStripMenuItem.Enabled = false;
                    logoutToolStripMenuItem.Visible = false;
                    loginToolStripMenuItem.Visible = true;
                    createNewProjectMenuItem.Enabled = false;
                    return;
                }
                viewStatisticsToolStripMenuItem.Enabled = true;
                pauseToolStripMenuItem.Enabled = true;
                logoutToolStripMenuItem.Visible = true;
                loginToolStripMenuItem.Visible = false;
                if (noop == 2 || noop == 8)
                {
                    createNewProjectMenuItem.Available = false;
                    return;
                }
                createNewProjectMenuItem.Available = true;
                createNewProjectMenuItem.Enabled = true;
                int s;
                for (s = 0; s < recentProjects.Count; s++)
                {
                    ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
                    toolStripMenuItem.ToolTipText = recentProjects[s].name;
                    toolStripMenuItem.Text = Tools.LimitByteLength(recentProjects[s].name, 50, " ...");
                    toolStripMenuItem.Name = "projectItem";
                    toolStripMenuItem.Click += recentProjectItemClicked;
                    if (recentProjects[s].tasks.Count > 0)
                    {
                        for (int num = recentProjects[s].tasks.Count - 1; num >= 0; num--)
                        {
                            ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem();
                            toolStripMenuItem2.Tag = recentProjects[s];
                            toolStripMenuItem2.Text = recentProjects[s].tasks[num].name;
                            toolStripMenuItem2.Click += TaskItemClicked;
                            toolStripMenuItem.DropDownItems.Add(toolStripMenuItem2);
                        }
                    }
                    else
                    {
                        ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem();
                        toolStripMenuItem2.Text = "No recent tasks";
                        toolStripMenuItem2.Enabled = false;
                        toolStripMenuItem.DropDownItems.Add(toolStripMenuItem2);
                    }
                    int num2 = allProjects.IndexOf(allProjects.Where((ProjectItem p) => p.id == recentProjects[s].id).FirstOrDefault());
                    if (num2 > -1 && allProjects[num2].tasks.Count > 0)
                    {
                        toolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
                        ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem();
                        toolStripMenuItem3.Text = "Show all";
                        for (int num3 = allProjects[num2].tasks.Count - 1; num3 >= 0; num3--)
                        {
                            ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem();
                            toolStripMenuItem2.Tag = allProjects[num2];
                            toolStripMenuItem2.Text = allProjects[num2].tasks[num3].name;
                            toolStripMenuItem2.Click += TaskItemClicked;
                            toolStripMenuItem3.DropDownItems.Add(toolStripMenuItem2);
                        }
                        toolStripMenuItem.DropDownItems.Add(toolStripMenuItem3);
                    }
                    notifyMenu.Items.Insert(11, toolStripMenuItem);
                }
            }
        }

        private void notifyMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (onboardingForm != null)
            {
                onboardingForm.Close();
                onboardingForm = null;
            }
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            if (session != null)
            {
                new Task(delegate
                {
                    OpenWebPage();
                }).Start();
            }
            else
            {
                PopUp();
            }
        }

        private void notifyMenu_Click(object sender, EventArgs e)
        {
            if (!notifyMenu.AutoClose)
            {
                notifyMenu.AutoClose = true;
                notifyMenu.Close();
            }
        }

        public void recentProjectItemClicked(object sender, EventArgs e)
        {
            ToolStripItem toolStripItem = sender as ToolStripItem;
            notifyMenu.Close();
            if (toolStripItem != null)
            {
                try
                {
                    StartProject(toolStripItem.ToolTipText, null);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Exception: " + ex.ToString());
                }
            }
        }

        public void TaskItemClicked(object sender, EventArgs e)
        {
            ToolStripItem toolStripItem = sender as ToolStripItem;
            if (toolStripItem != null)
            {
                try
                {
                    ProjectItem projectItem = toolStripItem.Tag as ProjectItem;
                    StartProject(projectItem.name, toolStripItem.Text);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Exception: " + ex.ToString());
                }
            }
        }

        private void proxySettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        public void createNewProjectMenuItem_Click(object sender, EventArgs e)
        {
            projWinShown = false;
            showProjWin(center: true, stop: true);
        }

        private void showProjWin(bool center = false, bool stop = false)
        {
            if (base.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    showProjWin(center, stop);
                });
            }
            else if (!projWinShown && recentProjectsUpdate > -1)
            {
                if (!isSilentVersion)
                {
                    projWin.ShowProjectWin(center, stop);
                }
                projWinShown = true;
            }
        }

        public void setProject()
        {
            if (notifyMenu.InvokeRequired)
            {
                notifyMenu.BeginInvoke(new MethodInvoker(setProject));
                return;
            }
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "setsetProject()");
            try
            {
                if (currentProjectName == null)
                {
                    projWinShown = false;
                    notifyIcon.Text = "DeskTime";
                    currentProjectMenuItem.Text = "";
                    stopProjectMenuItem.Text = "";
                    if (currentProjectMenuItem.Available)
                    {
                        currentProjectMenuItem.Available = false;
                        currentProjectMenuItem.Enabled = false;
                        stopProjectMenuItem.Available = false;
                        stopProjectSep.Available = false;
                        InfoIcon();
                    }
                }
                else
                {
                    int num = currentProjectTime / 3600;
                    int num2 = currentProjectTime % 3600 / 60;
                    currentProjectMenuItem.ToolTipText = currentProjectName;
                    if (currentTaskName != "")
                    {
                        ToolStripMenuItem toolStripMenuItem = currentProjectMenuItem;
                        toolStripMenuItem.ToolTipText = toolStripMenuItem.ToolTipText + " - " + currentTaskName;
                    }
                    notifyIcon.Text = Tools.LimitByteLength("DeskTime\r\n" + currentProjectName + ((currentTaskName != "") ? ("\r\n" + currentTaskName) : ""), 60, "...");
                    currentProjectMenuItem.Text = "Active project: " + Tools.LimitByteLength(currentProjectMenuItem.ToolTipText, 35, " ...");
                    stopProjectMenuItem.Text = "Stop project: " + num.ToString("D2") + ":" + num2.ToString("D2");
                    if (!currentProjectMenuItem.Available)
                    {
                        currentProjectMenuItem.Available = true;
                        currentProjectMenuItem.Enabled = true;
                        stopProjectMenuItem.Available = true;
                        stopProjectSep.Available = true;
                        InfoIcon();
                    }
                }
                lastProjectUiUpdate = secondsFromStart;
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "lastProjectUiUpdate: " + lastProjectUiUpdate);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Exception: " + ex.ToString());
                currentProjectTime = 0;
            }
        }

        public void addProjectToRecentList(ActiveProject activeProject)
        {
            bool flag = false;
            bool flag2 = false;
            for (int i = 0; i < recentProjects.Count; i++)
            {
                if (recentProjects[i].id != activeProject.Id)
                {
                    continue;
                }
                flag = true;
                if (activeProject.TaskId > 0)
                {
                    for (int j = 0; j < recentProjects[i].tasks.Count; j++)
                    {
                        if (recentProjects[i].tasks[j].id == activeProject.TaskId)
                        {
                            flag2 = true;
                            if (j < recentProjects[i].tasks.Count - 1)
                            {
                                TaskItem taskItem = recentProjects[i].tasks[j];
                                taskItem.name = activeProject.TaskName;
                                recentProjects[i].tasks.RemoveAt(j);
                                recentProjects[i].tasks.Add(taskItem);
                            }
                            break;
                        }
                    }
                    if (!flag2)
                    {
                        TaskItem taskItem2 = new TaskItem();
                        taskItem2.id = activeProject.TaskId;
                        taskItem2.name = activeProject.TaskName;
                        recentProjects[i].tasks.Add(taskItem2);
                        while (recentProjects[i].tasks.Count > 20)
                        {
                            recentProjects[i].tasks.RemoveAt(0);
                        }
                    }
                }
                if (i < recentProjects.Count - 1)
                {
                    ProjectItem projectItem = recentProjects[i];
                    projectItem.name = activeProject.Name;
                    recentProjects.RemoveAt(i);
                    recentProjects.Add(projectItem);
                }
                break;
            }
            if (!flag)
            {
                ProjectItem projectItem2 = new ProjectItem();
                projectItem2.id = activeProject.Id;
                projectItem2.name = activeProject.Name;
                projectItem2.tasks = new List<TaskItem>();
                if (activeProject.TaskId > 0)
                {
                    TaskItem taskItem3 = new TaskItem();
                    taskItem3.id = activeProject.TaskId;
                    taskItem3.name = activeProject.TaskName;
                    projectItem2.tasks.Add(taskItem3);
                }
                recentProjects.Add(projectItem2);
                while (recentProjects.Count > 20)
                {
                    recentProjects.RemoveAt(0);
                }
            }
        }

        private void updateProjectsList()
        {
            if (isSilentVersion || session == null)
            {
                return;
            }
            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Update porjects list");
            try
            {
                RESTService.GetResponse(new Uri(hosts.ActiveApiEndPoint() + "projects?session=" + session + "&epr=" + HttpUtility.UrlEncode(hosts.ActiveEndPointRegion())), delegate (Response Response)
                {
                    if (Response == null)
                    {
                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + " No response!");
                        isOnline = false;
                        InfoIcon();
                    }
                    else if (Response.Error != null)
                    {
                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Error: " + Response.Error.Code + " " + Response.Error.Description);
                        if (Response.Error.Code == "401")
                        {
                            ClearSession();
                        }
                        else
                        {
                            isOnline = false;
                            InfoIcon();
                        }
                    }
                    else
                    {
                        recentProjectsUpdate = Convert.ToInt32(Response.LastProjectListUpdate);
                        if (Response.Projects != null)
                        {
                            allProjects.Clear();
                            for (int i = 0; i < Response.Projects.Count; i++)
                            {
                                Project project = Response.Projects[i];
                                ProjectItem projectItem = new ProjectItem
                                {
                                    id = project.Id,
                                    name = project.Name,
                                    createdAt = project.CreatedAt,
                                    tasks = new List<TaskItem>()
                                };
                                for (int j = 0; j < project.Tasks.Count; j++)
                                {
                                    ProjectTask projectTask = project.Tasks[j];
                                    TaskItem item = new TaskItem
                                    {
                                        id = projectTask.Id,
                                        name = projectTask.Name
                                    };
                                    projectItem.tasks.Add(item);
                                }
                                allProjects.Add(projectItem);
                            }
                        }
                        if (Response.RecentProjects != null)
                        {
                            recentProjects.Clear();
                            for (int k = 0; k < Response.RecentProjects.Count; k++)
                            {
                                Project project2 = Response.RecentProjects[k];
                                ProjectItem projectItem2 = new ProjectItem
                                {
                                    id = project2.Id,
                                    name = project2.Name,
                                    createdAt = project2.CreatedAt,
                                    tasks = new List<TaskItem>()
                                };
                                for (int l = 0; l < project2.Tasks.Count; l++)
                                {
                                    ProjectTask projectTask2 = project2.Tasks[l];
                                    TaskItem item2 = new TaskItem
                                    {
                                        id = projectTask2.Id,
                                        name = projectTask2.Name
                                    };
                                    projectItem2.tasks.Add(item2);
                                }
                                recentProjects.Add(projectItem2);
                            }
                        }
                    }
                }).Wait();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Exception: " + ex.ToString());
                isOnline = false;
                InfoIcon();
            }
        }

        public bool StartProject(string projectName, string taskName)
        {
            projWinShown = false;
            bool result = false;
            try
            {
                DTLogData ld = new DTLogData
                {
                    IsApp = false,
                    App = projectName,
                    Title = taskName,
                    Cmd = "startProject"
                };
                LLAddLD(ld);
                currentProjectId = 1;
                currentProjectName = projectName;
                lastProjectName = projectName;
                currentTaskName = taskName;
                lastTaskName = taskName;
                currentProjectTime = 0;
                setProject();
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "projectstart Exception: " + ex.ToString());
                isOnline = false;
                InfoIcon();
                return result;
            }
        }

        private bool stopProject()
        {
            DTLogData ld = new DTLogData
            {
                IsApp = false,
                App = null,
                Title = null,
                Cmd = "stopProject"
            };
            LLAddLD(ld);
            currentProjectId = 0;
            currentProjectName = "";
            lastProjectName = "";
            currentTaskName = "";
            lastTaskName = "";
            currentProjectTime = 0;
            setProject();
            return true;
        }

        public void stopProjectMenuItem_Click(object sender, EventArgs e)
        {
            if (!stopProject())
            {
                DTLogData dTLogData = new DTLogData();
                dTLogData.IsApp = false;
                dTLogData.App = null;
                LLAddLD(dTLogData);
            }
            currentProjectId = 0;
            currentProjectName = null;
            currentTaskName = null;
            currentProjectTime = 0;
            setProject();
        }

        private void fb_login_Click(object sender, EventArgs e)
        {
            Process.Start(hosts.GlobalWebEndPoint() + "/auth/login/facebook?app_id=22");
        }

        private void twi_login_Click(object sender, EventArgs e)
        {
            Process.Start(hosts.GlobalWebEndPoint() + "/auth/login/twitter?app_id=22");
        }

        private void dlv_login_click(object sender, EventArgs e)
        {
            Process.Start(hosts.GlobalWebEndPoint() + "/auth/login/draugiem?app_id=22");
        }

        private void linkedin_login_Click(object sender, EventArgs e)
        {
            Process.Start(hosts.GlobalWebEndPoint() + "/auth/login/linkedin?app_id=22");
        }

        private void gg_login_Click(object sender, EventArgs e)
        {
            Process.Start(hosts.GlobalWebEndPoint() + "/auth/login/google?app_id=22");
        }

        private void dt_login_Click(object sender, EventArgs e)
        {
            Process.Start(hosts.GlobalWebEndPoint() + "/auth/login/magic?app_id=22");
        }

        private void currentProjectMenuItem_Click(object sender, EventArgs e)
        {
            projWin.Visible = true;
        }

        private void pingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RESTService.GetPing(new Uri(hosts.ActiveApiEndPoint() + "ping?version=" + HttpUtility.UrlEncode(Version.ToString() + (isSilentVersion ? " s" : "") + (isMsi ? " m" : "")) + "&name=" + HttpUtility.UrlEncode(Environment.MachineName) + "&epr=" + HttpUtility.UrlEncode(hosts.ActiveEndPointRegion())));
        }

        private void aboutDeskTimeClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.About();
        }

        private void ProcessLoginResponse(Response response)
        {
            if (response.Session != null && response.Session != "" && response.Name != null && response.Name != "")
            {
                session = response.Session;
                RegistryService.SetValue("Session", session);
                base.WindowState = FormWindowState.Minimized;
                isOnline = true;
                InfoIcon();
                string name = response.Name;
                userToolStripMenuItem.Text = string.Format(Resources.UserName, name);
                userToolStripMenuItem.Available = name != null;
                RegistryService.SetValue("Name", name);
                ShowIntroBalloon("DeskTime Intro", "This is your DeskTime app.");
            }
        }

        public async void MagicLogin()
        {
            try
            {
                object value = RegistryService.GetValue("mlt");
                if (value == null)
                {
                    return;
                }
                RegistryService.DeleteValue("mlt");
                Uri uri = new Uri(string.Format(hosts.ActiveApiEndPoint() + "login/magic"));
                string postData = $"app_id=22&token={HttpUtility.UrlEncode(value.ToString())}&epr={HttpUtility.UrlEncode(hosts.ActiveEndPointRegion())}";
                await RESTService.PostResponse(uri, postData, delegate (Response Response)
                {
                    if (Response == null)
                    {
                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + " No response!");
                        _ = isSilentVersion;
                    }
                    else if (Response.Error != null)
                    {
                        if (!isSilentVersion)
                        {
                            MessageBox.Show("Unable to connect to the DeskTime server(Error: " + Response.Error.Code + " " + Response.Error.Description + ")", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                    else
                    {
                        if (Response.Session == null)
                        {
                            throw new Exception("Cannot get Access Token");
                        }
                        ProcessLoginResponse(Response);
                    }
                });
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Exception: " + ex.ToString());
            }
        }

        private void timerMagicLink_Tick(object sender, EventArgs e)
        {
            MagicLogin();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeskTime.MainWin));
            notifyIcon = new System.Windows.Forms.NotifyIcon(components);
            notifyMenu = new System.Windows.Forms.ContextMenuStrip(components);
            devVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            statusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            userToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            viewStatisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            stopProjectSep = new System.Windows.Forms.ToolStripSeparator();
            createNewProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            stopProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            currentProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuRecentProjects = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            startWithWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            internetPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            checkForUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            hideTimeWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            proxySettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aboutDeskTimeClientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            privateTimeReminder = new System.Windows.Forms.ToolStripMenuItem();
            privateTimeRemind15 = new System.Windows.Forms.ToolStripMenuItem();
            privateTimeRemind30 = new System.Windows.Forms.ToolStripMenuItem();
            privateTimeRemind45 = new System.Windows.Forms.ToolStripMenuItem();
            privateTimeRemindDisable = new System.Windows.Forms.ToolStripMenuItem();
            reminderToTakeBreak = new System.Windows.Forms.ToolStripMenuItem();
            reminderToTakeBreak52 = new System.Windows.Forms.ToolStripMenuItem();
            reminderToTakeBreak60 = new System.Windows.Forms.ToolStripMenuItem();
            reminderToTakeBreak90 = new System.Windows.Forms.ToolStripMenuItem();
            reminderToTakeBreakDisabled = new System.Windows.Forms.ToolStripMenuItem();
            privateTimeToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            browserCancelPanel = new System.Windows.Forms.Panel();
            browserCancelButton = new System.Windows.Forms.Button();
            timerHooker = new System.Windows.Forms.Timer(components);
            timerUp = new System.Windows.Forms.Timer(components);
            timer = new System.Windows.Forms.Timer(components);
            loginPanel = new System.Windows.Forms.Panel();
            dt_login = new System.Windows.Forms.Button();
            gg_login = new System.Windows.Forms.Button();
            linkedin_login = new System.Windows.Forms.Button();
            native_login = new System.Windows.Forms.Button();
            forgotPassword = new System.Windows.Forms.LinkLabel();
            nativeEmail = new System.Windows.Forms.TextBox();
            nativePassword = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            twi_login = new System.Windows.Forms.Button();
            fb_login = new System.Windows.Forms.Button();
            IE_panel = new System.Windows.Forms.Panel();
            timerMagicLink = new System.Windows.Forms.Timer(components);
            notifyMenu.SuspendLayout();
            browserCancelPanel.SuspendLayout();
            loginPanel.SuspendLayout();
            IE_panel.SuspendLayout();
            SuspendLayout();
            notifyIcon.ContextMenuStrip = notifyMenu;
            notifyIcon.Icon = (System.Drawing.Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "DeskTime";
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);
            notifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[21]
            {
                devVersionToolStripMenuItem, statusToolStripMenuItem, userToolStripMenuItem, toolStripSeparator4, viewStatisticsToolStripMenuItem, stopProjectSep, createNewProjectMenuItem, stopProjectMenuItem, currentProjectMenuItem, toolStripSeparator1,
                toolStripMenuRecentProjects, toolStripSeparator2, settingsToolStripMenuItem, toolStripSeparator3, pauseToolStripMenuItem, privateTimeReminder, reminderToTakeBreak, privateTimeToolStripSeparator, loginToolStripMenuItem, logoutToolStripMenuItem,
                exitToolStripMenuItem
            });
            notifyMenu.Name = "notifyMenu";
            notifyMenu.Size = new System.Drawing.Size(271, 392);
            notifyMenu.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(notifyMenu_Closing);
            notifyMenu.Opening += new System.ComponentModel.CancelEventHandler(notifyMenu_Opening);
            notifyMenu.Click += new System.EventHandler(notifyMenu_Click);
            devVersionToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            devVersionToolStripMenuItem.ForeColor = System.Drawing.Color.DarkRed;
            devVersionToolStripMenuItem.Name = "devVersionToolStripMenuItem";
            devVersionToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            devVersionToolStripMenuItem.Text = "Development Version";
            devVersionToolStripMenuItem.Visible = false;
            statusToolStripMenuItem.Enabled = false;
            statusToolStripMenuItem.Name = "statusToolStripMenuItem";
            statusToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            statusToolStripMenuItem.Text = "...";
            statusToolStripMenuItem.Visible = false;
            userToolStripMenuItem.Enabled = false;
            userToolStripMenuItem.Name = "userToolStripMenuItem";
            userToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            userToolStripMenuItem.Text = "...";
            userToolStripMenuItem.Visible = false;
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(267, 6);
            viewStatisticsToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 186);
            viewStatisticsToolStripMenuItem.Name = "viewStatisticsToolStripMenuItem";
            viewStatisticsToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            viewStatisticsToolStripMenuItem.Text = "&Show DeskTime";
            viewStatisticsToolStripMenuItem.Click += new System.EventHandler(viewStatisticsToolStripMenuItem_Click);
            stopProjectSep.Name = "stopProjectSep";
            stopProjectSep.Size = new System.Drawing.Size(267, 6);
            stopProjectSep.Visible = false;
            createNewProjectMenuItem.Name = "createNewProjectMenuItem";
            createNewProjectMenuItem.Size = new System.Drawing.Size(270, 22);
            createNewProjectMenuItem.Text = "Create a project / Search for a project";
            createNewProjectMenuItem.Click += new System.EventHandler(createNewProjectMenuItem_Click);
            stopProjectMenuItem.Image = DeskTime.Properties.Resources.project_started;
            stopProjectMenuItem.Name = "stopProjectMenuItem";
            stopProjectMenuItem.Size = new System.Drawing.Size(270, 22);
            stopProjectMenuItem.Text = "Stop \"ProjectName\" timer";
            stopProjectMenuItem.Visible = false;
            stopProjectMenuItem.Click += new System.EventHandler(stopProjectMenuItem_Click);
            currentProjectMenuItem.Enabled = false;
            currentProjectMenuItem.Name = "currentProjectMenuItem";
            currentProjectMenuItem.Size = new System.Drawing.Size(270, 22);
            currentProjectMenuItem.Text = "Active project: ";
            currentProjectMenuItem.Visible = false;
            currentProjectMenuItem.Click += new System.EventHandler(currentProjectMenuItem_Click);
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(267, 6);
            toolStripMenuRecentProjects.BackColor = System.Drawing.SystemColors.Control;
            toolStripMenuRecentProjects.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            toolStripMenuRecentProjects.Enabled = false;
            toolStripMenuRecentProjects.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            toolStripMenuRecentProjects.Name = "toolStripMenuRecentProjects";
            toolStripMenuRecentProjects.Size = new System.Drawing.Size(270, 22);
            toolStripMenuRecentProjects.Text = "Recent projects and tasks";
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(267, 6);
            settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[7] { startWithWindowsToolStripMenuItem, internetPropertiesToolStripMenuItem, checkForUpdateToolStripMenuItem, hideTimeWindowToolStripMenuItem, proxySettingsToolStripMenuItem, pingToolStripMenuItem, aboutDeskTimeClientToolStripMenuItem });
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            settingsToolStripMenuItem.Text = "&Options";
            settingsToolStripMenuItem.DropDownOpening += new System.EventHandler(settingsToolStripMenuItem_DropDownOpening);
            startWithWindowsToolStripMenuItem.Name = "startWithWindowsToolStripMenuItem";
            startWithWindowsToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            startWithWindowsToolStripMenuItem.Text = "Start with &Windows";
            startWithWindowsToolStripMenuItem.Click += new System.EventHandler(startWithWindowsToolStripMenuItem_Click);
            internetPropertiesToolStripMenuItem.Name = "internetPropertiesToolStripMenuItem";
            internetPropertiesToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            internetPropertiesToolStripMenuItem.Text = "&Internet properties";
            internetPropertiesToolStripMenuItem.Click += new System.EventHandler(internetPropertiesToolStripMenuItem_Click);
            checkForUpdateToolStripMenuItem.Name = "checkForUpdateToolStripMenuItem";
            checkForUpdateToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            checkForUpdateToolStripMenuItem.Text = "&Check for update";
            checkForUpdateToolStripMenuItem.Click += new System.EventHandler(checkForUpdateToolStripMenuItem_Click);
            hideTimeWindowToolStripMenuItem.Name = "hideTimeWindowToolStripMenuItem";
            hideTimeWindowToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            hideTimeWindowToolStripMenuItem.Text = "&Hide time window";
            hideTimeWindowToolStripMenuItem.Visible = false;
            proxySettingsToolStripMenuItem.Enabled = false;
            proxySettingsToolStripMenuItem.Name = "proxySettingsToolStripMenuItem";
            proxySettingsToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            proxySettingsToolStripMenuItem.Text = "Proxy settings";
            proxySettingsToolStripMenuItem.Click += new System.EventHandler(proxySettingsToolStripMenuItem_Click);
            pingToolStripMenuItem.Name = "pingToolStripMenuItem";
            pingToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            pingToolStripMenuItem.Text = "Ping";
            pingToolStripMenuItem.Click += new System.EventHandler(pingToolStripMenuItem_Click);
            aboutDeskTimeClientToolStripMenuItem.Name = "aboutDeskTimeClientToolStripMenuItem";
            aboutDeskTimeClientToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            aboutDeskTimeClientToolStripMenuItem.Text = "About DeskTime client";
            aboutDeskTimeClientToolStripMenuItem.Click += new System.EventHandler(aboutDeskTimeClientToolStripMenuItem_Click);
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(267, 6);
            pauseToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            pauseToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("pauseToolStripMenuItem.Image");
            pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            pauseToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            pauseToolStripMenuItem.Text = "&Private time";
            pauseToolStripMenuItem.Click += new System.EventHandler(pauseToolStripMenuItem_Click);
            privateTimeReminder.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[4] { privateTimeRemind15, privateTimeRemind30, privateTimeRemind45, privateTimeRemindDisable });
            privateTimeReminder.Name = "privateTimeReminder";
            privateTimeReminder.Size = new System.Drawing.Size(270, 22);
            privateTimeReminder.Text = "Private time reminder";
            privateTimeRemind15.Name = "privateTimeRemind15";
            privateTimeRemind15.Size = new System.Drawing.Size(119, 22);
            privateTimeRemind15.Text = "15 min";
            privateTimeRemind15.Click += new System.EventHandler(privateTimeRemind15_Click);
            privateTimeRemind30.Name = "privateTimeRemind30";
            privateTimeRemind30.Size = new System.Drawing.Size(119, 22);
            privateTimeRemind30.Text = "30 min";
            privateTimeRemind30.Click += new System.EventHandler(privateTimeRemind30_Click);
            privateTimeRemind45.Name = "privateTimeRemind45";
            privateTimeRemind45.Size = new System.Drawing.Size(119, 22);
            privateTimeRemind45.Text = "45min";
            privateTimeRemind45.Click += new System.EventHandler(privateTimeRemind45_Click);
            privateTimeRemindDisable.Name = "privateTimeRemindDisable";
            privateTimeRemindDisable.Size = new System.Drawing.Size(119, 22);
            privateTimeRemindDisable.Text = "Disabled";
            privateTimeRemindDisable.Click += new System.EventHandler(privateTimeRemindDisable_Click);
            reminderToTakeBreak.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[4] { reminderToTakeBreak52, reminderToTakeBreak60, reminderToTakeBreak90, reminderToTakeBreakDisabled });
            reminderToTakeBreak.Name = "reminderToTakeBreak";
            reminderToTakeBreak.Size = new System.Drawing.Size(270, 22);
            reminderToTakeBreak.Text = "Reminder to take break";
            reminderToTakeBreak52.Name = "reminderToTakeBreak52";
            reminderToTakeBreak52.Size = new System.Drawing.Size(202, 22);
            reminderToTakeBreak52.Text = "52 min (Recommended)";
            reminderToTakeBreak52.Click += new System.EventHandler(reminderToTakeBreak52_Click);
            reminderToTakeBreak60.Name = "reminderToTakeBreak60";
            reminderToTakeBreak60.Size = new System.Drawing.Size(202, 22);
            reminderToTakeBreak60.Text = "60 min";
            reminderToTakeBreak60.Click += new System.EventHandler(reminderToTakeBreak60_Click);
            reminderToTakeBreak90.Name = "reminderToTakeBreak90";
            reminderToTakeBreak90.Size = new System.Drawing.Size(202, 22);
            reminderToTakeBreak90.Text = "90 min";
            reminderToTakeBreak90.Click += new System.EventHandler(reminderToTakeBreak90_Click);
            reminderToTakeBreakDisabled.Name = "reminderToTakeBreakDisabled";
            reminderToTakeBreakDisabled.Size = new System.Drawing.Size(202, 22);
            reminderToTakeBreakDisabled.Text = "Disabled";
            reminderToTakeBreakDisabled.Click += new System.EventHandler(reminderToTakeBreakDisabled_Click);
            privateTimeToolStripSeparator.Name = "privateTimeToolStripSeparator";
            privateTimeToolStripSeparator.Size = new System.Drawing.Size(267, 6);
            loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            loginToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            loginToolStripMenuItem.Text = "Log&in";
            loginToolStripMenuItem.Click += new System.EventHandler(loginToolStripMenuItem_Click);
            logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            logoutToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            logoutToolStripMenuItem.Text = "&Logout";
            logoutToolStripMenuItem.Click += new System.EventHandler(logoutToolStripMenuItem_Click);
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            exitToolStripMenuItem.Text = "&Quit DeskTime";
            exitToolStripMenuItem.Click += new System.EventHandler(exitToolStripMenuItem_Click);
            browserCancelPanel.Controls.Add(browserCancelButton);
            browserCancelPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            browserCancelPanel.Location = new System.Drawing.Point(0, 146);
            browserCancelPanel.Name = "browserCancelPanel";
            browserCancelPanel.Size = new System.Drawing.Size(547, 28);
            browserCancelPanel.TabIndex = 6;
            browserCancelButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            browserCancelButton.Location = new System.Drawing.Point(3, 2);
            browserCancelButton.Name = "browserCancelButton";
            browserCancelButton.Size = new System.Drawing.Size(75, 23);
            browserCancelButton.TabIndex = 0;
            browserCancelButton.TabStop = false;
            browserCancelButton.Text = "Cancel";
            browserCancelButton.UseVisualStyleBackColor = true;
            timerHooker.Interval = 300000;
            timerHooker.Tick += new System.EventHandler(timerHookerTick);
            timerUp.Enabled = true;
            timerUp.Interval = 3600000;
            timerUp.Tick += new System.EventHandler(timerUpTick);
            timer.Enabled = true;
            timer.Interval = 2000;
            timer.Tick += new System.EventHandler(timer_Tick);
            loginPanel.BackgroundImage = (System.Drawing.Image)resources.GetObject("loginPanel.BackgroundImage");
            loginPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            loginPanel.Controls.Add(dt_login);
            loginPanel.Controls.Add(gg_login);
            loginPanel.Controls.Add(linkedin_login);
            loginPanel.Controls.Add(native_login);
            loginPanel.Controls.Add(forgotPassword);
            loginPanel.Controls.Add(nativeEmail);
            loginPanel.Controls.Add(nativePassword);
            loginPanel.Controls.Add(label2);
            loginPanel.Controls.Add(label1);
            loginPanel.Controls.Add(twi_login);
            loginPanel.Controls.Add(fb_login);
            loginPanel.Location = new System.Drawing.Point(0, 3);
            loginPanel.Name = "loginPanel";
            loginPanel.Size = new System.Drawing.Size(547, 175);
            loginPanel.TabIndex = 0;
            dt_login.Image = DeskTime.Properties.Resources.Logo_Symbol;
            dt_login.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            dt_login.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            dt_login.Location = new System.Drawing.Point(29, 12);
            dt_login.Name = "dt_login";
            dt_login.Size = new System.Drawing.Size(214, 22);
            dt_login.TabIndex = 22;
            dt_login.Text = "Connect with DeskTime";
            dt_login.UseVisualStyleBackColor = true;
            dt_login.Click += new System.EventHandler(dt_login_Click);
            gg_login.Image = (System.Drawing.Image)resources.GetObject("gg_login.Image");
            gg_login.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            gg_login.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            gg_login.Location = new System.Drawing.Point(29, 38);
            gg_login.Name = "gg_login";
            gg_login.Size = new System.Drawing.Size(214, 22);
            gg_login.TabIndex = 21;
            gg_login.Text = "Connect with Google";
            gg_login.UseVisualStyleBackColor = true;
            gg_login.Click += new System.EventHandler(gg_login_Click);
            linkedin_login.Image = (System.Drawing.Image)resources.GetObject("linkedin_login.Image");
            linkedin_login.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            linkedin_login.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            linkedin_login.Location = new System.Drawing.Point(29, 116);
            linkedin_login.Name = "linkedin_login";
            linkedin_login.Size = new System.Drawing.Size(214, 22);
            linkedin_login.TabIndex = 20;
            linkedin_login.Text = "Connect with LinkedIn";
            linkedin_login.UseVisualStyleBackColor = true;
            linkedin_login.Click += new System.EventHandler(linkedin_login_Click);
            native_login.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            native_login.Location = new System.Drawing.Point(468, 105);
            native_login.Name = "native_login";
            native_login.Size = new System.Drawing.Size(56, 23);
            native_login.TabIndex = 13;
            native_login.Text = "Sign in";
            native_login.UseVisualStyleBackColor = true;
            native_login.Click += new System.EventHandler(native_login_Click);
            forgotPassword.ActiveLinkColor = System.Drawing.Color.Gray;
            forgotPassword.AutoSize = true;
            forgotPassword.BackColor = System.Drawing.Color.Transparent;
            forgotPassword.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            forgotPassword.LinkColor = System.Drawing.Color.Silver;
            forgotPassword.Location = new System.Drawing.Point(352, 110);
            forgotPassword.Name = "forgotPassword";
            forgotPassword.Size = new System.Drawing.Size(91, 13);
            forgotPassword.TabIndex = 14;
            forgotPassword.TabStop = true;
            forgotPassword.Text = "Forgot password?";
            forgotPassword.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(forgotPassword_LinkClicked);
            nativeEmail.Location = new System.Drawing.Point(355, 42);
            nativeEmail.Name = "nativeEmail";
            nativeEmail.Size = new System.Drawing.Size(169, 20);
            nativeEmail.TabIndex = 11;
            nativePassword.Location = new System.Drawing.Point(355, 76);
            nativePassword.Name = "nativePassword";
            nativePassword.PasswordChar = '*';
            nativePassword.Size = new System.Drawing.Size(169, 20);
            nativePassword.TabIndex = 12;
            nativePassword.KeyDown += new System.Windows.Forms.KeyEventHandler(nativePassword_KeyDown);
            label2.AutoSize = true;
            label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            label2.Location = new System.Drawing.Point(296, 79);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(53, 13);
            label2.TabIndex = 19;
            label2.Text = "Password";
            label1.AutoSize = true;
            label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            label1.Location = new System.Drawing.Point(296, 45);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(32, 13);
            label1.TabIndex = 18;
            label1.Text = "Email";
            twi_login.Image = (System.Drawing.Image)resources.GetObject("twi_login.Image");
            twi_login.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            twi_login.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            twi_login.Location = new System.Drawing.Point(29, 90);
            twi_login.Name = "twi_login";
            twi_login.Size = new System.Drawing.Size(214, 23);
            twi_login.TabIndex = 16;
            twi_login.Text = "Connect with Twitter";
            twi_login.UseVisualStyleBackColor = true;
            twi_login.Click += new System.EventHandler(twi_login_Click);
            fb_login.Image = (System.Drawing.Image)resources.GetObject("fb_login.Image");
            fb_login.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            fb_login.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            fb_login.Location = new System.Drawing.Point(29, 64);
            fb_login.Name = "fb_login";
            fb_login.Size = new System.Drawing.Size(214, 22);
            fb_login.TabIndex = 15;
            fb_login.Text = "Connect with Facebook";
            fb_login.UseVisualStyleBackColor = true;
            fb_login.Click += new System.EventHandler(fb_login_Click);
            IE_panel.Controls.Add(loginPanel);
            IE_panel.Location = new System.Drawing.Point(0, 0);
            IE_panel.Name = "IE_panel";
            IE_panel.Size = new System.Drawing.Size(999, 488);
            IE_panel.TabIndex = 1;
            timerMagicLink.Interval = 3500;
            timerMagicLink.Tick += new System.EventHandler(timerMagicLink_Tick);
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(252, 252, 252);
            base.ClientSize = new System.Drawing.Size(547, 174);
            base.Controls.Add(browserCancelPanel);
            base.Controls.Add(IE_panel);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "MainWin";
            base.ShowInTaskbar = false;
            base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Main Window";
            base.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(MainWin_FormClosed);
            base.Load += new System.EventHandler(MainWin_Load);
            base.Resize += new System.EventHandler(MainWin_Resize);
            notifyMenu.ResumeLayout(false);
            browserCancelPanel.ResumeLayout(false);
            loginPanel.ResumeLayout(false);
            loginPanel.PerformLayout();
            IE_panel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
