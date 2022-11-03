using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeskTime
{
    internal class RESTService
    {
        private static object mutexApi = new object();

        private static int timeout = 45000;

        private static bool IsDebug()
        {
            return false;
        }

        public static async Task GetPing(Uri uri)
        {
            string responseHeaders4 = "Version: " + MainWin.Version?.ToString() + "\n";
            responseHeaders4 = responseHeaders4 + "Ping request start: " + DateTime.UtcNow.ToString() + "\n";
            responseHeaders4 += "===========================\n";
            try
            {
                IPAddress[] hostAddresses = Dns.GetHostAddresses(uri.Host);
                responseHeaders4 = responseHeaders4 + uri.Host + ":" + Environment.NewLine;
                IPAddress[] array = hostAddresses;
                for (int i = 0; i < array.Length; i++)
                {
                    responseHeaders4 = responseHeaders4 + "    " + array[i]?.ToString() + Environment.NewLine;
                }
                responseHeaders4 += Environment.NewLine;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                responseHeaders4 = responseHeaders4 + "GetHostAddresses error: " + ex.Message + Environment.NewLine;
            }
            try
            {
                ServicePointManager.DnsRefreshTimeout = 0;
                HttpClient client = new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate)
                });
                client.DefaultRequestHeaders.UserAgent.ParseAdd("DeskTime Windows Client v" + MainWin.Version);
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                client.DefaultRequestHeaders.ConnectionClose = true;
                using HttpResponseMessage responseX = await client.GetAsync(uri);
                if (responseX.IsSuccessStatusCode)
                {
                    await responseX.Content.ReadAsStringAsync();
                    responseHeaders4 += Environment.NewLine;
                    responseHeaders4 = responseHeaders4 + "----- CLIENT HEADERS ----" + Environment.NewLine + client.DefaultRequestHeaders.ToString() + Environment.NewLine;
                    responseHeaders4 = responseHeaders4 + "----- MESSAGE HEADERS -----" + Environment.NewLine + responseX.Headers.ToString() + Environment.NewLine;
                    responseHeaders4 = responseHeaders4 + "----- CONTENT HEADERS -----" + Environment.NewLine + responseX.Content.Headers.ToString() + Environment.NewLine;
                    responseHeaders4 = responseHeaders4 + responseX.RequestMessage.Method?.ToString() + " " + responseX.Version?.ToString() + " " + responseX.StatusCode.ToString() + " " + responseX.Content.Headers.ContentType?.ToString() + Environment.NewLine;
                }
                else
                {
                    responseHeaders4 = responseHeaders4 + responseX.RequestMessage.Method?.ToString() + " " + responseX.Version?.ToString() + " " + responseX.StatusCode.ToString() + " " + responseX.Content.Headers.ContentType?.ToString() + Environment.NewLine;
                }
            }
            catch (HttpRequestException ex2)
            {
                responseHeaders4 = responseHeaders4 + Environment.NewLine + "HttpRequestException: " + ex2.Message + Environment.NewLine;
            }
            catch (Exception ex3)
            {
                Console.WriteLine(ex3.ToString());
                responseHeaders4 = responseHeaders4 + Environment.NewLine + "Exception: " + ex3.Message + Environment.NewLine;
            }
            responseHeaders4 += "===========================\n";
            responseHeaders4 = responseHeaders4 + "Ping request stop: " + DateTime.UtcNow.ToString() + "\n";
            responseHeaders4 = responseHeaders4 + "Last error: " + RegistryService.GetValue("Last Error", "-");
            MessageBox.Show(responseHeaders4, "Ping", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        public static async Task GetResponse(Uri uri, Action<Response> callback)
        {
            try
            {
                ServicePointManager.DnsRefreshTimeout = 0;
                HttpClient httpClient = new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate)
                });
                httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DeskTime Windows Client v" + MainWin.Version);
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                httpClient.DefaultRequestHeaders.ConnectionClose = true;
                using HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(uri);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (callback != null)
                    {
                        DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(Response));
                        if (IsDebug())
                        {
                            string s = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result, Encoding.UTF8).ReadToEnd();
                            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(s));
                            callback(dataContractJsonSerializer.ReadObject((Stream)stream) as Response);
                        }
                        else
                        {
                            Stream result = httpResponseMessage.Content.ReadAsStreamAsync().Result;
                            callback(dataContractJsonSerializer.ReadObject(result) as Response);
                        }
                    }
                    return;
                }
                throw new Exception(httpResponseMessage.StatusCode.ToString());
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex2)
            {
                Console.WriteLine(ex2.ToString());
            }
        }

        public static async Task PostResponse(Uri uri, string postData, Action<Response> callback)
        {
            try
            {
                ServicePointManager.DnsRefreshTimeout = 0;
                HttpClient httpClient = new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate)
                });
                httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DeskTime Windows Client v" + MainWin.Version);
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                httpClient.DefaultRequestHeaders.ConnectionClose = true;
                using HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(uri, new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded"));
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (callback != null)
                    {
                        DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(Response));
                        if (IsDebug())
                        {
                            string s = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result, Encoding.UTF8).ReadToEnd();
                            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(s));
                            callback(dataContractJsonSerializer.ReadObject((Stream)stream) as Response);
                        }
                        else
                        {
                            Stream result = httpResponseMessage.Content.ReadAsStreamAsync().Result;
                            callback(dataContractJsonSerializer.ReadObject(result) as Response);
                        }
                    }
                    return;
                }
                throw new Exception(httpResponseMessage.StatusCode.ToString());
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex2)
            {
                Console.WriteLine(ex2.ToString());
            }
        }

        public static async Task UploadFileResponse(Uri uri, NameValueCollection postData, byte[] file, string fileName, string contentType, Action<Response> callback)
        {
            try
            {
                ServicePointManager.DnsRefreshTimeout = 0;
                HttpClient httpClient = new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate)
                });
                httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DeskTime Windows Client v" + MainWin.Version);
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                httpClient.DefaultRequestHeaders.ConnectionClose = true;
                using MultipartFormDataContent multipartFormContent = new MultipartFormDataContent();
                foreach (string postDatum in postData)
                {
                    _ = postData[postDatum];
                    multipartFormContent.Add(new StringContent(postData[postDatum]), postDatum);
                }
                ByteArrayContent byteArrayContent = new ByteArrayContent(file);
                byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                multipartFormContent.Add(byteArrayContent, "file", fileName);
                using HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(uri, multipartFormContent);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (callback != null)
                    {
                        DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(Response));
                        if (IsDebug())
                        {
                            string s = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result, Encoding.UTF8).ReadToEnd();
                            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(s));
                            callback(dataContractJsonSerializer.ReadObject((Stream)stream) as Response);
                        }
                        else
                        {
                            Stream result = httpResponseMessage.Content.ReadAsStreamAsync().Result;
                            callback(dataContractJsonSerializer.ReadObject(result) as Response);
                        }
                    }
                    return;
                }
                throw new Exception(httpResponseMessage.StatusCode.ToString());
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex2)
            {
                Console.WriteLine(ex2.ToString());
            }
        }
    }
}
