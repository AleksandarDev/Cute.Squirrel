using System;
using Serilog;

namespace Cute.Squirrel.Babbler.SignalR
{
    public interface IBabblerSignalRClient<in T> : IBabbler<T> where T : class, IBabblerReportBase
    {
        bool CatchAllMessages { get; set; }

        event EventHandler OnConnected;

        event EventHandler OnDisconnected;

        ILogger Logger { get; set; }

        void Connect(string url, string hubName);
    }
}