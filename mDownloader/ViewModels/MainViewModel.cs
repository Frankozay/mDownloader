using mDownloader.Helpers;
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
using System.Windows.Input;

namespace mDownloader.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDownloadService _downloadService;
        private readonly IWindowService _windowService;

        private ICommand _addDownloadCommand;
        public ObservableCollection<DownloadObject> DownloadLists { get; } = new();

        public MainViewModel(IDownloadService downloadService, IWindowService windowService)
        {
            _downloadService = downloadService;
            _windowService = windowService;
            LoadTasks();
        }

        public ICommand AddDownloadCommand => _addDownloadCommand ??= new RelayCommand(_ => AddDownload());
        public void AddDownload()
        {
            _windowService.OpenAddWindow();
        }
        public void LoadTasks()
        {
            DownloadLists.Clear();
            var tasks = _downloadService.LoadTasks();
            foreach (var task in tasks)
            {
                DownloadLists.Add(task);
            }
        }
    }
}

