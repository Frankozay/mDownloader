using mDownloader.Converters;
using mDownloader.Helpers;
using mDownloader.Models;
using mDownloader.Services;
using System.Net.Http;

namespace mDownloader.Factories
{
    public class DownloadObjectFactory
    {
        private readonly HttpClient _httpClient;
        private readonly IEventAggregator _eventAggregator;
        public DownloadObjectFactory(HttpClient httpClient, IEventAggregator eventAggregator)
        {
            _httpClient = httpClient;
            _eventAggregator = eventAggregator;
        }

        public DownloadObject Create(string url, string downloadPath, long totalBytesDownload = 0, int id = -1)
        {
            var downloadObject = new DownloadObject(_httpClient, _eventAggregator)
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
            var download = SimpleMapper.Map<DownloadTask, DownloadObject>(downloadTask, new object[] { _httpClient, _eventAggregator }, false);
            return download;
        }

    }
}
