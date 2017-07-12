using System;

namespace Cute.Squirrel.Babbler
{
    public interface IBabblerReportBase
    {
        string Source { get; set; }

        string Identifier { get; set; }

        string Version { get; set; }

        string IpAddress { get; set; }

        string ComputerName { get; set; }

        string BabblerType { get; set; }

        string BabblerVersion { get; set; }
    }
}