using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mDownloader.Services
{
    public class DownloadObject
    {
        private HttpClient _httpClient;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private const int _BUFFER = 8192;

        public int? Id { get; private set; } = -1;
        public string? Url { get; set; }
        public string? DownloadPath { get; set; }
        public long TotalBytesDownload { get; private set; } = 0;
        public Task? Task { get; private set; }

        public DownloadObject(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Boolean SetAttribute(string Url, string DownloadPath, long TotalBytesDownload=0, int id=-1)
        {
            try
            {
                this.Url = Url;
                this.DownloadPath = DownloadPath;
                this.TotalBytesDownload = TotalBytesDownload;
                if(id > 0) { 
                    this.Id = id;
                }
                return true;
            }catch (Exception)
            {
                return false;
            }
        }

        public void Start()
        {
            Task = Task.Run(DownloadFileAsync);
        }


        private async Task DownloadFileAsync()
        {
            if(Url == null)
            {
                return;
            }
            if(DownloadPath == null)
            {
                return;
            }
            var request = new HttpRequestMessage { RequestUri = new Uri(Url), Method = HttpMethod.Get };
            request.Headers.Range = new RangeHeaderValue(TotalBytesDownload, null);
            try
            {
                using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, _cts.Token))
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        using (var outputStream = new FileStream(Path.Combine(DownloadPath, "qq.exe"), FileMode.Append, FileAccess.Write, FileShare.None))
                        {

                            var buffer = new byte[_BUFFER];
                            int bytesRead;
                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, _cts.Token)) > 0)
                            {
                                await outputStream.WriteAsync(buffer, 0, bytesRead, _cts.Token);
                            }
                        }
                    }
                }
            }
            catch (System.UnauthorizedAccessException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }


        }

        public void GrantAccess(string fullPath)
        {
            var directoryInfo = new DirectoryInfo(fullPath);
            var directorySecurity = directoryInfo.GetAccessControl();
            directorySecurity.AddAccessRule(new FileSystemAccessRule(
                Environment.UserName,
                FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.None,
                AccessControlType.Allow));
            directoryInfo.SetAccessControl(directorySecurity);
        }
    }
}
