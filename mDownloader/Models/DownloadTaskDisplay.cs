using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mDownloader.Models
{
    public class DownloadTaskDisplay : DownloadTask
    {
        public int Progress { get; set; }
        public DateTime EstimateTime { get; set; }
        public long TransferRate { get; set; }

    }
}
