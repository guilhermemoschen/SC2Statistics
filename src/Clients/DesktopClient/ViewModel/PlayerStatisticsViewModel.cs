using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class PlayerStatisticsViewModel : ModernViewModelBase
    {
        public static readonly Uri MissingPlayerImageUri = new Uri("http://wiki.teamliquid.net/commons/images/a/a4/PlayerImagePlaceholder.png");

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

        public IStatisticService StatisticService { get; protected set; }
        public ITeamLiquidService TeamLiquidService { get; protected set; }

        public IModernNavigationService NavigationService { get; protected set; }

        public ILoadingService LoadingService { get; protected set; }

        public IDownloader Downloader { get; protected set; }

        public IMapper Mapper { get; protected set; }

        public ICommand GenerateStatisticsCommand { get; private set; }

        public PlayerStatisticsViewModel(IStatisticService statisticService, ITeamLiquidService teamLiquidService, IModernNavigationService navigationService, ILoadingService loadingService, IDownloader downloader, IMapper mapper)
        {
            StatisticService = statisticService;
            TeamLiquidService = teamLiquidService;
            NavigationService = navigationService;
            LoadingService = loadingService;
            Downloader = downloader;
            Mapper = mapper;

            GenerateStatisticsCommand = new RelayCommand(GenerateStatistics);
            NavigatedToCommand = new RelayCommand<object>(Load);
            Expansions = new List<KeyValuePair<string, SC2DomainEntities.Expansion>>();
            Expansions.Add(new KeyValuePair<string, SC2DomainEntities.Expansion>("Hearth of the Swarm", SC2DomainEntities.Expansion.HeartOfTheSwarm));
            Expansions.Add(new KeyValuePair<string, SC2DomainEntities.Expansion>("Legacy of the Void", SC2DomainEntities.Expansion.LegacyOfTheVoid));
            Expansions.Add(new KeyValuePair<string, SC2DomainEntities.Expansion>("Wings of Liberty", SC2DomainEntities.Expansion.WingsOfLiberty));
            SelectedExpansion = Expansions[1];

            SuggestionProvider = Container.Resolve<PlayerSuggestionProvider>();

            missingPlayerImageSource = LoadImage(MissingPlayerImageUri);
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
                        StatisticService.LoadLatestPlayerMatches(SelectedPlayer.AligulacId, SC2DomainEntities.Expansion.LegacyOfTheVoid);
                        var domainStatistics = StatisticService.GeneratePlayerStatistics(SelectedPlayer.Id, SelectedExpansion.Value);
                        PlayerStatistics = Mapper.Map<SC2DomainEntities.PlayerStatistics, PlayerStatistics>(domainStatistics);
                        PlayerStatistics.Player = SelectedPlayer;
                        var playerImageUri = TeamLiquidService.GetPlayerImage(domainStatistics.Player);

                        if (playerImageUri != null && playerImageUri != MissingPlayerImageUri)
                            PlayerImageSource = LoadImage(playerImageUri);
                        else
                            PlayerImageSource = missingPlayerImageSource;
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

        private ImageSource LoadImage(Uri playerImageUri)
        {
            var imageData = Downloader.GetContentAsBytes(playerImageUri);
            var bitmapImage = new BitmapImage();

            using (var stream = new MemoryStream(imageData))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }
    }
}
