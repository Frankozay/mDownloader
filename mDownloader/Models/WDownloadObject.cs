using mDownloader.Enums;
using System;
using System.ComponentModel;
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
            try
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

    }
}
