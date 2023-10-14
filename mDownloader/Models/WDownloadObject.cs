using mDownloader.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mDownloader.Models
{
        public class WDownloadObject
        {
            public int Id { get; set; } = -1;

            public string? Url { get; set; }

            public string? Destination { get; set; }

            public DateTime? DateCreated { get; set; }

            public DateTime? DateFinished { get; set; }

            public long TotalBytesToDownload { get; set; } = 0;

            public int? StatusCode { get; set; }

            public Status? Status { get; set; }

            public bool? IsQueued { get; set; }

            public string? Name { get; set; }

            public long? Size { get; set; }

        }
}
