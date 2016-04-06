using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using AutoMapper;

using GalaSoft.MvvmLight.CommandWpf;

using SC2LiquipediaStatistics.DesktopClient.Common;
using SC2LiquipediaStatistics.DesktopClient.Model;
using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.DesktopClient.View;
using SC2LiquipediaStatistics.Utilities.DataBase;
using SC2LiquipediaStatistics.Utilities.Domain;
using SC2LiquipediaStatistics.Utilities.Unity;

using SC2Statistics.Proxy.TeamLiquied;
using SC2Statistics.StatisticDomain.Service;
using SC2Statistics.Utilities.Web;

using WpfControls.Editors;

using SC2DomainEntities = SC2Statistics.StatisticDomain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class SoloPlayerStatisticsViewModel : ViewModelBase
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

        protected SoloPlayerStatistics playerStatistics;
        public SoloPlayerStatistics PlayerStatistics
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

        protected ImageSource missingPlayerImageSource;

        protected ImageSource playerImageSource;
        public ImageSource PlayerImageSource
        {
            get
            {
                return playerImageSource;
            }
            set
            {
                if (value == null)
                    return;

                Set(() => PlayerImageSource, ref playerImageSource, value, true);
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

        public ITeamLiquidService TeamLiquidService { get; protected set; }
        public IImageService ImageService { get; protected set; }

        public ICommand GenerateStatisticsCommand { get; private set; }

        public SoloPlayerStatisticsViewModel(ITeamLiquidService teamLiquidService, IImageService imageService)
        {
            TeamLiquidService = teamLiquidService;
            ImageService = imageService;

            GenerateStatisticsCommand = new RelayCommand(GenerateStatistics);
            NavigatedToCommand = new RelayCommand<object>(Load);
            Expansions = new List<KeyValuePair<string, SC2DomainEntities.Expansion>>();
            Expansions.Add(new KeyValuePair<string, SC2DomainEntities.Expansion>("Hearth of the Swarm", SC2DomainEntities.Expansion.HeartOfTheSwarm));
            Expansions.Add(new KeyValuePair<string, SC2DomainEntities.Expansion>("Legacy of the Void", SC2DomainEntities.Expansion.LegacyOfTheVoid));
            Expansions.Add(new KeyValuePair<string, SC2DomainEntities.Expansion>("Wings of Liberty", SC2DomainEntities.Expansion.WingsOfLiberty));
            SelectedExpansion = Expansions[1];

            SuggestionProvider = Container.Resolve<PlayerSuggestionProvider>();

            var missingPlayerImageUri = new Uri(ConfigurationManager.AppSettings["MissingPlayerImageUri"]);
            missingPlayerImageSource = ImageService.LoadImage(missingPlayerImageUri);
        }

        private void Load(object obj)
        {
            HasPlayerStatistics = false;
        }

        private void GenerateStatistics()
        {
            if (SelectedExpansion.Value != SC2DomainEntities.Expansion.LegacyOfTheVoid)
            {
                Dialog.ShowMessage("Only Legacy of the Void is available.", "Sorry", MessageBoxButton.OK);
                return;
            }

            if (SelectedPlayer == null)
            {
                Dialog.ShowMessage("Please, select a player.\nYou have to click in the search results.", "Attention", MessageBoxButton.OK);
                return;
            }

            HasPlayerStatistics = false;
            ValidationException validationException = null;

            LoadingService.ShowAndExecuteAction(delegate
            {
                using (new NHibernateSessionContext())
                {
                    try
                    {
                        var domainStatistics = StatisticService.UpdateDataAndGenerateSoloPlayerStatistics(SelectedPlayer.AligulacId, SelectedExpansion.Value);
                        PlayerStatistics = Mapper.Map<SC2DomainEntities.SoloPlayerStatistics, SoloPlayerStatistics>(domainStatistics);
                        PlayerStatistics.Player = SelectedPlayer;
                        LoadPlayerImage(domainStatistics.Player);
                    }
                    catch (ValidationException ex)
                    {
                        validationException = ex;
                    }
                }
            });

            if (validationException != null)
            {
                Dialog.ShowMessage(validationException.GetFormatedMessage(), "Validation Message", MessageBoxButton.OK);
            }
            else
            {
                HasPlayerStatistics = true;
            }
        }

        public void LoadPlayerImage(SC2DomainEntities.Player player)
        {
            var playerImageUri = TeamLiquidService.GetPlayerImage(player);

            if (playerImageUri != null && playerImageUri.OriginalString != ConfigurationManager.AppSettings["MissingPlayerImageUri"])
            {
                var imageSource = ImageService.LoadImage(playerImageUri);
                if (imageSource.Width > 1024 || imageSource.Height > 1024)
                {
                    PlayerImageSource = missingPlayerImageSource;
                }
                else
                {
                    PlayerImageSource = imageSource;
                }
            }
            else
            {
                PlayerImageSource = missingPlayerImageSource;
            }
        }
    }
}
