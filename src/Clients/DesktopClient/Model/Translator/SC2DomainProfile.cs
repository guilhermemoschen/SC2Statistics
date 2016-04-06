using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using SC2LiquipediaStatistics.Utilities.Unity;

using SC2DomainEntities = SC2Statistics.StatisticDomain.Model;

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

        public override string ProfileName => "SC2LiquipediaStatistics.DesktopClient.Model.Translator.SC2DomainProfile";

        protected override void Configure()
        {
            // Domain -> UI
            CreateMap<SC2DomainEntities.Event, Event>()
                .ForMember(x => x.TotalMatches, opt => opt.ResolveUsing(sc2Event => sc2Event.GetTotalMatches()));

            CreateMap<SC2DomainEntities.Event, SubEvent>();

            CreateMap<SC2DomainEntities.Game, Game>();

            CreateMap<SC2DomainEntities.Match, Match>();

            CreateMap<SC2DomainEntities.Player, Player>();

            CreateMap<SC2DomainEntities.SoloPlayerStatistics, SoloPlayerStatistics>()
                .ForMember(x => x.TotalMatches, opt => opt.ResolveUsing(y => y.Matches.Count()));

            CreateMap<SC2DomainEntities.PlayerXPlayerStatistics, PlayerXPlayerStatistics>()
                .ForMember(x => x.MatchesBetweenPlayers, opt => opt.ResolveUsing(y => y.MatchesBetweenPlayers.Count()));

            // UI -> Domain
            CreateMap<Event, SC2DomainEntities.Event>()
                .ForMember(x => x.Matches, opt => opt.Ignore())
                .ForMember(x => x.ValidationResults, opt => opt.Ignore());
        }
    }
}