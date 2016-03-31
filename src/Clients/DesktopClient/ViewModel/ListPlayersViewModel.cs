using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using AutoMapper;

using FirstFloor.ModernUI.Windows.Controls;

using GalaSoft.MvvmLight.CommandWpf;

using SC2LiquipediaStatistics.DesktopClient.Model;
using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.Utilities.DataBase;
using SC2LiquipediaStatistics.Utilities.Domain;

using SC2Statistics.StatisticDomain.Service;

using DomainEntities = SC2Statistics.StatisticDomain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class ListPlayersViewModel : ModernViewModelBase
    {
        public ICommand UpdatePlayersCommand { get; private set; }

        public IStatisticService StatisticService { get; private set; }

        public ILoadingService LoadingService { get; private set; }

        public IMapper Mapper { get; private set; }

        private ObservableCollection<Player> players;
        public ObservableCollection<Player> Players
        {
            get
            {
                return players;
            }
            set
            {
                if (value == null)
                    return;

                Set(() => Players, ref players, value, true);
            }
        }

        private string filterCriteria;
        public string FilterCriteria
        {
            get
            {
                return filterCriteria;
            }
            set
            {
                if (value == null)
                    return;

                Set(() => FilterCriteria, ref filterCriteria, value, true);
            }
        }

        public ICommand NextPageCommand { get; private set; }
        public ICommand PreviousPageCommand { get; private set; }
        public ICommand FilterPlayersCommand { get; private set; }

        private int currentPageIndex = 0;

        public ListPlayersViewModel(IStatisticService statisticService, IMapper mapper, ILoadingService loadingService)
        {
            StatisticService = statisticService;
            Mapper = mapper;
            LoadingService = loadingService;

            UpdatePlayersCommand = new RelayCommand(UpdatePlayers);
            NextPageCommand = new RelayCommand(FindNextPage);
            PreviousPageCommand = new RelayCommand(FindPreviousPage);
            FilterPlayersCommand = new RelayCommand(FilterPlayers);
            NavigatedToCommand = new RelayCommand<object>(LoadGrid);
        }

        private void FilterPlayers()
        {
            currentPageIndex = 0;
            FindPlayers();
        }

        private void FindPreviousPage()
        {
            currentPageIndex--;
            if (currentPageIndex < 0)
                currentPageIndex = 0;
            FindPlayers();

        }

        private void FindNextPage()
        {
            currentPageIndex++;
            FindPlayers();
        }

        public void LoadGrid(object parameter)
        {
            FilterCriteria = null;
            currentPageIndex = 0;
            FindPlayers();
        }

        private void FindPlayers()
        {
            IEnumerable<Player> allPlayers;

            using (new NHibernateSessionContext())
            {
                IEnumerable<DomainEntities.Player> domainPlayers;

                if (string.IsNullOrEmpty(FilterCriteria))
                    domainPlayers = StatisticService.FindAllPlayers(currentPageIndex, 20);
                else
                    domainPlayers = StatisticService.FindPlayers(FilterCriteria, currentPageIndex, 20);

                allPlayers = Mapper.Map<IEnumerable<DomainEntities.Player>, IEnumerable<Player>>(domainPlayers);
            }

            Players = new ObservableCollection<Player>(allPlayers);
        }

        private void UpdatePlayers()
        {
            ValidationException validationException = null;

            LoadingService.ShowAndExecuteAction(delegate
            {
                using (new NHibernateSessionContext())
                {
                    try
                    {
                        StatisticService.UpdateAllPlayers();
                    }
                    catch (ValidationException ex)
                    {
                        validationException = ex;
                    }
                }
            });

            if (validationException != null)
            {
                ModernDialog.ShowMessage(validationException.GetFormatedMessage(), "Validation Message", MessageBoxButton.OK);
            }

            FindPlayers();
        }
    }
}
