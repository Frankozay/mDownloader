using mDownloader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
