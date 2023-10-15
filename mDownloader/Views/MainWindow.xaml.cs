using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using mDownloader;
using mDownloader.Models;
using mDownloader.Services;
using mDownloader.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace mDownloader.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IWindowService _windowService;

        public MainWindow(MainViewModel mainViewModel, IWindowService windowService)
        {
            InitializeComponent();
            DataContext = mainViewModel;
            _windowService = windowService;
            _windowService.SetOwner(this);
        }

    }
}
