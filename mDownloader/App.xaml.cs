using mDownloader.Factories;
using mDownloader.Helpers;
using mDownloader.Services;
using mDownloader.ViewModels;
using mDownloader.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace mDownloader
{
    public partial class App : Application
    {
        public static ServiceProvider? ServiceProvider { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);

                ServiceProvider = serviceCollection.BuildServiceProvider();
                base.OnStartup(e);
                var mainWindow = ServiceProvider.GetService<MainWindow>();
                mainWindow!.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw new Exception("crack");
            }

        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddTransient<MainWindow>();
            services.AddTransient<AddWindow>();
            services.AddTransient<AddViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddScoped<IDownloadService, DownloadService>();
            services.AddScoped<IEventAggregator, EventAggregator>();
            services.AddScoped<IWindowService, WindowService>();
            services.AddHttpClient<DownloadObjectFactory>();
        }
    }
}
