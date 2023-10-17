using mDownloader.Event;
using mDownloader.Helpers;
using mDownloader.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private ICommand _removeDownloadCommand;
        private ICommand _continueDownloadCommand;
        private ICommand _pauseDownloadCommand;
        public ObservableCollection<DownloadObject> DownloadLists { get; } = new();

        public ObservableCollection<DownloadObject> SelectedItems { get; private set; } = new ObservableCollection<DownloadObject>();

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
        public ICommand RemoveDownloadCommand => _removeDownloadCommand ??= new RelayCommand(_ => RemoveDownload(SelectedItems));
        public ICommand ContinueDownloadCommand => _continueDownloadCommand ??= new RelayCommand(_ => ContinueDownload(SelectedItems));
        public ICommand PauseDownloadCommand => _pauseDownloadCommand ??= new RelayCommand(_ => PauseDownload(SelectedItems));
        public void AddDownload()
        {
            _windowService.OpenAddWindow();
        }
        public async void RemoveDownload(ObservableCollection<DownloadObject> tasks)
        {
            var task = tasks.ToList();
            if (task.Count == 0) { return; }
            var isSuccess = await _downloadService.RemoveTask(task);
            if (isSuccess)
            {
                var idsToMove = new HashSet<int>(task.Select(t => t.Id));
                var itemsToMove = DownloadLists.Where(t => idsToMove.Contains(t.Id)).ToList();
                foreach (var item in itemsToMove)
                {
                    DownloadLists.Remove(item);
                }
            }
            ClearSelectedItems();
        }
        public void ContinueDownload(ObservableCollection<DownloadObject> tasks)
        {
            var task = tasks.ToList();
            if (task.Count == 0) { return; }
            _downloadService.ContinueTask(task);
            _eventAggregator.Publish(new RequestFocusEvent());
        }
        public void PauseDownload(ObservableCollection<DownloadObject> tasks)
        {
            var task = tasks.ToList();
            if (task.Count == 0) { return; }
            _downloadService.PauseTask(task);
            _eventAggregator.Publish(new RequestFocusEvent());
        }
        private void ClearSelectedItems()
        {
            SelectedItems = new ObservableCollection<DownloadObject>();
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
        public void OnExit()
        {
            var task = DownloadLists.ToList();
            if (task.Count == 0) { return; }
            _downloadService.PauseTask(task);
        }
        public void Dispose()
        {
            _eventAggregator.Unsubscribe(_handler);
        }
    }
}

