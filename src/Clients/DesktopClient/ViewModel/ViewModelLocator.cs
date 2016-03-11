using SC2LiquipediaStatistics.Utilities.Unity;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel
        {
            get
            {
                return Container.Resolve<MainViewModel>();
            }
        }

        public ListEventsViewModel ListEventsViewModel
        {
            get
            {
                return Container.Resolve<ListEventsViewModel>();
            }
        }

        public PlayerStatisticsViewModel PlayerStatisticsViewModel
        {
            get
            {
                return Container.Resolve<PlayerStatisticsViewModel>();
            }
        }

        public AddEventViewModel AddEventViewModel
        {
            get
            {
                return Container.Resolve<AddEventViewModel>();
            }
        }

        public LoadingViewModel LoadingViewModel
        {
            get
            {
                return Container.Resolve<LoadingViewModel>();
            }
        }

        public EditEventViewModel EditEventViewModel
        {
            get
            {
                return Container.Resolve<EditEventViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}