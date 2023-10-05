using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using mDownloader;
using mDownloader.Services;
using mDownloader.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAPICodePack.Dialogs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace mDownloader.Views
{
    /// <summary>
    /// AddWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddWindow : Window
    {
        private AddViewModel _viewModel;
        private string _selectedPath;
        public AddWindow()
        {
            InitializeComponent();
            _viewModel = new AddViewModel();
            _selectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            ChoosePathButton.Content = _selectedPath;

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
            _viewModel.DownloadNewTask(url, _selectedPath);
        }
    }
}
