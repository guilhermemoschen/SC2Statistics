using SC2LiquipediaStatistics.Utilities.Unity;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel => Container.Resolve<MainViewModel>();

        public ListEventsViewModel ListEventsViewModel => Container.Resolve<ListEventsViewModel>();

        public ListPlayersViewModel ListPlayersViewModel => Container.Resolve<ListPlayersViewModel>();

        public PlayerStatisticsViewModel PlayerStatisticsViewModel => Container.Resolve<PlayerStatisticsViewModel>();

        public LoadingViewModel LoadingViewModel => Container.Resolve<LoadingViewModel>();

        public EditEventViewModel EditEventViewModel => Container.Resolve<EditEventViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}