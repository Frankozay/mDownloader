using mDownloader.Enums;
using mDownloader.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace mDownloader.Models
{
    public class WDownloadObject : INotifyPropertyChanged
    {
        public int Id { get; set; } = -1;

        public string? Url { get; set; }

        public string? Destination { get; set; }

        public DateTime? DateCreated { get; set; }

        private DateTime? _dateFinished;
        public DateTime? DateFinished
        {
            get { return _dateFinished; }
            set
            {
                _dateFinished = value;
                OnPropertyChanged(nameof(DateFinished));
            }
        }

        public long TotalBytesToDownload { get; set; } = 0;

        public int? StatusCode { get; set; }

        private Status? _status;
        public Status? Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public bool? IsQueued { get; set; }

        public string? Name { get; set; }

        public long? Size { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Debug.WriteLine($"property changed {propertyName}");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

    }
}
