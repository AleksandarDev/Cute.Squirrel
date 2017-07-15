using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Cute.Squirrel.Babbler.SignalR;
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

            // Squirrel config
            using (var updateManager = new UpdateManager(updateUrl))
            {
                // Note, in most of these scenarios, the app exits after this method
                // completes!
                SquirrelAwareApp.HandleEvents(
                    onInitialInstall: v => updateManager.CreateShortcutForThisExe(),
                    onAppUpdate: v => updateManager.CreateShortcutForThisExe(),
                    onAppUninstall: v => updateManager.RemoveShortcutForThisExe());
            }

            // Updater config
            IUpdater updater = null;
            try
            {
                logger?.Information("Configuring updater...");

                var updateManager = new UpdateManager(updateUrl);
                updater = new RepeatedTimeUpdater(updateManager).SetCheckUpdatePeriod(TimeSpan.FromMinutes(1));
                
                logger?.Information("Updater configured.");
            }
            catch
            {
                // Not installed
                logger?.Warning("Updater not configured. The app is running outside Update.exe");
            }

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
                    service.ConstructUsing(_ => new T());
                    service.WhenStarted(instance => instance.Start());
                    service.WhenStopped(instance => instance.Stop());
                });
            });

            // Run the service
            if (updater != null)
            {
                logger?.Information("Starting service with updater...");

                var x = new SquirreledHost(new T(), Assembly.GetExecutingAssembly(), updater);
                x.ConfigureAndRun(new SquirreledHost.ConfigureExt(con));
            }
            else
            {
                logger?.Information("Starting service without updater...");

                HostFactory.Run(con);
            }
        }
    }

    public abstract class AppDaemonServiceBase : IAppDaemonService
    {
        protected readonly IBabblerSignalRClient<AppDaemonBabblerReport> babbler;

        protected static string daemonConfigurationLocation =>
            Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "daemon.conf");

        protected readonly string computerName;

        public IUpdater Updater { get; set; }


        public AppDaemonServiceBase()
        {
            // Retrieve environment details
            this.computerName = Environment.MachineName;

            // TODO Retrieve instance name
            // TODO Retrieve source/destination

            // Prepare babbler
            // TODO Use proper values
            this.babbler = new AppDaemonBabblerClient("1", "1");
        }

        public abstract string GetTribeAddress();

        public virtual void Start()
        {
            // Start babbler
            var tribeAddress = this.GetTribeAddress();
            if (!string.IsNullOrWhiteSpace(tribeAddress))
                this.babbler.Connect(tribeAddress, "Tribe");

            // Start updater
            if (this.Updater != null)
            {
                this.Updater.Start();
                Console.WriteLine("Updater started...");
            }
        }

        public virtual void Stop()
        {
               
        }

        protected static bool AskForTribeAddress()
        {
            var configuratorProcess = Process.Start(
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "Cute.Squirrel.App.Daemon.Configurator.exe"),
                $"\"{daemonConfigurationLocation}\"");
            if (configuratorProcess == null)
                return false;

            configuratorProcess.WaitForExit();

            return true;
        }
    }
}
