﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Settings;
using Syncfusion.Licensing;

namespace ProjectK.Notebook
{
    public partial class App : Application
    {
        private static ILogger _logger;
        private readonly IHost _host;
        private string _basePath;
        private MainWindow _window;
        private AppViewModel _viewModel;


        public App()
        {
            _host = Host.CreateDefaultBuilder()  // Use default settings
                                                 //new HostBuilder()          // Initialize an empty HostBuilder
                .ConfigureAppConfiguration((context, builder) =>
                    {
                        _basePath = Environment.CurrentDirectory;
                        builder.SetBasePath(_basePath);
                        // Add other configuration files...
                        // builder.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
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

            var _ = new LogManager(_host.Services);
            _logger = LogManager.GetLogger<App>();
            _logger.LogDebug("Test");

            SyncfusionLicenseProvider.RegisterLicense("MzEwNjY0QDMxMzgyZTMyMmUzMEdzZjVJbzlJbzdSTkNLWWJuNFlPRWlZOXBOWkZNc0N0cnVTRm9PcXVBNEE9");
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }
        private void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            var xx = configuration["Window"];
            services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
            // services.AddScoped<ISampleService, SampleService>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<AppViewModel>();
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
            await _host.StartAsync();

            // Get app settings

            // MainWindow
            _window = _host.Services.GetRequiredService<MainWindow>();
            _window.LoadSettings();
            _window.LoadDockLayout();

            // Created ViewModel
            _viewModel = _host.Services.GetRequiredService<AppViewModel>();
            _viewModel.LoadSettings();

            // Open Database
            // var key = "AlanDatabase";
            var key = "TestDatabase";
            var connectionString = _window._settings.Connections[key];
            _viewModel.OpenDatabase(connectionString);

            // Set MainWindow DataContext
            _window.DataContext = _viewModel;

            // 
            _window.Closing += async (sender, args) => await WindowOnClosing(sender, args);

             // Show 
            _window.Show();
            base.OnStartup(e);
        }

        private async Task WindowOnClosing(object sender, CancelEventArgs e)
        {
            await SaveSettingsAsync();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
            base.OnExit(e);
        }

        private async Task SaveSettingsAsync()
        {
            _window.SaveSettings();
            _window.SaveDockLayout();
            _viewModel.SaveSettings();

            await _viewModel.CloseDatabaseAsync();
            await _viewModel.SaveAppSettings(_basePath);
        }
    }
}