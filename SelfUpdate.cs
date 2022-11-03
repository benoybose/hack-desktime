#define TRACE
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using DeskTime.Properties;

namespace DeskTime
{
    internal class SelfUpdate
    {
        public static volatile bool UpLock;

        public static void StartUpdater(bool silent)
        {
            Thread thread = new Thread(StartUpdater);
            thread.IsBackground = true;
            thread.Start(silent);
        }

        private static void StartUpdater(object data)
        {
            bool silent = (bool)data;
            if (UpLock)
            {
                return;
            }
            try
            {
                UpLock = true;
                try
                {
                    string text = RegistryService.GetValue("Remove") as string;
                    if (!string.IsNullOrEmpty(text))
                    {
                        RegistryService.DeleteValue("Remove");
                        File.Delete(text);
                    }
                }
                catch (Exception)
                {
                }
                string text2 = MainWin.hosts.ActiveWebEndPoint() + "/updates/win/version/?json=true&current=" + MainWin.Version;
                if (!silent)
                {
                    text2 += "&loud=1";
                }
                Version ignore = null;
                Version newVersion = new Version("0.0.0");
                string updateUrl = null;
                RESTService.GetResponse(new Uri(string.Format(text2)), delegate (Response Response)
                {
                    if (Response == null)
                    {
                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + " No SelfUpdate response!");
                    }
                    else
                    {
                        newVersion = new Version(Response.Version);
                        updateUrl = Response.Url;
                        if (silent)
                        {
                            ignore = new Version(RegistryService.GetValue("IgnoreVersion", "0.0.0") as string);
                        }
                        if (newVersion > MainWin.Version && newVersion != ignore)
                        {
                            string text3 = DownloadUpdate(new Uri(updateUrl));
                            try
                            {
                                SaveNoExRegistry("Remove", text3);
                                if (silent)
                                {
                                    Process.Start(text3, "/verysilent");
                                }
                                else
                                {
                                    Process.Start(text3, "/silent");
                                }
                            }
                            catch (Exception ex3)
                            {
                                if (!silent)
                                {
                                    ShowInfo(new UpdateDlg(), Resources.UpdateRunFailedTitle, Resources.UpdateRunFailed);
                                }
                                RegistryService.SetValue("Last Error", MainWin.Version.ToString() + "\r\n" + ex3.ToString());
                            }
                        }
                        else if (!silent)
                        {
                            ShowInfo(new UpdateDlg(), Resources.NotAvailableTitle, string.Format(Resources.NotAvailable, MainWin.Version));
                        }
                    }
                }).Wait();
            }
            catch (Exception)
            {
                if (!silent)
                {
                    ShowInfo(new UpdateDlg(), Resources.WebFailedTitle, Resources.WebFailed);
                }
            }
            finally
            {
                UpLock = false;
            }
        }

        public static void ShowInfo(UpdateDlg ud, string title, string content)
        {
            ud.Text = title;
            ud.lbInfo.Text = content;
            ud.btYes.Visible = false;
            ud.btNo.Visible = false;
            ud.btOk.Visible = true;
            ud.AcceptButton = ud.btOk;
            ud.CancelButton = ud.btOk;
            ud.ShowDialog();
        }

        private static void SaveNoExRegistry(string key, object value)
        {
            try
            {
                RegistryService.SetValue(key, value);
            }
            catch (Exception)
            {
            }
        }

        public static string DownloadUpdate(Uri uri)
        {
            WebRequest webRequest = WebRequest.Create(uri);
            webRequest.Proxy = RESTHosts.ProxySettings();
            WebResponse response = webRequest.GetResponse();
            string tempFileName = Path.GetTempFileName();
            Stream responseStream = response.GetResponseStream();
            FileStream writeStream = new FileStream(tempFileName, FileMode.Create);
            ReadWriteStream(responseStream, writeStream);
            string text = tempFileName + ".exe";
            File.Move(tempFileName, text);
            return text;
        }

        private static void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int num = 256;
            byte[] buffer = new byte[num];
            for (int num2 = readStream.Read(buffer, 0, num); num2 > 0; num2 = readStream.Read(buffer, 0, num))
            {
                writeStream.Write(buffer, 0, num2);
            }
            readStream.Close();
            writeStream.Close();
        }
    }
}
