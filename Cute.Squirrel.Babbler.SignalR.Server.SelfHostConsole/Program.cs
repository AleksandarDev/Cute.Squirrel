using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
                foreach (var tribeMember in DemoTribeContext.Instance.Store.GetMembers())
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

    public class Singleton<T> where T : class
    {
        private static readonly Lazy<T> InstanceInternal;

        static Singleton()
        {
            InstanceInternal = new Lazy<T>(CreateProtectedInstance);
        }

        private static T CreateProtectedInstance()
        {
            return (T) Activator.CreateInstance(
                typeof(T),
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new object[0],
                CultureInfo.InvariantCulture);
        }

        public static T Instance => InstanceInternal.Value;
    }

    public class TribeContext<TMember, TData> 
        where TMember : ITribeMember<TData>
        where TData : IBabblerReportBase
    {
        public TribeStore<TMember, TData> Store = new TribeStore<TMember, TData>();

        public void ProcessReport(TData report)
        {
            // Got report, register member if not registered already
            var identifier = new TribeMemberIdentifier(report.Source, report.Identifier);
            // TODO Use Factory for converting report to member
            //if (!DemoTribeContext.Instance.Store.IsRegisteredMember(identifier))
                //DemoTribeContext.Instance.Store.RegisterMember(new TMember());(identifier));
        }

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

    public class DemoBabblerReport : IBabblerReportBase
    {
        public string Source { get; set; }
        public string Identifier { get; set; }
        public string Version { get; set; }
        public string IpAddress { get; set; }
        public string ComputerName { get; set; }
        public string BabblerType { get; set; }
        public string BabblerVersion { get; set; }
    }

    public class DemoTribeContext : TribeContext<DemoTribeMember, DemoBabblerReport>
    {
        public static DemoTribeContext Instance => Singleton<DemoTribeContext>.Instance;

        protected DemoTribeContext()
        {
        }
    }

    public class DemoTribeMember : TribeMember<DemoBabblerReport>
    {
        public DemoTribeMember(TribeMemberIdentifier identifier) : base(identifier)
        {
        }

        public DemoTribeMember(TribeMemberIdentifier identifier, DemoBabblerReport data) : base(identifier, data)
        {
        }
    }

    public abstract class TribeHubBase<TReport> : BabblerSignalRHub<TReport> where TReport : class, IBabblerReportBase, new()
    {
        public override void ReportAvailable(TReport report)
        {
            base.ReportAvailable(report);
        }

        public override void ReportRequested(string destination, string identifier)
        {
            base.ReportRequested(destination, identifier);
        }

        public override void SendReport(TReport report)
        {
            base.SendReport(report);
        }
    }
}
