using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Cute.Squirrel.App.Daemon.ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var address = DemoAppDaemon.ResolveUpdaterAddress();
            var host = new Uri(address, UriKind.Absolute).Host;

            // Configure logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.ColoredConsole(outputTemplate: "{Timestamp:HH:mm:ss} {Message}{NewLine}{Exception}")
                .WriteTo.Seq("http://" + host + ":20827")
                .Enrich.WithProperty("App", "DaemonConsoleDemo")
                .CreateLogger();

            // Configure daemon
            AppDaemonServiceHost.HostDaemon<DemoAppDaemon>(
                DemoAppDaemon.ResolveUpdaterAddress(),
                "CuteSquirrelAppDaemonConsoleDemo",
                "Demo App Daemon for Cute Squirrel",
                "CuteSquirrel Demo App Daemon",
                Log.Logger);
        }
    }
}
