using System;
using System.Reflection;
using Serilog;
using Squirrel;
using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.Squirrel.Updater;
using Topshelf.Squirrel.Updater.Interfaces;

namespace Cute.Squirrel.App.Daemon
{
    public static class AppDaemonServiceHost
    {
        public static void HostDaemon<T>(string updateUrl, string serviceName, string serviceDescription, string serviceDisplayName, ILogger logger = null)
            where T : class, IAppDaemonService, new()
        {
            logger?.Information("Hosting Daemon...");
            logger?.Information("Hosting Daemon Update URL: {UpdateUrl}", updateUrl);

            // Configure unhandled exception logging
            if (logger != null)
                AppDomain.CurrentDomain.UnhandledException +=
                    (sender, args) => logger.Fatal(args.ExceptionObject as Exception, "AppDomain Unhandled Exception");

            // Configure update manager
            UpdateManager updateManager = null;
            IUpdater updater = null;
            try
            {
                updateManager = new UpdateManager(updateUrl, serviceName);
            }
            catch
            {
                logger?.Information("Updater not configured. Some features may be unavailable because the app is running without Update.exe");
            }
            
            // Updater config
            if (updateManager != null)
            {
                var updatePeriod = TimeSpan.FromSeconds(30);
                updater = new RepeatedTimeUpdater(updateManager).SetCheckUpdatePeriod(updatePeriod);
                updater.Start();

                logger?.Information("Daemon auto-update period set to: {AutoUpdatePeriod} minutes", updatePeriod.TotalMinutes);
            }
            else logger?.Warning("Daemon auto-updates are disabled.");

            // Configuration
            var con = new Action<HostConfigurator>(config =>
            {
                config.SetServiceName(serviceName);
                config.SetDescription(serviceDescription);
                config.SetDisplayName(serviceDisplayName);
                config.RunAsLocalSystem();
                config.StartAutomatically();
                config.EnableShutdown();

                config.Service<T>(service =>
                {
                    service.ConstructUsing(_ => new T
                    {
                        Logger = logger,
                        Identifier = serviceName
                    });
                    service.WhenStarted(instance => instance.Start());
                    service.WhenStopped(instance => instance.Stop());
                });

                if (logger != null)
                    config.UseSerilog(logger);
            });


            // Run the service
            if (updater != null)
            {
                logger?.Information("Starting service with updater...");

                var service = new T
                {
                    Logger = logger,
                    Identifier = serviceName
                };
                var x = new SquirreledHost(service, Assembly.GetExecutingAssembly(), updater);
                x.ConfigureAndRun(new SquirreledHost.ConfigureExt(con));
            }
            else
            {
                logger?.Information("Starting service without updater...");

                HostFactory.Run(con);
            }
        }
    }
}
