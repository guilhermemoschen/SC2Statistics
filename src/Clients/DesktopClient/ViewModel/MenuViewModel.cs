using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class MenuViewModel : ViewModelBase
    {
        public ICommand UpdateDatabaseCommand { get; protected set; }
        public ICommand EventsListCommand { get; protected set; }
        public ICommand PlayerStatisticsCommand { get; protected set; }

        public MenuViewModel()
        {
            UpdateDatabaseCommand = new RelayCommand(UpdateDatabase);
            EventsListCommand = new RelayCommand(NavegateToEventsList);
            PlayerStatisticsCommand = new RelayCommand(NavegateToPlayerStatistics);
        }

        private void UpdateDatabase()
        {
            throw new NotImplementedException();
        }

        private void NavegateToEventsList()
        {
            throw new NotImplementedException();
        }

        private void NavegateToPlayerStatistics()
        {
            throw new NotImplementedException();
        }
    }
}
