using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using mDownloader;
using mDownloader.Services;
using mDownloader.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace mDownloader.Views
{
    /// <summary>
    /// AddWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddWindow : Window
    {
        private AddViewModel? _viewModel;
        private MainViewModel? _mainViewModel;
        private string _selectedPath;
        public AddWindow(MainViewModel? mainViewModel)
        {
            InitializeComponent();
            _viewModel = App.ServiceProvider!.GetService<AddViewModel>();
            _selectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            ChoosePathButton.Content = _selectedPath;
            _mainViewModel = mainViewModel;
        }
        private void ChoosePath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            var result = dialog.ShowDialog();
            if(result == CommonFileDialogResult.Ok)
            {
                ChoosePathButton.Content = dialog.FileName;
                _selectedPath = dialog.FileName!;
            }
        }
        private void Download_Click(object sender, RoutedEventArgs e)
        {
            var url = UrlText.Text;
            _viewModel!.DownloadNewTask(url, _selectedPath);
            this.Close();

        }
    }
}
