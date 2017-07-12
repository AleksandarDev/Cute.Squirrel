namespace Cute.Squirrel.Babbler.SignalR.Server
{
    public interface IBabblerHubClient<in T> : IBabbler<T> where T : class, IBabblerReportBase
    {
    }
}