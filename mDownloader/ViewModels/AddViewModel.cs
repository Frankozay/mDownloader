using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using mDownloader.Services;
using Microsoft.Extensions.DependencyInjection;

namespace mDownloader.ViewModels
{
    public class AddViewModel
    {
        public void DownloadNewTask(string url, string downloadPath)
        {
            var downloadObj = App.ServiceProvider?.GetService<DownloadObject>();
            if (downloadObj == null) { return; }
            downloadObj.SetAttribute(url, downloadPath);
            downloadObj.Start();
        }
    }
}
