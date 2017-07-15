using Topshelf.Squirrel.Updater.Interfaces;

namespace Cute.Squirrel.App.Daemon
{
    public interface IAppDaemonService : ISelfUpdatableService
    {
        IUpdater Updater { get; set; }

        void Start();

        void Stop();
    }
}