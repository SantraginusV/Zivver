using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Zivver.Models;
using Zivver.Services;
using Zivver.ViewModels;

namespace Zivver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        public App()
        {
            _host = new HostBuilder()
                    .ConfigureAppConfiguration((context, configurationBuilder) =>
                    {
                        configurationBuilder.SetBasePath(context.HostingEnvironment.ContentRootPath);
                        configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    })
                    .ConfigureServices((context, services) =>
                    {
                        // add logging to app
                        services.AddLogging(conf => conf.AddSerilog(LoggingConfig()));
                        // add access to appsettings.json
                        services.Configure<AppSettings>(context.Configuration);
                        // add custom made services
                        services.AddSingleton<IRestService, RestService>();
                        // add viewModels
                        services.AddSingleton(InitializePostPanelViewModel);
                        // add views 
                        services.AddSingleton<MainWindow>(s => new MainWindow(s.GetRequiredService<PostPanelViewModel>()));
                    })
                    .Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }

        // if unhandled error occurs, here it will be handled
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs ex)
        {
            var kill = false;
            if (ex.Exception is DivideByZeroException || ex.Exception.InnerException is DivideByZeroException)
            {
                // we won't kill the app, this is not fatal, just bad
                kill = false;
            }
            else if (ex.Exception is ArgumentNullException)
            {
                // hm... should we kill it ? I guess we should :)
                kill = true;
            }

            if (kill)
            {
                // Show some info before killing it
                MessageBox.Show("Oh no... App just crashed because of: " + ex.Exception.Message, "Zivver App",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                // Log error in event log. Can be useful sometimes.
                EventLog.WriteEntry("Zivver App", "Unrecoverable Exception: " + ex.Exception.Message, EventLogEntryType.Error);
                Shutdown(-1);
            }

            // we need to set it 
            ex.Handled = true;
        }

        private Serilog.ILogger LoggingConfig()
        {
            return new LoggerConfiguration()
                .WriteTo.File(@".\Logs\log.txt")
                .CreateLogger();
        }

        private static PostPanelViewModel InitializePostPanelViewModel(IServiceProvider services)
        {
            return new PostPanelViewModel(
                services.GetRequiredService<ILogger<PostPanelViewModel>>(),
                services.GetRequiredService<IConfiguration>(),
                services.GetRequiredService<IRestService>());
        }
    }
}