using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using _3_project.Data;
using _3_project.Repositories;
using _3_project.Services;
using _3_project.ViewModels;

namespace _3_project
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Настройка Dependency Injection
                var services = new ServiceCollection();
                ConfigureServices(services);
                _serviceProvider = services.BuildServiceProvider();

                // Создание главного окна через DI
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Startup Error:\n\n{ex.Message}\n\nInner: {ex.InnerException?.Message}\n\nStack:\n{ex.StackTrace}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Server=localhost;Database=CsvPeopleDb;Trusted_Connection=True;TrustServerCertificate=True;"));

            // Repositories
            services.AddScoped<IPersonRepository, PersonRepository>();

            // Services
            services.AddSingleton<CsvImportService>();
            services.AddSingleton<ExcelExportService>();
            services.AddSingleton<XmlExportService>();

            // ViewModels
            services.AddTransient<MainViewModel>();

            // Views
            services.AddTransient<MainWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
