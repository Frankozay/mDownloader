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
