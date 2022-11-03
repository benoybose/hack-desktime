using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using CrashReporterDotNET;

namespace DeskTime
{
    internal static class Program
    {
        private static ReportCrash _reportCrash;

        [STAThread]
        private static void Main()
        {
            Application.ThreadException += delegate (object sender, ThreadExceptionEventArgs args)
            {
                SendReportSilently(args.Exception);
            };
            AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs args)
            {
                SendReportSilently((Exception)args.ExceptionObject);
            };
            _reportCrash = new ReportCrash("mikus@desktime.com")
            {
                Silent = true,
                IncludeScreenshot = false,
                AnalyzeWithDoctorDump = true,
                DoctorDumpSettings = new DoctorDumpSettings
                {
                    ApplicationID = new Guid("eb18fde4-5e8d-4563-b31c-2441fc287cd5"),
                    OpenReportInBrowser = true
                }
            };
            try
            {
                string text = "";
                if (Environment.GetCommandLineArgs().Length > 1)
                {
                    text = Environment.GetCommandLineArgs()[1];
                    if (text.Contains("desktime://magic-login/"))
                    {
                        string text2 = text.Replace("desktime://magic-login/", "");
                        if (text2 != "")
                        {
                            RegistryService.SetValue("mlt", text2);
                        }
                    }
                }
                bool createdNew = true;
                int wmDesktime = Win32.RegisterWindowMessage("{7096C69E-67B0-425E-8D11-485A7C74A337}");
                using Mutex mutex = new Mutex(initiallyOwned: true, "{7096C69E-67B0-425E-8D11-485A7C74A337}", out createdNew);
                if (!createdNew)
                {
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(defaultValue: false);
                try
                {
                    Application.Run(new MainWin
                    {
                        WmDesktime = wmDesktime
                    });
                }
                catch (Exception)
                {
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            catch (Exception exception)
            {
                SendReportSilently(exception);
            }
        }

        public static void SendReport(Exception exception, string developerMessage = "")
        {
            _reportCrash.DeveloperMessage = developerMessage;
            _reportCrash.Silent = false;
            _reportCrash.Send(exception);
        }

        public static void SendReportSilently(Exception exception, string developerMessage = "")
        {
            developerMessage += "\r\n";
            developerMessage = developerMessage + "DeskTime version: " + MainWin.Version.ToString() + "\r\n";
            developerMessage = developerMessage + "DeskTime user ID: " + MainWin.ActiveUserId + "\r\n";
            developerMessage = developerMessage + "Computer name: " + Environment.MachineName + "\r\n";
            if (exception is WebException)
            {
                string text = "";
                text = text + "DeskTime version: " + MainWin.Version.ToString() + "\r\n";
                text = text + "Operating system: " + Environment.OSVersion.ToString() + "\r\n";
                text = text + "Computer name: " + Environment.MachineName + "\r\n";
                text = text + "CLR runtime version: " + Environment.Version.ToString() + "\r\n";
                text += "ExceptionType: ThreadException\r\n";
                text = text + "Exception: " + exception.ToString() + "\r\n";
                text = text + "Backtrace: " + MainWin.Backtrace + "\r\n";
                RegistryService.SetValue("Last Error", text);
            }
            else
            {
                try
                {
                    _reportCrash.DeveloperMessage = developerMessage;
                    _reportCrash.Silent = true;
                    _reportCrash.Send(exception);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
