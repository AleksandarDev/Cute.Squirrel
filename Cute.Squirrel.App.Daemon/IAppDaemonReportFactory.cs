namespace Cute.Squirrel.App.Daemon
{
    public interface IAppDaemonReportFactory<out TReport> where TReport : IAppDaemonBabblerReport
    {
        TReport CreateReport();
    }
}