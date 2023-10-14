using mDownloader.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mDownloader.Converters;

namespace mDownloader.Services
{
    public class DownloadService : IDownloadService
    {
        List<DownloadTaskDisplay> IDownloadService.LoadTasks()
        {
            List<DownloadTaskDisplay> res = new();
            using (var db = new Models.AppContext())
            {
                List<DownloadTask> tasks = db.DownloadTasks.ToList();
                foreach (var task in tasks)
                {
                    var taskDisplay = SimpleMapper.Map<DownloadTask, DownloadTaskDisplay>(task);
                    if (taskDisplay.Size != 0)
                    {
                        taskDisplay.Progress = (int)(taskDisplay.TotalBytesToDownload / taskDisplay.Size);
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
