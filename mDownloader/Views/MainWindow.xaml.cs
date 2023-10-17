using mDownloader.Event;
using mDownloader.Helpers;
using mDownloader.Services;
using mDownloader.ViewModels;
using System.Windows;

namespace mDownloader.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IWindowService _windowService;
        private readonly IEventAggregator _eventAggregator;

        public MainWindow(MainViewModel mainViewModel, IWindowService windowService, IEventAggregator eventAggregator)
        {
            InitializeComponent();
            DataContext = mainViewModel;
            _windowService = windowService;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe<RequestFocusEvent>(OnRequestFocus);
            _windowService.SetOwner(this);
            this.Closed += (s, e) => _eventAggregator.Unsubscribe<RequestFocusEvent>(OnRequestFocus);
        }
        private void OnRequestFocus(RequestFocusEvent @event)
        {
            DownloadDataGrid.Focus();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var mainViewModel = (MainViewModel)DataContext;
            mainViewModel.OnExit();
        }

    }
}
