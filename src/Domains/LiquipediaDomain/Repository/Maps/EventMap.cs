using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

using SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.SC2Domain.Repository.Maps
{
    public class EventMap : ClassMapping<Event>
    {
        public EventMap()
        {
            Table("Events");
            Lazy(false);

            Id(x => x.Id, mapper => mapper.Generator(Generators.Identity));

            Property(x => x.IsActive, mapper => mapper.NotNullable(true));
            Property(x => x.Name, mapper => mapper.NotNullable(true));
            Property(x => x.LiquipediaReference, mapper => mapper.NotNullable(true));
            Property(x => x.StartDate, mapper => mapper.NotNullable(false));
            Property(x => x.EndDate, mapper => mapper.NotNullable(false));
            Property(x => x.LiquipediaTier, mapper => { mapper.NotNullable(true); mapper.Type<EnumStringType<LiquipediaTier>>(); } );
            Property(x => x.PrizePool, mapper => mapper.NotNullable(false));
            Property(x => x.Expansion, mapper => { mapper.NotNullable(true); mapper.Type<EnumStringType<Expansion>>(); });

            ManyToOne(x => x.MainEvent, mapper =>
            {
                mapper.Column("FK_MainEvent");
                mapper.NotNullable(false);
                mapper.Cascade(Cascade.None);
                mapper.Lazy(LazyRelation.NoLazy);
            });

            Bag(
                x => x.SubEvents,
                mapper =>
                {
                    mapper.Key(keyMapper => keyMapper.Column("FK_MainEvent"));
                    mapper.Inverse(false);
                    mapper.Cascade(Cascade.All | Cascade.DeleteOrphans);
                    mapper.Lazy(CollectionLazy.Lazy);
                },
                relation => relation.OneToMany()
            );

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
