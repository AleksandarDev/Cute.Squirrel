namespace Cute.Squirrel.Babbler
{
    public interface IBabblerEvents<in T> where T : class, IBabblerReportBase
    {
        void ReportAvailable(T report);
    }
}