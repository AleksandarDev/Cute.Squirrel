namespace Cute.Squirrel.Babbler.SignalR.Server
{
    public interface IBabblerSignalRHub<in T> : IBabbler<T> where T : class, IBabblerReportBase
    {
    }
}