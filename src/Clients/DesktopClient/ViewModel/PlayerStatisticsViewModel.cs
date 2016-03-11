using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using AutoMapper;

using FirstFloor.ModernUI.Windows.Controls;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using SC2LiquipediaStatistics.DesktopClient.Model;
using SC2LiquipediaStatistics.Utilities.DataBase;

using SC2Statistics.SC2Domain.Service;

using SC2DomainEntities = SC2Statistics.SC2Domain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class PlayerStatisticsViewModel : ModernViewModelBase
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
                if (value == null || (selectedPlayer != null && selectedPlayer.Id == value.Id))
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

        protected KeyValuePair<string, SC2DomainEntities.Expansion> selectedExpansion;
        public KeyValuePair<string, SC2DomainEntities.Expansion> SelectedExpansion
        {
            get
            {
                return selectedExpansion;
            }
            set
            {
                if (selectedExpansion.Value == value.Value)
                    return;

                Set(() => SelectedExpansion, ref selectedExpansion, value, true);
            }
        }

        public IList<KeyValuePair<string, SC2DomainEntities.Expansion>> Expansions { get; set; }

        public ISC2Service SC2Service { get; protected set; }

        public IStatisticsService StatisticsService { get; protected set; }

        public IMapper Mapper { get; protected set; }

        public ICommand GenerateStatisticsCommand { get; private set; }

        public PlayerStatisticsViewModel(ISC2Service sc2Service, IStatisticsService statisticsService, IMapper mapper)
        {
            SC2Service = sc2Service;
            StatisticsService = statisticsService;
            Mapper = mapper;

            GenerateStatisticsCommand = new RelayCommand(GenerateStatistics);
            Expansions = new List<KeyValuePair<string, SC2DomainEntities.Expansion>>();
            Expansions.Add(new KeyValuePair<string, SC2DomainEntities.Expansion>("Hearth of the Swarm", SC2DomainEntities.Expansion.HeartOfTheSwarm));
            Expansions.Add(new KeyValuePair<string, SC2DomainEntities.Expansion>("Legacy of the Void", SC2DomainEntities.Expansion.LegacyOfTheVoid));
            Expansions.Add(new KeyValuePair<string, SC2DomainEntities.Expansion>("Wings of Liberty", SC2DomainEntities.Expansion.WingsOfLiberty));
        }

        private void GenerateStatistics()
        {
            if (SelectedExpansion.Value != SC2DomainEntities.Expansion.LegacyOfTheVoid)
            {
                ModernDialog.ShowMessage("Only Legacy of the Void is available.", "Sorry", MessageBoxButton.OK);
                return;
            }

            IList<Event> events;

            using (var context = new NHibernateSessionContext())
            {
                var domainPlayer = Mapper.Map<Player, SC2DomainEntities.Player>(SelectedPlayer);
                var domainStatistics = StatisticsService.GeneratePlayerStatistics(domainPlayer, SelectedExpansion.Value);
                PlayerStatistics = Mapper.Map<SC2DomainEntities.PlayerStatistics, PlayerStatistics>(domainStatistics);
                var domainEvents = SC2Service.FindEventsByPlayer(SelectedPlayer.Id);
                events = Mapper.Map<IList<SC2DomainEntities.Event>, IList<Event>>(domainEvents);
            }

            EventsParticipated = new ObservableCollection<Event>(events);
        }

        public void View_OnLoad()
        {
            if (IsInDesignMode)
                return;

            var domainPlayers = SC2Service.FindAllPlayers();
            var players = Mapper.Map<IList<SC2DomainEntities.Player>, IList<Player>>(domainPlayers);
            Players = new ObservableCollection<Player>(players);
        }
    }
}
