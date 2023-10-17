using mDownloader.Services;

namespace mDownloader.Event
{
    public class DownloadEvent
    {
        public DownloadObject obj;
        public DownloadEvent(DownloadObject obj)
        {
            this.obj = obj;
        }
    }
}
