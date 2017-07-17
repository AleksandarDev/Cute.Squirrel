namespace Cute.Squirrel.App.Daemon.ConsoleDemo
{
    public class DemoAppDaemon : AppDaemonServiceBase<AppDaemonBabblerReport>
    {
        public static string ResolveTribeAddress() => "http://localhost:20825";

        public static string ResolveUpdaterAddress() => ResolveTribeAddress() + "/clients/demo/releases";

        public override string GetTribeAddress() => ResolveTribeAddress();
    }
}