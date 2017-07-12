using Microsoft.AspNet.SignalR;

namespace Cute.Squirrel.Babbler.SignalR.Server
{
    public abstract class BabblerSignalRHub<T> : Hub<IBabblerHubClient<T>>, IBabblerSignalRHub<T> where T : class, IBabblerReportBase
    {
        public virtual void ReportAvailable(T report)
        {
            if (report == null)
                return;

            this.Clients.Others.ReportAvailable(report);
        }

        public virtual void ReportRequested(string destination, string identifier)
        {
            if (string.IsNullOrWhiteSpace(destination) ||
                string.IsNullOrWhiteSpace(identifier))
                return;

            this.Clients.Others.ReportRequested(destination, identifier);
        }

        public virtual void SendReport(T report)
        {
            if (report == null)
                return;

            this.Clients.Others.ReportAvailable(report);
        }
    }
}
