using AutoMapper;

using SC2Statistics.Proxy.Aligulac.Contracts;
using SC2Statistics.SC2Domain.Common;

using Converter = SC2Statistics.SC2Domain.Common.Converter;
using DomainEntities = SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.Proxy.Aligulac.Translators
{
    public class AligulacProfile : Profile
    {
        public override string ProfileName { get { return "AligulacProfile"; } }

        protected override void Configure()
        {
            CreateMap<Player, DomainEntities.Player>()
                .ForMember(x => x.AligulacReferenceUrl, opt => opt.ResolveUsing(y => y.ResourceUri))
                .ForMember(x => x.AligulacId, opt => opt.ResolveUsing(y => y.Id))
                .ForMember(x => x.Race, opt => opt.ResolveUsing(y => Converter.ToRace(y.Race)));

            CreateMap<Match, DomainEntities.Match>()
                .ForMember(x => x.Player1, opt => opt.ResolveUsing(y => y.PlayerA))
                .ForMember(x => x.Player1Race, opt => opt.ResolveUsing(y => Converter.ToRace(y.RacePlayerA)))
                .ForMember(x => x.Player1Score, opt => opt.ResolveUsing(y => y.ScorePlayerA))
                .ForMember(x => x.Player2, opt => opt.ResolveUsing(y => y.PlayerB))
                .ForMember(x => x.Player2Race, opt => opt.ResolveUsing(y => Converter.ToRace(y.RacePlayerB)))
                .ForMember(x => x.Player2Score, opt => opt.ResolveUsing(y => y.ScorePlayerB))
                .ForMember(x => x.AligulacReference, opt => opt.ResolveUsing(y => y.ResourceUri))
                .ForMember(x => x.Expansion, opt => opt.ResolveUsing(y => Converter.ToExpansion(y.Game)));

            CreateMap<Event, DomainEntities.Event>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Matches, opt => opt.Ignore())
                .ForMember(x => x.Tier, opt => opt.Ignore())
                .ForMember(x => x.PrizePool, opt => opt.Ignore())
                .ForMember(x => x.Name, opt => opt.ResolveUsing(y => y.FullName))
                .ForMember(x => x.AligulacId, opt => opt.ResolveUsing(y => y.Id))
                .ForMember(x => x.AligulacReference, opt => opt.ResolveUsing(y => y.ResourceUri));
        }
    }
}