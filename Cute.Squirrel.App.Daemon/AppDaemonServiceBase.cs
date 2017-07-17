using System;
using System.Reflection;
using Cute.Squirrel.Babbler.SignalR;
using Serilog;

namespace Cute.Squirrel.App.Daemon
{
    public abstract class AppDaemonServiceBase<TReport> : IAppDaemonService where TReport : class, IAppDaemonBabblerReport, new()
    {
        private ILogger logger;

        public abstract string GetTribeAddress();

        public virtual void Start()
        {
            // Create report factory
            var version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            this.ReportFactory = new AppDaemonReportFactory<TReport>(this.Source, this.Identifier, version);

            // Start babbler
            var tribeAddress = this.GetTribeAddress();
            if (!string.IsNullOrWhiteSpace(tribeAddress))
            {
                this.logger?.Information("Tribe address: {TribeAddress} as {Source} {Identifier}", tribeAddress, this.Source, this.Identifier);
                this.Babbler = new AppDaemonBabblerClient<TReport>(this.Source, this.Identifier, this.ReportFactory, this.Logger);
                this.Babbler.Connect(tribeAddress, "Tribe");
            }
        }

        public virtual void Stop()
        {
        }

        public string Source { get; set; } = Environment.MachineName;

        public string Identifier { get; set; } = "AppDaemon";

        protected IAppDaemonReportFactory<TReport> ReportFactory { get; set; }

        protected IBabblerSignalRClient<TReport> Babbler { get; set; }

        public ILogger Logger
        {
            get => this.logger;
            set
            {
                this.logger = value;

                if (this.Babbler != null)
                    this.Babbler.Logger = value;
            }
        }
    }
}