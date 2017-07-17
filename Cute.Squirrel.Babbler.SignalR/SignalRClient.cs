using System;
using System.Timers;
using Microsoft.AspNet.SignalR.Client;
using Serilog;

namespace Cute.Squirrel.Babbler.SignalR
{
    public class SignalRClient
    {
        private const double DefaultReconnectTimeout = 1000;
        private const double DefaultDisconnectTimeout = 10000;

        private HubConnection connection;
        private IHubProxy proxy;

        private bool isConnected = true;

        private readonly Timer reconnectTimer;

        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;

        public ILogger Logger { get; set; }


        public SignalRClient()
        {
            this.reconnectTimer = new Timer(DefaultReconnectTimeout)
            {
                AutoReset = false
            };
            this.reconnectTimer.Elapsed += ReconnectTimerOnElapsed;
        }


        private void ReconnectTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            this.reconnectTimer.Stop();

            // Reconnect if disconnected
            if (this.connection.State == ConnectionState.Disconnected)
                this.Connect();
        }

        public void Connect(string url, string hubName)
        {
            this.connection = new HubConnection(url);
            this.connection.Closed += ConnectionClosed;
            this.connection.StateChanged += ConnectionOnStateChanged;

            // Create proxy
            this.proxy = this.connection.CreateHubProxy(hubName);

            // Initiate connection
            this.Connect();
        }

        private void ConnectionOnStateChanged(StateChange stateChange)
        {
            if (this.IsConnected)
            {
                this.isConnected = true;
                this.OnConnected?.Invoke(this, null);
            }
            else
            {
                if (this.isConnected)
                {
                    this.isConnected = false;
                    this.OnDisconnected?.Invoke(this, null);
                }
            }
        }

        private async void Connect()
        {
            if (this.connection == null)
                return;

            try
            {
                await this.connection.Start();
                if (this.connection.State != ConnectionState.Connected)
                    throw new Exception("Connection state is not `Connected`.");
            }
            catch
            {
                this.ConnectionClosed();
            }
        }

        public void Disconnect()
        {
            try
            {
                // Don't reconnect
                this.reconnectTimer.Stop();

                // Disconnect
                this.connection?.Stop(TimeSpan.FromMilliseconds(DefaultDisconnectTimeout));
                this.connection?.Dispose();
                this.connection = null;
            }
            catch (Exception ex)
            {
                this.Logger?.Debug(ex, "Babbler SignalR client - error disconnecting");
            }
        }

        private void ConnectionClosed()
        {
            this.reconnectTimer.Stop();
            this.reconnectTimer.Start();
        }

        public bool IsConnected => this.connection?.State == ConnectionState.Connected;

        public IHubProxy Proxy => this.proxy;
    }
}