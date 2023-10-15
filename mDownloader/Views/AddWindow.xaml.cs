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
        public AddWindow(AddViewModel addViewModel)
        {
            InitializeComponent();
            DataContext = addViewModel;
        }
    }
}
