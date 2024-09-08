using Civ6TranslationToolWPF.Levie;
using Civ6TranslationToolWPF.Pages;
using Civ6TranslationToolWPF.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace Civ6TranslationToolWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly IServiceProvider _serviceProvider;
        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            AppState appState = _serviceProvider.GetRequiredService<AppState>();
            appState.Load(); // Khôi phục trạng thái ứng dụng
            var viewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
            var mainWindow = new MainWindow(viewModel);
            mainWindow.Show();
            

            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<AppState>();
            services.AddSingleton<MainPage>();
            services.AddTransient<WindowStateManager>();
            services.AddSingleton<MainWindowViewModel>(); // Đăng ký ViewModel
            services.AddTransient<MainWindow>();          // Đăng ký View
        }
    }

}
