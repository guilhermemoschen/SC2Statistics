using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

using SC2Statistics.StatisticDomain.Model;

namespace SC2Statistics.StatisticDomain.Database.Mapping
{
    public class EventMap : ClassMapping<Event>
    {
        public EventMap()
        {
            Table("Events");
            Lazy(false);

            Id(x => x.Id, mapper => mapper.Generator(Generators.Identity));

            Property(x => x.Name, mapper => mapper.NotNullable(true));
            Property(x => x.AligulacId, mapper => mapper.NotNullable(true));
            Property(x => x.AligulacReference, mapper => mapper.NotNullable(true));
            Property(x => x.Tier, mapper => { mapper.NotNullable(true); mapper.Type<EnumStringType<Tier>>(); } );
            Property(x => x.PrizePool, mapper => mapper.NotNullable(false));

            Bag(
                x => x.Matches,
                mapper =>
                {
                    mapper.Key(keyMapper => keyMapper.Column("FK_Event"));
                    mapper.Inverse(false);
                    mapper.Cascade(Cascade.All | Cascade.DeleteOrphans);
                    mapper.Lazy(CollectionLazy.Lazy);
                },
                relation => relation.OneToMany()
            );
        }
    }
}
