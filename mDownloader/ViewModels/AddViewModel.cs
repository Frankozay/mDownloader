using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using mDownloader.Factories;
using mDownloader.Services;
using Microsoft.Extensions.DependencyInjection;

namespace mDownloader.ViewModels
{
    public class AddViewModel
    {
        public IDownloadService DownloadService { get; set; }
        public AddViewModel(IDownloadService downloadService)
        {
            DownloadService = downloadService;
        }

        public void DownloadNewTask(string url, string downloadPath)
        {
            var downloadObjFactory = App.ServiceProvider?.GetService<DownloadObjectFactory>();
            if (downloadObjFactory == null) { return; }
            DownloadObject downloadObj = downloadObjFactory.Create(url, downloadPath);
            try
            {
                downloadObj.Start();
                DownloadService.LoadTasks();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
