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
        private const int _POINT = 1024 * 1000;

        public int? Id { get; set; } = -1;
        public string? Url { get; set; }
        public string? DownloadPath { get; set; }
        public long TotalBytesDownload { get; set; } = 0;
        public DateTime? DateCreated { get; set; }

        public DateTime? DateFinished { get; set; }
        public int? StatusCode { get; set; }

        public Status? Status { get; set; }

        public bool? IsQueued { get; set; }

        public string? Name { get; set; }

        public long? Size { get; set; }
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

                    if(Id < 0)
                    {
                        Name = !string.IsNullOrEmpty(response.Content.Headers.ContentDisposition?.FileName)
                            ? response.Content.Headers.ContentDisposition?.FileName
                            : Path.GetFileName(uri.LocalPath);

                        var statusCode = response.StatusCode;
                        StatusCode = (int?)statusCode;
                        Size = response.Content.Headers.ContentLength;
                        if (this.Id < 0 && Name == null)
                        {
                            throw new Exception("Not have file name");
                        }
                        if (statusCode != HttpStatusCode.OK && statusCode != HttpStatusCode.PartialContent)
                        {
                            throw new Exception($"{statusCode}");
                        }
                        if (this.Id < 0 && Size == null)
                        {
                            throw new Exception("Size is null");
                        }
                        using (var context = new AppContext())
                        {
                            var task = new DownloadTask
                            {
                                Url = Url,
                                Destination = DownloadPath,
                                Name = Name!,
                                DateCreated = DateTime.Now,
                                TotalBytesToDownload = 0,
                                Status = Enums.Status.Downloading,
                                StatusCode = (int?)response.StatusCode,
                                Size = (long)Size!,
                            };
                            context.DownloadTasks.Add(task);
                            await context.SaveChangesAsync();
                            this.Id = task.Id;
                        }
                    }


                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        if (File.Exists(Path.Combine(DownloadPath, Name!)))
                        {
                            // 恢复下载时，建立连接时检查一次就可以了
                            var fileInfo = new FileInfo(Path.Combine(DownloadPath, Name!));
                            TotalBytesDownload = fileInfo.Length;
                            using (var context = new AppContext())
                            {
                                var task = context.DownloadTasks.First(t => t.Id == this.Id);
                                task.TotalBytesToDownload = TotalBytesDownload;
                                context.Update(task);
                                await context.SaveChangesAsync();
                            }
                        }
                        using (var outputStream = new FileStream(Path.Combine(DownloadPath, Name!), FileMode.Append, FileAccess.Write, FileShare.None))
                        {
                            var buffer = new byte[_BUFFER];
                            int bytesRead;
                            int streamBytesDownload = 0;
                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, _cts.Token)) > 0)
                            {
                                await outputStream.WriteAsync(buffer.AsMemory(0, bytesRead), _cts.Token);
                                streamBytesDownload += bytesRead;
                                if(streamBytesDownload >= _POINT)
                                {
                                    using (var context = new AppContext())
                                    {
                                        var task = context.DownloadTasks.First(t => t.Id == this.Id);
                                        task.TotalBytesToDownload += streamBytesDownload;
                                        context.Update(task);
                                        await context.SaveChangesAsync();
                                        this.TotalBytesDownload = task.TotalBytesToDownload;
                                        streamBytesDownload = 0;
                                    }
                                }
                            }
                            if(TotalBytesDownload + streamBytesDownload != Size)
                            {
                                throw new Exception("Download file size is not equal real size.");
                            } else
                            {
                                using (var context = new AppContext())
                                {
                                    var task = context.DownloadTasks.First(t => t.Id == this.Id);
                                    task.TotalBytesToDownload += streamBytesDownload;
                                    context.Update(task);
                                    await context.SaveChangesAsync();
                                    this.TotalBytesDownload = task.TotalBytesToDownload;
                                    streamBytesDownload = 0;
                                }
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
