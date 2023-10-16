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

        public async Task<bool> RemoveTask(List<DownloadObject> objs)
        {
            try
            {
                using (var db = new Models.AppContext())
                {
                    foreach (var obj in objs)
                    {
                        var task = db.DownloadTasks.Single((t) => t.Id == obj.Id);
                        if (task == null) {
                            return false;
                        }
                        db.DownloadTasks.Remove(task);
                        await db.SaveChangesAsync();
                    }
                    return true;
                }

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
                        taskDisplay.Progress = (double?)(taskDisplay.TotalBytesToDownload / taskDisplay.Size);
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
