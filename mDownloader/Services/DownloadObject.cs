using mDownloader.Enums;
using mDownloader.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppContext = mDownloader.Models.AppContext;

namespace mDownloader.Services
{
    public class DownloadObject
    {
        private HttpClient _httpClient;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private const int _BUFFER = 8192;

        public int? Id { get; set; } = -1;
        public string? Url { get; set; }
        public string? DownloadPath { get; set; }
        public long TotalBytesDownload { get; set; } = 0;
        public Task? Task { get; private set; }

        public DownloadObject(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
                    Uri uri = new(Url);
                    string? fileName = !string.IsNullOrEmpty(response.Content.Headers.ContentDisposition?.FileName)
                                       ? response.Content.Headers.ContentDisposition?.FileName
                                       : Path.GetFileName(uri.LocalPath);
                    var statusCode = response.StatusCode;
                    var size = response.Content.Headers.ContentLength;
                    if (this.Id < 0 &&fileName == null)
                    {
                        throw new Exception("Not have file name");
                    }
                    if (statusCode != HttpStatusCode.OK && statusCode != HttpStatusCode.PartialContent)
                    {
                        throw new Exception($"{statusCode}");
                    }
                    if (this.Id < 0 && size == null)
                    {
                        throw new Exception("Size is null");
                    }



                    using(var context = new AppContext())
                    {
                        var task = new DownloadTask
                        {
                            Url = Url,
                            Destination = DownloadPath,
                            Name = fileName!,
                            DateCreated = DateTime.Now,
                            TotalBytesToDownload = 0,
                            Status = Status.Downloading,
                            StatusCode = (int?)response.StatusCode,
                            Size = (long)size!,
                        };
                        context.DownloadTasks.Add(task);
                        await context.SaveChangesAsync();
                        this.Id = task.Id;
                    }

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        using (var outputStream = new FileStream(Path.Combine(DownloadPath, fileName!), FileMode.Append, FileAccess.Write, FileShare.None))
                        {
                            var buffer = new byte[_BUFFER];
                            int bytesRead;
                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, _cts.Token)) > 0)
                            {
                                await outputStream.WriteAsync(buffer.AsMemory(0, bytesRead), _cts.Token);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }


        }
    }
}
