using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using AutoMapper;

using GalaSoft.MvvmLight.CommandWpf;

using SC2LiquipediaStatistics.DesktopClient.Common;
using SC2LiquipediaStatistics.DesktopClient.Model;
using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.Utilities.DataBase;
using SC2LiquipediaStatistics.Utilities.Domain;
using SC2LiquipediaStatistics.Utilities.Unity;

using SC2Statistics.Proxy.TeamLiquied;

using WpfControls.Editors;

using DomainEntities = SC2Statistics.StatisticDomain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class PlayerXPlayerStatisticsViewModel : ViewModelBase
    {
        protected PlayerXPlayerStatistics statistics;
        public PlayerXPlayerStatistics Statistics
        {
            get
            {
                return statistics;
            }
            set
            {
                if (statistics == value || value == null)
                    return;

                Set(() => Statistics, ref statistics, value, true);
            }
        }

        protected Player selectedPlayer1;
        public Player SelectedPlayer1
        {
            get
            {
                return selectedPlayer1;
            }
            set
            {
                if (selectedPlayer1 == value || value == null)
                    return;

                Set(() => SelectedPlayer1, ref selectedPlayer1, value, true);
            }
        }

        protected Player selectedPlayer2;
        public Player SelectedPlayer2
        {
            get
            {
                return selectedPlayer2;
            }
            set
            {
                if (selectedPlayer2 == value || value == null)
                    return;

                Set(() => SelectedPlayer2, ref selectedPlayer2, value, true);
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

        protected ImageSource missingPlayerImageSource;

        protected ImageSource player1ImageSource;
        public ImageSource Player1ImageSource
        {
            get
            {
                return player1ImageSource;
            }
            set
            {
                if (value == null)
                    return;

                Set(() => Player1ImageSource, ref player1ImageSource, value, true);
            }
        }

        protected ImageSource player2ImageSource;
        public ImageSource Player2ImageSource
        {
            get
            {
                return player2ImageSource;
            }
            set
            {
                if (value == null)
                    return;

                Set(() => Player2ImageSource, ref player2ImageSource, value, true);
            }
        }

        public ITeamLiquidService TeamLiquidService { get; protected set; }
        public IImageService ImageService { get; protected set; }

        public ISuggestionProvider SuggestionProvider { get; set; }

        public ICommand SearchCommand { get; private set; }

        public PlayerXPlayerStatisticsViewModel(ITeamLiquidService teamLiquidService, IImageService imageService)
        {
            TeamLiquidService = teamLiquidService;
            ImageService = imageService;

            SuggestionProvider = Container.Resolve<PlayerSuggestionProvider>();
            SearchCommand = new RelayCommand(GenerateStatistics);

            NavigatedToCommand = new RelayCommand<object>(Load);

            var missingPlayerImageUri = new Uri(ConfigurationManager.AppSettings["MissingPlayerImageUri"]);
            missingPlayerImageSource = ImageService.LoadImage(missingPlayerImageUri);
        }

        private void Load(object o)
        {
            HasPlayerStatistics = false;
        }

        private void GenerateStatistics()
        {
            if (SelectedPlayer1 == null || SelectedPlayer2 == null)
            {
                Dialog.ShowMessage("Please, select both a players first.\nYou have to click in the search results.", "Attention", MessageBoxButton.OK);
                return;
            }

            if (SelectedPlayer1.AligulacId == SelectedPlayer2.AligulacId)
            {
                Dialog.ShowMessage("Select different players.", "Attention", MessageBoxButton.OK);
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
                        var statistics = StatisticService.UpdateDataAndGeneratePlayerXPlayerStatistics(SelectedPlayer1.AligulacId, SelectedPlayer2.AligulacId, DomainEntities.Expansion.LegacyOfTheVoid);
                        if (statistics != null)
                        {
                            Statistics = Mapper.Map<DomainEntities.PlayerXPlayerStatistics, PlayerXPlayerStatistics>(statistics);
                            GetPlayersImages(statistics.Player1Statistics.Player, statistics.Player2Statistics.Player);
                        }
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

        private void GetPlayersImages(DomainEntities.Player player1, DomainEntities.Player player2)
        {
            var player1ImageUri = TeamLiquidService.GetPlayerImage(player1);

            if (player1ImageUri != null && player1ImageUri.OriginalString != ConfigurationManager.AppSettings["MissingPlayerImageUri"])
            {
                var imageSource = ImageService.LoadImage(player1ImageUri);
                if (imageSource.Width > 1024 || imageSource.Height > 1024)
                    Player1ImageSource = missingPlayerImageSource;
                else
                    Player1ImageSource = imageSource;
            }
            else
                Player1ImageSource = missingPlayerImageSource;

            var player2ImageUri = TeamLiquidService.GetPlayerImage(player2);

            if (player2ImageUri != null &&
                player2ImageUri.OriginalString != ConfigurationManager.AppSettings["MissingPlayerImageUri"])
            {
                var imageSource = ImageService.LoadImage(player2ImageUri);
                if (imageSource.Width > 1024 || imageSource.Height > 1024)
                    Player2ImageSource = missingPlayerImageSource;
                else
                    Player2ImageSource = imageSource;
            }
            else
                Player2ImageSource = missingPlayerImageSource;
        }
    }
}
