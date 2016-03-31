using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using AutoMapper;

using FirstFloor.ModernUI.Windows.Controls;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using SC2LiquipediaStatistics.DesktopClient.Common;
using SC2LiquipediaStatistics.DesktopClient.Model;
using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.DesktopClient.View;
using SC2LiquipediaStatistics.Utilities.DataBase;
using SC2LiquipediaStatistics.Utilities.Domain;
using SC2LiquipediaStatistics.Utilities.Unity;

using SC2Statistics.StatisticDomain.Service;

using WpfControls.Editors;

using SC2DomainEntities = SC2Statistics.StatisticDomain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class PlayerStatisticsViewModel : ModernViewModelBase
    {
        protected Player selectedPlayer;
        public Player SelectedPlayer
        {
            get
            {
                return selectedPlayer;
            }
            set
            {
                if (selectedPlayer == value || value == null)
                    return;

                Set(() => SelectedPlayer, ref selectedPlayer, value, true);
            }
        }

        private bool hasPlayerStatistics;
        public bool HasPlayerStatistics
        {
            get
            {
                return hasPlayerStatistics;
            }
            set
            {
                if (hasPlayerStatistics == value)
                    return;

                Set(() => HasPlayerStatistics, ref hasPlayerStatistics, value, true);
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

        protected Event selectedEvent;
        public Event SelectedEvent
        {
            get
            {
                return selectedEvent;
            }
            set
            {
                if (value == null || value == selectedEvent)
                    return;

                Set(() => SelectedEvent, ref selectedEvent, value, true);
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

        public ISuggestionProvider SuggestionProvider { get; set; }

        public IList<KeyValuePair<string, SC2DomainEntities.Expansion>> Expansions { get; set; }

        public IStatisticService StatisticService { get; protected set; }

        public IModernNavigationService NavigationService { get; protected set; }

        public ILoadingService LoadingService { get; protected set; }

        public IMapper Mapper { get; protected set; }

        public ICommand GenerateStatisticsCommand { get; private set; }

        public PlayerStatisticsViewModel(IStatisticService statisticService, IModernNavigationService navigationService, ILoadingService loadingService, IMapper mapper)
        {
            StatisticService = statisticService;
            NavigationService = navigationService;
            LoadingService = loadingService;
            Mapper = mapper;

            GenerateStatisticsCommand = new RelayCommand(GenerateStatistics);
            Expansions = new List<KeyValuePair<string, SC2DomainEntities.Expansion>>();
            Expansions.Add(new KeyValuePair<string, SC2DomainEntities.Expansion>("Hearth of the Swarm", SC2DomainEntities.Expansion.HeartOfTheSwarm));
            Expansions.Add(new KeyValuePair<string, SC2DomainEntities.Expansion>("Legacy of the Void", SC2DomainEntities.Expansion.LegacyOfTheVoid));
            Expansions.Add(new KeyValuePair<string, SC2DomainEntities.Expansion>("Wings of Liberty", SC2DomainEntities.Expansion.WingsOfLiberty));
            SelectedExpansion = Expansions[1];

            SuggestionProvider = Container.Resolve<PlayerSuggestionProvider>();
        }

        private void GenerateStatistics()
        {
            if (SelectedExpansion.Value != SC2DomainEntities.Expansion.LegacyOfTheVoid)
            {
                ModernDialog.ShowMessage("Only Legacy of the Void is available.", "Sorry", MessageBoxButton.OK);
                return;
            }

            if (SelectedPlayer == null)
            {
                ModernDialog.ShowMessage("Please, select a player.\nYou have to click in the search results.", "Attention", MessageBoxButton.OK);
                return;
            }

            ValidationException validationException = null;

            LoadingService.ShowAndExecuteAction(delegate
            {
                using (new NHibernateSessionContext())
                {
                    try
                    {
                        StatisticService.LoadLatestPlayerMatches(SelectedPlayer.AligulacId, SC2DomainEntities.Expansion.LegacyOfTheVoid);
                        var domainStatistics = StatisticService.GeneratePlayerStatistics(SelectedPlayer.Id, SelectedExpansion.Value);
                        PlayerStatistics = Mapper.Map<SC2DomainEntities.PlayerStatistics, PlayerStatistics>(domainStatistics);
                        PlayerStatistics.Player = SelectedPlayer;
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
            else
            {
                HasPlayerStatistics = true;
            }
        }
    }
}
