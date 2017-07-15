using Cute.Squirrel.Babbler;

namespace Cute.Squirrel.App.Daemon
{
    public class AppDaemonBabblerReport : IBabblerReportBase
    {
        public string Source { get; set; }
        public string Identifier { get; set; }
        public string Version { get; set; }
        public string IpAddress { get; set; }
        public string ComputerName { get; set; }

        public string BabblerType { get; set; }

        public string BabblerVersion { get; set; }
    }
}