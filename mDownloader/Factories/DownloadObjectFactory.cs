using mDownloader.Converters;
using mDownloader.Models;
using mDownloader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace mDownloader.Factories
{
    public class DownloadObjectFactory
    {
        private readonly HttpClient _httpClient;
        public DownloadObjectFactory(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public DownloadObject Create(string url, string downloadPath, long totalBytesDownload = 0, int id = -1)
        {
            var downloadObject = new DownloadObject(_httpClient)
            {
                Url = url,
                Destination = downloadPath,
                TotalBytesToDownload = totalBytesDownload,
                Id = id
            };

            return downloadObject;
        }
        public DownloadObject Create(DownloadTask downloadTask)
        {
            var download = SimpleMapper.Map<DownloadTask, DownloadObject>(downloadTask, new object[] { _httpClient }, false);
            return download;
        }

    }
}
