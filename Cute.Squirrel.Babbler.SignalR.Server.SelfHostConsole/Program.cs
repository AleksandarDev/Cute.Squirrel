using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cute.Squirrel.Tribe;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json;
using Owin;
using Serilog;

namespace Cute.Squirrel.Babbler.SignalR.Server.SelfHostConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configure logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.ColoredConsole()
                .WriteTo.Seq("http://localhost:20827")
                .CreateLogger();

            string url = "http://localhost:20825";
            using (WebApp.Start(url))
            {
                Console.WriteLine("Server is running on: {0}", url);

                Task.Run((Action)ReportRequester);

                Console.ReadLine();
            }
        }

        private static void ReportRequester()
        {
            while (true)
            {
                foreach (var tribeMember in TribeContext.tracker.GetMembers())
                {
                    Console.WriteLine(
                        "Requesting report from {0} {1}", 
                        tribeMember.Identifier.Source,
                        tribeMember.Identifier.Identifier);

                    GlobalHost.ConnectionManager.GetHubContext<SelfHostHub>().Clients.All.ReportRequested(tribeMember.Identifier.Source, tribeMember.Identifier.Identifier);
                }

                Thread.Sleep(5000);
            }
        }
    }

    public static class TribeContext
    {
        public static TribeTracker<string> tracker = new TribeTracker<string>();


    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseFileServer(new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                StaticFileOptions =
                {
                    ServeUnknownFileTypes = true
                }
            });
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }

    public class BabblerReport : IBabblerReportBase
    {
        public string Source { get; set; }
        public string Identifier { get; set; }
        public string Version { get; set; }
        public string IpAddress { get; set; }
        public string ComputerName { get; set; }
        public string BabblerType { get; set; }
        public string BabblerVersion { get; set; }
    }

    public class DemoTribeMember : TribeMember<string>
    {
        public DemoTribeMember(TribeMemberIdentifier identifier) : base(identifier)
        {
        }

        public DemoTribeMember(TribeMemberIdentifier identifier, string data) : base(identifier, data)
        {
        }
    }

    [HubName("Tribe")]
    public class SelfHostHub : BabblerSignalRHub<BabblerReport>
    {
        public override Task OnConnected()
        {
            Console.WriteLine("Connected.");

            return base.OnConnected();
        }

        public override void ReportAvailable(BabblerReport report)
        {
            Console.WriteLine("Report available: " + JsonConvert.SerializeObject(report));

            base.ReportAvailable(report);
        }

        public override void ReportRequested(string destination, string identifier)
        {
            Console.WriteLine("Report requested: {0} {1}", destination, identifier);

            base.ReportRequested(destination, identifier);
        }

        public override void SendReport(BabblerReport report)
        {
            Console.WriteLine("Send report: " + JsonConvert.SerializeObject(report));

            var identifier = new TribeMemberIdentifier(report.Source, report.Identifier);
            if (!TribeContext.tracker.IsRegisteredMember(identifier))
                TribeContext.tracker.RegisterMember(new DemoTribeMember(identifier));

            base.SendReport(report);
        }
    }
}
