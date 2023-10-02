using System;
using System.Collections.Generic;

namespace mDownloader.Models;

public partial class DownloadTask
{
    public int Id { get; set; }

    public string Url { get; set; } = null!;

    public string Destination { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public DateTime? DateFinished { get; set; }

    public long TotalBytesToDownload { get; set; }

    public int? StatusCode { get; set; }

    public int Status { get; set; }

    public bool IsQueued { get; set; }
}
