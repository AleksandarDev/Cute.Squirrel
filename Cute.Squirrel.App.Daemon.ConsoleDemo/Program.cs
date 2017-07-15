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

namespace Cute.Squirrel.App.Daemon.ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDaemonServiceHost.HostDaemon<DemoAppDaemon>(
                "CuteSquirrel.DemoAppDaemon",
                "Demo App Daemon for Cute Squirrel",
                "CuteSquirrel Demo App Daemon");
        }
    }

    public class DemoAppDaemon : AppDaemonServiceBase
    {
        public override string GetTribeAddress()
        {
            if (string.IsNullOrWhiteSpace(Properties.DaemonSettings.Default.TribeAddress))
            {
                this.ReadFromDaemonConfiguration();
                if (string.IsNullOrWhiteSpace(Properties.DaemonSettings.Default.TribeAddress))
                {
                    this.AskForTribeAddress();
                    this.ReadFromDaemonConfiguration();
                }
            }

            return Properties.DaemonSettings.Default.TribeAddress;
        }

        private void ReadFromDaemonConfiguration()
        {
            try
            {
                var configuration = File.ReadAllText(this.daemonConfigurationLocation);
                Properties.DaemonSettings.Default.TribeAddress = configuration;
                Properties.DaemonSettings.Default.Save();
            }
            catch
            {
                // Nothing to do (maybe needs to be created)
            }
        }
    }
}
