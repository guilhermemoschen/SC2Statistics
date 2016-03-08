using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using GalaSoft.MvvmLight;

using SC2LiquipediaStatistics.DesktopClient.Model;

using SC2Statistics.SC2Domain.Service;

using SC2DomainEntities = SC2Statistics.SC2Domain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class PlayerStatisticsViewModel : ViewModelBase
    {
        protected ObservableCollection<Player> players;
        public ObservableCollection<Player> Players
        {
            get
            {
                return players;
            }
            set
            {
                if (value == null || value == players)
                    return;

                Set(() => Players, ref players, value, true);
            }
        }

        protected Player selectedPlayer;
        public Player SelectedPlayer
        {
            get
            {
                return selectedPlayer;
            }
            set
            {
                if (selectedPlayer != null && selectedPlayer.Id == value.Id)
                    return;

                Set(() => SelectedPlayer, ref selectedPlayer, value, true);
            }
        }

        protected PlayerStatistics playerStatistics;
        public PlayerStatistics PlayerStatistics
        {
            get
            {
                return playerStatistics;
            }
            set
            {
                if (playerStatistics == value || value == null)
                    return;

                Set(() => PlayerStatistics, ref playerStatistics, value, true);
            }
        }

        protected ObservableCollection<Event> eventsParticipated;
        public ObservableCollection<Event> EventsParticipated
        {
            get
            {
                return eventsParticipated;
            }
            set
            {
                if (value == null || value == eventsParticipated)
                    return;

                Set(() => EventsParticipated, ref eventsParticipated, value, true);
            }
        }

        public ISC2Service SC2Service { get; protected set; }

        public IStatisticsService StatisticsService { get; protected set; }

        public IMapper Mapper { get; protected set; }

        public PlayerStatisticsViewModel(ISC2Service sc2Service, IStatisticsService statisticsService, IMapper mapper)
        {
            SC2Service = sc2Service;
            StatisticsService = statisticsService;
            Mapper = mapper;

            var domainPlayers = sc2Service.FindAllPlayers();
            var players = Mapper.Map<IList<SC2DomainEntities.Player>, IList<Player>>(domainPlayers);
            Players = new ObservableCollection<Player>(players);

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "SelectedPlayer")
            {
                UpdatePlayerStatistics();
            }
        }

        private void UpdatePlayerStatistics()
        {
            var domainPlayer = Mapper.Map<Player, SC2DomainEntities.Player>(SelectedPlayer);
            var domainStatistics = StatisticsService.GeneratePlayerStatistics(domainPlayer);
            PlayerStatistics = Mapper.Map<SC2DomainEntities.PlayerStatistics, PlayerStatistics>(domainStatistics);
        }
    }
}
