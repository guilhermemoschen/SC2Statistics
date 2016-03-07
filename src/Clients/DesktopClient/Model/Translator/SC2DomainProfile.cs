using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using SC2DomainEntities = SC2Statistics.SC2Domain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.Model.Translator
{
    public class SC2DomainProfile : Profile
    {
        public override string ProfileName { get { return "SC2LiquipediaStatistics.DesktopClient.Model.Translator.SC2DomainProfile"; } }

        protected override void Configure()
        {
            // Domain -> UI
            CreateMap<SC2DomainEntities.Event, Event>();

            CreateMap<SC2DomainEntities.Game, Game>();

            CreateMap<SC2DomainEntities.Match, Match>();

            CreateMap<SC2DomainEntities.Player, Player>();

            CreateMap<SC2DomainEntities.PlayerStatistics, PlayerStatistics>();

            // UI -> Domain
            CreateMap<Player, SC2DomainEntities.Player>()
                .ForMember(x => x.EventsParticipaed, opt => opt.Ignore());

            CreateMap<Event, SC2DomainEntities.Event>()
                .ForMember(x => x.Matches, opt => opt.Ignore());
        }
    }
}