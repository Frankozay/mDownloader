﻿using mDownloader.Services;
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
                DownloadPath = downloadPath,
                TotalBytesDownload = totalBytesDownload,
                Id = id
            };

            return downloadObject;
        }

    }
}