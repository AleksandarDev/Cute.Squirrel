using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace Cute.Squirrel.Babbler.SignalR.SelfHostClientConsole
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

            var client = new SelfHostClient();
            client.CatchAllMessages = true;
            client.Connect("http://localhost:47447", "SelfHost");

            Console.ReadLine();
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

    class SelfHostClient : BabblerSignalRClient<BabblerReport>
    {
        public SelfHostClient() : base("1", "BabblerSelfHostClientConsole")
        {
        }

        public override void ReportAvailable(BabblerReport report)
        {
            base.ReportAvailable(report);

            Console.WriteLine("Report available: " + JsonConvert.SerializeObject(report));
        }

        public override void ReportRequested(string destination, string identifier)
        {
            base.ReportRequested(destination, identifier);

            Console.WriteLine("Report requested: {0} {1}", destination, identifier);

            this.SendReport(new BabblerReport
            {
                Version = "1.0.0",
                Source = "1",
                Identifier = "BabblerSelfHostClientConsole",
                BabblerType = "SignalR",
                BabblerVersion = "1.0.0",
                ComputerName = "name",
                IpAddress = "address"
            });
        }
    }
}
