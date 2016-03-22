using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using GalaSoft.MvvmLight.CommandWpf;

using SC2LiquipediaStatistics.DesktopClient.Model;

using SC2Statistics.SC2Domain.Service;

using SC2DomainEntities = SC2Statistics.SC2Domain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class PlayerByEventStatisticsViewModel : ModernViewModelBase
    {
        public IStatisticsService StatisticsService { get; private set; }

        public ISC2Service SC2Service { get; private set; }

        public IMapper Mapper { get; private set; }

        private ObservableCollection<PlayerMatches> bracketMatches;
        public ObservableCollection<PlayerMatches> BracketMatches
        {
            get
            {
                return bracketMatches;
            }
            set
            {
                if (value == null || bracketMatches == value)
                    return;

                Set(() => BracketMatches, ref bracketMatches, value, true);
            }
        }

        private Player selectedPlayer;
        public Player SelectedPlayer
        {
            get
            {
                return selectedPlayer;
            }
            set
            {
                if (value == null || selectedPlayer == value)
                    return;

                Set(() => SelectedPlayer, ref selectedPlayer, value, true);
            }
        }

        private Event selectedEvent;
        public Event SelectedEvent
        {
            get
            {
                return selectedEvent;
            }
            set
            {
                if (value == null || selectedEvent == value)
                    return;

                Set(() => SelectedEvent, ref selectedEvent, value, true);
            }
        }

        private ObservableCollection<GroupMatches> groupsMatches;
        public ObservableCollection<GroupMatches> GroupsMatches
        {
            get
            {
                return groupsMatches;
            }
            set
            {
                if (value == null || groupsMatches == value)
                    return;

                Set(() => GroupsMatches, ref groupsMatches, value, true);
            }
        }

        public PlayerByEventStatisticsViewModel(ISC2Service sc2Service, IStatisticsService statisticsService, IMapper mapper)
        {
            SC2Service = sc2Service;
            StatisticsService = statisticsService;
            Mapper = mapper;

            NavigatedToCommand = new RelayCommand<object>(LoadStatistics);
        }

        private void LoadStatistics(object parameter)
        {
            var realParameter = parameter as PlayerByEventStatisticsParameter;

            if (realParameter == null)
                throw new ValidationException("Invalid Parameter.");

            SelectedEvent = realParameter.Event;
            SelectedPlayer = realParameter.Player;

            var allMatches = StatisticsService.GeneratePlayerPathbyEvent(SelectedPlayer.Id, SelectedEvent.Id);
            BracketMatches = new ObservableCollection<PlayerMatches>(GetBracketMatches(allMatches));
            GroupsMatches = new ObservableCollection<GroupMatches>(GetGroupMatches(allMatches));
        }

        private IEnumerable<PlayerMatches> GetBracketMatches(List<List<SC2DomainEntities.Match>> allMatches)
        {
            var domainBracketMatches = allMatches.Where(x => x.Any(y => y.Format == SC2DomainEntities.MatchFormat.Bracket));
            var bracketMatches = Mapper.Map<IEnumerable<List<SC2DomainEntities.Match>>, IEnumerable<PlayerMatches>>(domainBracketMatches);
            SetTargetPlayerInPlayerMatches(bracketMatches);
            return bracketMatches;
        }

        private IEnumerable<GroupMatches> GetGroupMatches(List<List<SC2DomainEntities.Match>> allMatches)
        {
            var matches = allMatches.SelectMany(x => x).Where(x => x.Format == SC2DomainEntities.MatchFormat.Group);
            return matches
                .GroupBy(x => x.GroupName)
                .Select(groupOfMatches => new GroupMatches()
                {
                    Title = groupOfMatches.Key,
                    Matches = Mapper.Map<IEnumerable<SC2DomainEntities.Match>, IEnumerable<Match>>(groupOfMatches)
                });
        }

        private void SetTargetPlayerInPlayerMatches(IEnumerable<PlayerMatches> playerMatches)
        {
            foreach (var playerMatch in playerMatches)
            {
                playerMatch.TargetPlayer = SelectedPlayer;
                SetTargetPlayerInPlayerMatches(playerMatch.NextMatches);
            }
        }
    }

    public class PlayerByEventStatisticsParameter
    {
        public Player Player { get; set; }
        public Event Event { get; set; }
    }
}
