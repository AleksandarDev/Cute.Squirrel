using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Owin;

namespace Cute.Squirrel.Babbler.SignalR.Server.SelfHostConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:47447";
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
                Console.WriteLine("Requesting report...");

                GlobalHost.ConnectionManager.GetHubContext<SelfHostHub>().Clients.All.ReportRequested("any", "any");

                Thread.Sleep(5000);
            }
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
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

    [HubName("SelfHost")]
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

            base.SendReport(report);
        }
    }
}
