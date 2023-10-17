using System.Collections.Generic;
using System.Threading.Tasks;

namespace mDownloader.Services
{
    public interface IDownloadService
    {
        public List<DownloadObject> LoadTasks();
        public Task<bool> RemoveTask(List<DownloadObject> objects);
        public void PauseTask(List<DownloadObject> objects);
        public void ContinueTask(List<DownloadObject> objects);
    }
}
