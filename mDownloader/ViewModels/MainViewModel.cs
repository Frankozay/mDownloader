using mDownloader.Models;
using mDownloader.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mDownloader.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<DownloadTaskDisplay> DownloadLists { get; } = new();
        public IDownloadService DownloadService { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel(IDownloadService downloadService) { 
            DownloadService = downloadService;
        }


        public void LoadTasks()
        {
            DownloadLists.Clear();
            var tasks = DownloadService.LoadTasks();
            foreach (var task in tasks) { 
                DownloadLists.Add(task);
            }
            RaisePropertyChanged(nameof(DownloadLists));
        }
    }
}
