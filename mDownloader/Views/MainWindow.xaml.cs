using System;
using System.Collections.Generic;
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
using mDownloader.ViewModels;

namespace mDownloader.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AddWindow? _addWindow;
        private MainViewModel _mainViewModel;
        public MainWindow()
        {
            InitializeComponent();
            _mainViewModel = new MainViewModel();
        }

        private void AddDownload_Click(object sender, RoutedEventArgs e)
        {
            if(_addWindow == null || !_addWindow.IsVisible)
            {
                _addWindow = new AddWindow();
                _addWindow.Closed += (sender, e) => _addWindow = null;
                _addWindow.Owner = this;
                _addWindow?.Show();
            }
            else
            {
                if(_addWindow.WindowState == WindowState.Minimized)
                {
                    _addWindow.WindowState = WindowState.Normal;
                }
                _addWindow.Activate();
            }
        }

    }
}
