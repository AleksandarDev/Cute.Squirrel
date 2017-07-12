namespace Cute.Squirrel.Babbler
{
    public interface IBabblerRequests<in T> where T : class, IBabblerReportBase
    {
        void ReportRequested(string destination, string identifier);

        void SendReport(T report);
    }
}