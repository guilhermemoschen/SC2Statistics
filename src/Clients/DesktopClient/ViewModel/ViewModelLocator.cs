using SC2LiquipediaStatistics.Utilities.Unity;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel => Container.Resolve<MainViewModel>();

        public ListEventsViewModel ListEventsViewModel => Container.Resolve<ListEventsViewModel>();

        public ListPlayersViewModel ListPlayersViewModel => Container.Resolve<ListPlayersViewModel>();

        public SoloPlayerStatisticsViewModel SoloPlayerStatisticsViewModel => Container.Resolve<SoloPlayerStatisticsViewModel>();

        public PlayerXPlayerStatisticsViewModel PlayerXPlayerStatisticsViewModel => Container.Resolve<PlayerXPlayerStatisticsViewModel>();

        public LoadingViewModel LoadingViewModel => Container.Resolve<LoadingViewModel>();

        public EditEventViewModel EditEventViewModel => Container.Resolve<EditEventViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}