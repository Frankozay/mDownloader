using mDownloader.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mDownloader.Converters;
using mDownloader.Factories;

namespace mDownloader.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly DownloadObjectFactory _downloadObjectFactory;
        public DownloadService(DownloadObjectFactory downloadObjectFactory) {
            _downloadObjectFactory = downloadObjectFactory;
        }
        List<DownloadObject> IDownloadService.LoadTasks()
        {
            List<DownloadObject> res = new();
            using (var db = new Models.AppContext())
            {
                List<DownloadTask> tasks = db.DownloadTasks.ToList();
                foreach (var task in tasks)
                {
                    var taskDisplay = _downloadObjectFactory.Create(task);

                    if (taskDisplay.Size != 0)
                    {
                        taskDisplay.Progress = (int?)(taskDisplay.TotalBytesToDownload / taskDisplay.Size);
                        res.Add(taskDisplay);
                    } else
                    {
                        throw new Exception("Size is null");
                    }

                }
            }
            return res;
        }
    }
}
