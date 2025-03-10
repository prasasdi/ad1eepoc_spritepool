using MainAplikasi.DepedencyInjection;
using MainAplikasi.Presentations.MainWindow;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace MainAplikasi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        public App()
        {
            var services = new ServiceCollection();
            services.InitializeServices();
            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = new MainWindow(_serviceProvider.GetRequiredService<MainWindowViewModel>());
            mainWindow.Show();
        }
    }

}
