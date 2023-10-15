using mDownloader.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace mDownloader.Services
{
    public class WindowService : IWindowService
    {
        private AddWindow? _addWindow;
        private Window _owner;

        public void CloseAddWindow()
        {
            _addWindow?.Close();
        }

        public void OpenAddWindow()
        {
            if (_addWindow == null || !_addWindow.IsVisible)
            {
                _addWindow = App.ServiceProvider!.GetService<AddWindow>();
                _addWindow!.Closed += (sender, e) => _addWindow = null;
                _addWindow.Owner = _owner;
                _addWindow?.Show();
            }
            else
            {
                if (_addWindow.WindowState == WindowState.Minimized)
                {
                    _addWindow.WindowState = WindowState.Normal;
                }
                _addWindow.Activate();
            }
        }

        public void SetOwner(Window owner)
        {
            _owner = owner;
        }
    }
}
