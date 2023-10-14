using mDownloader.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mDownloader.Services
{
    public interface IDownloadService
    {
        public List<DownloadTaskDisplay> LoadTasks();
    }
}
