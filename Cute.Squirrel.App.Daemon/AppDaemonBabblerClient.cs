using Cute.Squirrel.Babbler.SignalR;

namespace Cute.Squirrel.App.Daemon
{
    public class AppDaemonBabblerClient : BabblerSignalRClient<AppDaemonBabblerReport>
    {
        public AppDaemonBabblerClient(string destination, string identifier) : base(destination, identifier)
        {
        }
    }
}