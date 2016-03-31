using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

using SC2Statistics.StatisticDomain.Model;

namespace SC2Statistics.StatisticDomain.Database.Mapping
{
    public class MatchMap : ClassMapping<Match>
    {
        public MatchMap()
        {
            Table("Matches");
            Lazy(false);

            Id(x => x.Id, mapper => mapper.Generator(Generators.Identity));

            Property(x => x.Date, mapper => mapper.NotNullable(false));
            Property(x => x.Player1Race, mapper => { mapper.NotNullable(true); mapper.Type<EnumStringType<Race>>(); });
            Property(x => x.Player2Race, mapper => { mapper.NotNullable(true); mapper.Type<EnumStringType<Race>>(); });
            Property(x => x.Player1Score, mapper => mapper.NotNullable(true));
            Property(x => x.Player2Score, mapper => mapper.NotNullable(true));
            Property(x => x.AligulacReference, mapper => mapper.NotNullable(true));
            Property(x => x.Expansion, mapper => { mapper.NotNullable(true); mapper.Type<EnumStringType<Expansion>>(); });

            ManyToOne(
                x => x.Player1,
                mapper =>
                {
                    mapper.Column("FK_Player1");
                    mapper.NotNullable(true);
                    mapper.Cascade(Cascade.None);
                }
            );

            ManyToOne(
                x => x.Player2,
                mapper =>
                {
                    mapper.Column("FK_Player2");
                    mapper.NotNullable(true);
                    mapper.Cascade(Cascade.None);
                }
            );

            ManyToOne(
                x => x.Event,
                mapper =>
                {
                    mapper.Column("FK_Event");
                    mapper.NotNullable(false);
                    mapper.Cascade(Cascade.None);
                }
            );
        }
    }
}
