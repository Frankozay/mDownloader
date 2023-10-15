using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using mDownloader.Factories;
using mDownloader.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using mDownloader.Helpers;
using System.IO;

namespace mDownloader.ViewModels
{
    public class AddViewModel : ViewModelBase
    {
        private readonly IDownloadService _downloadService;
        private readonly IWindowService _windowService;
        private readonly DownloadObjectFactory _downloadObjFactory;
        private string _url;
        private string _selectedPath;
        private ICommand _downloadCommand;
        private ICommand _choosePathCommand;

        public AddViewModel(IDownloadService downloadService, IWindowService windowService, DownloadObjectFactory downloadObjectFactory)
        {
            _downloadService = downloadService;
            _windowService = windowService;
            _downloadObjFactory = downloadObjectFactory;
            _selectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        }

        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                OnPropertyChanged();
            }
        }

        public string SelectedPath
        {
            get => _selectedPath;
            set
            {
                _selectedPath = value;
                OnPropertyChanged();
            }
        }

        public ICommand DownloadCommand => _downloadCommand ??= new RelayCommand(_ => DownloadNewTask());

        public ICommand ChoosePathCommand => _choosePathCommand ??= new RelayCommand(_ => ChoosePath());

        private void DownloadNewTask()
        {
            if (_downloadObjFactory == null) { return; }
            DownloadObject downloadObj = _downloadObjFactory.Create(Url, SelectedPath);
            try
            {
                downloadObj.Start();
                _windowService.CloseAddWindow();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void ChoosePath()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                SelectedPath = dialog.FileName!;
            }
        }
    }
}