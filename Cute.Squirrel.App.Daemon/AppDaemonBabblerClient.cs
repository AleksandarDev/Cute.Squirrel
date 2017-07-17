using System;
using Cute.Squirrel.Babbler.SignalR;
using Serilog;

namespace Cute.Squirrel.App.Daemon
{
    public class AppDaemonBabblerClient<TReport> : BabblerSignalRClient<TReport> where TReport : class, IAppDaemonBabblerReport
    {
        private readonly IAppDaemonReportFactory<TReport> reportFactory;

        public AppDaemonBabblerClient(string destination, string identifier, IAppDaemonReportFactory<TReport> reportFactory, ILogger logger) : base(destination, identifier)
        {
            this.reportFactory = reportFactory ?? throw new ArgumentNullException(nameof(reportFactory));

            this.Logger = logger;

            this.OnConnected += OnOnConnected;
        }

        private void OnOnConnected(object sender, EventArgs eventArgs)
        {
            this.SendReport();
        }

        public override void ReportRequested(string destination, string identifier)
        {
            this.SendReport();
        }

        public void SendReport()
        {
            this.SendReport(this.reportFactory.CreateReport());
        }
    }
}