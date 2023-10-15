using mDownloader.Event;
using mDownloader.Helpers;
using mDownloader.Models;
using mDownloader.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace mDownloader.ViewModels
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private readonly IDownloadService _downloadService;
        private readonly IWindowService _windowService;
        private readonly IEventAggregator _eventAggregator;
        private readonly Action<DownloadEvent> _handler;

        private ICommand _addDownloadCommand;
        public ObservableCollection<DownloadObject> DownloadLists { get; } = new();

        public MainViewModel(IDownloadService downloadService, IWindowService windowService, IEventAggregator eventAggregator)
        {
            _downloadService = downloadService;
            _windowService = windowService;
            _eventAggregator = eventAggregator;
            _handler = HandleDownloadEvent;
            eventAggregator.Subscribe<DownloadEvent>(_handler);
            LoadTasks();
        }

        private void HandleDownloadEvent(DownloadEvent @event)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                DownloadLists.Add(@event.obj);
            });
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
        public void Dispose()
        {
            _eventAggregator.Unsubscribe(_handler);
        }
    }
}

