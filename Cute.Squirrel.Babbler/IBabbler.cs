namespace Cute.Squirrel.Babbler
{
    public interface IBabbler<in T> : IBabblerEvents<T>, IBabblerRequests<T>
        where T : class, IBabblerReportBase
    {
    }
}
