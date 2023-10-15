using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace mDownloader.Services
{
    public interface IWindowService
    {
        void OpenAddWindow();
        void SetOwner(Window owner);
        void CloseAddWindow();
    }
}
