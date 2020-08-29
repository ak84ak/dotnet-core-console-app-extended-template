using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

using $safeprojectname$.App.AppCustomization;

namespace $safeprojectname$
{
    static class Program
    {
        private static CancellationTokenSource _appCancellationToken;
        private static Task _mainTask;

        static async Task Main(string[] args)
        {
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "appsettings.json")))
            {
                Console.WriteLine("appsettings.json does not exist");
                Environment.Exit(-1);
            }

            IConfiguration configuration = new ConfigurationBuilder()
                                   .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"), false, false)
                                   .AddEnvironmentVariables()
                                   .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.local.json"), true, false)
                                   .AddCommandLine(args)
                                   .AddAppConfiguration()
                                   .Build();

            Log.Logger = new LoggerConfiguration()
                .ConfigureAppLogger(configuration)
                .CreateLogger();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, configuration);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var application = serviceProvider.GetRequiredService<IApp>();

            _appCancellationToken = new CancellationTokenSource();
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            Console.CancelKeyPress += Console_CancelKeyPress;

            _mainTask = Task.Run(() => application.Run(_appCancellationToken.Token));
            await _mainTask;

            Environment.Exit(0);
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _appCancellationToken.Cancel();
            _mainTask.Wait();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            _appCancellationToken.Cancel();
            _mainTask.Wait();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.AddSingleton(configuration);

            services.AddLogging(configure => configure.AddSerilog());

            services.ConfigureAppServices(configuration);
        }
    }
}
