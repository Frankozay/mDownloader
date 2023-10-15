using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using mDownloader.Services;
using mDownloader.ViewModels;
using mDownloader.Views;
using mDownloader.Factories;
using System;

namespace mDownloader
{
    public partial class App : Application
    {
        public static ServiceProvider? ServiceProvider { get;  set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();
            base.OnStartup(e);
            var mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow!.Show();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddHttpClient<DownloadObjectFactory>();
            services.AddTransient<MainWindow>();
            services.AddTransient<AddWindow>();
            services.AddTransient<AddViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddScoped<IDownloadService, DownloadService>();
            services.AddScoped<IWindowService, WindowService>();
        }
    }
}
