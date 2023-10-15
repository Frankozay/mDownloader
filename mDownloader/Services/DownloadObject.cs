using mDownloader.Enums;
using mDownloader.Models;
using mDownloader.Converters;
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
    public class DownloadObject : WDownloadObject
    {
        private HttpClient _httpClient;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private const int _BUFFER = 8192;
        private const int _POINT = 1024 * 1000;

        public int? Progress { get; set; }
        public DateTime? EstimateTime { get; set; }
        public long? TransferRate { get; set; }

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
            if(Destination == null)
            {
                return;
            }
            var request = new HttpRequestMessage { RequestUri = new Uri(Url), Method = HttpMethod.Get };
            request.Headers.Range = new RangeHeaderValue(TotalBytesToDownload, null);
            try
            {
                using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, _cts.Token))
                {
                    Uri uri = new(Url);

                    if(Id < 0)
                    {
                        Name = !string.IsNullOrEmpty(response.Content.Headers.ContentDisposition?.FileName)
                            ? response.Content.Headers.ContentDisposition!.FileName
                            : Path.GetFileName(uri.LocalPath);

                        var statusCode = response.StatusCode;
                        StatusCode = (int?)statusCode;
                        Size = (long?)response.Content.Headers.ContentLength;
                        DateCreated = DateTime.Now;
                        TotalBytesToDownload = 0;
                        Status = Enums.Status.Downloading;
                        StatusCode = (int?)response.StatusCode;
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
                            var dbObj = SimpleMapper.Map<WDownloadObject, DownloadTask>(this, true);
                            context.DownloadTasks.Add(dbObj);
                            await context.SaveChangesAsync();
                            this.Id = dbObj.Id;
                        }
                    }


                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        if (File.Exists(Path.Combine(Destination, Name!)))
                        {
                            // 恢复下载时，建立连接时检查一次就可以了
                            var fileInfo = new FileInfo(Path.Combine(Destination, Name!));
                            TotalBytesToDownload = fileInfo.Length;
                            using (var context = new AppContext())
                            {
                                var task = context.DownloadTasks.First(t => t.Id == this.Id);
                                task.TotalBytesToDownload = TotalBytesToDownload;
                                context.Update(task);
                                await context.SaveChangesAsync();
                            }
                        }
                        using (var outputStream = new FileStream(Path.Combine(Destination, Name!), FileMode.Append, FileAccess.Write, FileShare.None))
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
                                        this.TotalBytesToDownload = task.TotalBytesToDownload;
                                        streamBytesDownload = 0;
                                    }
                                }
                            }
                            if(TotalBytesToDownload + streamBytesDownload != Size)
                            {
                                throw new Exception("Download file size is not equal real size.");
                            } else
                            {
                                using (var context = new AppContext())
                                {
                                    var task = context.DownloadTasks.First(t => t.Id == this.Id);
                                    task.TotalBytesToDownload += streamBytesDownload;
                                    task.Status = Enums.Status.Completed;
                                    task.DateFinished = DateTime.Now;
                                    context.Update(task);
                                    await context.SaveChangesAsync();
                                    this.TotalBytesToDownload = task.TotalBytesToDownload;
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
