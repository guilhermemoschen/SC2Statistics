using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using SC2LiquipediaStatistics.Utilities.Unity;

using SC2DomainEntities = SC2Statistics.SC2Domain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.Model.Translator
{
    public class SC2DomainProfile : Profile
    {
        private IMapper mapper;
        public IMapper Mapper
        {
            get
            {
                return mapper ?? (mapper = Container.Resolve<IMapper>());
            }
        }

        public override string ProfileName { get { return "SC2LiquipediaStatistics.DesktopClient.Model.Translator.SC2DomainProfile"; } }

        protected override void Configure()
        {
            // Domain -> UI
            CreateMap<SC2DomainEntities.Event, Event>()
                .ForMember(x => x.TotalMatches, opt => opt.ResolveUsing(sc2Event => sc2Event.GetTotalMatches()));

            CreateMap<SC2DomainEntities.Event, SubEvent>();

            CreateMap<SC2DomainEntities.Game, Game>();

            CreateMap<SC2DomainEntities.Match, Match>();

            CreateMap<SC2DomainEntities.Player, Player>();

            CreateMap<SC2DomainEntities.PlayerStatistics, PlayerStatistics>();

            CreateMap<IEnumerable<List<SC2DomainEntities.Match>>, IEnumerable<PlayerMatches>>()
                .ConvertUsing(ConvertToPlayerMatches);

            // UI -> Domain
            CreateMap<Event, SC2DomainEntities.Event>()
                .ForMember(x => x.Matches, opt => opt.Ignore())
                .ForMember(x => x.ValidationResults, opt => opt.Ignore());
        }

        private IEnumerable<PlayerMatches> ConvertToPlayerMatches(IEnumerable<List<SC2DomainEntities.Match>> allMatches)
        {
            var allPlayerMatches = new List<PlayerMatches>();

            foreach (var bracketMatches in allMatches)
            {
                var playerMatches = new PlayerMatches();
                var currentPlayerMatches = playerMatches;
                PlayerMatches previewsPlayerMatches = null;

                foreach (var match in bracketMatches)
                {
                    if (previewsPlayerMatches != null)
                    {
                        previewsPlayerMatches.NextMatches.Add(currentPlayerMatches);
                    }

                    currentPlayerMatches.Match = Mapper.Map<SC2DomainEntities.Match, Match>(match);
                    previewsPlayerMatches = currentPlayerMatches;
                    currentPlayerMatches = new PlayerMatches();
                }

                allPlayerMatches.Add(playerMatches);
            }

            return allPlayerMatches;
        }
    }
}