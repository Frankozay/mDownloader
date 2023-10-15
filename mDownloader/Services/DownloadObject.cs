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
using mDownloader.Helpers;
using mDownloader.Event;

namespace mDownloader.Services
{
    public class DownloadObject : WDownloadObject
    {
        private HttpClient _httpClient;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private const int _BUFFER = 8192;
        private const int _POINT = 1024 * 1000;
        private readonly IEventAggregator _eventAggregator;
        private object _totalBytesToDownloadLock = new object();

        public int? Progress { get; set; }
        public DateTime? EstimateTime { get; set; }
        public long? TransferRate { get; set; }

        public Task? Task { get; private set; }

        public DownloadObject(HttpClient httpClient, IEventAggregator eventAggregator)
        {
            _httpClient = httpClient;
            _eventAggregator = eventAggregator;
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
                    StatusCode = (int?)response.StatusCode;
                    if (Id < 0)
                    {
                        Name = !string.IsNullOrEmpty(response.Content.Headers.ContentDisposition?.FileName)
                            ? response.Content.Headers.ContentDisposition!.FileName
                            : Path.GetFileName(uri.LocalPath);
                        var filePath = Path.Combine(Destination, Name!);
                        if(File.Exists(filePath))
                        {
                            Name = Path.GetFileName(GenerateUniqueFileName(filePath));
                        }
                        var statusCode = response.StatusCode;
                        Size = (long?)response.Content.Headers.ContentLength;
                        DateCreated = DateTime.Now;
                        TotalBytesToDownload = 0;
                        Status = Enums.Status.Downloading;
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
                            var dbObj = SimpleMapper.Map<WDownloadObject, DownloadTask>(this, new object[0], true);
                            context.DownloadTasks.Add(dbObj);
                            var dbTask = Task.Run(() => context.SaveChangesAsync());
                            await dbTask;
                            this.Id = dbObj.Id;
                            _eventAggregator.Publish(new DownloadEvent(this));
                        }
                    }
                    else
                    {
                        var filePath = Path.Combine(Destination, Name!);
                        if (File.Exists(filePath))
                        {
                            // 恢复下载时，建立连接时检查一次就可以了
                            var fileInfo = new FileInfo(Path.Combine(Destination, Name!));
                            TotalBytesToDownload = fileInfo.Length;
                        } else
                        {
                            TotalBytesToDownload = 0;
                        }
                        using (var context = new AppContext())
                        {
                            var task = context.DownloadTasks.First(t => t.Id == this.Id);
                            task.TotalBytesToDownload = TotalBytesToDownload;
                            context.Update(task);
                            var dbTask = Task.Run(() => context.SaveChangesAsync());
                            await dbTask;
                        }
                    }


                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {

                        using (var outputStream = new FileStream(Path.Combine(Destination, Name!), FileMode.Append, FileAccess.Write, FileShare.None))
                        {
                            var buffer = new byte[_BUFFER];
                            int bytesRead;
                            int streamBytesDownload = 0;
                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, _cts.Token)) > 0)
                            {
                                await outputStream.WriteAsync(buffer.AsMemory(0, bytesRead), _cts.Token);
                                streamBytesDownload += bytesRead;
                                if (streamBytesDownload >= _POINT)
                                {
                                    UpdateTotalBytesToDownload(streamBytesDownload);
                                    streamBytesDownload = 0;
                                }
                            }

                            if (streamBytesDownload > 0)
                            {
                                UpdateTotalBytesToDownload(streamBytesDownload);
                                streamBytesDownload = 0;
                            }

                        }
                        if (TotalBytesToDownload != Size)
                        {
                            throw new Exception("Download file size is not equal real size.");
                        }
                        else
                        {
                            using (var context = new AppContext())
                            {
                                var task = context.DownloadTasks.First(t => t.Id == this.Id);
                                task.Status = Enums.Status.Completed;
                                task.DateFinished = DateTime.Now;
                                context.Update(task);
                                var dbTask = Task.Run(() => context.SaveChangesAsync());
                                await dbTask;
                                this.TotalBytesToDownload = task.TotalBytesToDownload;
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
        private void UpdateTotalBytesToDownload(int bytesDownloaded)
        {
            using (var context = new AppContext())
            {
                var task = context.DownloadTasks.First(t => t.Id == this.Id);
                task.TotalBytesToDownload += bytesDownloaded;
                context.Update(task);
                var dbTask = Task.Run(() => context.SaveChangesAsync());
                dbTask.Wait();
                this.TotalBytesToDownload = task.TotalBytesToDownload;
            }
        }
        private string GenerateUniqueFileName(string originalName)
        {
            string directory = Path.GetDirectoryName(originalName)!;
            string fileName = Path.GetFileNameWithoutExtension(originalName);
            string extension = Path.GetExtension(originalName);
            int count = 1;

            while (File.Exists(Path.Combine(directory, fileName + "(" + count + ")" + extension)))
            {
                count++;
            }

            return Path.Combine(directory, fileName + "(" + count + ")" + extension);
        }
    }
}
