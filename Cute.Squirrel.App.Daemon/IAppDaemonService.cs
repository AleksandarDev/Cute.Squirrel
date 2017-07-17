using Serilog;
using Topshelf.Squirrel.Updater.Interfaces;

namespace Cute.Squirrel.App.Daemon
{
    public interface IAppDaemonService : ISelfUpdatableService
    {
        string Identifier { get; set; }

        string Source { get; set; }

        ILogger Logger { get; set; }
    }
}