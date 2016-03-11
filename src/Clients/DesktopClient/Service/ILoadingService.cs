using System;

namespace SC2LiquipediaStatistics.DesktopClient.Service
{
    public interface ILoadingService
    {
        void ShowAndExecuteAction(Action action);
    }
}