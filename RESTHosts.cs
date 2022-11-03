#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DeskTime
{
    internal class RESTHosts
    {
        private readonly bool isDev;

        public long lastConnectionErrorTime;

        public long availabilityCheckTime;

        private static readonly object checkServersLock = new object();

        private readonly string apiPath = "/api/v3/json/";

        private readonly List<Server> servers = new List<Server>();

        private static readonly HttpClient client = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate)
        });

        public RESTHosts(bool isDev)
        {
            this.isDev = isDev;
            Trace.WriteLine("Is DEV: " + isDev);
            ServicePointManager.DefaultConnectionLimit = 20;
            ServicePointManager.DnsRefreshTimeout = 0;
            if (this.isDev)
            {
                ServicePointManager.ServerCertificateValidationCallback = (object senderX, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
                servers.Add(new Server
                {
                    Id = 1,
                    Host = "https://local.desktime.com",
                    Region = "GLOBAL"
                });
            }
            else
            {
                servers.Add(new Server
                {
                    Id = 1,
                    Host = "https://desktime.com",
                    Region = "GLOBAL"
                });
                servers.Add(new Server
                {
                    Id = 2,
                    Host = "https://in.desktime.com",
                    Region = "ASIA"
                });
                servers.Add(new Server
                {
                    Id = 3,
                    Host = "https://us.desktime.com",
                    Region = "US"
                });
            }
            client.DefaultRequestHeaders.UserAgent.ParseAdd("DeskTime Windows Client v" + MainWin.Version);
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            client.DefaultRequestHeaders.ConnectionClose = true;
        }

        private string ActiveEndPoint(string param = "Host")
        {
            Server server = servers.First();
            return (string)server.GetType().GetProperty(param).GetValue(server, null);
        }

        public string ActiveWebEndPoint()
        {
            return ActiveEndPoint();
        }

        public string ActiveApiEndPoint()
        {
            return ActiveEndPoint() + apiPath;
        }

        public string GlobalWebEndPoint()
        {
            return servers.First().Host;
        }

        public string GlobalApiEndPoint()
        {
            return servers.First().Host + apiPath;
        }

        public string ActiveEndPointRegion()
        {
            return ActiveEndPoint("Region");
        }

        public static IWebProxy ProxySettings()
        {
            try
            {
                Uri uri = new Uri(MainWin.hosts.GlobalWebEndPoint());
                IWebProxy systemWebProxy = WebRequest.GetSystemWebProxy();
                systemWebProxy.Credentials = CredentialCache.DefaultCredentials.GetCredential(uri, "Basic");
                return systemWebProxy;
            }
            catch (Exception ex)
            {
                IWebProxy systemWebProxy = new WebProxy();
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Proxy Exception: " + ex.ToString());
                return systemWebProxy;
            }
        }

        public void RecheckServersAvailability(long utcNow)
        {
            if (MainWin.hosts.availabilityCheckTime != 0L && utcNow - MainWin.hosts.availabilityCheckTime > 300)
            {
                _ = servers.First().Host != ActiveEndPoint();
                availabilityCheckTime = 0L;
            }
        }

        public async void CheckServersAvailability(long utcNow = 0L)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (utcNow == 0L || (availabilityCheckTime == 0L && utcNow - MainWin.hosts.lastConnectionErrorTime >= 120))
            {
                Trace.WriteLine("=============== Ping servers ===============");
                IEnumerable<Task> source = servers.Select((Server server) => PingServerAsync(server, client));
                List<Task> pingTasks = source.ToList();
                while (pingTasks.Any())
                {
                    Task task = await Task.WhenAny(pingTasks);
                    pingTasks.Remove(task);
                    await task;
                }
            }
            stopwatch.Stop();
            Trace.WriteLine("=============== Available servers > ===============");
            foreach (Server server in servers)
            {
                Trace.WriteLine(server);
            }
            Trace.WriteLine("=============== Available servers < ===============");
            availabilityCheckTime = utcNow;
        }

        private async Task PingServerAsync(Server u, HttpClient client)
        {
            _ = 1;
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                using (HttpResponseMessage response = await client.GetAsync(new Uri(u.Host + apiPath + "ping")))
                {
                    Trace.WriteLine("ping");
                    response.EnsureSuccessStatusCode();
                    u.Response = await response.Content.ReadAsStringAsync();
                    u.Status = response.StatusCode;
                    u.Error = null;
                }
                stopwatch.Stop();
                u.Time = stopwatch.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                u.Status = HttpStatusCode.BadRequest;
                u.Error = ex.Message;
                u.Response = null;
                u.Time = -1L;
            }
        }
    }
}
