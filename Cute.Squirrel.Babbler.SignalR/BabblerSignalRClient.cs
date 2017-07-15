using System;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;

namespace Cute.Squirrel.Babbler.SignalR
{
    public abstract class BabblerSignalRClient<T> : IBabblerSignalRClient<T> where T : class, IBabblerReportBase
    {
        private readonly string destinationFilter;
        private readonly string identifierFilter;
        private readonly SignalRClient client;

        public bool CatchAllMessages { get; set; } = false;

        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;


        protected BabblerSignalRClient(string destination, string identifier)
        {
            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(destination));
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(identifier));

            this.destinationFilter = destination;
            this.identifierFilter = identifier;
            this.client = new SignalRClient();
            this.client.OnConnected += ClientOnOnConnected;
            this.client.OnDisconnected += ClientOnOnDisconnected;
        }

        private void ClientOnOnDisconnected(object sender, EventArgs eventArgs)
        {
            this.OnDisconnected?.Invoke(this, eventArgs);
        }

        private void ClientOnOnConnected(object sender, EventArgs eventArgs)
        {
            this.ReportRequested(this.destinationFilter, this.identifierFilter);

            this.OnConnected?.Invoke(this, eventArgs);
        }

        public void Connect(string url, string hubName)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(url));
            if (string.IsNullOrWhiteSpace(hubName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(hubName));

            this.client.Connect(url, hubName);
            this.client.Proxy?.On<T>("ReportAvailable", this.ReportAvailableInternal);
            this.client.Proxy?.On<string, string>("ReportRequested", this.ReportRequestedInternal);
        }

        private void ReportAvailableInternal(T report)
        {
            if (report == null ||
                !this.CatchAllMessages && 
                report.Source == this.destinationFilter && 
                report.Identifier == this.identifierFilter)
                return;

            this.ReportAvailable(report);
        }

        public virtual void ReportAvailable(T report)
        {
        }

        private void ReportRequestedInternal(string destination, string identifier)
        {
            // Ignore if not for this client
            if (!this.CatchAllMessages && 
                this.destinationFilter == destination && 
                this.identifierFilter == identifier)
                return;

            this.ReportRequested(destination, identifier);
        }

        public virtual void ReportRequested(string destination, string identifier)
        {
        }

        public void SendReport(T report)
        {
            if (report == null)
                return;

            if (this.client.IsConnected)
                this.client.Proxy.Invoke("SendReport", report);
        }
    }
}
