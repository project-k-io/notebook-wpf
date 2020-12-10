using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Extensions;
using ProjectK.Notebook.ViewModels;
using ProjectK.Utils.Extensions;
using Syncfusion.Licensing;

namespace ProjectK.Notebook
{
    public partial class App : Application
    {
        private readonly IHost host;

        private static ILogger _logger;

        public App()
        {
            host = Host.CreateDefaultBuilder()  // Use default settings
                                                //new HostBuilder()          // Initialize an empty HostBuilder
                .ConfigureAppConfiguration((context, builder) =>
                {
                    // Add other configuration files...
                    builder.AddJsonFile("appsettings.local.json", optional: true);
                }).ConfigureServices((context, services) =>
                {
                    ConfigureServices(context.Configuration, services);
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddDebug();
                    // logging.AddProvider(new OutputLoggerProvider(Output.LogEvent));
                    // Add other loggers...
                })
                .Build();

            var _ = new LogManager(host.Services);
            _logger = LogManager.GetLogger<App>();
            _logger.LogDebug("Test");

            SyncfusionLicenseProvider.RegisterLicense("MzEwNjY0QDMxMzgyZTMyMmUzMEdzZjVJbzlJbzdSTkNLWWJuNFlPRWlZOXBOWkZNc0N0cnVTRm9PcXVBNEE9");
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }
        private void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            // services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
            // services.AddScoped<ISampleService, SampleService>();

            services.AddSingleton<MainWindow>();
            services.Configure<LoggerFilterOptions>(o => o.MinLevel = LogLevel.Debug);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.LogError(e.ToString());
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            _logger.LogDebug("OnStartup()");
            // Start Host
            await host.StartAsync();

            // Get app settings
            var appSettings = ConfigurationManager.AppSettings;

            // MainWindow
            var window = host.Services.GetRequiredService<MainWindow>();
            window.LoadSettings(appSettings);
            window.LoadDockLayout();
             
            // Created ViewModel
            var model = new AppViewModel();
            model.LoadSettings(appSettings);

            // Open Database
            // var key = "AlanDatabase";
            var key = "TestDatabase";
            var connectionString = ConfigurationManager.ConnectionStrings[key].ConnectionString;
            model.OpenDatabase(connectionString);

            // Set MainWindow DataContext
            window.DataContext = model;

            // Show 
            window.Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            _logger?.LogDebug("OnExit");
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            if (Current.MainWindow is MainWindow window)
            {
                window.SaveSettings(settings);
                window.SaveDockLayout();
                window.Close();

                if (window.DataContext is AppViewModel appModel)
                {
                    appModel.SaveSettings(settings);
                    await appModel.CloseDatabaseAsync();
                }

            }
            configFile.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            _logger?.LogDebug("Host");

            using (host)
            {
                await host.StopAsync(TimeSpan.FromSeconds(5));
            }

            base.OnExit(e);
        }
    }
}