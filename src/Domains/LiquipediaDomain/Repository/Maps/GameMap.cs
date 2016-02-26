using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

using SC2LiquipediaStatistics.LiquipediaDomain.Model;

namespace SC2LiquipediaStatistics.LiquipediaDomain.Repository.Maps
{
    public class GameMap : ClassMapping<Game>
    {
        public GameMap()
        {
            Table("Games");
            Lazy(false);

            Id(x => x.Id, mapper => mapper.Generator(Generators.Identity));

            Property(x => x.Map, mapper => mapper.NotNullable(true));
            Property(x => x.Number, mapper => mapper.NotNullable(true));


            ManyToOne(
                x => x.Match,
                mapper =>
                {
                    mapper.Column("FK_Match");
                    mapper.NotNullable(true);
                    mapper.Cascade(Cascade.None);
                }
            );

            ManyToOne(
                x => x.Winner,
                mapper =>
                {
                    mapper.Column("FK_Winner");
                    mapper.NotNullable(true);
                    mapper.Cascade(Cascade.None);
                }
            );
        }
    }
}
