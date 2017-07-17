using System;
using System.Reflection;

namespace Cute.Squirrel.App.Daemon
{
    public class AppDaemonReportFactory<TReport> : IAppDaemonReportFactory<TReport> where TReport : class, IAppDaemonBabblerReport, new()
    {
        private readonly string source;
        private readonly string identifier;
        private readonly string version;

        public AppDaemonReportFactory(string source, string identifier, string version)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(identifier));
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(source));
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(version));

            this.source = source;
            this.identifier = identifier;
            this.version = version;
        }

        public virtual TReport CreateReport()
        {
            return new TReport
            {
                Source = this.source,
                Identifier = this.identifier,
                Version = this.version
            };
        }
    }
}